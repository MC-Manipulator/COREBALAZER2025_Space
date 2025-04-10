//代码使用工具生成，请勿随意修改
using System;
using UnityEngine;
using EggFramework;
using EggFramework.Util.Excel;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
namespace EggFramework.Generator
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
		[OnValueChanged("__OnValueChanged__choicePreview", true)]
		[ListDrawerSettings(CustomAddFunction = "_add__choicePreview")]
		public List<string> choicePreview;

		#if UNITY_EDITOR
		private string _add__choicePreview => "Empty";
		private Action _action__choicePreview;
		public void RegisterOnCollectionChanged__choicePreview(Action action)
		{
		    _action__choicePreview ??= action;
		}
		private void __OnValueChanged__choicePreview(List<string> list)
		{
		    _action__choicePreview?.Invoke();
		}
		#endif
		[LabelText("节点指针")]
		[OnValueChanged("__OnValueChanged__nextNodeIndex", true)]
		public List<int> nextNodeIndex;

		#if UNITY_EDITOR
		private Action _action__nextNodeIndex;
		public void RegisterOnCollectionChanged__nextNodeIndex(Action action)
		{
		    _action__nextNodeIndex ??= action;
		}
		private void __OnValueChanged__nextNodeIndex(List<int> list)
		{
		    _action__nextNodeIndex?.Invoke();
		}
		#endif
	}
}
