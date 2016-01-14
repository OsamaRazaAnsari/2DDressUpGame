//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using Utage;



/// <summary>
/// タイトル表示のサンプル
/// </summary>
[AddComponentMenu("Utage/TemplateUI/Title")]
public class UtageUguiTitle : UguiView
{
	/// <summary>スターター</summary>
	public AdvEngineStarter Starter { get { return this.starter ?? (this.starter = FindObjectOfType<AdvEngineStarter>()); } }
	[SerializeField]
	AdvEngineStarter starter;
	public GameObject txt;
	public GameObject score;
    public GameObject token;
	/// <summary>メインゲーム画面</summary>
	public UtageUguiMainGame mainGame;

	/// <summary>コンフィグ画面</summary>
	public UtageUguiConfig config;

	/// <summary>セーブデターのロード画面</summary>
	public UtageUguiSaveLoad load;

	///<summary>ギャラリー画面</summary>
	public UtageUguiGallery gallery;

	///<summary>ダウンロード画面</summary>
	public UtageUguiLoadWait download;

	///<summary>ダウンロードボタン</summary>
	public GameObject downloadButton;

	void Update()
	{
		txt.SetActive (false);
		score.SetActive (false);
        token.SetActive(false);
	}
	void OnOpen()
	{
		if (Starter.ScenarioDataLoadType != AdvEngineStarter.LoadType.Server)
		{
			if (downloadButton != null)
			{
				downloadButton.SetActive(false);
			}
		}
	}
	///「はじめから」ボタンが押された
	public void OnTapStart()
	{
		Close();
		mainGame.OpenStartGame();
	}

	///「つづきから」ボタンが押された
	public void OnTapLoad()
	{
		Close();
		load.OpenLoad(this);
	}

	///「コンフィグ」ボタンが押された
	public void OnTapConfig()
	{
		Close();
		config.Open(this);
	}
	
	//「ギャラリー」ボタンが押された
	public void OnTapGallery()
	{
		Close();
		gallery.Open(this);
	}

	//「ダウンロード」ボタンが押された
	public void OnTapDownLoad()
	{
		Close();
		download.Open(this);
	}
}
