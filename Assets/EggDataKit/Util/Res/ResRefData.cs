#region

//文件创建者：Egg
//创建时间：10-24 01:27

#endregion

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EggFramework.Util.Res
{
    public abstract class ResRefData<T> : ScriptableObject where T : Object
    {
        [Serializable]
        public struct ResRef
        {
            [LabelText("资源名"), LabelWidth(50f), HorizontalGroup("", Width = 400f), Multiline(3)]
            public string Name;

            [HorizontalGroup(""), LabelText("资源"), LabelWidth(50f),
             PreviewField(ObjectFieldAlignment.Center, Height = 100f)]
            public T Data;
        }

        [ValueDropdown("@EggFramework.Util.DirectoryUtil.GetFilePaths()"), LabelText("路径过滤器"),
         OnValueChanged("@ResUtil.RefreshResRefData(this.GetType(),true)", IncludeChildren = true)]
        public List<string> Filters = new();

        [LabelText("资源引用"), ListDrawerSettings(NumberOfItemsPerPage = 10)]
        public List<ResRef> ResRefs = new();
    }
}