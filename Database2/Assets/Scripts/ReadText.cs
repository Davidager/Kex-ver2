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
    public StringBuilder sb;
    public Transform CylinderPre;
    public ReadText()
    {
        readFromFile("crowds_zara01_copy.txt", 9014);
        Debug.Log(SaveData.exampleContainer.examples.Count);
        readFromFile("crowds_zara02_copy.txt", 10518);
        Debug.Log(SaveData.exampleContainer.examples.Count);
        readFromFile("crowds_zara03_copy.txt", 7526);
        Debug.Log(SaveData.exampleContainer.examples.Count);


        string desktopPath = Environment.GetFolderPath(
                         System.Environment.SpecialFolder.DesktopDirectory);

        //string path = desktopPath + @"\MyTest.txt";
        //File.WriteAllText(path, sb.ToString());
        
        string xmlPath = desktopPath + @"\database2.proto"; ;
        Configuration[] exampleConfigurations = createExampleConfigurations(SaveData.exampleContainer);
        Debug.Log(exampleConfigurations.Length);
        Configuration[] noInfExampleConfigurations = createExampleConfigurations(SaveData.noInfExamplecontainer);
        
        Array.Sort(noInfExampleConfigurations);
        
        //Debug.Log(noInfExampleConfigurations.Length);
        DatabaseWrapper databaseWrapper = new DatabaseWrapper();
        databaseWrapper.exampleConfigurations = exampleConfigurations;
        databaseWrapper.noInfExampleConfigurations = noInfExampleConfigurations;
        databaseWrapper.sortedExampleConfigurations = setUpSortedExampleConfigurations(exampleConfigurations);
        SaveData.save(xmlPath, databaseWrapper);
    }

    // för att skapa en sorts sortering av configs, listan har listor med configs som ska vara lika varandra. 
    private List<BucketWrapper> setUpSortedExampleConfigurations(Configuration[] exampleConfigurations)
    {
        List<List<Configuration>> outerList = new List<List<Configuration>>();
        System.Random rnd = new System.Random();
        int numberOfConfigs = exampleConfigurations.Length;
        List<int> randomNumberList = new List<int>();

        while (randomNumberList.Count < 30)
        {
            int randomNum = rnd.Next(numberOfConfigs);
            if (!randomNumberList.Contains(randomNum))
            {
                randomNumberList.Add(randomNum);
            }
        }

        for (int i = 0; i < 30; i++)
        {
            outerList.Add(new List<Configuration>());
            outerList[i].Add(exampleConfigurations[randomNumberList[i]]);
        }

        float maxVal;
        float currentVal;
        List<Configuration> insertList;
        for (int i = 0; i < numberOfConfigs; i++)
        {
            if (!randomNumberList.Contains(i))
            {
                maxVal = 0;
                insertList = new List<Configuration>();
                foreach (List<Configuration> innerList in outerList)
                {
                    currentVal = MatchingFunctions.matchingFunction(exampleConfigurations[i], innerList[0]);
                    if (currentVal > maxVal)
                    {
                        maxVal = currentVal;
                        insertList = innerList;
                    }
                }
                insertList.Add(exampleConfigurations[i]);

            }
        }
        foreach (List<Configuration> innerList in outerList)
        {
            Debug.Log(innerList.Count);
        }
        List<BucketWrapper> returnList = new List<BucketWrapper>();
        foreach (List<Configuration> bucket in outerList)
        {
            returnList.Add(new BucketWrapper(bucket));
        }
        return returnList;
    }

    private Configuration[] createExampleConfigurations(ExampleContainer exampleContainer)
    {
        Configuration[] returnArray = new Configuration[exampleContainer.examples.Count];
        int i = 0;
        foreach (ExampleData exampleData in exampleContainer.examples)
        {
            //raden under överflödig?
            returnArray[i] = new Configuration();
            returnArray[i].exampleNumber = i;
            ComparatorAgent subAgent = new ComparatorAgent();
            ComparatorAgent[] infAgentArray = new ComparatorAgent[exampleData.frames[0].jAgents.Count];
            float[] influenceValues = new float[infAgentArray.Length];
            /*int k = 0;
             är vi säkra på att influenceValues är i samma ordning som influenceagents i databasen?
            foreach (InfluenceValue infValue in exampleData.influenceValues)
            {
                influenceValues[k] = infValue.value;
                k++;
            }

            for (int temp = 0; temp < infAgentArray.Length; temp++)
            {
                infAgentArray[temp] = new ComparatorAgent();
            }
            */
            //7 raderna under istället för det ovan?
            int k = 0;
            foreach (InfluenceValue infValue in exampleData.influenceValues)
            {
                influenceValues[k] = infValue.value;
                infAgentArray[k] = new ComparatorAgent();
                k++;
            }
            foreach (FrameData frameData in exampleData.frames)
            {
                subAgent.addToTrajectory(frameData.subject.localPosition.x,
                    frameData.subject.localPosition.y, frameData.subject.speed,
                    frameData.subject.direction);
                int j = 0;
                foreach (AgentData jAgent in frameData.jAgents)
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
        Debug.Log("Alla exampleconfigs skapade");
        /*Debug.Log(returnArray.Length);
        foreach (Configuration config in returnArray)
        {
            Debug.Log(config.infAgentArray.Length);
        }*/
        return returnArray;
    }

    private void readFromFile(String fileString, int numberOfFrames)
    {
        string textContents;
        try
        {
            using (StreamReader sr = new StreamReader(fileString))
            {
                textContents = sr.ReadToEnd();

            }
        }
        catch (Exception e)
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
                coordArray[i] = ((float)Decimal.Parse(infoStringArray[0])) / 100;  // x-coord
                coordArray[i + 1] = ((float)Decimal.Parse(infoStringArray[1])) / 100;  // z-coord

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
        for (int i = 0; i < numberOfFrames + 1; i++) //9015
            nextFrame();
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

