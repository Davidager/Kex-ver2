using UnityEngine;
using System.Collections;

// An FPS counter.
// It calculates frames/second over each updateInterval,
// so the display does not keep changing wildly.
public class FPSCounter : MonoBehaviour
{
    public float updateInterval = 0.5F;
    private double lastInterval;
    private int frames = 0;
    private float fps;
    private float fps2;
    private float lowestFPS2 = 60;
    private float avg = 0f;
    int qty = 0;
    float currentAvgFPS = 0;
    private float lowestFPS = 60;
    private bool first = true;
    private int counter = 0;

    void Start()
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
    }

    void OnGUI()
    {
        GUILayout.Label("" + lowestFPS.ToString("f2"));
        float displayValue = (1F / avg); //display this value
        GUILayout.Label("" + currentAvgFPS.ToString("f2"));
        GUILayout.Label("" + lowestFPS2.ToString("f2"));


    }

    void Update()
    {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval)
        {
            fps2 = (float)(frames / (timeNow - lastInterval));
            frames = 0;
            lastInterval = timeNow;

        }
        
        if (counter > 80) 
        {
            if (fps2 < lowestFPS2)
            {
                lowestFPS2 = fps2;
            }
            UpdateCumulativeMovingAverageFPS(fps2);
        }
           
        
        avg += ((Time.deltaTime / Time.timeScale) - avg) * 0.03f; //run this every frame
        
        fps = 1 / Time.deltaTime;
        if (fps < lowestFPS)
        {
            lowestFPS = fps;
        }
        
        counter++;
    }

    float UpdateCumulativeMovingAverageFPS(float newFPS)
    {
        ++qty;
        currentAvgFPS += (newFPS - currentAvgFPS) / qty;

        return currentAvgFPS;
    }
}