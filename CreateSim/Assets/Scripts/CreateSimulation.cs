using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Xml;


public class CreateSimulation : MonoBehaviour{
    /*

    private Dictionary<int, float> affinityTable;
    private float matchingCUTOFF = new float{};
    private Dictionary<int, Agent> activeAgentTable;

    void Start()
    {
        createExampleConfigurations();
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
                float matchingValue = matchingFunction(agentArray, subjectIdentifier);
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

    private void create

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

	void Update()
    {
        // Låter assigntrajectory ske i Agents updatefunktion istället!
        /*foreach (Agent agent in agentArray)
        {
            assignTrajectory(agent);
        }
    }*/
}
