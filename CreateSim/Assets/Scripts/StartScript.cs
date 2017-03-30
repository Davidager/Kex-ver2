using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //trams som behövs
        int[] arrayen = new int[10];
        int[] jKeys = new int[3];
        jKeys[0] = 1;
        jKeys[2] = 1;

        int hej = arrayen[jKeys[1]];
        Debug.Log(jKeys[1]);

        if (!ProtoBuf.Meta.RuntimeTypeModel.Default.IsDefined(typeof(Vector2)))
        {
            ProtoBuf.Meta.RuntimeTypeModel.Default.Add(typeof(Vector2), false).Add("x", "y");
        }
        new CreateSimulation(ReadDatabase.readDatabase());
        Debug.Log("End of code; finished");

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
