using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// Class for containing the positions of the cylinders
public class Agent : MonoBehaviour
{
    private float[] coordArray; //coordArray includes the framenumber for the pair of coords in the order x z framenumber
    private GameObject movingSpline;
    private Transform movingSplineTransform;
    public GameObject CylinderPre;
    public GameObject Nose;
    private GameObject nose;
    private Transform noseTransform;
    private float noseRadius;
    private int agentNumber;

    private Vector3 targetPos;

    public Configuration lastConfiguration;

    public List<float> xCoordList;
    public List<float> zCoordList;
    public List<float> speedList;
    public List<float> directionList;
    private int updateCounter;
    private float yHeightCoord;
    private int currentTargetFrame;
    private int controlPointNumber; // the control point which the spline is moving towards 
                                    // (starts with 1 (0 is the starting point))

   
    // Start is used in unity in place of the usual constructor
    void Start ()
    {
        CylinderPre = Resources.Load("CylinderPre") as GameObject;
        Nose = Resources.Load("Nose") as GameObject;
        noseRadius = 0.69f;
        yHeightCoord = 0.2f;

        xCoordList = new List<float>();
        zCoordList = new List<float>();
        speedList = new List<float>();
        directionList = new List<float>();
        setStartValues();
        startSpline();

        updateCounter = 0;

    }

    void Update ()
    {
        //ändrar targetframe 

        // ändrar riktningen till riktningen hos den framen vi precis kommit fram till 
        //changeLookingDirection(coordArray[controlPointNumber * 4 - 1]);
        // nästa frame fås genom att öka controlpointnumber

        // currentTargetFrame = (int)coordArray[(controlPointNumber * 4) + 2];
        // targetPos = new Vector3(coordArray[controlPointNumber * 4], yHeightCoord, coordArray[controlPointNumber * 4 + 1]);
        // controlPointNumber++;
        // float dist = Vector3.Distance(targetPos, movingSplineTransform.position);
        // speed = dist / ((currentTargetFrame - frameCounter));

        CreateSimulation.assignTrajectory(this, agentNumber);
        moveSpline();
        

        updateCounter++;
        /*if (frameCounter >= lastFrame)
        {
            Destroy(movingSpline);
            Destroy(this);
        }*/
    }

    private void setStartValues()
    {
        Random random = new Random();

        xCoordList.Add(Random.Range(-3.5f, 3.5f));
        zCoordList.Add(Random.Range(-3f, 2.25f));
        speedList.Add(Random.Range(0.015f, 0.03f));
        directionList.Add(Random.Range(0f, 2 * Mathf.PI));
    }

    public void setAgentNumber(int agentNumber)
    {
        this.agentNumber = agentNumber;
    }

    public int getAgentNumber()
    {
        return agentNumber;
    }

    public void addToTrajectory(float xCoord, float yCoord, float speed, float direction)
    {
        xCoordList.Add(xCoord);
        zCoordList.Add(yCoord);
        speedList.Add(speed);
        directionList.Add(direction);
    }

    private void startSpline ()
    {
        /*float startx = Random.Range(-3.5f, 3.5f);
        float startz = Random.Range(-3f, 2.25f);
        */

        float startx = xCoordList[0];
        float startz = zCoordList[0];

        Vector3 startPosition = new Vector3(startx, yHeightCoord, startz);
        movingSpline = Instantiate(CylinderPre, startPosition, Quaternion.identity);
        movingSplineTransform = movingSpline.transform;

        nose = Instantiate(Nose, Vector3.zero, Quaternion.identity);
        noseTransform = nose.transform;
        noseTransform.parent = movingSplineTransform;
        changeLookingDirection(directionList[0]);
        

        
    }

    public bool hasTrajectory()  // TODO: Ta bort ur linkedlists vid användning
    {
        if (xCoordList.Count > 1)   // TODO: Ändra till 1 troligtvis!!
        {
            return true;
        }

        return false;
    }

    private void moveSpline ()
    {
        movingSplineTransform.position = Vector3.MoveTowards(movingSplineTransform.position, targetPos, 0.05f);// speed);
        // TODO: om utanför viss gräns; remove agent!   
    }

    public int getUpdateCounter()
    {
        return updateCounter;
    }

    private void changeLookingDirection (float direction)
    {
        float xCoord = -noseRadius * Mathf.Sin(Mathf.Deg2Rad * direction);
        float zCoord = noseRadius * Mathf.Cos(Mathf.Deg2Rad * direction);
        noseTransform.localPosition = new Vector3(xCoord, 0.784f, zCoord);
    }


    
}

