using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ReadText : MonoBehaviour {
    public TextAsset textAsset;
    private string textContents;
    private StringReader myStringReader;
    private int numberOfSplines;
    public int frameCounter;
    private Agent[] splineArray;


    public Transform CylinderPre;

    // Use this for initialization
    void Start ()
    {
        textAsset = (TextAsset)Resources.Load("crowds_zara01_copy");
        textContents = textAsset.text;
        frameCounter = 1;
        myStringReader = new StringReader(textContents);
        string firstLine = myStringReader.ReadLine();
        string[] firstLineArray = firstLine.Split(' ');

        //Ha med try?
        numberOfSplines = Int32.Parse(firstLineArray[0]);
        
        splineArray = new Agent[numberOfSplines];
        
        for (int j = 0; j < numberOfSplines; j++) {
            string controlPointLine = myStringReader.ReadLine();
            string[] controlPointArray = controlPointLine.Split(' ');
            int numOfControlPoints = Decimal.ToInt32(Decimal.Parse(controlPointArray[0]));
            float[] coordArray = new float[4 * numOfControlPoints];  //coordArray are to be sent to the Spline object (includes coords and frame number and looking direction)
            int cccounter = 0;
            for (int i = 0; i < 4 * numOfControlPoints; i = i + 4)
            {   
               
                string infoLine = myStringReader.ReadLine();  //infoline are the line with coord-info and frame number
                string[] infoStringArray = infoLine.Split(' ');
                // divided by 100 to fit the ground plane
                coordArray[i] = (float)Decimal.Parse(infoStringArray[0])/100;  // x-coord
                coordArray[i + 1] =(float)Decimal.Parse(infoStringArray[1])/100;  // z-coord

                int frameNumber = Decimal.ToInt32(Decimal.Parse(infoStringArray[2]));
                coordArray[i + 2] = frameNumber;
                float lookingDirection = (float)Decimal.Parse(infoStringArray[3]);
                coordArray[i + 3] = lookingDirection;
                cccounter++;
            }
            splineArray[j] = (Agent)GameObject.Find("Ground").AddComponent(typeof(Agent));
            //splineArray[j].setCoordArray(coordArray);
        }      

    }
	
	// Update is called once per frame
	void Update ()
    {
    }
}
