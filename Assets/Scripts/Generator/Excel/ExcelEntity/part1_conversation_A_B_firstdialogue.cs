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
	public sealed class part1_conversation_A_B_firstdialogue : IExcelEntity
	{
		[LabelText("对话数据")]
		public Test1 DialogueData;

		public part1_conversation_A_B_firstdialogue Clone(){
		    return CloneMapUtil<part1_conversation_A_B_firstdialogue>.Clone(this);
		}
	}
}
