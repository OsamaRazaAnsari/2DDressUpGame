//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System;
using System.Collections;
using UnityEngine;
using Utage;


/// <summary>
/// 画面管理コンポーネントの基本クラス（各画面制御はこれを継承する）
/// </summary>
public abstract class UtageUiView : MonoBehaviour
{

	//前の画面
	[SerializeField]
	protected UtageUiView prevView;

	/// <summary>
	/// 画面閉じたときのコールバック
	/// </summary>
	protected Action callBackClose;

	/// <summary>
	/// 画面を開く
	/// </summary>
	public virtual void Open()
	{
		Open(prevView, null);
	}

	/// <summary>
	/// 画面を開く
	/// </summary>
	/// <param name="callBackClose">閉じたときに呼ばれるコールバック</param>
	public virtual void Open(Action callBackClose)
	{
		Open(prevView, callBackClose);
	}

	/// <summary>
	/// 画面を開く
	/// </summary>
	/// <param name="prevView">前の画面</param>
	public virtual void Open(UtageUiView prevView)
	{
		Open(prevView, null);
	}

	/// <summary>
	/// 画面を開く
	/// </summary>
	/// <param name="prevView">前の画面</param>
	/// <param name="callBackClose">閉じたときに呼ばれるコールバック</param>
	public virtual void Open(UtageUiView prevView, Action callBackClose)
	{
		this.prevView = prevView;
		this.callBackClose = callBackClose;
		this.gameObject.SetActive(true);
		this.gameObject.SendMessage("OnOpen",SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// 画面を閉じる
	/// </summary>
	public virtual void Close()
	{
		this.gameObject.SendMessage("OnClose", SendMessageOptions.DontRequireReceiver);
		this.gameObject.SetActive(false);
		if (null != callBackClose) callBackClose();
	}

	/// <summary>
	/// 前の画面に戻る
	/// </summary>
	public virtual void Back()
	{
		Close();
		if (null != prevView) prevView.Open(prevView.prevView);
	}

	/// <summary>
	/// 戻るボタンが押された
	/// </summary>
	/// <param name="button">押されたボタン</param>
	protected virtual void OnTapBack(LegacyUiButton button)
	{
		Back();
	}
}
