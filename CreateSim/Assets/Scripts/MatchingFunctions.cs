using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchingFunctions{

    Agent infFactork;
    Agent infFactorj;
    float AffValue;
    //kolla storleksordningen på cutoffen
    private static float affinityCUTOFF = 0.3f;
    private static int topInfKey;
    private float newXcoord;
    private float newZcoord;
    private float newSpeed;
    private float newDirection;
    private static float[] affinityValueList;
    private static float simVal;
    private static float gauss1;
    private static float gauss2;
    private static float gauss3;
    private static float sumInfk;
    private static float sumInfj;
    private static float Um;
    private static float speedGaussian;
    private static float affinitySum;
    private static float matchingValue;
    private static float topAffValue = 0;
    private static float affValue;
    private static Dictionary<int, float> topAffValues;
    //private static float[] TopAffValues;
    private static Dictionary<int, float> innerAffVals;
    private static Dictionary<int, Dictionary<int, float>> outerAffVals;
    private static Dictionary<int, int> compareCounter;
    //private static Dictionary<int, float> compareDic;
    //private static int[] jKeys;
    private static Dictionary<int, int> jKeys;
    private static List<int> jUnmatched;
    private static List<int> kUnmatched;
    private static List<float> xCoordListCopyk;
    private static List<float> zCoordListCopyk;
    private static List<float> speedListCopyk;
    private static List<float> directionListCopyk;
    private static List<float> xCoordListCopyj;
    private static List<float> zCoordListCopyj;
    private static List<float> speedListCopyj;
    private static List<float> directionListCopyj;
    private static List<float> speedListCopySubject;
    private static float[] simVals = new float[40];
    private static float Aff;
    //alla infAgents i configuration måste ha lokala parametrar
    //Ska returnera nånting?
    public static float matchingFunction(Configuration query, Configuration comparator)
    {
        
        outerAffVals = new Dictionary<int, Dictionary<int, float>>();
        //infAgentDic ska vara indexerad från 0 och inte hoppa över någon key
        for (int k = 0; k < query.infAgentArray.Length; k++)
        {
            innerAffVals = new Dictionary<int, float>();
            for (int j = 0; j < comparator.infAgentArray.Length; j++)
            {
                
                affValue = affinityFunction(query.subAgent, query.infAgentArray[k], comparator.infAgentArray[j]);
                innerAffVals.Add(j, affValue);               
            }
            outerAffVals.Add(k, innerAffVals);
        }

        //jKeys = new int[query.infAgentArray.Length];
        jKeys = new Dictionary<int, int>();
        topAffValues = new Dictionary<int, float>();
        for (int i = 0; i < query.infAgentArray.Length; i++)   // Kanske kan optimeras genom att hålla koll på största vid skapande?
        {            
            foreach (KeyValuePair<int, float> AffValue in outerAffVals[i])
            {         
                if (AffValue.Value >= topAffValue)
                {
                    topAffValue = AffValue.Value;
                    topInfKey = AffValue.Key;
                }
            }
            if (topAffValue > 0)
            {
                jKeys.Add(i,topInfKey);
            }
            topAffValues.Add(i, topAffValue);
            topAffValue = 0;

        }
        compareCounter = new Dictionary<int, int>();
        foreach (KeyValuePair<int, int> e in jKeys)
        {
            if (compareCounter.ContainsKey(e.Value))
            {
                compareCounter[e.Value]++;
            }
            else
            {
                compareCounter.Add(e.Value, 1);
            }
        }
        //Debug.Log()
        affinityValueList = new float[query.infAgentArray.Length];
        
        foreach (KeyValuePair<int, int> e in jKeys)
        {
            affinityValueList[e.Key] = (query.influenceValues[e.Key] + (((comparator.influenceValues[e.Value])
                /compareCounter[e.Value]))*topAffValues[e.Key] /2);   // topaff är stor för stor
        }
        jUnmatched = new List<int>();
        for (int j = 0; j < comparator.infAgentArray.Length; j++)
        {
            if (!compareCounter.ContainsKey(j))
            {
                jUnmatched.Add(j);
            }
        }

        kUnmatched = new List<int>();
        foreach (KeyValuePair<int, float> topAffValue in topAffValues)
        {
            if (topAffValue.Value == 0)
            {
                kUnmatched.Add(topAffValue.Key);
            }
        }

        sumInfk = 0;
        sumInfj = 0;
        foreach (int k in kUnmatched)
        {
            sumInfk = sumInfk + (query.influenceValues[k] * query.influenceValues[k]);
        }
        foreach (int j in jUnmatched)
        {
            sumInfj = sumInfj + (comparator.influenceValues[j] * comparator.influenceValues[j]);
        }
        Um = (sumInfk + sumInfj)/2;
    
        speedGaussian = (float)Math.Exp(Math.Pow(comparator.subAgent.speedList[0] 
            - query.subAgent.speedList[0], 2) * query.subAgent.speedList[0]);

        affinitySum = 0;
        foreach (float aff in affinityValueList)
        {
            affinitySum += aff;
        }
        //Debug.Log(affinitySum);      // kan blir större än 0 ibland... meningen????  :/   Det är det som gör att matchingvalue blir > 1...   2/4-17 här är felet just nu!
        matchingValue = speedGaussian * (affinitySum - Um);
        return matchingValue;

    }

    public static float affinityFunction(ComparatorAgent querySub, ComparatorAgent k, ComparatorAgent j)
    {
        //FillConfig(k);
        //FillConfig(j);
        xCoordListCopyk = k.xCoordList;
        zCoordListCopyk = k.zCoordList;
        speedListCopyk = k.speedList;
        directionListCopyk = k.directionList;
        xCoordListCopyj = j.xCoordList;
        zCoordListCopyj = j.zCoordList;
        speedListCopyj = j.speedList;
        directionListCopyj = j.directionList;
        speedListCopySubject = querySub.speedList;

        for (int i = 0; i < 40; i++)
        {
            gauss1 = (float)Math.Exp(-(Math.Pow((xCoordListCopyk[i] - xCoordListCopyj[i]), 2) 
                + Math.Pow((zCoordListCopyk[i] - zCoordListCopyj[i]), 2) / (1 / speedListCopySubject[i])));
            gauss2 = (float)Math.Exp(-(Math.Pow((speedListCopyk[i] - speedListCopyj[i]), 2) 
                / (1 / speedListCopySubject[i])));
            gauss3 = (float)Math.Exp(-(Math.Pow((directionListCopyk[i] - directionListCopyj[i]), 2) 
                / (1 / speedListCopySubject[i])));
            simVal = gauss1 * gauss2 * gauss3;
            simVals[i] = simVal;
        }
        Aff = Average(simVals);
        if (Aff < affinityCUTOFF)
        {
            Aff = 0;
        }
        //Debug.Log(Aff);
        return Aff;

    }

    

    private static float Average(float[] floatArray)
    {
        float result = 0;
        for (int i = 0; i < floatArray.Length; i++)
        {
            result += floatArray[i];
        }
        result = result / floatArray.Length;
        return result;
    }
}
/*
struct Tuple<T, U> : IEquatable<Tuple<T, U>>
{
    readonly T first;
    readonly U second;

    public Tuple(T first, U second)
    {
        this.first = first;
        this.second = second;
    }

    public T First { get { return first; } }
    public U Second { get { return second; } }

    public override int GetHashCode()
    {
        return first.GetHashCode() ^ second.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        return Equals((Tuple<T, U>)obj);
    }

    public bool Equals(Tuple<T, U> other)
    {
        return other.first.Equals(first) && other.second.Equals(second);
    }
}*/



