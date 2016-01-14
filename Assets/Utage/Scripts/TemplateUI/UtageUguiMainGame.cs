//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utage;


/// <summary>
/// メインゲーム画面のサンプル
/// 入力処理に起点になるため、スクリプトの実行順を通常よりも少しはやくすること
/// http://docs-jp.unity3d.com/Documentation/Components/class-ScriptExecution.html
/// </summary>
[AddComponentMenu("Utage/TemplateUI/MainGame")]
public class UtageUguiMainGame : UguiView
{
	public GameObject txt;
	public GameObject score;

	/// <summary>ADVエンジン</summary>
	public AdvEngine Engine { get { return this.engine ?? (this.engine = FindObjectOfType<AdvEngine>()); } }
	[SerializeField]
	AdvEngine engine;

	/// <summary>ADVエンジン</summary>
	public UguiLetterBoxCamera LetterBoxCamera { get { return this.letterBoxCamera ?? (this.letterBoxCamera = FindObjectOfType<UguiLetterBoxCamera>()); } }
	[SerializeField]
	UguiLetterBoxCamera letterBoxCamera;


	/// <summary>タイトル画面</summary>
	public UtageUguiTitle title;
	
	/// <summary>コンフィグ画面</summary>
	public UtageUguiConfig config;

	/// <summary>セーブロード画面</summary>
	public UtageUguiSaveLoad saveLoad;

	/// <summary>ギャラリー画面</summary>
	public UtageUguiGallery gallery;

	/// <summary>ボタン</summary>
	public GameObject buttons;

	/// <summary>スキップボタン</summary>
	public Toggle checkSkip;

	/// <summary>自動で読み進むボタン</summary>
	public Toggle checkAuto;

	//起動タイプ
	enum BootType
	{
		Default,
		Start,
		Load,
		SceneGallery,
	};
	BootType bootType;

	//ロードするセーブデータ
	AdvSaveData loadData;

	bool isInit = false;

	/// <summary>起動するシナリオラベル</summary>
	string scenaioLabel;


	/// <summary>
	/// 画面を閉じる
	/// </summary>
	public override void Close()
	{
		base.Close();
		Engine.UiManager.Close();
		Engine.Config.IsSkip = false;
	}

	//起動データをクリア
	void ClearBootData()
	{
		bootType = BootType.Default;
		isInit = false;
		loadData = null;
	}

	/// <summary>
	/// ゲームをはじめから開始
	/// </summary>
	public void OpenStartGame()
	{
		ClearBootData();
		bootType = BootType.Start;
		Open();
	}

	/// <summary>
	/// セーブデータをロードしてゲーム再開
	/// </summary>
	/// <param name="loadData">ロードするセーブデータ</param>
	public void OpenLoadGame(AdvSaveData loadData)
	{
		ClearBootData();
		bootType = BootType.Load;
		this.loadData = loadData;
		Open();
	}

	/// <summary>
	/// シーン回想としてシーンを開始
	/// </summary>
	/// <param name="scenaioLabel">シーンラベル</param>
	public void OpenSceneGallery(string scenaioLabel)
	{
		ClearBootData();
		bootType = BootType.SceneGallery;
		this.scenaioLabel = scenaioLabel;
		Open();
	}

	/// <summary>
	/// オープンしたときに呼ばれる
	/// </summary>
	void OnOpen()
	{
		//スクショをクリア
		Engine.SaveManager.ClearCaptureTexture();
		StartCoroutine(CoWaitOpen());
	}


	//起動待ちしてから開く
	IEnumerator CoWaitOpen()
	{
		while (Engine.IsWaitBootLoading) yield return 0;

		switch (bootType)
		{
			case BootType.Default:
				Engine.UiManager.Open();
				break;
			case BootType.Start:
				Engine.StartGame();
				break;
			case BootType.Load:
				Engine.OpenLoadGame(loadData);
				break;
			case BootType.SceneGallery:
				Engine.StartSceneGallery(scenaioLabel);
				break;
		}
		ClearBootData();
		loadData = null;
		Engine.Config.IsSkip = false;
		isInit = true;
	}

	//更新中
	void Update()
	{


		if (!isInit) return;

		if (LegacyUiSystemUi.GetInstance())
		{
			if (Engine.IsLoading)
			{
				LegacyUiSystemUi.GetInstance().StartIndicator(this);
			}
			else
			{
				LegacyUiSystemUi.GetInstance().StopIndicator(this);
			}
		}
		if (SystemUi.GetInstance())
		{
			if (Engine.IsLoading)
			{
				SystemUi.GetInstance().StartIndicator(this);
			}
			else
			{
				SystemUi.GetInstance().StopIndicator(this);
			}
		}


		if (Engine.IsEndScenario)
		{
			Close();
			if (Engine.IsSceneGallery)
			{
				//回想シーン終了したのでギャラリーに
				gallery.Open();
			}
			else
			{
				//シナリオ終了したのでタイトルへ
				title.Open(this);
			}
		}
	}

	void LateUpdate()
	{
		buttons.SetActive(Engine.Page.IsWaitPage && Engine.UiManager.Status == AdvUiManager.UiStatus.Default );

		//スキップフラグを反映
		if (checkSkip)
		{
			if (checkSkip.isOn != Engine.Config.IsSkip)
			{
				checkSkip.isOn = Engine.Config.IsSkip;
			}
		}
		//オートフラグを反映
		if (checkAuto)
		{
			if (checkAuto.isOn != Engine.Config.IsAutoBrPage)
			{
				checkAuto.isOn = Engine.Config.IsAutoBrPage;
			}
		}
	}

	//スキップボタンが押された
	public void OnTapSkip( bool isOn )
	{
		Engine.Config.IsSkip = isOn;
	}

	//自動読み進みボタンが押された
	public void OnTapAuto( bool isOn )
	{
		Engine.Config.IsAutoBrPage = isOn;
	}

	//コンフィグボタンが押された
	public void OnTapConfig()
	{
		Close();
		config.Open(this);
	}

	//セーブボタンが押された
	public void OnTapSave()
	{
		if (Engine.IsSceneGallery) return;

		StartCoroutine(CoSave());
	}
	IEnumerator CoSave()
	{
		yield return new WaitForEndOfFrame();
		//セーブ用のスクショを撮る
		Engine.SaveManager.CaptureTexture = LetterBoxCamera.CaptureScreen();
		//セーブ画面開く
		Close();
		saveLoad.OpenSave(this);
	}

	//ロードボタンが押された
	public void OnTapLoad()
	{
		if (Engine.IsSceneGallery) return;
		
		Close();
		saveLoad.OpenLoad(this);
	}

	//クイックセーブボタンが押された
	public void OnTapQSave()
	{
		if (Engine.IsSceneGallery) return;
		
		Engine.Config.IsSkip = false;
		StartCoroutine(CoQSave());
	}
	IEnumerator CoQSave()
	{
		yield return new WaitForEndOfFrame();
		//セーブ用のスクショを撮る
		Engine.SaveManager.CaptureTexture = LetterBoxCamera.CaptureScreen();
		//クイックセーブ
		Engine.QuickSave();
		//スクショをクリア
		Engine.SaveManager.ClearCaptureTexture();
	}

	//クイックロードボタンが押された
	public void OnTapQLoad()
	{
		if (Engine.IsSceneGallery) return;
		
		Engine.Config.IsSkip = false;
		Engine.QuickLoad();
	}
}
