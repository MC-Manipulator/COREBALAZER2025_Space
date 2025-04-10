//代码使用工具生成，请勿随意修改
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
	public sealed class TestExcel : IExcelEntity
	{
		[LabelText("对话数据")]
		public Test1 DialogueData;

		public TestExcel Clone(){
		    return CloneMapUtil<TestExcel>.Clone(this);
		}
	}
}
