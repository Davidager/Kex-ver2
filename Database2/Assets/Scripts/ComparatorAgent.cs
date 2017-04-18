using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;

[Serializable]
[ProtoContract]
public class ComparatorAgent {
    [ProtoMember(1)]
    public List<float> xCoordList;

    [ProtoMember(2)]
    public List<float> zCoordList;

    [ProtoMember(3)]
    public List<float> speedList;

    [ProtoMember(4)]
    public List<float> directionList;


    public ComparatorAgent()
    {
        xCoordList = new List<float>();
        zCoordList = new List<float>();
        speedList = new List<float>();
        directionList = new List<float>();
    }

    public ComparatorAgent(int size)
    {
        xCoordList = new List<float>(size);
        zCoordList = new List<float>(size);
        speedList = new List<float>(size);
        directionList = new List<float>(size);
    }

    public void addToTrajectory(float xCoord, float yCoord, float speed, float direction)
    {
        xCoordList.Add(xCoord);
        zCoordList.Add(yCoord);
        speedList.Add(speed);
        directionList.Add(direction);
    }
}
