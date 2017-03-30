using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Xml;


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
    private static Vector2 qPosition;
    //set radiusCUTOFF
    private static float radiusCUTOFF = 0.02f;
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
    private static Agent agentkCopy;
    private static Agent agentqCopy;
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

        currentQueryConfiguration = createQueryConf(agentNumber);
        if (agent.hasTrajectory())
        {
            if (agent.getUpdateCounter() > 14)
            {
               
                float matchingValue = MatchingFunctions.matchingFunction
                    (currentQueryConfiguration, agent.lastConfiguration);
                // use matching function to compare current configuration with the configuration when the last trajectory was assigned
                if (matchingValue < matchingCUTOFF)
                {
                    updateTrajectory(agent, agentNumber);
                }
            }
        }else
        {
            updateTrajectory(agent, agentNumber);
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

        if (currentQueryConfiguration.influenceValues.Length == 0) {
            fillConfig(Agentq);
        }
        else
        {
   
            for (int temp = 0; temp < exampleConfigurations.Length; temp++)
            {
                float matchingValue = MatchingFunctions.matchingFunction(currentQueryConfiguration, exampleConfigurations[temp]);
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
            matchingFunctionList.Sort();
            checkCollision(matchingFunctionList, matchingFunctionDic, agentNumber);

            if (Agentq.xCoordList.Count < 2)
            {
                foreach (int confIndex in negativeMatchingIndexList)
                {
                    tempAff = MatchingFunctions.affinityFunction(currentQueryConfiguration.subAgent, currentQueryConfiguration.subAgent, exampleConfigurations[confIndex].subAgent);
                    affinityValueList.Add(tempAff);
                    affinityValueDic.Add(tempAff, confIndex);
                }
                affinityValueList.Sort();
                checkCollision(affinityValueList, affinityValueDic, agentNumber);
            }

        }
       
        matchingFunctionList.Clear();
        affinityValueList.Clear();
        
    }

    private static void checkCollision(List<float> valueList, Dictionary<float, int> indexDic, int agentNumber)
    {
        int i = valueList.Count - 1;
        originVector = new Vector2(currentQueryConfiguration.subAgent.xCoordList[0], currentQueryConfiguration.subAgent.zCoordList[0]);
        originDirection = currentQueryConfiguration.subAgent.directionList[0];
        while (i >= 0)
        {
            confIndex = indexDic[valueList[i]];
            exampleSubAgent = exampleConfigurations[confIndex].subAgent;
            agentqCopy = activeAgentTable[agentNumber];
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
                    agentkCopy = agentk.Value;
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
                    if (collision == true) break;
                }
            }
            if (collision == false)
            {
                for (int temp = 1; temp < 40; temp++)
                {
                    qPosition = new Vector2(agentqCopy.xCoordList[temp], agentqCopy.zCoordList[temp]);
                    activeAgentTable[agentNumber].xCoordList[temp] = qPosition.x;
                    activeAgentTable[agentNumber].zCoordList[temp] = qPosition.y;
                    activeAgentTable[agentNumber].speedList[temp] = agentqCopy.speedList[temp];
                    activeAgentTable[agentNumber].directionList[temp] = agentqCopy.directionList[temp];
                }
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
        foreach (KeyValuePair<int, Agent> e in activeAgentTable)
        {
            ComparatorAgent tempComparatorAgent = new ComparatorAgent();
            for (int i = 0; i < e.Value.xCoordList.Count; i++)
            {
                tempVector = new Vector2(e.Value.xCoordList[i], e.Value.zCoordList[i]);
                tempVector = globalToLocalVector2(tempVector, newOrigin, originDirection);
                tempDirection = e.Value.directionList[i];
                tempDirection = globalToLocalDirection(tempDirection, originDirection);

                tempComparatorAgent.addToTrajectory(tempVector.x, tempVector.y, 
                    e.Value.speedList[i], tempDirection);
            }

            if (e.Key != agentNumber)
            {
                retConf.infAgentArray[j] = tempComparatorAgent;
                j++;
            } else
            {
                retConf.subAgent = tempComparatorAgent;
            }
        }

        retConf.fillAndCalcInfluences();
        return retConf;
    }

    private Configuration[] createExampleConfigurations(ExampleContainer exampleContainer)
    {
        Configuration[] returnArray = new Configuration[exampleContainer.examples.Count];
        int i = 0;
        foreach (ExampleData exampleData in exampleContainer.examples)
        {
            returnArray[i] = new Configuration();
            ComparatorAgent subAgent = new ComparatorAgent();
            ComparatorAgent[] infAgentArray = new ComparatorAgent[exampleData.frames[0].jAgents.Count];
            float[] influenceValues = new float[infAgentArray.Length];
            int k = 0;
            foreach (InfluenceValue infValue in exampleData.influenceValues)
            {
                influenceValues[k] = infValue.value;
                k++;
            }

            for (int temp = 0; temp < infAgentArray.Length; temp++)
            {
                infAgentArray[temp] = new ComparatorAgent();
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



    // Låter assigntrajectory ske i Agents updatefunktion istället!
    /*foreach (Agent agent in agentArray)
    {
        assignTrajectory(agent);
    }
}*/
}
