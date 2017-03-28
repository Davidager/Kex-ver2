using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for containing the positions of the cylinders
public class Spline : MonoBehaviour
{
    private float[] coordArray; //coordArray includes the framenumber for the pair of coords in the order x z framenumber
    private int frameCounter;
    private GameObject movingSpline;
    private Transform movingSplineTransform;
    public GameObject CylinderPre;
    public GameObject Nose;
    private GameObject nose;
    private Transform noseTransform;
    private float noseRadius;

    private Vector3 targetPos;
    private float speed;
    private int lastFrame;
    private float yCoord;
    private int firstFrame;
    private int currentTargetFrame;
    private int controlPointNumber; // the control point which the spline is moving towards 
                                    // (starts with 1 (0 is the starting point))

   
    // Start is used in unity in place of the usual constructor
    void Start ()
    {
        frameCounter = 0;
        controlPointNumber = 1;
        CylinderPre = Resources.Load("CylinderPre") as GameObject;
        Nose = Resources.Load("Nose") as GameObject;
        noseRadius = 0.69f  ;
        yCoord = 0.2f;
       

    }

    void Update ()
    {
        if (frameCounter >= firstFrame)
        {
            if (frameCounter == firstFrame)
            {
                startSpline();
            }
            //ändrar targetframe 
            if (frameCounter == currentTargetFrame)
            {                
                // ändrar riktningen till riktningen hos den framen vi precis kommit fram till 
                changeLookingDirection(coordArray[controlPointNumber * 4 - 1]);
                // nästa frame fås genom att öka controlpointnumber
                
                currentTargetFrame = (int)coordArray[(controlPointNumber * 4) + 2];
                targetPos = new Vector3(coordArray[controlPointNumber * 4], yCoord, coordArray[controlPointNumber * 4 + 1]);
                controlPointNumber++;
                float dist = Vector3.Distance(targetPos, movingSplineTransform.position);
                speed = dist / ((currentTargetFrame - frameCounter));

                
            }
            moveSpline();
        }

        frameCounter++;
        if (frameCounter >= lastFrame)
        {
            Destroy(movingSpline);
            Destroy(this);
        }
    }

    public void setCoordArray(float[] coordArray)
    {
        controlPointNumber = 1;
        this.coordArray = coordArray;
        firstFrame = (int)coordArray[2];
        //currentTargetFrame = (int)coordArray[(controlPointNumber * 4) + 2];
        currentTargetFrame = firstFrame;
        lastFrame = (int)coordArray[coordArray.Length-2];
    }

    private void startSpline ()
    {
        float startx = coordArray[0];
        float startz = coordArray[1];
        Vector3 startPosition = new Vector3(startx, yCoord, startz);
        movingSpline = Instantiate(CylinderPre, startPosition, Quaternion.identity);
        movingSplineTransform = movingSpline.transform;

        nose = Instantiate(Nose, Vector3.zero, Quaternion.identity);
        noseTransform = nose.transform;
        noseTransform.parent = movingSplineTransform;
        changeLookingDirection(coordArray[3]);
        

        
    }

    private void moveSpline ()
    {   
        movingSplineTransform.position = Vector3.MoveTowards(movingSplineTransform.position, targetPos, speed);

    }

    private void changeLookingDirection (float direction)
    {
        float xCoord = -noseRadius * Mathf.Sin(Mathf.Deg2Rad * direction);
        float zCoord = noseRadius * Mathf.Cos(Mathf.Deg2Rad * direction);
        noseTransform.localPosition = new Vector3(xCoord, 0.784f, zCoord);
    }

}

