using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComparatorAgent {
    public List<float> xCoordList;
    public List<float> zCoordList;
    public List<float> speedList;
    public List<float> directionList;


    public ComparatorAgent()
    {
        xCoordList = new List<float>();
        zCoordList = new List<float>();
        speedList = new List<float>();
        directionList = new List<float>();
    }

    public void addToTrajectory(float xCoord, float yCoord, float speed, float direction)
    {
        xCoordList.Add(xCoord);
        zCoordList.Add(yCoord);
        speedList.Add(speed);
        directionList.Add(direction);
    }
}
