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
	/// データ管理
	/// </summary>
	[AddComponentMenu("Utage/ADV/Internal/DataManager ")]
	public partial class AdvDataManager : MonoBehaviour
	{
		//バックグランドでリソースのDLをするか
		[SerializeField]
		bool isBackGroundDownload = true;
		public bool IsBackGroundDownload
		{
			get { return isBackGroundDownload; }
			set { isBackGroundDownload = value; }
		}

		/// <summary>
		/// 設定データ
		/// </summary>
		public AdvSettingDataManager SettingDataManager { get { return this.settingDataManager; } }
		AdvSettingDataManager settingDataManager;

		/// <summary>
		/// エクスポートしたシナリオデータがあればここに設定可能
		/// </summary>
		AdvScenarioDataExported[] exportedScenarioDataTbl;

		//シナリオデータ
		Dictionary<string, AdvScenarioData> scenarioDataTbl = new Dictionary<string, AdvScenarioData>();

		/// <summary>
		/// 設定データが準備済みか
		/// </summary>
		public bool IsReadySettingData { get { return (settingDataManager != null); } }

		/// <summary>
		/// 全てのシナリオがロード済みか
		/// </summary>
		public bool IsLoadEndAllScenario { get { return ( loadingScenarioCount <= 0 ); } }
		int loadingScenarioCount = int.MaxValue;


		/// <summary>
		/// データを直接設定して初期化
		/// </summary>
		public void InitData(AdvSettingDataManager settingDataManager, AdvScenarioDataExported[] exportedScenarioDataTbl)
		{
			this.settingDataManager = settingDataManager;
			this.exportedScenarioDataTbl = exportedScenarioDataTbl;
		}

		/// <summary>
		/// CSV形式ファイルをロードして初期化
		/// </summary>
		/// <param name="url">CSVのパス</param>
		/// <param name="version">シナリオバージョン（-1以下で必ずサーバーからデータを読み直す）</param>
		/// <returns></returns>
		public IEnumerator LoadCsvAsync(string url, int version )
		{
			exportedScenarioDataTbl = new AdvScenarioDataExported[0];
			scenarioDataTbl.Clear();
			settingDataManager = ScriptableObject.CreateInstance<AdvSettingDataManager>();
			yield return StartCoroutine(SettingDataManager.LoadCsvAsync(this, url, version));
		}
		
		/// <summary>
		/// 起動時の初期化
		/// </summary>
		/// <param name="rootDirResource">ルートディレクトリのリソース</param>
		public void BootInit( string rootDirResource )
		{
			settingDataManager.BootInit(rootDirResource);
		}

		/// <summary>
		/// シナリオデータのロードと初期化を開始
		/// </summary>
		public void StartLoadAndInitScenariodData()
		{
			//シナリオのエクスポート済みのデータをまず初期化
			foreach (AdvScenarioDataExported data in exportedScenarioDataTbl)
			{
				foreach (var exportedScenarioData in data.List)
				{
					exportedScenarioData.Grid.InitLink();
					AdvScenarioData scenarioData = new AdvScenarioData();
					scenarioData.Init(exportedScenarioData.Key, exportedScenarioData.Grid, SettingDataManager);
					scenarioDataTbl.Add(exportedScenarioData.Key, scenarioData);
				}
			}
			//エクスポートされたデータはもういらない。
			//メモリ食うので参照を外す
			exportedScenarioDataTbl = null;

			loadingScenarioCount = SettingDataManager.ScenarioSetting.List.Count;
			//シナリオファイルをロード
			foreach (AdvScenarioSettingData scenarioSetting in SettingDataManager.ScenarioSetting.List)
			{
				StartCoroutine( CoLoadAndInitScenariodData(scenarioSetting) );
			}
		}

		IEnumerator CoLoadAndInitScenariodData(AdvScenarioSettingData scenarioSetting)
		{
			string scenaioFileName = scenarioSetting.ScenaioFile;
			//既にある（エクスポートされたデータの可能性あり）
			if (scenarioDataTbl.ContainsKey(scenaioFileName)) yield break;

			string path = SettingDataManager.ScenarioSetting.ScenaioFileToPath(scenaioFileName);
			AdvScenarioData data = new AdvScenarioData();
			AssetFile file = AssetFileManager.BackGroundLoad(path, this);
			while (!file.IsLoadEnd) yield return 0;

			data.Init(scenaioFileName, file.Csv, SettingDataManager);
			file.Unuse(this);
			scenarioDataTbl.Add(scenaioFileName, data);

			--loadingScenarioCount;
		}

		/// <summary>
		/// リソースファイル(画像やサウンド)のダウンロードをバックグラウンドで進めておく
		/// </summary>
		/// <param name="startScenario">開始シナリオ名</param>
		public void StartBackGroundDownloadResource( string startScenario )
		{
			if (isBackGroundDownload)
			{
				StartCoroutine(CoBackGroundDownloadResource(startScenario));
				SettingDataManager.DownloadAll();
			}
		}
		IEnumerator CoBackGroundDownloadResource(string startScenario)
		{
			while (loadingScenarioCount > 0) yield return 0;

			AdvScenarioData data = FindScenarioData(startScenario);
			if (!data.IsAlreadyBackGroundLoad)
			{
				data.Download(this);
				foreach (AdvScenarioJumpData jumpData in data.JumpScenarioData)
				{
					//シナリオファイルのロード待ち
					while (!IsLoadEndScenarioLabel(jumpData))
					{
						yield return 0;
					}
					yield return StartCoroutine(CoBackGroundDownloadResource(jumpData.ToLabel));
				}
			}
		}
		
		/// <summary>
		/// 指定のシナリオラベルが既にロード終了しているか
		/// </summary>
		public bool IsLoadEndScenarioLabel(AdvScenarioJumpData jumpData)
		{
			AdvScenarioData scenarioData = FindScenarioData(jumpData.ToLabel);
			if (null != scenarioData) return true;
			
			if( IsLoadEndAllScenario )
			{
				string msg = LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.NotFoundScnarioLabel, jumpData.ToLabel);
				Debug.LogError(jumpData.FromRow.ToErrorString(msg));
			}

			return false;
		}

		/// <summary>
		/// 指定のシナリオラベルが既にロード終了しているか
		/// </summary>
		public bool IsLoadEndScenarioLabel(string label)
		{
			AdvScenarioData scenarioData = FindScenarioData(label);
			if (null != scenarioData) return true;

			if (IsLoadEndAllScenario)
			{
				string msg = LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.NotFoundScnarioLabel, label);
				Debug.LogError(msg);
			}
			return false;
		}

		/// <summary>
		///  シナリオデータを検索して取得
		/// </summary>
		/// <param name="ScebarioLabel">シナリオラベル</param>
		/// <returns>シナリオデータ。見つからなかったらnullを返す</returns>
		public AdvScenarioData FindScenarioData(string label)
		{
			foreach (AdvScenarioData data in scenarioDataTbl.Values )
			{
				if (data.IsContainsScenarioLabel(label))
				{
					return data;
				}
			}
			return null;
		}

		/// <summary>
		///  シナリオデータを検索して取得
		/// </summary>
		/// <param name="ScebarioLabel">シナリオラベル</param>
		/// <returns>シナリオデータ。見つからなかったらnullを返す</returns>
		public AdvScenarioLabelData FindScenarioLabelData(string scenarioLabel)
		{
			foreach (AdvScenarioData data in scenarioDataTbl.Values)
			{
				AdvScenarioLabelData labelData = data.FindScenarioLabelData(scenarioLabel);
				if (labelData!=null)
				{
					return labelData;
				}
			}
			return null;
		}


		public AdvScenarioLabelData NextScenarioLabelData(string scenarioLabel)
		{
			foreach (AdvScenarioData data in scenarioDataTbl.Values)
			{
				AdvScenarioLabelData labelData = data.FindNextScenarioLabelData(scenarioLabel);
				if (labelData != null)
				{
					return labelData;
				}
			}
			return null;
		}

		public HashSet<AssetFile> MakePreloadFileList(string scenarioLabel, int page, int maxFilePreload)
		{
			foreach (AdvScenarioData data in scenarioDataTbl.Values)
			{
				if (data.IsContainsScenarioLabel(scenarioLabel))
				{
					return data.MakePreloadFileList( scenarioLabel, page, maxFilePreload);
				}
			}
			return null;
		}
	}
}
