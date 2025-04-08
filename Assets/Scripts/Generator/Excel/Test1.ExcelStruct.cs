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
	public struct Test1 : IExcelStruct
	{
		[LabelText("角色名称")]
		public string m_name;

		[LabelText("对话内容")]
		public string text;

		[LabelText("立绘")]
		public string Illustration;

		[LabelText("节点类型")]
		public string type;

		[LabelText("选项描述")]
		public List<string> choicePreview;

		[LabelText("节点指针")]
		public List<int> nextNodeIndex;

	}
}
