using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Configuration {
    public ComparatorAgent[] infAgentArray;
    public ComparatorAgent subAgent;
    public float[] influenceValues;
    public Vector2 newOrigin;
    public float originDirection;
    private Dictionary<int, float> maxInfluenceTable;
    private static float CUTOFF = 0.1f;
    private ComparatorAgent subAgentCopy;

    public Configuration()
    {

    }

    private void calculateInfluences()
    {
        maxInfluenceTable = new Dictionary<int, float>();
        for (int j = 0; j < infAgentArray.Length; j++)
        {
            for (int i = 0; i < 40; i++)
            {

                float distance = (float)Math.Sqrt((infAgentArray[j].xCoordList[i] - subAgentCopy.xCoordList[i]) *
                    (infAgentArray[j].xCoordList[i] - subAgentCopy.xCoordList[i]) +
                    (infAgentArray[j].zCoordList[i] - subAgentCopy.zCoordList[i]) *
                    (infAgentArray[j].zCoordList[i] - subAgentCopy.zCoordList[i]));
                float influencePj;
                influencePj = (float)Math.Exp(-subAgentCopy.speedList[i] * Math.Pow(distance, 2) / 2);
                float infFactor;
                if (jInFrontofi(infAgentArray[j].xCoordList[i], infAgentArray[j].zCoordList[i], i))
                {
                    infFactor = (float)Math.Exp(-1 * Math.Pow(distance, 2) / (2 * subAgentCopy.speedList[i]));
                }
                else
                {
                    infFactor = (float)Math.Exp(-subAgentCopy.speedList[i] * Math.Pow(distance, 2));
                }

                float infOut = influencePj * infFactor;
                if (!maxInfluenceTable.ContainsKey(j))
                {
                    maxInfluenceTable.Add(j, infOut);
                }
                else
                {
                    if (maxInfluenceTable[j] < infOut)
                    {
                        maxInfluenceTable[j] = infOut;
                    }
                }
            }
        }
        List<int> keyRemoveList = new List<int>();
        foreach (KeyValuePair<int, float> e in maxInfluenceTable)
        {
            if (maxInfluenceTable[e.Key] < CUTOFF)  // kolla storleksordning så CUTOFF är rätt!
            {
                keyRemoveList.Add(e.Key);
            }
        }
        foreach (int key in keyRemoveList)
        {
            maxInfluenceTable.Remove(key);
        }

        float maxInfSum = 0;
        foreach (KeyValuePair<int, float> e in maxInfluenceTable)
        {
            maxInfSum += e.Value;
        }

        Dictionary<int, float> finalInfluencetable = new Dictionary<int, float>();
        foreach (KeyValuePair<int, float> e in maxInfluenceTable)
        {
            finalInfluencetable.Add(e.Key, e.Value / maxInfSum);
        }

        ComparatorAgent[] newInfAgentArray = new ComparatorAgent[finalInfluencetable.Count];
        influenceValues = new float[finalInfluencetable.Count];
        int temp = 0;
        foreach (KeyValuePair<int, float> e in finalInfluencetable)
        {
            newInfAgentArray[temp] = infAgentArray[e.Key];
            influenceValues[temp] = e.Value;
            temp++;
        }
        infAgentArray = newInfAgentArray;
    }

    private Boolean jInFrontofi(float jxCoord, float jzCoord, int i)
    {
        Vector2 retvec = new Vector2(jxCoord - subAgentCopy.xCoordList[i], jzCoord - subAgentCopy.zCoordList[i]);
        //float retvecDirection = Spline.calculateDirection(retvec.x, retvec.y);
        Vector2 iVec = new Vector2(Mathf.Cos(subAgentCopy.directionList[i]), Mathf.Sin(subAgentCopy.directionList[i]));
        //retvec = Quaternion.Euler(new Vector3(0, 0, (iDirection - (Mathf.PI / 2)) * 180 / Mathf.PI)) * retvec;

        if (Vector2.Dot(retvec, iVec) < 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void fillConfig(ComparatorAgent agent)
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

    public void fillAndCalcInfluences()
    {
        //fyll subject
        
        foreach (ComparatorAgent agent in infAgentArray)
        {
            fillConfig(agent);
            
        }
        subAgentCopy = subAgent;
        fillConfig(subAgentCopy);
        
        calculateInfluences();

        
    }
    


    


}
