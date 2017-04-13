using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAgent : MonoBehaviour {
    public static bool first = true;
    private float xCoord;
    private float zCoord = 0f;
    private float yHeightCoord = 0.2f;


    private GameObject movingSpline;
    private Transform movingSplineTransform;
    public GameObject CylinderPre;
   
    void Start () {
        if (first) xCoord = 0f;
        else xCoord = 0.2f;
        CylinderPre = Resources.Load("CylinderPre") as GameObject;

        Vector3 startPosition = new Vector3(xCoord, yHeightCoord, zCoord);
        movingSpline = Instantiate(CylinderPre, startPosition, Quaternion.identity);




        first = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
