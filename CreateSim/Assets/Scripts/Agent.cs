using System.Collections;
using System.Collections.Generic;
using UnityEngine.Profiling;
using UnityEngine;

// Class for containing the positions of the cylinders
public class Agent : MonoBehaviour
{
    private float[] coordArray; //coordArray includes the framenumber for the pair of coords in the order x z framenumber
    private GameObject movingSpline;
    private Transform movingSplineTransform;
    //public GameObject CylinderPre;
    public GameObject HumanoidAgentNoRigid;
    //public GameObject Nose;
    //private GameObject nose;
    //private Transform noseTransform;
    private float noseRadius;
    private int agentNumber;



    public Configuration lastConfiguration;
    private CreateSimulation createSimulation;
    private static int spawnCounter = 0;

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
    void Awake ()
    {
        //CylinderPre = Resources.Load("CylinderPre") as GameObject;
        HumanoidAgentNoRigid = Resources.Load("HumanoidAgentNoRigid") as GameObject;
        //Nose = Resources.Load("Nose") as GameObject;
        noseRadius = 0.69f;
        yHeightCoord = 0.2f;

        xCoordList = new List<float>();
        zCoordList = new List<float>();
        speedList = new List<float>();
        directionList = new List<float>();
        setStartValues();
        startSpline();

        Time.fixedDeltaTime = 0.04f;
        updateCounter = 0;

    }

    void FixedUpdate ()
    {

        Profiler.BeginSample("assignTrajectory");
        //*Debug.Log("yo111");
        CreateSimulation.assignTrajectory(this, agentNumber);
        Profiler.EndSample();
        //*Debug.Log(xCoordList.Count);
        moveSpline();
        updateCounter++;
      
    }

    private void setStartValues()
    {
        Random random = new Random();

        /*xCoordList.Add(Random.Range(-3.5f, 3.5f));
        zCoordList.Add(Random.Range(-3f, 2.25f));*/
        
        

        
        speedList.Add(Random.Range(0.015f, 0.03f));
        //directionList.Add(Random.Range(0f, 2 * Mathf.PI));
        if (spawnCounter <= 2)
        {
            directionList.Add(0);
            xCoordList.Add(-3f);
            zCoordList.Add(-1.5f + (float)0.4*spawnCounter);
        } else
        {
            directionList.Add(Mathf.PI);
            xCoordList.Add(3f + (float)0.2*(spawnCounter-6f));
            zCoordList.Add(-1.5f + (float)0.4*spawnCounter - 3f);
        }
        spawnCounter++;
    }
    
    public void initialiseLists()
    {
        xCoordList = new List<float>();
        zCoordList = new List<float>();
        speedList = new List<float>();
        directionList = new List<float>();

    }

    public void setAgentNumber(int agentNumber)
    {
        this.agentNumber = agentNumber;
    }

    public int getAgentNumber()
    {
        return agentNumber;
    }

    public void setCreateSimulation(CreateSimulation createSimulation)
    {
        this.createSimulation = createSimulation;
    }

    public void addToTrajectory(float xCoord, float yCoord, float speed, float direction)
    {
        xCoordList.Add(xCoord);
        zCoordList.Add(yCoord);
        speedList.Add(speed);
        directionList.Add(direction);
        //if (xCoordList.Count > 40) Debug.Log(xCoordList.Count);
    }

    private void startSpline ()
    {
        /*float startx = Random.Range(-3.5f, 3.5f);
        float startz = Random.Range(-3f, 2.25f);
        */

        float startx = xCoordList[0];
        float startz = zCoordList[0];

        Vector3 startPosition = new Vector3(startx, yHeightCoord, startz);
        //movingSpline = Instantiate(CylinderPre, startPosition, Quaternion.identity);
        movingSpline = Instantiate(HumanoidAgentNoRigid, startPosition, Quaternion.identity);
        movingSplineTransform = movingSpline.transform;

        /*nose = Instantiate(Nose, Vector3.zero, Quaternion.identity);
        noseTransform = nose.transform;
        noseTransform.parent = movingSplineTransform;*/

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

    private void moveSpline()
    {
        changeLookingDirection(directionList[0]);
        //Debug.Log(directionList[0] + "spline; " + agentNumber);
        Vector3 targetPos = new Vector3(xCoordList[1], yHeightCoord, zCoordList[1]);
        movingSplineTransform.position = Vector3.MoveTowards(movingSplineTransform.position, targetPos, speedList[0]);// speed);

        // om utanför viss gräns; remove agent!   
        /*if (movingSplineTransform.position.x > 3.5f || movingSplineTransform.position.x < -3.5f 
            || movingSplineTransform.position.z < -3f || movingSplineTransform.position.z > 2.25f)
        {
            createSimulation.removeFromActiveAgentTable(agentNumber);
            Destroy(movingSpline);
            Destroy(this);
        }*/

        // Detta kan vara mycket tidskrävande!!!!!
        xCoordList.RemoveAt(0);
        zCoordList.RemoveAt(0);
        speedList.RemoveAt(0);
        directionList.RemoveAt(0);
    }

    public int getUpdateCounter()
    {
        return updateCounter;
    }

    private void changeLookingDirection (float direction)
    {

        /*float xCoord = -noseRadius * Mathf.Sin(direction - Mathf.PI/2);
        float zCoord = noseRadius * Mathf.Cos(direction - Mathf.PI / 2);*/
        float xCoord = -1 * Mathf.Sin(direction - Mathf.PI/2);
        float zCoord =  Mathf.Cos(direction - Mathf.PI / 2);
        //Debug.Log("x: " + xCoord + " z: " + zCoord + " number:" + agentNumber);
        //noseTransform.localPosition = new Vector3(xCoord, 0.784f, zCoord);
        movingSplineTransform.rotation = Quaternion.LookRotation(new Vector3(xCoord, 0, zCoord));
    }


    
}

