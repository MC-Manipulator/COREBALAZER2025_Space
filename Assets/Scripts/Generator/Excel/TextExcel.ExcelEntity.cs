//代码使用工具生成，请勿随意修改
using System;
using UnityEngine;
using EggFramework;
using EggFramework.Util.Excel;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Test;
namespace EggFramework.Generator
{
	[Serializable]
	public sealed class TextExcel : IExcelEntity
	{
		[LabelText("对话数据")]
		public Test1 dialogueData;

		public TextExcel Clone(){
		    return new TextExcel{
		      dialogueData = dialogueData,
		     };
		}
	}
}
