//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;

/// <summary>
/// セーブロード画面のサンプル
/// </summary>
[AddComponentMenu("Utage/Legacy/Examples/SaveLoad")]
public class UtageUiSaveLoad : UtageUiView
{
	/// <summary>
	/// リストビュー
	/// </summary>
	public LegacyUiListView listView;

	/// <summary>
	/// リストビューアイテムのリスト
	/// </summary>
	List<AdvSaveData> itemDataList;

	/// <summary>ADVエンジン</summary>
	public AdvEngine Engine { get { return this.engine ?? (this.engine = FindObjectOfType<AdvEngine>() as AdvEngine); } }
	[SerializeField]
	AdvEngine engine;

	/// <summary>メイン画面</summary>
	public UtageUiMainGame mainGame;

	/// <summary>タイトル表記（セーブ画面かロード画面か）</summary>
	public TextArea2D title;

	bool isSave;

	bool isInit = false;


	/// <summary>
	/// セーブ画面を開く
	/// </summary>
	/// <param name="prev">前の画面</param>
	public void OpenSave(UtageUiView prev)
	{
		isSave = true;
		title.text = LanguageSystemText.LocalizeText(SystemText.Save);
		Open(prev);
	}

	/// <summary>
	/// ロード画面を開く
	/// </summary>
	/// <param name="prev">前の画面</param>
	public void OpenLoad(UtageUiView prev)
	{
		isSave = false;
		title.text = LanguageSystemText.LocalizeText(SystemText.Load);
		Open(prev);
	}

	/// <summary>
	/// オープンしたときに呼ばれる
	/// </summary>
	void OnOpen()
	{
		isInit = false;
		this.listView.Close();	///いったん閉じる
		StartCoroutine(CoWaitOpen());
	}

	/// <summary>
	/// クローズしたときに呼ばれる
	/// </summary>
	void OnClose()
	{
		this.listView.Close();
	}

	//起動待ちしてから開く
	IEnumerator CoWaitOpen()
	{
		while (Engine.IsWaitBootLoading)
		{
			yield return 0;
		}
		AdvSaveManager saveManager = Engine.SaveManager;
		saveManager.ReadAllSaveData();
		List<AdvSaveData> list = new List<AdvSaveData>();
		if (saveManager.IsAutoSave) list.Add(saveManager.AutoSaveData);
		foreach( var data in saveManager.SaveDataList ){
			list.Add(data);
		}
		this.itemDataList = list;
		listView.Open(itemDataList.Count, CallBackCreateItem);
		isInit = true;
	}


	/// <summary>
	/// リストビューのアイテムが作成されるときに呼ばれるコールバック
	/// </summary>
	/// <param name="go">作成されたアイテムのGameObject</param>
	/// <param name="index">作成されたアイテムのインデックス</param>
	void CallBackCreateItem(GameObject go, int index)
	{
		UtageUiSaveLoadItem item = go.GetComponent<UtageUiSaveLoadItem>();
		AdvSaveData data = itemDataList[index];
		item.Init(data, index, isSave );
		LegacyUiButton button = go.GetComponent<LegacyUiButton>();
		button.Target = this.gameObject;
	}

	void Update()
	{
		//右クリックで戻る
		if (isInit && InputUtil.IsMousceRightButtonDown())
		{
			Back();
		}
	}


	/// <summary>
	/// 各アイテムが押された
	/// </summary>
	/// <param name="button">押されたアイテム</param>
	void OnTap(LegacyUiButton button)
	{
		AdvSaveData data = itemDataList[button.Index];
		if (isSave)
		{
			UtageUiSaveLoadItem item = button.GetComponent<UtageUiSaveLoadItem>();

			//セーブ画面なら、セーブ処理
			Engine.WriteSaveData(data);
			item.Init( data, button.Index, isSave);
		}
		else
		{
			//ロード画面
			if (data.IsSaved)
			{
				//セーブ済みのデータならこの画面は閉じてロードをする
				Close();
				mainGame.OpenLoadGame(data);
			}
		}
	}
}
