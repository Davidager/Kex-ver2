﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Xml;
using UnityEngine.Profiling;


public class CreateSimulation {
    

    private Dictionary<int, float> affinityTable;
    //set matching cutoff to the right level
    private static float matchingCUTOFF = 0.8f;
    //set inglencecutoff to the sama as in database
    private static float influenceCUTOFF = 0.3f;
    private static ComparatorAgent exampleSubAgent;
    private static int confIndex = new int();
    //private static float matchingValue = new float();
    private static float tempx;
    private static float tempz;
    private static float tempSpeed;
    private static float tempDirection;
    private static float tempAff;
    private static float xDiff;
    private static float zDiff;
    private static bool collision;
    private static float maxTemp;
    private static Vector2 qPosition;
    //set radiusCUTOFF
    private static float radiusCUTOFF = 0.00002f;
    private static float kDirectionGlobal;
    private static Vector2 originVector;
    private static float originDirection;
    private static Vector2 kPositionGlobal;
    private static Vector2 tempVector;
    private static List<float> matchingFunctionList = new List<float>();
    private static List<float> affinityValueList = new List<float>();
    private static Dictionary<float, int> affinityValueDic = new Dictionary<float, int>();
    private static List<float> xCoordListCopy;
    private static List<float> zCoordListCopy;
    private static List<float> speedListCopy;
    private static List<float> directionListCopy;
    private static ComparatorAgent agentkCopy;
    private static ComparatorAgent agentqCopy;
    private static Dictionary<float, int> matchingFunctionDic = new Dictionary<float, int>();
    private static List<int> negativeMatchingIndexList = new List<int>();
    private static int matchingIndex = new int();
    private static Dictionary<int, Agent> activeAgentTable;
    private static Configuration[] exampleConfigurations;
    private static Configuration currentQueryConfiguration = null;

    public CreateSimulation(ExampleContainer exampleContainer)
    {
        exampleConfigurations = createExampleConfigurations(exampleContainer);
        activeAgentTable = new Dictionary<int, Agent>();
        for (int i = 0; i < 2; i++)
        {
            activeAgentTable.Add(i,(Agent)GameObject.Find("Ground").AddComponent(typeof(Agent)));
            activeAgentTable[i].setAgentNumber(i);
            activeAgentTable[i].setCreateSimulation(this);
        }
    }

    public void removeFromActiveAgentTable(int agentNumber)
    {
        activeAgentTable.Remove(agentNumber);
    }

    public static void assignTrajectory(Agent agent, int agentNumber){
        Profiler.BeginSample("createQuery");
        currentQueryConfiguration = createQueryConf(agentNumber);
        Profiler.EndSample();
        if (agent.hasTrajectory())
        {
            if (agent.getUpdateCounter() > 14)
            {
                Profiler.BeginSample("MatchingValue");
                float matchingValue = MatchingFunctions.matchingFunction
                    (currentQueryConfiguration, agent.lastConfiguration);
                Profiler.EndSample();
                // use matching function to compare current configuration with the configuration when the last trajectory was assigned
                if (matchingValue < matchingCUTOFF)
                {
                    Profiler.BeginSample("updateTrajectory_1");
                    updateTrajectory(agent, agentNumber);
                    Profiler.EndSample();
                }
            }
        }else
        {
            Profiler.BeginSample("updateTrajectory_2");
            updateTrajectory(agent, agentNumber);
            Profiler.EndSample();
        }
        agent.lastConfiguration = currentQueryConfiguration;
    }

    private static void updateTrajectory(Agent Agentq, int agentNumber){
        tempx = Agentq.xCoordList[0];
        Agentq.xCoordList = new List<float>();
        Agentq.xCoordList.Add(tempx);
        tempz = Agentq.zCoordList[0];
        Agentq.zCoordList = new List<float>();
        Agentq.zCoordList.Add(tempz);
        tempSpeed = Agentq.speedList[0];
        Agentq.speedList = new List<float>();
        Agentq.speedList.Add(tempSpeed);
        tempDirection = Agentq.directionList[0];
        Agentq.directionList = new List<float>();
        Agentq.directionList.Add(tempDirection);
        //Debug.Log(Agentq.xCoordList.Count);
        Debug.Log(activeAgentTable[agentNumber].xCoordList.Count);


        if (currentQueryConfiguration.influenceValues.Length == 0) {
            fillConfig(Agentq);
            Debug.Log("if fill grejen");
        }
        else
        {

            Profiler.BeginSample("updatePart1");
            for (int temp = 0; temp < exampleConfigurations.Length; temp++)
            {
                //Debug.Log(currentQueryConfiguration.infAgentArray.Length);     ger 1
                Profiler.BeginSample("MatchingFunctionUpdateCall");
                float matchingValue = MatchingFunctions.matchingFunction(currentQueryConfiguration, exampleConfigurations[temp]);
                Profiler.EndSample();
                if (matchingValue > 0)
                {
                    matchingFunctionList.Add(matchingValue);
                    if (!matchingFunctionDic.ContainsKey(matchingValue))
                    {
                        matchingFunctionDic.Add(matchingValue, temp);
                    }                
                }
                else
                {
                    negativeMatchingIndexList.Add(temp);
                }
            }
            Debug.Log(matchingFunctionList.Count);
            Profiler.EndSample();
            Profiler.BeginSample("updatePart2");
            matchingFunctionList.Sort();
            Debug.Log("check1");
            checkCollision(matchingFunctionList, matchingFunctionDic, agentNumber, true);

            if (Agentq.xCoordList.Count < 2)
            {
                foreach (int confIndex in negativeMatchingIndexList)
                {
                    tempAff = MatchingFunctions.affinityFunction(currentQueryConfiguration.subAgent, currentQueryConfiguration.subAgent, exampleConfigurations[confIndex].subAgent);
                    affinityValueList.Add(tempAff);
                    affinityValueDic.Add(tempAff, confIndex);
                }
                affinityValueList.Sort();
                Debug.Log("check2");
                                
                checkCollision(affinityValueList, affinityValueDic, agentNumber, false);
            }
            Profiler.EndSample();

        }
       
        matchingFunctionList.Clear();
        affinityValueList.Clear();
        
    }

    private static void checkCollision(List<float> valueList, Dictionary<float, int> indexDic, int agentNumber, bool matched)
    {
        Debug.Log(valueList.Count);
        int i = valueList.Count - 1;
        originVector = new Vector2(currentQueryConfiguration.subAgent.xCoordList[0], currentQueryConfiguration.subAgent.zCoordList[0]);
        originDirection = currentQueryConfiguration.subAgent.directionList[0];
        while (i >= 0)
        {
            confIndex = indexDic[valueList[i]];
            exampleSubAgent = exampleConfigurations[confIndex].subAgent;

            agentqCopy = new ComparatorAgent(40);  
            tempVector = globalToLocalVector2(new Vector2(activeAgentTable[agentNumber].xCoordList[0], activeAgentTable[agentNumber].zCoordList[0])
                , originVector, originDirection);
            /*agentqCopy.directionList[0] = globalToLocalDirection(activeAgentTable[agentNumber].directionList[0], originDirection);
            agentqCopy.xCoordList[0] = tempVector.x;
            agentqCopy.zCoordList[0] = tempVector.y;
            agentqCopy.speedList[0] = activeAgentTable[agentNumber].speedList[0];*/
            agentqCopy.addToTrajectory(tempVector.x, tempVector.y, activeAgentTable[agentNumber].speedList[0]
                , globalToLocalDirection(activeAgentTable[agentNumber].directionList[0], originDirection));
            for (int ii = 1; ii < 40; ii++)
            {
                agentqCopy.addToTrajectory(exampleSubAgent.xCoordList[ii], exampleSubAgent.zCoordList[ii],
                    exampleSubAgent.speedList[ii], exampleSubAgent.directionList[ii]);

                /*agentqCopy.xCoordList[ii] = exampleSubAgent.xCoordList[ii];
                agentqCopy.zCoordList[ii] = exampleSubAgent.zCoordList[ii];
                agentqCopy.speedList[ii] = exampleSubAgent.speedList[ii];
                agentqCopy.directionList[ii] = exampleSubAgent.directionList[ii];*/

            }
            collision = false;
            foreach (KeyValuePair<int, Agent> agentk in activeAgentTable)
            {
                if (!(agentk.Key == agentNumber))
                {
                    agentkCopy = new ComparatorAgent(40);
                    for (int jj = 0; jj < agentk.Value.xCoordList.Count; jj++)
                    {
                        agentkCopy.addToTrajectory(agentk.Value.xCoordList[jj], agentk.Value.zCoordList[jj]
                            , agentk.Value.speedList[jj], agentk.Value.directionList[jj]);
                    }
                    fillConfig(agentkCopy);
                    for (int temp = 0; temp < 40; temp++)
                    {
                        kPositionGlobal = new Vector2(agentkCopy.xCoordList[temp], agentkCopy.zCoordList[temp]);
                        kDirectionGlobal = agentkCopy.directionList[temp];
                        
                        tempVector = globalToLocalVector2(kPositionGlobal, originVector, originDirection);
                        agentkCopy.directionList[temp] = globalToLocalDirection(kDirectionGlobal, originDirection);
                        agentkCopy.xCoordList[temp] = tempVector.x;
                        agentkCopy.zCoordList[temp] = tempVector.y;
                    }

                    for (int temp = 0; temp < 40; temp++)
                    {
                        xDiff = agentkCopy.xCoordList[temp] - agentqCopy.xCoordList[temp];
                        zDiff = agentkCopy.zCoordList[temp] - agentqCopy.zCoordList[temp];
                        if (xDiff > -1*radiusCUTOFF && xDiff < radiusCUTOFF)
                        {
                            collision = true;
                            break;

                        }
                        if (zDiff > -1 * radiusCUTOFF && zDiff < radiusCUTOFF)
                        {
                            collision = true;
                            break;

                        }
                    }
                    Debug.Log("collisioncheck2");
                    if (collision == true)
                    {
                        Debug.Log("collision = true");
                        break;
                        
                    }
                }
                
            }
            if (collision == false)
            {
                Debug.Log(valueList[i]);
                if (matched) maxTemp = 40 * valueList[i];                // TODO: verkar som att valueList kan bli > 1!
                else maxTemp = 40;
                //Debug.Log(activeAgentTable[agentNumber].xCoordList.Count);
                //Debug.Log(maxTemp);
                for (int temp = 1; temp < (int)maxTemp; temp++)
                {
                    qPosition = new Vector2(agentqCopy.xCoordList[temp], agentqCopy.zCoordList[temp]);
                    /*activeAgentTable[agentNumber].xCoordList[temp] = qPosition.x;
                    activeAgentTable[agentNumber].zCoordList[temp] = qPosition.y;
                    activeAgentTable[agentNumber].speedList[temp] = agentqCopy.speedList[temp];
                    activeAgentTable[agentNumber].directionList[temp] = agentqCopy.directionList[temp];*/
                    activeAgentTable[agentNumber].addToTrajectory(qPosition.x, qPosition.y
                        , agentqCopy.speedList[temp], agentqCopy.directionList[temp]);
                }
                //Debug.Log(activeAgentTable[agentNumber].xCoordList.Count);
                break;

            }
            i--;
        }
    }

   
    private static Configuration createQueryConf(int agentNumber)
    {
        Vector2 tempVector;
        float tempDirection;
        Agent subjectAgent = activeAgentTable[agentNumber];
        Vector2 newOrigin = new Vector2(subjectAgent.xCoordList[0], subjectAgent.zCoordList[0]);
        float originDirection = subjectAgent.directionList[0];

        Configuration retConf = new Configuration();
        int j = 0;
        retConf.infAgentArray = new ComparatorAgent[activeAgentTable.Count - 1];
        Profiler.BeginSample("OtherPart");
        Debug.Log("otherPart");
        int c = 0;
        foreach (KeyValuePair<int, Agent> e in activeAgentTable)
        {
            int b = 0;
            Debug.Log(e.Value.xCoordList.Count);                              //DENNA!!!!!
            ComparatorAgent tempComparatorAgent = new ComparatorAgent();
            for (int i = 0; i < e.Value.xCoordList.Count; i++)
            {
                tempVector = new Vector2(e.Value.xCoordList[i], e.Value.zCoordList[i]);
                Profiler.BeginSample("gtlvec");
                c++;
                b++;
                tempVector = globalToLocalVector2(tempVector, newOrigin, originDirection);
                Profiler.EndSample();
                tempDirection = e.Value.directionList[i];
                Profiler.BeginSample("gtldirec");
                tempDirection = globalToLocalDirection(tempDirection, originDirection);
                Profiler.EndSample();

                tempComparatorAgent.addToTrajectory(tempVector.x, tempVector.y, 
                    e.Value.speedList[i], tempDirection);
            }
            //Debug.Log(b);
            if (e.Key != agentNumber)
            {
                retConf.infAgentArray[j] = tempComparatorAgent;
                j++;
            } else
            {
                retConf.subAgent = tempComparatorAgent;
            }
        }
        //Debug.Log(c);
        Profiler.EndSample();
        Profiler.BeginSample("FillandCalc");
        retConf.fillAndCalcInfluences();
        Profiler.EndSample();
        return retConf;
    }

    private Configuration[] createExampleConfigurations(ExampleContainer exampleContainer)
    {
        Configuration[] returnArray = new Configuration[exampleContainer.examples.Count];
        int i = 0;
        foreach (ExampleData exampleData in exampleContainer.examples)
        {
            //raden under överflödig?
            returnArray[i] = new Configuration();
            ComparatorAgent subAgent = new ComparatorAgent();
            ComparatorAgent[] infAgentArray = new ComparatorAgent[exampleData.frames[0].jAgents.Count];
            float[] influenceValues = new float[infAgentArray.Length];
            /*int k = 0;
             är vi säkra på att influenceValues är i samma ordning som influenceagents i databasen?
            foreach (InfluenceValue infValue in exampleData.influenceValues)
            {
                influenceValues[k] = infValue.value;
                k++;
            }

            for (int temp = 0; temp < infAgentArray.Length; temp++)
            {
                infAgentArray[temp] = new ComparatorAgent();
            }
            */
            //7 raderna under istället för det ovan?
            int k = 0;
            foreach (InfluenceValue infValue in exampleData.influenceValues)
            {
                influenceValues[k] = infValue.value;
                infAgentArray[k] = new ComparatorAgent();
                k++;
            }
            foreach (FrameData frameData in exampleData.frames)
            {
                subAgent.addToTrajectory(frameData.subject.localPosition.x,
                    frameData.subject.localPosition.y, frameData.subject.speed,
                    frameData.subject.direction);
                int j = 0;
                foreach (agentData jAgent in frameData.jAgents)
                {
                    infAgentArray[j].addToTrajectory(jAgent.localPosition.x,
                        jAgent.localPosition.y, jAgent.speed, jAgent.direction);
                    j++;
                }
                
            } 


            returnArray[i].subAgent = subAgent;
            returnArray[i].infAgentArray = infAgentArray;
            returnArray[i].influenceValues = influenceValues;
            i++;
        }
        Debug.Log("Alla exampleconfigs skapade");

        return returnArray;
    }

    private static Vector2 globalToLocalVector2(Vector2 position, Vector2 newOrigin, float originDirection)
    {
        Vector2 retvec = new Vector2(position.x - newOrigin.x, position.y - newOrigin.y);
        retvec = Quaternion.Euler(new Vector3(0, 0, (originDirection - (Mathf.PI / 2)) * 180 / Mathf.PI)) * retvec;
        return retvec;
    }

    private static float globalToLocalDirection(float direction, float originDirection)
    {
        direction = direction - originDirection + Mathf.PI / 2;
        if (direction < 0) direction = direction + 2 * Mathf.PI;
        if (direction > 2 * Mathf.PI) direction = direction - 2 * Mathf.PI;
        return direction;
    }

    private static Vector2 localToGlobalVector2(Vector2 position, Vector2 newOrigin, float originDirection)
    {
        Vector2 retvec = Quaternion.Euler(new Vector3(0, 0, -(originDirection - (Mathf.PI / 2)) * 180 / Mathf.PI)) * position;
        retvec = new Vector2(retvec.x + newOrigin.x, retvec.y + newOrigin.y);
        return retvec;
    }

    private static float localToGlobalDirection(float direction, float originDirection)
    {
        direction = direction + originDirection - Mathf.PI / 2;
        if (direction > 2*Mathf.PI) direction = direction - 2 * Mathf.PI;
        if (direction < 0) direction = direction + 2 * Mathf.PI;
        return direction;
    }

    private static void fillConfig(Agent agent)
    {
        while (agent.xCoordList.Count < 40)
        {
            float newXcoord = agent.xCoordList[agent.xCoordList.Count - 1] + agent.speedList[agent.speedList.Count - 1] * (float)Math.Cos(agent.directionList[agent.directionList.Count - 1]);
            float newZcoord = agent.zCoordList[agent.zCoordList.Count - 1] + agent.speedList[agent.speedList.Count - 1] * (float)Math.Sin(agent.directionList[agent.directionList.Count - 1]);
            float newSpeed = agent.speedList[agent.speedList.Count - 1];
            float newDirection = agent.directionList[agent.directionList.Count - 1];
            agent.xCoordList.Add(newXcoord);
            agent.zCoordList.Add(newZcoord);
            agent.speedList.Add(newSpeed);
            agent.directionList.Add(newDirection);

        }
    }

    private static void fillConfig(ComparatorAgent agent)
    {
        while (agent.xCoordList.Count < 40)
        {
            float newXcoord = agent.xCoordList[agent.xCoordList.Count - 1] + agent.speedList[agent.speedList.Count - 1] * (float)Math.Cos(agent.directionList[agent.directionList.Count - 1]);
            float newZcoord = agent.zCoordList[agent.zCoordList.Count - 1] + agent.speedList[agent.speedList.Count - 1] * (float)Math.Sin(agent.directionList[agent.directionList.Count - 1]);
            float newSpeed = agent.speedList[agent.speedList.Count - 1];
            float newDirection = agent.directionList[agent.directionList.Count - 1];
            agent.xCoordList.Add(newXcoord);
            agent.zCoordList.Add(newZcoord);
            agent.speedList.Add(newSpeed);
            agent.directionList.Add(newDirection);

        }
    }



    // Låter assigntrajectory ske i Agents updatefunktion istället!
    /*foreach (Agent agent in agentArray)
    {
        assignTrajectory(agent);
    }
}*/
}
