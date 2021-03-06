//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Utage;
using System;


/// <summary>
/// バックログ用UI
/// </summary>
[AddComponentMenu("Utage/ADV/UiBacklog")]
[RequireComponent(typeof(Button))]	
public class AdvUguiBacklog : MonoBehaviour
{
	/// <summary>テキスト</summary>
	public UguiNovelText text;

	/// <summary>キャラ名</summary>
	public Text	characterName;

	/// <summary>ボイス再生アイコン</summary>
	public GameObject soundIcon;

	public Button Button { get { return button ?? (button = GetComponent<Button>()); } }
	Button button;

	public AdvBacklog Data { get { return data; } }
	AdvBacklog data;

	/// <summary>
	/// 初期化
	/// </summary>
	/// <param name="data">バックログのデータ</param>
	/// <param name="target">サウンドボタン押されたときのメッセージ送信先</param>
	/// <param name="index">バックログのインデックス</param>
	public void Init(Utage.AdvBacklog data, Action<AdvUguiBacklog> ButtonClickedEvent, int index)
	{
		this.data = data;
		text.text = data.Text;
		characterName.text = data.CharacterName;
		Button.onClick.AddListener(() => ButtonClickedEvent(this));
		if (!data.IsVoice)
		{
			soundIcon.SetActive(false);
			Button.interactable = false;
		}
	}
}

