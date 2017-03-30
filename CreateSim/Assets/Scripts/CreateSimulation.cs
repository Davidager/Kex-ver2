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
    private static float matchingValue = new float();
    private static float tempx;
    private static float tempz;
    private static float tempSpeed;
    private static float tempDirection;
    private static float tempAff;
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
        for (int i = 0; i < 10; i++)
        {
            activeAgentTable.Add(i,(Agent)GameObject.Find("Ground").AddComponent(typeof(Agent)));
            activeAgentTable[i].setAgentNumber(i);
        }
    }

    public static void assignTrajectory(Agent agent, int agentNumber){

        currentQueryConfiguration = createQueryConf(agentNumber);
        if (agent.hasTrajectory())
        {
            if (agent.getUpdateCounter() > 14)
            {
               
                float matchingValue = MatchingFunctions.matchingFunction
                    (currentQueryConfiguration, agent.lastConfiguration);
                agent.lastConfiguration = currentQueryConfiguration;   // flytta ner!
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
        
        if (currentQueryConfiguration.influenceValues.Length == 0)
        {
            //de 5 raderna under kan ev istället lösas med fill config
            while (Agentq.xCoordList.Count < 40)
            {
                Agentq.xCoordList.Add(Agentq.xCoordList[0] + Agentq.speedList[0] * Mathf.Cos(Agentq.directionList[0]));
                Agentq.zCoordList.Add(Agentq.zCoordList[0] + Agentq.speedList[0] * Mathf.Sin(Agentq.directionList[0]));
                Agentq.speedList.Add(Agentq.speedList[0]);
                Agentq.directionList.Add(Agentq.directionList[0]);
            }
        }else
        {
            // use matching function to compare current configuration to the configuration of all examples
            for (int temp = 0; temp < exampleConfigurations.Length; temp++)
            {
                matchingValue = MatchingFunctions.matchingFunction(currentQueryConfiguration, exampleConfigurations[temp]);
                if (matchingValue > 0)
                {
                    matchingFunctionList.Add(matchingValue);
                    matchingFunctionDic.Add(matchingValue, temp);
                }
                else
                {
                    negativeMatchingIndexList.Add(temp); 
                }
            }
            matchingFunctionList.Sort();
            checkCollision(matchingFunctionList, matchingFunctionDic);
            
            if (Agentq.xCoordList.Count<2)
            {
                foreach (int confIndex in negativeMatchingIndexList)
                {
                    tempAff = MatchingFunctions.affinityFunction(//comparator agents);
                    affinityValueList.Add(tempAff);
                    affinityValueDic.Add(tempAff, confIndex);
                }
                affinityValueList.Sort();
                checkCollision(affinityValueList, affinityValueDic);
            }
        }
        matchingFunctionList.Clear;
        
    }

    private static void checkCollision(List<float> valueList, Dictionary<float, int> indexDic)
    {
        int i = matchingFunctionList.Count - 1;
        while (i >= 0)
        {
            matchingFunctionDic.TryGetValue(matchingFunctionList[i], out confIndex);
            exampleSubAgent = exampleConfigurations[confIndex].subAgent;
            foreach (KeyValuePair<int, Agent> agentk in activeAgentTable)
            {
                if (!(agentk.Key == agentNumber))
                {
                    agentkCopy = agentk.Value;
                    fillConfig(agentkCopy)

                        agentqCopy = activeAgentTable[agentNumber];
                    fillConfig(agentqCopy);

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
        return direction;
    }


    // Låter assigntrajectory ske i Agents updatefunktion istället!
    /*foreach (Agent agent in agentArray)
    {
        assignTrajectory(agent);
    }
}*/
}
