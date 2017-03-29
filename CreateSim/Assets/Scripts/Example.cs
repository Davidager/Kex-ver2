using System.Collections;
using System.Collections.Generic;
using UnityEngine;   // tömma agentlistor osv
using System;
using System.Xml.Serialization;
using ProtoBuf;

public class Example {
    private float[] subjPosArray;
    private int influences;
    private float iSpeed;
    private float iDirection;
    private static float CUTOFF = 0.1f;

    private float influenceSum;
    private float maxInfluence;
    public Spline exampleSpline;
    private List<float> jPosList;
    private List<float> jSpeed;
    private List<float> jDirection;
    private List<int> jSplineNumber;
    private Vector2 newOrigin;
    private Dictionary<int, float> maxInfluenceTable;

    private float currentixCoord;
    private float currentizCoord;

    public ExampleData data = new ExampleData();
    private Transform originTransform;
    private float originDirection;

    public Example(int exampleNumber)
    {
        data.exampleNumber = exampleNumber;
        subjPosArray = new float[40];
        influences = new int();
        jPosList = new List<float>(20);   // i snitt 10 jAgents
        jSpeed = new List<float>();
        jDirection = new List<float>();
        jSplineNumber = new List<int>();
        maxInfluenceTable = new Dictionary<int, float>();
        
        maxInfluence = 0;
        influenceSum = 0;
        

    }

    private void calculateInfluences(float jxCoord, float jzCoord, int splineNumber)
    {
        float distance = (float)Math.Sqrt((jxCoord - currentixCoord) * (jxCoord - currentixCoord) +
            (jzCoord - currentizCoord) * (jzCoord - currentizCoord));
        float influencePj;
        influencePj = (float)Math.Exp(-iSpeed * Math.Pow(distance, 2) / 2);
        float infFactor;
        if (jInFrontofi(jxCoord, jzCoord))
        {
            infFactor = (float)Math.Exp(-1 * Math.Pow(distance, 2) / (2 * iSpeed));
        }
        else
        {
            infFactor = (float)Math.Exp(-iSpeed * Math.Pow(distance, 2));
        }

        float infOut = influencePj * infFactor;
        if (!maxInfluenceTable.ContainsKey(splineNumber))
        {
            maxInfluenceTable.Add(splineNumber, infOut);
        }
        else
        {
            if (maxInfluenceTable[splineNumber] < infOut)
            {
                maxInfluenceTable[splineNumber] = infOut;
            }
        }        
    }   

    private Boolean jInFrontofi(float jxCoord, float jzCoord)
    {
        Vector2 retvec = new Vector2(jxCoord - currentixCoord, jzCoord - currentizCoord);
        //float retvecDirection = Spline.calculateDirection(retvec.x, retvec.y);
        Vector2 iVec = new Vector2(Mathf.Cos(iDirection), Mathf.Sin(iDirection));
        //retvec = Quaternion.Euler(new Vector3(0, 0, (iDirection - (Mathf.PI / 2)) * 180 / Mathf.PI)) * retvec;

        if (Vector2.Dot(retvec, iVec) < 0 ) 
        {
            return false;
        } else
        {
            return true;
        }
    }

    public void savejInformation(Spline jSpline)
    {   
        
        Transform splineTransformj = jSpline.movingSplineTransform;
        jPosList.Add(splineTransformj.position.x);
        jPosList.Add(splineTransformj.position.z);

        jSpeed.Add(jSpline.speed);
        jDirection.Add(jSpline.movingDirection);
        jSplineNumber.Add(jSpline.splineNumber);

        calculateInfluences(splineTransformj.position.x, splineTransformj.position.z, jSpline.splineNumber);
    }
    
    public void saveiInformation(Spline iSpline)
    {
        
        Transform splineTransformi = iSpline.movingSplineTransform;
        this.currentixCoord = splineTransformi.position.x;
        this.currentizCoord = splineTransformi.position.z;

        iSpeed = iSpline.speed;
        iDirection = iSpline.movingDirection;
        
        if (newOrigin == default(Vector2))
        {
            exampleSpline = iSpline;
            newOrigin = new Vector2(currentixCoord, currentizCoord);
            originDirection = iDirection;
        }
    }

    private Vector2 globalToLocalVector2 (Vector2 position)
    {
        Vector2 retvec = new Vector2(position.x - newOrigin.x, position.y - newOrigin.y);
        retvec = Quaternion.Euler(new Vector3(0, 0, (originDirection - (Mathf.PI / 2)) * 180 / Mathf.PI))*retvec;
        return retvec;
    }

    private float globalToLocalDirection (float direction)
    {
        direction = direction - originDirection + Mathf.PI / 2;
        if (direction < 0) direction = direction + 2 * Mathf.PI;
        return direction;
    }

    public void storeData(int frameNumber)
    {
        FrameData frameData = new FrameData();
        frameData.frameNumber = frameNumber;
        frameData.subject = new agentData();
        frameData.subject.agentNumber = exampleSpline.splineNumber;
        frameData.subject.localPosition = globalToLocalVector2
            (new Vector2(currentixCoord, currentizCoord));
        frameData.subject.direction = globalToLocalDirection(iDirection);
        frameData.subject.speed = iSpeed;

        for (int i = 0; i < jPosList.Count; i = i + 2)
        {
            agentData newjAgent = new agentData();
            newjAgent.agentNumber = jSplineNumber[i / 2];
            newjAgent.localPosition = globalToLocalVector2
                (new Vector2(jPosList[i], jPosList[i + 1]));
            newjAgent.direction = globalToLocalDirection(jDirection[i / 2]);
            newjAgent.speed = jSpeed[i / 2];

            frameData.jAgents.Add(newjAgent); 
        }

        jSplineNumber.Clear();
        jSpeed.Clear();
        jDirection.Clear();
        jPosList.Clear();



        data.frames.Add(frameData);
    }

    public void endCurrentExample()
    {
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

            //lägger in influencefunction i ouppdaterade data, egentligen onödigt, kan nog kommenteras bort!
            /*XmlInfluenceFunction newInfFunc = new XmlInfluenceFunction();
            newInfFunc.jAgentIndex = e.Key;
            newInfFunc.value = finalInfluencetable[e.Key];         
            
            data.influenceFunctions.Add(newInfFunc);*/

        }

        // skapar en ny ExampleData där endast agents med influence över CUTOFF får vara med.
        ExampleData updatedData = new ExampleData();
        updatedData.exampleNumber = data.exampleNumber;
        foreach (FrameData framedata in data.frames)
        {
            FrameData updatedFrameData = new FrameData();
            updatedFrameData.frameNumber = framedata.frameNumber;
            updatedFrameData.subject = framedata.subject;
            foreach (agentData jAgent in framedata.jAgents)
            {
                if (maxInfluenceTable.ContainsKey(jAgent.agentNumber))
                {
                    updatedFrameData.jAgents.Add(jAgent);
                }
            }
            updatedData.frames.Add(updatedFrameData);
        }

        foreach (KeyValuePair<int, float> e in maxInfluenceTable)
        {
            InfluenceValue newInfFunc = new InfluenceValue();
            newInfFunc.jAgentNumber = e.Key;
            newInfFunc.value = finalInfluencetable[e.Key];

            updatedData.influenceValues.Add(newInfFunc);
        }

        //SaveData.addExampleData(data);        den gamla sparningen, som inte görs nu när vi har uppdaterat.
        if (!(updatedData.influenceValues.Count==0))
        {
            SaveData.addExampleData(updatedData);
        }
        
    }


}
[Serializable]
[ProtoContract]
public class ExampleData
{
    [ProtoMember(1)]
    public int exampleNumber;

    [ProtoMember(2)]
    public List<FrameData> frames = new List<FrameData>();

    [ProtoMember(3)]
    public List<InfluenceValue> influenceValues = new List<InfluenceValue>();
}

[Serializable]
[ProtoContract]
public class FrameData
{
    [ProtoMember(1)]
    public int frameNumber;

    [ProtoMember(2)]
    public agentData subject;

    [ProtoMember(3)]
    public List<agentData> jAgents = new List<agentData>();
}

[Serializable]
[ProtoContract]
public class InfluenceValue
{
    [ProtoMember(1)]
    public int jAgentNumber;

    [ProtoMember(2)]
    public float value;
}

[Serializable]
[ProtoContract]
public class agentData
{
    [ProtoMember(1)]
    public int agentNumber;

    [ProtoMember(2)]
    public Vector2 localPosition;

    /*[ProtoMember(2)]
    public float localX;

    [ProtoMember(3)]
    public float localZ;*/

    [ProtoMember(4)]
    public float direction;

    [ProtoMember(5)]
    public float speed;
}