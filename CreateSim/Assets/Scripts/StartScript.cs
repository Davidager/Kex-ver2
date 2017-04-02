using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //trams som behövs

        //List<int> list = new List<int>();
        //list[4] = 2;


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
