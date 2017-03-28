using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ReadDatabase.readDatabase();
        Debug.Log("End of code; finished");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
