using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //trams som behövs

        //List<int> list = new List<int>();
        //list[4] = 2;
        /*Vector2 newOrigin = new Vector2(-1.463964f, -1.784349f);
        float originDirection = Mathf.PI;
        Vector2 printVector = globalToLocalVector2(new Vector2(-0.4729152f, -0.8625353f), newOrigin, originDirection);
        Debug.Log(printVector.x);
        Debug.Log(printVector.y);
        printVector = localToGlobalVector2(printVector, newOrigin, originDirection);
        Debug.Log(printVector.x);
        Debug.Log(printVector.y);*/


        if (!ProtoBuf.Meta.RuntimeTypeModel.Default.IsDefined(typeof(Vector2)))
        {
            ProtoBuf.Meta.RuntimeTypeModel.Default.Add(typeof(Vector2), false).Add("x", "y");
        }
        new CreateSimulation(ReadDatabase.readDatabase());
        Debug.Log("End of code; finished");

    }

    private static Vector2 globalToLocalVector2(Vector2 position, Vector2 newOrigin, float originDirection)
    {
        Vector2 retvec = new Vector2(position.x - newOrigin.x, position.y - newOrigin.y);
        retvec = Quaternion.Euler(new Vector3(0, 0, (-originDirection + (Mathf.PI / 2)) * 180 / Mathf.PI)) * retvec;
        return retvec;
    }

    private static Vector2 localToGlobalVector2(Vector2 position, Vector2 newOrigin, float originDirection)
    {
        Vector2 retvec = Quaternion.Euler(new Vector3(0, 0, (originDirection - (Mathf.PI / 2)) * 180 / Mathf.PI)) * position;
        retvec = new Vector2(retvec.x + newOrigin.x, retvec.y + newOrigin.y);
        return retvec;
    }

    private static float localToGlobalDirection(float direction, float originDirection)
    {
        direction = direction + originDirection - Mathf.PI / 2;
        if (direction > 2 * Mathf.PI) direction = direction - 2 * Mathf.PI;
        if (direction < 0) direction = direction + 2 * Mathf.PI;
        return direction;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
