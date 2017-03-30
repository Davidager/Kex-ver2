using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Xml;


public class CreateSimulation {
    

    private Dictionary<int, float> affinityTable;
    private float matchingCUTOFF = new float{};
    private static Dictionary<int, Agent> activeAgentTable;
    private Configuration[] exampleConfigurations;
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

        if (agent.hasTrajectory())
        {
            if (agent.getUpdateCounter() > 14)
            {
                currentQueryConfiguration = createQueryConf(agentNumber);
                float matchingValue = MatchingFunctions.matchingFunction
                    (currentQueryConfiguration, agent.lastConfiguration);
                agent.lastConfiguration = currentQueryConfiguration;
                // use matching function to compare current configuration with the configuration when the last trajectory was assigned
                if (matchingValue < matchingCUTOFF)
                {
                    updateTrajectory(agent);
                }
            }
        }else
        {
            updateTrajectory(agent);
        }
    }

    private static void updateTrajectory(Agent Agentj){

        Agentj.calculateInfluences;
        int i = 0;
        int j = 0;
        while (i < Agentj.influenceList.length())
        {
            if (Agentj.influenceList(i) > influenceCUTOFF)
            {
                j++;
            }
            i++;
        }
        if (j == 0)
        {
            //give agent trajectory straight forward for 40 frames
        }else
        {
            // use matching function to compare current configuration to the configuration of all examples
            int i = 0;
            foreach (example in ReadDatabase.exampleContainer)
            {

            }
            //empty Agentj.positionList
            //create trajectoryList and add the trajectories for all examples with positive matching function
            //sort trajectoryList by decending matching values
            i = 0;
            while (i < trajectoryList.length())
            {
                if (trajectoryList(i) will lead to collision){
                    i++
                }else{
                    Agentj.trajectory = trajectoryList(i);
                    i = trajectoryList.length();
                }
            }
            if (Agentj.trajectory.isEmpty())
            {
                foreach (exampletrajectory in database)
                {
                    affinityValueList.add(exampletrajectory and affintyvalue);
                    //sort affintyValueList by decending affinty value 
                }
                i = 0;
                while (i < affinityValueList.length())
                {
                    if (trajectory i doesnt lead to collision){
                        Agentj.trajectory = trajectory i;
                        i = affinityValueList.length(); 
                    }
                    i++
                }
            }
        }

    }

    public float matchingFunction(example,j)
    {
        float simval = 
        return sim;
    }

    public float affinityFunction()
    {

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
