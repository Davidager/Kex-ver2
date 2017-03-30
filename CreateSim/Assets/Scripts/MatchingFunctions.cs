using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchingFunctions{

    Agent infFactork;
    Agent infFactorj;
    float AffValue;
    //kolla storleksordningen på cutoffen
    private float affinityCUTOFF = 0.3f;
    private static int topInfKey;
    private float newXcoord;
    private float newZcoord;
    private float newSpeed;
    private float newDirection;
    private static float[] affinityValueList;
    private float simVal;
    private float gauss1;
    private float gauss2;
    private float gauss3;
    private float sumInfk;
    private float sumInfj;
    private float Um;
    private float speedGaussian;
    private float affinitySum;
    private float matchingValue;
    private static float topAffValue = 0;
    private static float affValue;
    private static Dictionary<int, float> topAffValues;
    //private static float[] TopAffValues;
    private static Dictionary<int, float> innerAffVals;
    private static Dictionary<int, Dictionary<int, float>> outerAffVals;
    private static Dictionary<int, int> compareCounter;
    //private static Dictionary<int, float> compareDic;
    private static int[] jKeys;
    private List<int> jUnmatched;
    private List<int> kUnmatched;
    private float[] xCoordListCopyk;
    private float[] zCoordListCopyk;
    private float[] speedListCopyk;
    private float[] directionListCopyk;
    private float[] xCoordListCopyj;
    private float[] zCoordListCopyj;
    private float[] speedListCopyj;
    private float[] directionListCopyj;
    private float[] speedListCopySubject;
    private float[] simVals = new float[40];
    private float Aff;
    //alla infAgents i configuration måste ha lokala parametrar
    //Ska returnera nånting?
    public static float matchingFunction(Configuration query, Configuration comparator)
    {
        //infAgentDic ska vara indexerad från 0 och inte hoppa över någon key
        for (int k = 0; k < query.infAgentArray.Length; k++)
        {
            for (int j = 0; j < comparator.infAgentArray.Length; j++)
            {
                affValue = affinityFunction(query.subAgent, query.infAgentArray[k], comparator.infAgentArray[j]);
                innerAffVals.Add(j, affValue);
                outerAffVals.Add(k, innerAffVals);
            }
        }


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
                jKeys[i] = topInfKey;
            }
            topAffValues.Add(i, topAffValue);
            topAffValue = 0;

        }
        foreach (int jKey in jKeys)
        {
            if (compareCounter.ContainsKey(jKey))
            {
                compareCounter[jKey]++;
            }
            else
            {
                compareCounter.Add(jKey, 1);
            }
        }
        for (int i = 0; i < query.infAgentArray.Length; i++)
        {
            affinityValueList[i] = (query.influenceValues[i] + ((comparator.influenceValues[jKeys[i]].)/compareCounter[jKeys[i]]))*TopAffValues[i]/2;
        }

        for (int j = 0; j < comparator.infAgentArray.Length; j++)
        {
            if (!compareCounter.ContainsKey(i))
            {
                jUnmatched.Add(j);
            }
        }

        foreach (KeyValuePair<int, float> topAffValue in topAffValues)
        {
            if (topAffValue.Value = 0)
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
            sumInfj = sumInfj + (query.influenceValues[j] * query.influenceValues[j]);
        }
        Um = (sumInfk + sumInfj)/2;
    
        speedGaussian = (float)Math.Exp(Math.Pow(comparator.subAgent.speedList[0] - query.subAgent.speedList[0], 2) * query.subAgent.speedList[0]);

        affinitySum = 0;
        foreach (KeyValuePair<int, float> topAffValue in topAffValues)
        {
            affinitySum += topAffValue.Value;
        }
        matchingValue = speedGaussian * (affinitySum - Um);

        return new float { matchingValue };

    }

    public static float affinityFunction(ComparatorAgent querySub, ComparatorAgent k, ComparatorAgent j)
    {
        //FillConfig(k);
        //FillConfig(j);
        k.xCoordList.CopyTo(xCoordListCopyk, 0);
        k.zCoordList.CopyTo(zCoordListCopyk, 0);
        k.speedList.CopyTo(speedListCopyk, 0);
        k.directionList.CopyTo(directionListCopyk, 0);
        j.xCoordList.CopyTo(xCoordListCopyj, 0);
        j.zCoordList.CopyTo(zCoordListCopyj, 0);
        j.speedList.CopyTo(speedListCopyj, 0);
        j.directionList.CopyTo(directionListCopyj, 0);
        querySub.speedList.CopyTo(speedListCopySubject, 0);

        for (int i = 0; i < 40; i++)
        {
            gauss1 = (float)Math.Exp((Math.Pow((xCoordListCopyk[i] - xCoordListCopyj[i]), 2) + Math.Pow((zCoordListCopyk[i] - zCoordListCopyj[i]), 2) / (1 - speedListCopySubject[i])));
            gauss2 = (float)Math.Exp((Math.Pow((speedListCopyk[i] - speedListCopyj[i]), 2) / (1 - speedListCopySubject[i])));
            gauss3 = (float)Math.Exp((Math.Pow((directionListCopyk[i] - directionListCopyj[i]), 2) / (1 - speedListCopySubject[i])));
            simVal = gauss1 * gauss2 * gauss3;
            simVals[i] = simVal;
        }
        Aff = Average(simVals);
        if (Aff < affinityCUTOFF)
        {
            Aff = 0;
        }
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



