//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Utage
{

	/// <summary>
	/// シナリオラベルで区切られたデータ
	/// </summary>
	public class AdvScenarioLabelData
	{
		List<AdvScenarioPageData> pageDataList = new List<AdvScenarioPageData>();
		//シナリオラベル
		string scenaioLabel;
		public string ScenaioLabel
		{
			get { return scenaioLabel; }
		}
		public int PageNum
		{
			get { return pageDataList.Count; }
		}

		AdvScenarioPageData cuurentPageData;

		StringGridRow rowData;

		//コンストラクタ
		public AdvScenarioLabelData(string scenaioLabel)
		{
			Init(scenaioLabel);
		}

		//コンストラクタ
		public AdvScenarioLabelData(string scenaioLabel, StringGridRow rowData)
		{
			this.rowData = rowData;
			Init(scenaioLabel);
		}

		void Init(string scenaioLabel)
		{
			this.scenaioLabel = scenaioLabel;
			cuurentPageData = new AdvScenarioPageData();
			pageDataList.Add(cuurentPageData);
		}

		//コマンドの追加
		public void AddCommand(AdvCommand command)
		{
			cuurentPageData.AddCommand(command);
			if (command.IsTypePageEnd())
			{
				cuurentPageData = new AdvScenarioPageData();
				pageDataList.Add(cuurentPageData);
			}
		}

		//データのダウンロード
		public void Download(AdvDataManager dataManager)
		{
			pageDataList.ForEach( (item)=>item.Download(dataManager) );
		}

		public AdvScenarioPageData GetPageData(int page)
		{
			return (page < pageDataList.Count) ? pageDataList[page] : null;
		}

		//エラー文字列
		public string ToErrorString(string str, string gridName)
		{
			if (rowData!=null)
			{
				return rowData.ToErrorString(str);
			}
			else
			{
				return str + " "+ gridName;
			}
		}
	}
}