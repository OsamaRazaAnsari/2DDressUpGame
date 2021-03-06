//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using Utage;

/// <summary>
/// メッセージウィドウ処理のサンプル
/// </summary>
[AddComponentMenu("Utage/ADV/UiMessageWindow")]
public class AdvUguiMessageWindow : MonoBehaviour
{
	/// <summary>ADVエンジン</summary>
	[SerializeField]
	AdvEngine engine;

	/// <summary>本文テキスト</summary>
	[SerializeField]
	UguiNovelText text;

	/// <summary>名前表示テキスト</summary>
	[SerializeField]
	Text nameText;

	/// <summary>ウインドウのルート</summary>
	[SerializeField]
	GameObject rootChildren;

	/// <summary>コンフィグの透明度を反映させるUIのルート</summary>
	[SerializeField]
	CanvasGroup transrateMessageWindowRoot;

	/// <summary>改ページ待ちアイコン</summary>
	[SerializeField]
	GameObject iconBrPage;

	[SerializeField]
	bool isLinkPositionIconBrPage = true;

	AdvUguiMessageWindowBgController MessageWindowBgController { get { return messageWindowBgController ?? (messageWindowBgController = GetComponent<AdvUguiMessageWindowBgController>()); } }
	[SerializeField]
	AdvUguiMessageWindowBgController messageWindowBgController;

	void Awake()
	{
		engine.OnPageTextChange.AddListener( (AdvEngine arg)=>OnTextChanged(arg) );
	}

	/// <summary>
	/// ウィンドウのクローズ
	/// </summary>
	public void Close()
	{
		this.gameObject.SetActive(false);
		rootChildren.SetActive(false);
	}

	/// <summary>
	/// ウィンドウのオープン
	/// </summary>
	public void Open()
	{
		this.gameObject.SetActive(true);
		rootChildren.SetActive(false);
	}

	void Update()
	{
		if (engine.UiManager.Status == AdvUiManager.UiStatus.Default)
		{
			rootChildren.SetActive( !engine.UiManager.IsHide );
			if (!engine.UiManager.IsHide)
			{
				//ウィンドのアルファ値反映
				transrateMessageWindowRoot.alpha = engine.Config.MessageWindowAlpha;
				//テキスト表示の反映
				engine.Page.UpdateText();
				text.LengthOfView = engine.Page.CurrentTextLen;
			}
			LinkIcon();
		}
	}

	//テキストに変更があった場合
	public void OnTextChanged( AdvEngine engine )
	{
		//パラメーターを反映させるために、一度クリアさせてからもう一度設定
		nameText.text = "";
		nameText.text = engine.Page.NameText;
		text.text = "";
		text.text = engine.Page.TextData.ParsedText.OriginalText;
		text.LengthOfView = engine.Page.CurrentTextLen;
		if (MessageWindowBgController)
		{
			MessageWindowBgController.ChangeWindowType(engine.Page.WindowType);
		}
		LinkIcon();
	}

	//アイコンの場所をテキストの末端にあわせる
	void LinkIcon()
	{
		if (iconBrPage == null) return;

		if (engine.UiManager.IsHide)
		{
			iconBrPage.SetActive(false);
		}
		else
		{
			iconBrPage.SetActive(engine.Page.IsWaitBrPage);
			if (isLinkPositionIconBrPage) iconBrPage.transform.localPosition = text.CurrentEndPosition;
		}
	}
	//ウインドウ閉じるボタンが押された
	public void OnTapCloseWindow()
	{
		engine.UiManager.Status = AdvUiManager.UiStatus.HideMessageWindow;
	}

	//バックログボタンが押された
	public void OnTapBackLog()
	{
		engine.UiManager.Status = AdvUiManager.UiStatus.Backlog;
	}
}

