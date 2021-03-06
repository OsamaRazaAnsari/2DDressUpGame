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
	/// シナリオのページデータ
	/// </summary>
	public class AdvScenarioPageData
	{
		List<AdvCommand> commnadList = new List<AdvCommand>();

		//コマンドの追加
		public void AddCommand(AdvCommand command)
		{
			commnadList.Add(command);
		}

		//データのダウンロード
		public void Download(AdvDataManager dataManager)
		{
			commnadList.ForEach((item) => item.Download(dataManager));
		}

		public AdvCommand GetCommand(int index)
		{
			return (index < commnadList.Count) ? commnadList[index] : null;
		}

		//
		public void AddToFileSet( HashSet<AssetFile> fileSet)
		{
			foreach( AdvCommand command in commnadList )
			{
				if (command.IsExistLoadFile())
				{
					command.LoadFileList.ForEach((item) => fileSet.Add(item));
				}
			}
		}
	}
}