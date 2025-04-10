using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Stat : ScriptableObject
{
}



[System.Serializable]
public class StatValuePair
{
    public Stat stat;

    [ShowIf("@stat != null && stat is BoolStat")]
    public bool boolValue;

    [ShowIf("@stat != null && stat is IntStat")]
    public int intValue;

    [ShowIf("@stat != null && stat is FloatStat")]
    public float floatValue;


    public void ApplyValueToStat()
    {
        switch (stat)
        {
            case IntStat intStat:
                intStat.value = intValue;
                break;
            case BoolStat boolStat:
                boolStat.value = boolValue;
                break;
            case FloatStat floatStat:
                floatStat.value = floatValue;
                break;
        }
    }
}