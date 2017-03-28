using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for containing the positions of the cylinders
public class Spline
{
    private float[] coordArray; //coordArray includes the framenumber for the pair of coords in the order x z framenumber
    //private int frameCounter;
    //private GameObject movingSpline;
    public Transform movingSplineTransform;
    public GameObject CylinderPre;

    private float currentx;
    private float currentz;
    private ReadText readText;
    private int lastFrame;

    private Vector3 targetPos;
    public float speed;
    public float movingDirection;
    public float lookingDirection;
    //private int lastFrame;
    private float yCoord;
    
    public int splineNumber;
    private int firstFrame;
    private int currentTargetFrame;
    private int controlPointNumber; // the control point which the spline is moving towards 
                                    // (starts with 1 (0 is the starting point))

   
    // Start is used in unity in place of the usual constructor
    public Spline (ReadText readText, int splineNumber)
    {
        //frameCounter = 0;
        controlPointNumber = 1;
        
        yCoord = 0.2f;

        this.readText = readText;
        this.splineNumber = splineNumber;
       

    }

    public void updateSpline ()
    {
        if (readText.frameCounter == lastFrame)
        {
            moveSpline();
        }
        else
        {
            if (readText.frameCounter == firstFrame)
            {
                startSpline();

                // ändrar riktningen till riktningen hos den framen vi precis kommit fram till 
                // nästa frame fås genom att öka controlpointnumber

                currentTargetFrame = (int)coordArray[(controlPointNumber * 4) + 2];
                targetPos = new Vector3(coordArray[controlPointNumber * 4], yCoord, coordArray[controlPointNumber * 4 + 1]);
                controlPointNumber++;
                float dist = Vector3.Distance(targetPos, movingSplineTransform.position);
                movingDirection = calculateDirection(targetPos.x - movingSplineTransform.position.x, targetPos.z - movingSplineTransform.position.z);

                speed = dist / ((currentTargetFrame - readText.frameCounter));

                //readText.sb.AppendLine(readText.frameCounter.ToString() + movingSplineTransform.position.ToString("F4"));
            }
            else
            {
                moveSpline();
                if (readText.frameCounter == currentTargetFrame)
                {
                    // ändrar riktningen till riktningen hos den framen vi precis kommit fram till
                    lookingDirection = coordArray[controlPointNumber * 4 - 1]; 
                    // nästa frame fås genom att öka controlpointnumber

                    currentTargetFrame = (int)coordArray[(controlPointNumber * 4) + 2];
                    targetPos = new Vector3(coordArray[controlPointNumber * 4], yCoord, coordArray[controlPointNumber * 4 + 1]);
                    controlPointNumber++;
                    float dist = Vector3.Distance(targetPos, movingSplineTransform.position);
                    movingDirection = calculateDirection(targetPos.x - movingSplineTransform.position.x, targetPos.z - movingSplineTransform.position.z);
                    speed = dist / ((currentTargetFrame - readText.frameCounter));


                }
                
            }
            
        } 
        //ändrar targetframe      
    }
    
    // beräknar vinkeln från x-axeln till vektorn, 0 < vinkeln < 2pi
    public static float calculateDirection(float x, float z)
    {
        float direction;
        if (x == 0)
        {
            if (z > 0)
            {
                direction = Mathf.PI / 2;
            } else if (z < 0)
            {
                direction = 3*Mathf.PI / 2;
            } else
            {
                direction = 0;
            }
        } else
        {
            direction = Mathf.Atan(z / x);
            if (x < 0)
            {
                direction = direction + Mathf.PI;
            }
            else if (z < 0)
            {
                direction = 2 * Mathf.PI + direction;
            }
        }
        return direction;
    }
  
    public void setCoordArray(float[] coordArray)
    {
        controlPointNumber = 1;
        this.coordArray = coordArray;
        firstFrame = (int)coordArray[2];
        lastFrame = (int)coordArray[coordArray.Length - 2];
        //currentTargetFrame = (int)coordArray[(controlPointNumber * 4) + 2];
        currentTargetFrame = firstFrame;
        //lastFrame = (int)coordArray[coordArray.Length-2];
    }

    private void startSpline ()
    {
        float startx = coordArray[0];
        float startz = coordArray[1];
        Vector3 startPosition = new Vector3(startx, yCoord, startz);
        //movingSpline = Instantiate(CylinderPre, startPosition, Quaternion.identity);
        movingSplineTransform = new GameObject().transform;
        movingSplineTransform.position = startPosition;
        lookingDirection = coordArray[3];
        //nose = Instantiate(Nose, Vector3.zero, Quaternion.identity);
                
    }

    private void moveSpline ()
    {   
        movingSplineTransform.position = Vector3.MoveTowards(movingSplineTransform.position, targetPos, speed);

    }

    public int getLastFrame()
    {
        return lastFrame;
    }

}

