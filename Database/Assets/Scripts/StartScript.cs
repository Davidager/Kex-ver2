using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ProtoBuf;

public class StartScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (!ProtoBuf.Meta.RuntimeTypeModel.Default.IsDefined(typeof(Vector2)))
        {
            ProtoBuf.Meta.RuntimeTypeModel.Default.Add(typeof(Vector2), false).Add("x", "y");
        }
        new ReadText();
        Debug.Log("End of code; finished");
        // Kod för att testa att läsa:
        ExampleContainer exampleContainer = Serializer.Deserialize<ExampleContainer>(
            new FileStream(@"C:\Users\David\Documents\GitHub\Kex\DatabaseTest2\xmlTest.proto", FileMode.Open, FileAccess.Read));
        Debug.Log(exampleContainer.examples[10].frames[38].subject.localPosition.y);
        Debug.Log(exampleContainer.examples.Count);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
