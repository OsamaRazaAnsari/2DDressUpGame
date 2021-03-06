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
	/// シナリオのデータ
	/// </summary>
	public class AdvScenarioData
	{
		/// <summary>
		/// シナリオ名
		/// </summary>
		string Name { get { return this.name; } }
		string name;

		/// <summary>
		/// データグリッド名
		/// </summary>
		public string DataGridName { get { return dataGridName; } }
		string dataGridName;

		/// <summary>
		/// 初期化済みか
		/// </summary>
		public bool IsInit { get { return this.isInit; } }
		bool isInit = false;

		/// <summary>
		/// バックグランドでのロード処理を既にしているか
		/// </summary>
		public bool IsAlreadyBackGroundLoad { get { return this.isAlreadyBackGroundLoad; } }
		bool isAlreadyBackGroundLoad = false;
	
		/// <summary>
		/// このシナリオからリンクするジャンプ先のシナリオラベル
		/// </summary>
		public List<AdvScenarioJumpData> JumpScenarioData { get { return this.jumpScenarioData; } }
		List<AdvScenarioJumpData> jumpScenarioData = new List<AdvScenarioJumpData>();

		/// <summary>
		/// このシナリオ内のページデータ
		/// </summary>
		public List<AdvScenarioLabelData> ScenarioLabelData { get { return this.scenarioLabelData; } }
		List<AdvScenarioLabelData> scenarioLabelData = new List<AdvScenarioLabelData>();


		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="name">シナリオ名</param>
		/// <param name="grid">シナリオデータ</param>
		/// <param name="dataManager">各種設定データ</param>
		public void Init(string name, StringGrid grid, AdvSettingDataManager dataManager)
		{
			this.name = name;
			this.dataGridName = grid.Name;
			ParseFromStringGrid(grid, dataManager);
		}


		/// <summary>
		/// バックグランドでダウンロードだけする
		/// </summary>
		/// <param name="dataManager">各種設定データ</param>
		public void Download(AdvDataManager dataManager)
		{
			ScenarioLabelData.ForEach( (item)=>item.Download(dataManager) );
			isAlreadyBackGroundLoad = true;
		}

		/// <summary>
		/// 指定のシナリオラベルがあるかチェック
		/// </summary>
		/// <param name="scenarioLabel">シナリオラベル</param>
		/// <returns>あったらtrue。なかったらfalse</returns>
		public bool IsContainsScenarioLabel(string scenarioLabel)
		{
			return FindScenarioLabelData(scenarioLabel) != null;
		}

		/// <summary>
		/// 指定のシナリオラベルがあるかチェック
		/// </summary>
		/// <param name="scenarioLabel">シナリオラベル</param>
		/// <returns>あったらtrue。なかったらfalse</returns>
		public AdvScenarioLabelData FindScenarioLabelData(string scenarioLabel)
		{
			return ScenarioLabelData.Find((item) => item.ScenaioLabel == scenarioLabel);
		}

		public AdvScenarioLabelData FindNextScenarioLabelData(string scenarioLabel)
		{
			for (int i = 0; i < ScenarioLabelData.Count-1; ++i)
			{
				if (ScenarioLabelData[i].ScenaioLabel == scenarioLabel)
				{
					return ScenarioLabelData[i + 1];
				}
			}
			return null;
		}

		public HashSet<AssetFile> MakePreloadFileList(string scenarioLabel, int page, int maxFilePreload)
		{
			for (int i = 0; i < ScenarioLabelData.Count; ++i)
			{
				if (ScenarioLabelData[i].ScenaioLabel == scenarioLabel)
				{
					return MakePreloadFileListSub(i, page, maxFilePreload);
				}
			}
			return null;
		}

		HashSet<AssetFile> MakePreloadFileListSub( int index, int page, int maxFilePreload)
		{
			HashSet<AssetFile> fileList = new HashSet<AssetFile>();
			for (int i = index; i < ScenarioLabelData.Count; ++i)
			{
				AdvScenarioLabelData data = ScenarioLabelData[i];
				for (int j = page; j < data.PageNum; ++j)
				{
					data.GetPageData(j).AddToFileSet(fileList);
					if (fileList.Count >= maxFilePreload)
					{
						return fileList;
					}
				}
				page = 0;
			}
			return fileList;
		}


/*
		/// <summary>
		/// 指定インデックスのコマンドを取得
		/// </summary>
		/// <param name="index">インデックス</param>
		/// <returns>コマンド</returns>
		public AdvCommand GetCommand(int index)
		{
			return null;
		}
		
		/// <summary>
		/// 指定シナリオラベルの指定ページのコマンドインデックスを取得
		/// </summary>
		/// <param name="scenarioLabel">シナリオラベル</param>
		/// <param name="page">ページ</param>
		/// <returns>ページのコマンドインデックス</returns>
		public int SeekPageIndex(string scenarioLabel, int page)
		{
			int index = 0;

			if (Name == scenarioLabel)
			{
				//シナリオ名そのものだった場合は一番最初から
				index = 0;
			}
			else
			{
				//シナリオラベルをシーク
				while (true)
				{
					AdvCommand command = GetCommand(index);
					if (null == GetCommand(index))
					{
						Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.NotFoundScnarioLabel,scenarioLabel));
						return 0;
					}

					if ( command.GetScenarioLabel() == scenarioLabel)
					{
						break;
					}
					++index;
				}
			}
			if (page < 0)
			{	//シナリオラベル冒頭
				return index;
			}

			int pageCount = 0;
			//シナリオラベルからの指定のページまでシーク
			while (true)
			{
				AdvCommand command = GetCommand(index);
				if (null == command)
				{
					//指定のページ数がなかったので、ここまでで終了
					return index-1;
				}
				if (command.IsTypePageEnd())
				{
					if (pageCount >= page)
					{
						return index;
					}
					++pageCount;
				}
				++index;
			}
		}
*/
		//コマンドデータの解析・初期化
		void ParseFromStringGrid(StringGrid grid, AdvSettingDataManager dataManager)
		{
			isInit = false;
			List<AdvCommand> commandList = new List<AdvCommand>();
			AdvCommand continuousCommand = null;	//連続的なコマンド処理

			foreach (StringGridRow row in grid.Rows)
			{
				if (row.RowIndex < grid.DataTopRow ) continue;			//データの行じゃない
				if (row.IsEmpty) continue;								//データがない

				AdvCommand command = AdvCommandParser.CreateCommand( row, dataManager);
				if (null != command)
				{
					//連続するコマンドの場合は、連続が途切れたら終了コマンドを追加
					TryAddContinusCommand(continuousCommand, command, commandList, dataManager);
					//コマンド追加
					if (null != command) commandList.Add(command);
					//連続するコマンドの場合は、連続が途切れたら終了コマンドを追加
					continuousCommand = command.IsContinusCommand ? command : null; ;
				}
			}
			//連続するコマンドの場合は、連続が途切れたら終了コマンドを追加
			TryAddContinusCommand(continuousCommand, null, commandList, dataManager);

			MakeScanerioLabelData(commandList);
			isInit = true;
		}

		/// <summary>
		/// 選択肢など連続するタイプのコマンドの場合は、連続が途切れたら終了コマンドを追加
		/// </summary>
		/// <param name="continuousCommand">連続しているコマンド</param>
		/// <param name="currentCommand">現在のコマンド</param>
		void TryAddContinusCommand(AdvCommand continuousCommand, AdvCommand currentCommand, List<AdvCommand> commandList, AdvSettingDataManager dataManager)
		{
			if (continuousCommand != null)
			{
				if ( currentCommand == null || !continuousCommand.CheckContinues(currentCommand))
				{
					AdvCommand command = AdvCommandParser.CreateContiunesEndCommand(continuousCommand, dataManager);
					if (null != command) commandList.Add(command);
				}
			}
		}

		//シナリオラベル区切りのデータを作成
		void MakeScanerioLabelData( List<AdvCommand> commandList)
		{
			//最初のラベル区切りデータは自身の名前（シート名）
			AdvScenarioLabelData currentScenariodata = new AdvScenarioLabelData(Name);
			scenarioLabelData.Add(currentScenariodata);

			//シナリオラベルの解析
			foreach (AdvCommand command in commandList)
			{
				///シナリオラベルを取得
				string scenarioLabel = command.GetScenarioLabel();
				if (!string.IsNullOrEmpty(scenarioLabel) )
				{
					if (IsContainsScenarioLabel(scenarioLabel))
					{
						Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.RedefinitionScenarioLabel, scenarioLabel,DataGridName));
					}
					else
					{
						currentScenariodata = new AdvScenarioLabelData(scenarioLabel,command.RowData);
						scenarioLabelData.Add(currentScenariodata);
					}
				}
				currentScenariodata.AddCommand(command);

				///このシナリオからリンクするジャンプ先のシナリオラベルを取得
				string jumpLabel = command.GetJumpLabel();
				if (!string.IsNullOrEmpty(jumpLabel))
				{
					jumpScenarioData.Add(new AdvScenarioJumpData(jumpLabel, command.RowData));
				}
			}
		}
	}
}