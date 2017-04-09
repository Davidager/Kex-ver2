using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        UnityEngine.Debug.Log("End of code; finished");
        Time.fixedDeltaTime = 0.04f;
        /*float gauss1 = new float { };
        float gauss2 = new float { };
        float gauss3 = new float { };
        float simVal = new float { };


        float xCoord1 = UnityEngine.Random.Range(-3.5f, 3.5f);
        float yCoord1 = UnityEngine.Random.Range(-3f, 2.25f);
        float direction1 = UnityEngine.Random.Range(0f, 2 * Mathf.PI);
        float speed1 = UnityEngine.Random.Range(0.015f, 0.03f);

        float xCoord2 = UnityEngine.Random.Range(-3.5f, 3.5f);
        float yCoord2 = UnityEngine.Random.Range(-3f, 2.25f);
        float direction2 = UnityEngine.Random.Range(0f, 2 * Mathf.PI);
        float speed2 = UnityEngine.Random.Range(0.015f, 0.03f);

        float speed3 = UnityEngine.Random.Range(0.015f, 0.03f);

        Stopwatch sw = Stopwatch.StartNew();
        for (int i = 0; i < 400000; i++)
        {
            
            gauss1 = fastExp(-(myPowSquared(xCoord1 - xCoord2)
                + myPowSquared(yCoord1 - yCoord2)) / (1 / speed3));
            gauss2 = fastExp(-(myPowSquared(speed1 - speed2)
                / (1 / speed3)));
            gauss3 = fastExp(-(myPowSquared(direction1 - direction2)
                / (1 / speed3)));
            simVal = gauss1 * gauss2 * gauss3;
        }
        sw.Stop();
        UnityEngine.Debug.Log("Time taken: " + sw.Elapsed.TotalMilliseconds);
        UnityEngine.Debug.Log(gauss1);
        UnityEngine.Debug.Log(gauss2);
        UnityEngine.Debug.Log(gauss3);
        UnityEngine.Debug.Log(simVal);

        sw = Stopwatch.StartNew();
        for (int i = 0; i < 400000; i++)
        {

            gauss1 = Mathf.Exp(-(myPowSquared(xCoord1 - xCoord2)
                + myPowSquared(yCoord1 - yCoord2)) / (1 / speed3));
            gauss2 = Mathf.Exp(-(myPowSquared(speed1 - speed2)
                / (1 / speed3)));
            gauss3 = Mathf.Exp(-(myPowSquared(direction1 - direction2)
                / (1 / speed3)));
            simVal = gauss1 * gauss2 * gauss3;
        }
        sw.Stop();
        UnityEngine.Debug.Log("Time taken: " + sw.Elapsed.TotalMilliseconds);
        UnityEngine.Debug.Log(simVal);


        sw = Stopwatch.StartNew();
        for (int i = 0; i < 400000; i++)
        {

            gauss1 = Mathf.Exp(-(myPowSquared(xCoord1 - xCoord2)
                + myPowSquared(yCoord1 - yCoord2)) / (1 / speed3));
            gauss2 = Mathf.Exp(-(myPowSquared(speed1 - speed2)
                / (1 / speed3)));
            gauss3 = Mathf.Exp(-(myPowSquared(direction1 - direction2)
                / (1 / speed3)));
        }
        sw.Stop();
        UnityEngine.Debug.Log("Time taken: " + sw.Elapsed.TotalMilliseconds);
        
        sw = Stopwatch.StartNew();
        for (int i = 0; i < 400000; i++)
        {

            gauss1 = Mathf.Exp(-(myPowSquared(xCoord1 - xCoord2)
                + myPowSquared(yCoord1 - yCoord2) + myPowSquared(speed1 - speed2) 
                + myPowSquared(direction1 - direction2)) / (1 / speed3));
            
        }
        sw.Stop();
        UnityEngine.Debug.Log("Time taken: " + sw.Elapsed.TotalMilliseconds);
        UnityEngine.Debug.Log(gauss1);

        sw = Stopwatch.StartNew();
        for (int i = 0; i < 400000; i++)
        {

            gauss1 = Mathf.Exp(-((xCoord1 - xCoord2)* (xCoord1 - xCoord2)
                + (yCoord1 - yCoord2)* (yCoord1 - yCoord2) + (speed1 - speed2)* (speed1 - speed2)
                + (direction1 - direction2)* (direction1 - direction2)) * (speed3));

        }
        sw.Stop();
        UnityEngine.Debug.Log("Time taken: " + sw.Elapsed.TotalMilliseconds);
        UnityEngine.Debug.Log(gauss1);

        sw = Stopwatch.StartNew();
        for (int i = 0; i < 400000; i++)
        {

            gauss1 = fastExp(-((xCoord1 - xCoord2) * (xCoord1 - xCoord2)
                + (yCoord1 - yCoord2) * (yCoord1 - yCoord2) + (speed1 - speed2) * (speed1 - speed2)
                + (direction1 - direction2) * (direction1 - direction2)) * (speed3));

        }
        sw.Stop();
        UnityEngine.Debug.Log("Time taken: " + sw.Elapsed.TotalMilliseconds);
        UnityEngine.Debug.Log(gauss1);*/
    }

    public static float fastExp(float val)
    {

        long tmp = (long)(1512775 * (double)val + (1072693248 - 60801));
        return (float)BitConverter.Int64BitsToDouble(tmp << 32);
    }

    private static float myPowSquared(float x)
    {
        return x * x;
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
