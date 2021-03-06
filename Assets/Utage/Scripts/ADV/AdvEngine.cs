//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Utage
{


	/// <summary>
	/// メインエンジン
	/// </summary>/
	[AddComponentMenu("Utage/ADV/MainEngine")]
	[RequireComponent(typeof(DontDestoryOnLoad))]
	[RequireComponent(typeof(AdvDataManager))]
	[RequireComponent(typeof(AdvScenarioPlayer))]
	[RequireComponent(typeof(AdvPage))]
	[RequireComponent(typeof(AdvSelectionManager))]
	[RequireComponent(typeof(AdvBacklogManager))]
	[RequireComponent(typeof(AdvConfig))]	
	[RequireComponent(typeof(AdvSystemSaveData))]	
	[RequireComponent(typeof(AdvSaveManager))]	
	public partial class AdvEngine : MonoBehaviour
	{
		/// <summary>
		/// 最初からはじめる場合のシナリオ名
		/// </summary>
		public string StartScenarioLabel
		{
			get {
				return startScenarioLabel;
			}
			set {
				startScenarioLabel = value;
			}
		}
		string startScenarioLabel = "Start";


		/// <summary>
		/// シナリオや設定等のデータ
		/// </summary>
		public AdvDataManager DataManager { get { return this.dataManager ?? (this.dataManager = GetComponent<AdvDataManager>()); } }
		AdvDataManager dataManager;

		/// <summary>
		/// シナリオの実行部分
		/// </summary>
		public AdvScenarioPlayer ScenarioPlayer { get { return this.scenarioPlayer ?? (this.scenarioPlayer = GetComponent<AdvScenarioPlayer>()); } }
		AdvScenarioPlayer scenarioPlayer;

		/// <summary>
		/// ページ情報
		/// </summary>
		public AdvPage Page { get { return this.page ?? (this.page = GetComponent<AdvPage>()); } }
		AdvPage page;


		/// <summary>
		/// 選択肢
		/// </summary>
		public AdvSelectionManager SelectionManager { get { return this.selectionManager ?? (this.selectionManager = GetComponent<AdvSelectionManager>()); } }
		AdvSelectionManager selectionManager;

		/// <summary>
		/// バックログ
		/// </summary>
		public AdvBacklogManager BacklogManager { get { return this.backlogManager ?? (this.backlogManager = GetComponent<AdvBacklogManager>()); } }
		AdvBacklogManager backlogManager;

		/// <summary>
		/// コンフィグデータ
		/// </summary>
		public AdvConfig Config { get { return this.config ?? (this.config = GetComponent<AdvConfig>()); } }
		AdvConfig config;

		/// <summary>
		/// システムセーブデータ
		/// </summary>
		public AdvSystemSaveData SystemSaveData { get { return this.systemSaveData ?? (this.systemSaveData = GetComponent<AdvSystemSaveData>()); } }
		AdvSystemSaveData systemSaveData;

		/// <summary>
		/// 通常のセーブデータ
		/// </summary>
		public AdvSaveManager SaveManager { get { return this.saveManager ?? (this.saveManager = GetComponent<AdvSaveManager>()); } }
		AdvSaveManager saveManager;

		/// <summary>
		/// 表示レイヤー管理
		/// </summary>
		public AdvLayerManager LayerManager { get { return this.layerManager ?? (this.layerManager = FindObjectOfType<AdvLayerManager>()); } }
		[SerializeField]
		AdvLayerManager layerManager;
/*
		/// <summary>
		/// 表示モデル管理
		/// </summary>
		public Adv3DModelManager ModelManager { get { return this.modelManager ?? (this.modelManager = UtageToolKit.GetCompoentInChildrenCreateIfMissing<Adv3DModelManager>(this.transform)); } }
		[SerializeField]
		Adv3DModelManager modelManager;
*/
		/// <summary>
		/// グラフィック管理
		/// </summary>
		public AdvGraphicManager GraphicManager
		{
			get
			{
				if (this.graphicManager == null)
				{
					this.graphicManager = UtageToolKit.GetCompoentInChildrenCreateIfMissing<AdvGraphicManager>(this.transform);
					this.graphicManager.transform.localPosition = new Vector3(0,0,20);
				}
				return this.graphicManager;
			}
		}
		[SerializeField]
		AdvGraphicManager graphicManager;

		/// <summary>
		/// エフェクト管理
		/// </summary>
		public AdvEffectManager EffectManager { get { return this.effectManager ?? (this.effectManager = UtageToolKit.GetCompoentInChildrenCreateIfMissing<AdvEffectManager>(this.transform)); } }
		[SerializeField]
		AdvEffectManager effectManager;

		/// <summary>
		/// トランジション管理
		/// </summary>
		public AdvTransitionManager TransitionManager { get { return this.transitionManager ?? (this.transitionManager = FindObjectOfType<AdvTransitionManager>()); } }
		[SerializeField]
		AdvTransitionManager transitionManager;

		/// <summary>
		/// UI管理
		/// </summary>
		public AdvUiManager UiManager { get { return this.uiManager ?? (this.uiManager = FindObjectOfType<AdvUiManager>()); } }
		[SerializeField]
		AdvUiManager uiManager;

		/// <summary>
		/// サウンドマネージャー
		/// </summary>
		public SoundManager SoundManager { get { return this.soundManger ?? (this.soundManger = FindObjectOfType<SoundManager>()); } }
		[SerializeField]
		SoundManager soundManger;

		/// <summary>
		/// パラメータ管理
		/// </summary>
		public AdvParamSetting Param { get { return this.param; } }
		[SerializeField]
		AdvParamSetting param;

		/// <summary>
		/// カスタムコマンド用のコンポーネントリスト
		/// </summary>
		public List<AdvCustomCommandManager> CustomCommandManagerList
		{
			get
			{
				if(customCommandManagerList==null)
				{
					customCommandManagerList = new List<AdvCustomCommandManager>();
					this.GetComponentsInChildren<AdvCustomCommandManager>(true,customCommandManagerList);
				}
				return this.customCommandManagerList;
			}
		}
		List<AdvCustomCommandManager> customCommandManagerList;

		/// <summary>
		/// 初期化の際に呼ばれるコールバック
		/// </summary>
		public UnityEvent onPreInit;

		/// <summary>
		/// ダイアログ呼び出し
		/// </summary>
		public OpenDialogEvent OnOpenDialog
		{
			set { this.onOpenDialog = value; }
			get
			{
				//ダイアログイベントに登録がないなら、SystemUiのダイアログを使う
				if (this.onOpenDialog.GetPersistentEventCount() == 0)
				{
					if (SystemUi.GetInstance() != null)
					{
						onOpenDialog.AddListener(SystemUi.GetInstance().OpenDialog);
					}
				}
				return onOpenDialog;
			}
		}
		[SerializeField]
		OpenDialogEvent onOpenDialog;

		/// <summary>
		/// ページ内のテキストが変更されたら呼ばれる
		/// </summary>
		public AdvEvent OnPageTextChange { get { return this.onPageTextChange; } }
		[SerializeField]
		AdvEvent onPageTextChange;

		/// <summary>
		/// 起動時ロード待ちか判定
		/// </summary>
		public bool IsWaitBootLoading { get { return isWaitBootLoading; } }
		bool isWaitBootLoading = true;

		/// <summary>
		/// 起動したか
		/// </summary>
		public bool IsStarted { get { return isStarted; } }
		bool isStarted = false;

		/// <summary>
		/// シーン回想を再生中か
		/// </summary>
		public bool IsSceneGallery { get { return isSceneGallery; } }
		bool isSceneGallery = false;

		/// <summary>
		/// ロード待ちか判定
		/// </summary>
		public bool IsLoading
		{
			get
			{
				if (IsWaitBootLoading) return true;
				if (GraphicManager.IsLoading) return true;

				return ScenarioPlayer.IsWaitLoading;
			}
		}

		/// <summary>
		/// シナリオが終了したか判定
		/// </summary>
		public bool IsEndScenario
		{
			get
			{
				if (ScenarioPlayer == null ) return false;
				if (IsLoading) return false;

				return ScenarioPlayer.IsEndScenario;
			}
		}

		/// <summary>
		/// 初期化。スクリプトから動的に生成する場合に
		/// </summary>
		public void InitOnCreate( AdvLayerManager layerManager, AdvTransitionManager transitionManager, AdvUiManager uiManager )
		{
			this.layerManager = layerManager;
			this.transitionManager = transitionManager;
			this.uiManager = uiManager;
		}

		/// <summary>
		/// 設定されたエクスポートデータからゲームを開始
		/// </summary>
		/// <param name="rootDirResource">リソースディレクトリ</param>
		public void BootFromExportData(AdvSettingDataManager settingDataManager, AdvScenarioDataExported[] exportedScenarioDataTbl, string resourceDir )
		{
			StopAllCoroutines();
			Clear();
			isStarted = true;
			isWaitBootLoading = true;
			onPreInit.Invoke();
			DataManager.InitData( settingDataManager, exportedScenarioDataTbl);
			BootInit(resourceDir);
			isWaitBootLoading = false;
		}

		/// <summary>
		/// 指定のパスのゲームを開始
		/// </summary>
		/// <param name="url">ファイルパス</param>
		/// <param name="resourceDir">リソースディレクトリ</param>
		/// <param name="version">シナリオバージョン（-1以下で必ずサーバーからデータを読み直す）</param>
		public void BootFromCsv(string url, string resourceDir, int version )
		{
			StopAllCoroutines();
			Clear();
			isStarted = true;
			isWaitBootLoading = true;
			onPreInit.Invoke();
			StartCoroutine(LoadSettingDataCsvAsync(url, resourceDir, version));
		}

		IEnumerator LoadSettingDataCsvAsync(string url, string resourceDir, int version)
		{
			//ロード
			yield return StartCoroutine(DataManager.LoadCsvAsync(url, version));
			BootInit(resourceDir);
			isWaitBootLoading = false;
		}

		void OnClicked()
		{}


		public void Clear()
		{
			Page.Clear();
			SelectionManager.Clear();
			BacklogManager.Clear();
			GraphicManager.Clear();
			TransitionManager.Clear();
			if (UiManager!=null) UiManager.Close();
			SoundManager.StopBgm();
			SoundManager.StopAmbience();
			ClearCustomCommand();
			ScenarioPlayer.Clear();
		}

		/// <summary>
		/// シナリオ終了
		/// </summary>
		public void EndScenario()
		{
			ScenarioPlayer.EndScenario();
		}

		/// <summary>
		/// 起動時の初期化
		/// </summary>
		/// <param name="rootDirResource">ルートディレクトリのリソース</param>
		void BootInit(string rootDirResource )
		{
			//カスタムコマンドの初期化
			BootInitCustomCommand();

			DataManager.BootInit(rootDirResource);
			//設定データを反映
			GraphicManager.BootInit(this, DataManager.SettingDataManager.LayerSetting);

			//パラメーターをデフォルト値でリセット
			Param.InitDefaultAll(DataManager.SettingDataManager.DefaultParam);
			//パラメーターを反映
			GraphicInfo.CallbackExpression = Param.CalcExpressionBoolean;
			TextParser.CallbackCalcExpression += Param.CalcExpressionNotSetParam;
			iTweenData.CallbackGetValue += Param.GetParameter;

			//システムセーブデータの初期化＆ロード
			SystemSaveData.Init(this);
			//通常セーブデータの初期化
			SaveManager.Init();
			
			//シナリオデータのロードと初期化を開始
			DataManager.StartLoadAndInitScenariodData();

			//リソースファイル(画像やサウンド)のダウンロードをバックグラウンドで進めておく
			DataManager.StartBackGroundDownloadResource(StartScenarioLabel);
		}

		//カスタムコマンドの初期化
		public void BootInitCustomCommand()
		{
			AdvCommandParser.OnCreateCustomCommnadFromID = null;
			foreach (var item in CustomCommandManagerList)
			{
				item.OnBootInit();
			}
		}

		//カスタムコマンドの関係のクリア処理
		public void ClearCustomCommand()
		{
			foreach (var item in CustomCommandManagerList)
			{
				item.OnClear();
			}
		}


		/// <summary>
		/// システムセーブデータを書き込み
		/// </summary>
		public void WriteSystemData()
		{
			systemSaveData.Write();
		}

		/// <summary>
		/// セーブデータを書き込み
		/// </summary>
		/// <param name="saveData">書き込むセーブデータ</param>
		public void WriteSaveData(AdvSaveData saveData)
		{
			SaveManager.WriteSaveData(this, saveData);
		}

		/// <summary>
		/// セーブデータのロード
		/// </summary>
		/// <param name="saveData">ロードするセーブデータ</param>
		void LoadSaveData(AdvSaveData saveData)
		{
			Clear();
			saveData.LoadGameData(this);

			//古いセーブデータかを設定しておく
			ScenarioPlayer.IsOldVersion = (saveData.FileVersion <= AdvSaveData.Version2);

			StartScenario(saveData.CurrentSenarioLabel, saveData.CurrentPage, saveData.CurrentGallerySceneLabel);
		}

		/// <summary>
		/// クイックセーブ
		/// </summary>
		public void QuickSave()
		{
			WriteSaveData(SaveManager.QuickSaveData);
		}

		/// <summary>
		/// クイックロード
		/// </summary>
		/// <returns>成否</returns>
		public bool QuickLoad()
		{
			if (SaveManager.ReadQuickSaveData())
			{
				LoadSaveData(SaveManager.QuickSaveData);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// シナリオを一番最初から開始
		/// </summary>
		public void StartGame()
		{
			StartGame(StartScenarioLabel);
		}

		/// <summary>
		/// シナリオを指定のシーンから開始
		/// </summary>
		public void StartGame(string scenarioLabel)
		{
			isSceneGallery = false;
			StartGameSub(scenarioLabel);
		}

		void StartGameSub(string scenarioLabel)
		{
			StartCoroutine(CoStartGameSub(scenarioLabel));
		}

		IEnumerator CoStartGameSub(string scenarioLabel)
		{
			while (IsWaitBootLoading) yield return 0;

			//基本的なパラメーターをデフォルト値でリセット（システムデータ以外）
			Param.InitDefaultNormal(DataManager.SettingDataManager.DefaultParam);
			Clear();
			StartScenario(scenarioLabel, 0, "");
		}

		/// <summary>
		/// セーブデータをロードして開始
		/// </summary>
		/// <param name="saveData">ロードするセーブデータ</param>
		public void OpenLoadGame(AdvSaveData saveData)
		{
			isSceneGallery = false;
			LoadSaveData(saveData);
		}

		/// <summary>
		/// シーン回想を開始
		/// </summary>
		/// <param name="label">シーンラベル</param>
		public void StartSceneGallery(string label)
		{
			isSceneGallery = true;
			StartGameSub(label);
		}

		/// <summary>
		/// 指定のラベルにシナリオジャンプ
		/// </summary>
		/// <param name="label">ジャンプ先のラベル</param>
		public void JumpScenario(string label)
		{
			StartScenario(label, 0, ScenarioPlayer.CurrentGallerySceneLabel );
		}

		void StartScenario(string label, int page, string gallerySceneLabel)
		{
			StartCoroutine( CoStartScenario(label, page, gallerySceneLabel ));
		}

		IEnumerator CoStartScenario(string label, int page, string gallerySceneLabel)
		{
			while (IsWaitBootLoading) yield return 0;
			while (GraphicManager.IsLoading) yield return 0;
			while (SoundManager.IsLoading) yield return 0;
//			yield return 0;

			if (UiManager != null) UiManager.Open();
			ScenarioPlayer.StartScenario( label, page, gallerySceneLabel);
		}
	}
}
