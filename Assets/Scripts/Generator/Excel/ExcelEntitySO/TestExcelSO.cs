//代码使用工具生成，请勿随意修改
using System;
using UnityEngine;
using EggFramework;
using EggFramework.Util.Excel;
using EggFramework.Util;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using EggFramework.Generator;
namespace EggFramework.Generator
{
	public sealed class TestExcelSO : ScriptableObject , IExcelEntitySO
	{
		#if UNITY_EDITOR
		[Button("读取数据", ButtonSizes.Large), PropertyOrder(-1)]
		private void ReadData()
		{
		    ExcelUtil.ReadDataByExcelEntityType(typeof(TestExcel));
		}
		[Button("刷新字符集", ButtonSizes.Large), PropertyOrder(-2)]
		private void RefreshEncode()
		{
		    EncodeUtil.RefreshEncode();
		}
		#endif
		[ReadOnly, LabelText("数据库"), ListDrawerSettings(NumberOfItemsPerPage = 10)]
		public List<TestExcel> RawDataList;

	}
}
