using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;

public class ReadText {
    public TextAsset textAsset;
    private string textContents;
    private StringReader myStringReader;
    private int numberOfSplines;
    private Spline[] splineArray;
    public int frameCounter;
    public Dictionary<int, ArrayList> firstFrameTable;
    private Dictionary<int, ArrayList> lastFrameTable;
    private Dictionary<int, Spline> activeSplineTable;
    private Dictionary<int, Example> exampleTable;
    private ArrayList exampleInfluencesList;
    private int exampleCounter;
    public static ExampleContainer exampleContainer = new ExampleContainer();

    public StringBuilder sb;
    public Transform CylinderPre;
    public ReadText()
    {
        string textContents;
        try
        {
            using (StreamReader sr = new StreamReader("crowds_zara01_copy.txt"))
            {
                textContents = sr.ReadToEnd();
                               
            }
        } catch (Exception e)
        {
            Debug.Log("Reading file failed: " + e);
            return; 
            
            //return;
        }

        frameCounter = 0;
        exampleCounter = 1;
        //string textContents = sr.ReadToEnd();

        exampleInfluencesList = new ArrayList();
        exampleTable = new Dictionary<int, Example>();
        sb = new StringBuilder();
        myStringReader = new StringReader(textContents);
        string firstLine = myStringReader.ReadLine();
        string[] firstLineArray = firstLine.Split(' ');

        //Ha med try?
        numberOfSplines = Int32.Parse(firstLineArray[0]);

        splineArray = new Spline[numberOfSplines];
        firstFrameTable = new Dictionary<int, ArrayList>(80);
        lastFrameTable = new Dictionary<int, ArrayList>(100);
        activeSplineTable = new Dictionary<int, Spline>(10);
        int firstFrame;
        int lastFrame;
        for (int j = 0; j < numberOfSplines; j++)
        {
            string controlPointLine = myStringReader.ReadLine();
            string[] controlPointArray = controlPointLine.Split(' ');
            int numOfControlPoints = Decimal.ToInt32(Decimal.Parse(controlPointArray[0]));
            float[] coordArray = new float[4 * numOfControlPoints];  //coordArray are to be sent to the Spline object (includes coords and frame number and looking direction)
            int cccounter = 0;
            for (int i = 0; i < 4 * numOfControlPoints; i = i + 4)
            {

                string infoLine = myStringReader.ReadLine();  //infoline is the line with coord-info and frame number
                string[] infoStringArray = infoLine.Split(' ');
                // divided by 100 to fit the ground plane
                coordArray[i] = (float)Decimal.Parse(infoStringArray[0]) / 100;  // x-coord
                coordArray[i + 1] = (float)Decimal.Parse(infoStringArray[1]) / 100;  // z-coord

                int frameNumber = Decimal.ToInt32(Decimal.Parse(infoStringArray[2]));
                coordArray[i + 2] = frameNumber;
                float lookingDirection = (float)Decimal.Parse(infoStringArray[3]);
                coordArray[i + 3] = lookingDirection;
                cccounter++;
            }
            firstFrame = (int)coordArray[2];
            if (!firstFrameTable.ContainsKey(firstFrame))
            {
                firstFrameTable.Add(firstFrame, new ArrayList());
            } 
            firstFrameTable[firstFrame].Add(j);

            lastFrame = (int)coordArray[coordArray.Length - 2];
            if (!lastFrameTable.ContainsKey(lastFrame))
            {
                lastFrameTable.Add(lastFrame, new ArrayList());
            }
            lastFrameTable[lastFrame].Add(j);

            splineArray[j] = (Spline)new Spline(this, j);
            splineArray[j].setCoordArray(coordArray);
        }
        for(int i = 0; i < 9015; i++) //9015
        nextFrame();
        
        string path = @"C:\Users\David\Documents\GitHub\Kex\Database\MyTest.txt";
        File.WriteAllText(path, sb.ToString());
        
        string xmlPath = @"C:\Users\David\Documents\GitHub\Kex\Database\xmlTest.txt";
        SaveData.save(xmlPath, SaveData.exampleContainer);
    }

    private void nextFrame() 
    {
        if (firstFrameTable.ContainsKey(frameCounter))
        {
            foreach (int j in firstFrameTable[frameCounter])
            {
                activeSplineTable.Add(j, splineArray[j]);
            }
        }
        if (lastFrameTable.ContainsKey(frameCounter - 1))
        {
            foreach (int j in lastFrameTable[frameCounter - 1])
            {
                activeSplineTable.Remove(j);
            }
        }
        if (frameCounter%40 == 0)
        {
            exampleTable.Clear();
        }

        foreach (KeyValuePair<int, Spline> e in activeSplineTable)
        {
            e.Value.updateSpline();
            sb.AppendLine(frameCounter.ToString() + e.Value.movingSplineTransform.position.ToString("F4"));
            if(frameCounter%40 == 0 && e.Value.getLastFrame() >= 40 + frameCounter)
            {
                //exampleTable.Clear();
                // clear exampleTable??????`??`?      <----VIKTIGT
                exampleTable.Add(e.Key, new Example(exampleCounter));
                exampleCounter++;           
            }/*
            if (exampleTable.ContainsKey(e.Key))   // Saves the speed for this frame if there
            {                                               // exists an Example instance.
                //exampleTable[s.Key].exampleSpline = s.Value;
                exampleTable[e.Key].saveiInformation(e.Value);

                foreach (KeyValuePair<int, Example> s in exampleTable)
                {
                    if (s.Key == e.Key)   // if s is i   (the subject of an example)
                    {

                        // saves the speed for influence calculations
                    }
                    else     // if s is j   (an influencing agent of an example)
                    {
                        exampleTable[e.Key].savejInformation(splineArray[s.Key]);
                    }

                }

            }    */ 
        }

        foreach (KeyValuePair<int, Example> e in exampleTable)
        {
            exampleTable[e.Key].saveiInformation(splineArray[e.Key]);
            foreach (KeyValuePair<int, Example> s in exampleTable)
            {
                if(s.Key != e.Key)   //  if s is not i (the subject of an example)
                {
                    exampleTable[e.Key].savejInformation(splineArray[s.Key]);
                }
            }
        }

        storeFrameData();
        if (frameCounter % 40 == 39)
        {
            foreach (KeyValuePair<int, Example> e in exampleTable)
            {
                e.Value.endCurrentExample();
            }
        }
        frameCounter++;
    }

    private void storeFrameData()
    {
        foreach (KeyValuePair<int, Example> e in exampleTable)
        {
            e.Value.storeData(frameCounter);
        }
    }
  

    // Use this for initialization
    
	
	// Update is called once per frame
	void Update ()
    {
    }
}
