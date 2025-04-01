//代码使用工具生成，请勿随意修改
using System;
using UnityEngine;
using EggFramework;
using EggFramework.Util.Excel;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Test;
namespace EggFramework.Generator
{
	public sealed class TextExcelSO : ScriptableObject , IExcelEntitySO
	{
		#if UNITY_EDITOR
		[Button("读取数据", ButtonSizes.Large), PropertyOrder(-1)]
		private void ReadData()
		{
		    ExcelUtil.ReadDataByExcelEntityType(typeof(TextExcel));
		}
		#endif
		[LabelText("数据库"), ListDrawerSettings(NumberOfItemsPerPage = 10)]
		public List<TextExcel> RawDataList;

	}
}
