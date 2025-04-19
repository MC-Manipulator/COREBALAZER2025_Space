using Language.Lua;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Condition/BranchCondtion")]
[Serializable]
[ShowInInspector]
public class BranchCondition : SerializedScriptableObject
{
    public IJudge judge;
    [AssetSelector(Paths = "Assets/Resources/ScriptableObject/Dialogue/Properties Data.asset")]
    public Stat stat;
    public bool IsConditionTrue()
    {
        return judge.IsConditionTrue(stat);
    }
}

    public interface IJudge
    {
        public abstract bool IsConditionTrue(Stat value);
    }

    public class BoolCondition : IJudge
    {

        [ValueDropdown("GetFilteredItems")]
        public string m_operator;

        private ValueDropdownList<string> GetFilteredItems()
        {
            var dropdown = new ValueDropdownList<string>();

            dropdown.Add("是否为假", "IsFalse");
            dropdown.Add("是否为真", "IsTrue");

            return dropdown;
        }

        public bool IsConditionTrue(Stat value)
        {
            if (value is BoolStat boolValue)
            {
                switch (m_operator)
                {
                    case "IsTrue": return boolValue.value;
                    case "IsFalse": return !boolValue.value;
                }
            }
            Debug.LogError($"类型不匹配，期望bool类型，实际收到: {value?.GetType().Name ?? "null"}");
            return false;
        }
    }

    public class IntCondition : IJudge
    {
        public int value;

        [ValueDropdown("GetFilteredItems")]
        public string m_operator;

        private ValueDropdownList<string> GetFilteredItems()
        {
            var dropdown = new ValueDropdownList<string>();

            dropdown.Add("大于", "Greater");
            dropdown.Add("等于", "Equal");
            dropdown.Add("小于", "Less");
            dropdown.Add("大于等于", "GreaterOrEqual");
            dropdown.Add("小于等于", "LessOrEqual");

            return dropdown;
        }
        public bool IsConditionTrue(Stat _value)
        {
            if (_value is IntStat intValue)
            {
                switch (m_operator)
                {
                    case "Greater": return intValue.value > value;
                    case "Equal": return Mathf.Approximately(intValue.value, value);
                    case "Less": return intValue.value < value;
                    case "GreaterOrEqual": return intValue.value >= value;
                    case "LessOrEqual": return intValue.value <= value;
                }
            }
            Debug.LogError($"类型不匹配，期望int类型，实际收到: {_value?.GetType().Name ?? "null"}");
            return false;
        }
    }

    public class FloatCondition : IJudge
    {
        public float value;

        [ValueDropdown("GetFilteredItems")]
        public string m_operator;

        private ValueDropdownList<string> GetFilteredItems()
        {
            var dropdown = new ValueDropdownList<string>();

            dropdown.Add("大于", "Greater");
            dropdown.Add("等于", "Equal");
            dropdown.Add("小于", "Less");
            dropdown.Add("大于等于", "GreaterOrEqual");
            dropdown.Add("小于等于", "LessOrEqual");

            return dropdown;
        }
        public bool IsConditionTrue(Stat _value)
        {
        if (_value is FloatStat intValue)
        {
            switch (m_operator)
            {
                case "Greater": return intValue.value > value;
                case "Equal": return Mathf.Approximately(intValue.value, value);
                case "Less": return intValue.value < value;
                case "GreaterOrEqual": return intValue.value >= value;
                case "LessOrEqual": return intValue.value <= value;
            }
        }
        Debug.LogError($"类型不匹配，期望int类型，实际收到: {_value?.GetType().Name ?? "null"}");
        return false;
    }
}
