//代码使用工具生成，请勿随意修改
using System;
using UnityEngine;
using EggFramework;
using EggFramework.Util.Excel;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
namespace Test
{
	[Serializable]
	public struct Choice : IExcelStruct
	{
		[LabelText("选项描述")]
		public string choicePreview;

		[LabelText("节点指针")]
		public int nextNodeIndex;

	}
}
