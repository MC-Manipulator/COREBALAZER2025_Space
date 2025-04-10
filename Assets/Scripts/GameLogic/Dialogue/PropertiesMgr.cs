using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertiesMgr : BaseManager<PropertiesMgr>
{
    public PropertiesData statData;
    public PropertiesMgr()
    {
        statData = ResMgr.GetInstance().Load<PropertiesData>("ScriptableObject/Dialogue/Properties Data");
    }

    //public void ChangeStatValue(string index,Stat value)
    //{
    //    statData.customProperties[index] = value;
    //}

    //public Stat GetStatValue(string index)
    //{
    //    return statData.customProperties[index];
    //}
}
