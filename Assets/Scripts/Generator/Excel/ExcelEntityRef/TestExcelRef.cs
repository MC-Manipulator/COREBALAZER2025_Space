//代码使用工具生成，请勿随意修改
#if UNITY_EDITOR
using System;
using UnityEngine;
using EggFramework;
using EggFramework.Util.Excel;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using EggFramework.Generator;
namespace EggFramework.Generator
{
	[Serializable]
	public sealed class TestExcelRef : ExcelEntityRef<TestExcel>
	{
		[ShowInInspector, HideLabel, FoldoutGroup("对话数据"), OnValueChanged("__OnValueChanged__DialogueData", true)]
		public Test1 DialogueData
		{
		    get
		    {
		        var ret = new Test1
		        {
		            m_name = ExcelUtil.ParseData<String>(GetRawData(0)),
		            text = ExcelUtil.ParseData<String>(GetRawData(1)),
		            Illustration = ExcelUtil.ParseData<String>(GetRawData(2)),
		            type = ExcelUtil.ParseData<String>(GetRawData(3)),
		            choicePreview = ExcelUtil.ParseData<List<String>>(GetRawData(4)),
		            nextNodeIndex = ExcelUtil.ParseData<List<Int32>>(GetRawData(5)),
		        };
		        ret.RegisterOnCollectionChanged__choicePreview(() =>
		        {
		             DialogueData = CloneMapUtil<Test1>.Clone(ret);
		        });
		        ret.RegisterOnCollectionChanged__nextNodeIndex(() =>
		        {
		             DialogueData = CloneMapUtil<Test1>.Clone(ret);
		        });
		        return ret;
		    }
		    set
		    {
		         SetRawData(0, ExcelUtil.SerializeData(value.m_name));
		         SetRawData(1, ExcelUtil.SerializeData(value.text));
		         SetRawData(2, ExcelUtil.SerializeData(value.Illustration));
		         SetRawData(3, ExcelUtil.SerializeData(value.type));
		         SetRawData(4, ExcelUtil.SerializeData(value.choicePreview));
		         SetRawData(5, ExcelUtil.SerializeData(value.nextNodeIndex));
		    }
		}
		private void __OnValueChanged__DialogueData(Test1 val)
		{
		    SetRawData(0, ExcelUtil.SerializeData(val.m_name));
		    SetRawData(1, ExcelUtil.SerializeData(val.text));
		    SetRawData(2, ExcelUtil.SerializeData(val.Illustration));
		    SetRawData(3, ExcelUtil.SerializeData(val.type));
		    SetRawData(4, ExcelUtil.SerializeData(val.choicePreview));
		    SetRawData(5, ExcelUtil.SerializeData(val.nextNodeIndex));
		}
		public override int MaxRowIndex => 5;
	}
}
#endif
