//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using Utage;
using System.Collections;



/// <summary>
/// コンフィグ画面のサンプル
/// </summary>
[AddComponentMenu("Utage/Legacy/Examples/Config")]
public class UtageUiConfig : UtageUiView
{
	/// <summary>ADVエンジン</summary>
	public AdvEngine Engine { get { return this.engine ?? (this.engine = FindObjectOfType<AdvEngine>() as AdvEngine); } }
	[SerializeField]
	AdvEngine engine;

	//コンフィグデータへのインターフェース
	AdvConfig Config { get { return Engine.Config; } }

	/// <summary>タイトル画面</summary>
	[SerializeField]
	UtageUiTitle title;

	/// <summary>「フルスクリーン表示」のチェックボックス</summary>
	[SerializeField]
	LegacyUiCheckBox checkFullscreen;

	/// <summary>「マウスホイールでメッセージ送り」のチェックボックス</summary>
	[SerializeField]
	LegacyUiCheckBox checkMouseWheel;

	/// <summary>「未読スキップ」のチェックボックス</summary>
	[SerializeField]
	LegacyUiCheckBox checkSkipUnread;

	/// <summary>「選択肢でスキップを解除」チェックボックス</summary>
	[SerializeField]
	LegacyUiCheckBox checkStopSkipInSelection;

	/// <summary>「メッセージ速度」のスライダー</summary>
	[SerializeField]
	LegacyUiSlider sliderMessageSpeed;

	/// <summary>「自動メッセージ速度」のスライダー</summary>
	[SerializeField]
	LegacyUiSlider sliderAutoBrPageSpeed;

	/// <summary>「ウインドウの透明度」のスライダー</summary>
	[SerializeField]
	LegacyUiSlider sliderMessageWindowTransparency;

	/// <summary>「サウンド」の音量スライダー</summary>
	[SerializeField]
	LegacyUiSlider sliderSoundMasterVolume;

	/// <summary>「BGM」の音量スライダー</summary>
	[SerializeField]
	LegacyUiSlider sliderBgmVolume;

	/// <summary>「SE」の音量スライダー</summary>
	[SerializeField]
	LegacyUiSlider sliderSeVolume;

	/// <summary>「環境音」の音量スライダー</summary>
	[SerializeField]
	LegacyUiSlider sliderAmbienceVolume;

	/// <summary>「ボイス」の音量スライダー</summary>
	[SerializeField]
	LegacyUiSlider sliderVoiceVolume;

	/// <summary>音声の再生タイプのラジオボタン</summary>
	[SerializeField]
	LegacyUiRadioButtonGroup radioButtonsVoiceStopType;

	bool isInit = false;


	/// <summary>
	/// オープンしたときに呼ばれる
	/// </summary>
	void OnOpen()
	{
		isInit = false;
		//スクショをクリア
		Engine.SaveManager.ClearCaptureTexture();
		StartCoroutine(CoWaitOpen());
	}


	//起動待ちしてから開く
	IEnumerator CoWaitOpen()
	{
		while (Engine.IsWaitBootLoading) yield break;
		LoadValues();
		isInit = true;
	}

	/// <summary>
	/// 画面を閉じる処理
	/// </summary>
	public override void Close()
	{
		Engine.WriteSystemData();
		base.Close();
	}

	void Update()
	{
		//右クリックで戻る
		if (isInit && InputUtil.IsMousceRightButtonDown())
		{
			Back();
		}
	}

	//各UIに値を反映
	void LoadValues()
	{
		checkFullscreen.IsChecked = Config.IsFullScreen;
		checkMouseWheel.IsChecked = Config.IsMouseWheelSendMessage;
		checkSkipUnread.IsChecked = Config.IsSkipUnread;
		checkStopSkipInSelection.IsChecked = Config.IsStopSkipInSelection;

		sliderMessageSpeed.SliderValue = Config.MessageSpeed;
		sliderAutoBrPageSpeed.SliderValue = Config.AutoBrPageSpeed;
		sliderMessageWindowTransparency.SliderValue = Config.MessageWindowTransparency;
		sliderSoundMasterVolume.SliderValue = Config.SoundMasterVolume;
		sliderBgmVolume.SliderValue = Config.BgmVolume;
		sliderSeVolume.SliderValue = Config.SeVolume;
		sliderAmbienceVolume.SliderValue = Config.AmbienceVolume;
		sliderVoiceVolume.SliderValue = Config.VoiceVolume;

		radioButtonsVoiceStopType.RadioIndex = ((int)Config.VoiceStopType);

		//フルスクリーンやマウスホイールは、PC版のみの操作
		if (!UtageToolKit.IsPlatformStandAloneOrEditor())
		{
			checkFullscreen.gameObject.SetActive(false);
			checkMouseWheel.gameObject.SetActive(false);
		}
	}

	//タイトルに戻る
	void OnTapBackTitle()
	{
		callBackClose = null;
		Engine.EndScenario();
		this.Close();
		title.Open();
	}

	//全てデフォルト値で初期化
	void OnTapInitDefaultAll()
	{
		Config.InitDefaultAll();
		LoadValues();
	}

	//フルスクリーン切り替え
	void OnTapToggleFullScreen()
	{
		Config.ToggleFullScreen();
	}

	//マウスホイールでメッセージ送り切り替え
	void OnTapToggleMouseWheel()
	{
		Config.ToggleMouseWheelSendMessage();
	}
	
	//エフェクトON・OFF切り替え
	void OnTapToggleEffect()
	{
		Config.ToggleEffect();
	}

	//未読スキップON・OFF切り替え
	void OnTapToggleSkipUnread()
	{
		Config.ToggleSkipUnread();
	}

	//選択肢でスキップ解除ON・OFF切り替え
	void OnTapToggleStopSkipInSelection()
	{
		Config.ToggleStopSkipInSelection();
	}

	//文字送り速度
	void OnSliderChangeMessageSpeed(float val)
	{
		Config.MessageSpeed = val;
	}
	//オート文字送り速度
	void OnSliderChangeAutoBrPageSpeed(float val)
	{
		Config.AutoBrPageSpeed = val;
	}
	//メッセージウィンドウの透過色（バー）
	void OnSliderChangeMessageWindowTransparency(float val)
	{
		Config.MessageWindowTransparency = val;
	}

	//音量設定 サウンド全体
	void OnSliderChangeSoundMasterVolume(float val)
	{
		Config.SoundMasterVolume = val;
	}
	//音量設定 BGM
	void OnSliderChangeBgmVolume(float val)
	{
		Config.BgmVolume = val;
	}
	//音量設定 SE
	void OnSliderChangeSeVolume(float val)
	{
		Config.SeVolume = val;
	}
	//音量設定 環境音
	void OnSliderChangeAmbienceVolume(float val)
	{
		Config.AmbienceVolume = val;
	}
	//音量設定 ボイス
	void OnSliderChangeVoiceVolume(float val)
	{
		Config.VoiceVolume = val;
	}

	//音声設定（クリックで停止、次の音声まで再生を続ける）
	void OnTapRadioButtonVoiceStopType(LegacyUiRadioButtonGroup radioGroup)
	{
		Config.VoiceStopType = (VoiceStopType)radioGroup.RadioIndex;
	}
/*
	//キャラ別ボイスのON・OFF
	void OnTapToggleVoiceActive(int index)
	{
		config.ToggleVoiceActive(index);
	}*/
}
