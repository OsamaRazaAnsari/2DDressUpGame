//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using Utage;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// ロード待ち画面のサンプル
/// </summary>
[AddComponentMenu("Utage/TemplateUI/LoadWait")]
public class UtageUguiLoadWait : UguiView
{
	/// <summary>ADVエンジン</summary>
	public AdvEngine Engine { get { return this.engine ?? (this.engine = FindObjectOfType<AdvEngine>() as AdvEngine); } }
	[SerializeField]
	AdvEngine engine;

	/// <summary>スターター</summary>
	public AdvEngineStarter Starter { get { return this.starter ?? (this.starter = FindObjectOfType<AdvEngineStarter>()); } }
	[SerializeField]
	AdvEngineStarter starter;

	public bool isAutoCacheFileLoad;

	public UtageUguiTitle title;

	public string bootSceneName;

	public GameObject buttonSkip;
	public GameObject buttonBack;
	public GameObject buttonDownload;		
	public GameObject loadingBarRoot;
	public Image loadingBar;
	public Text textMain;
	public Text textCount;
	
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

	enum State
	{
		Start,
		Downloding,
		DownlodFinished,
	};
	State CurrentState { get; set; }
	bool isOpenOnBoot = false;


	//起動時に開く
	public void OpenOnBoot()
	{
		isOpenOnBoot = true;
		this.Open();
	}
	void OnClose()
	{
		isOpenOnBoot = false;
	}

	void OnOpen()
	{
		if (isOpenOnBoot)
		{
			buttonBack.gameObject.SetActive(false);
			buttonSkip.gameObject.SetActive(true);
		}
		else
		{
			buttonBack.gameObject.SetActive(true);
			buttonSkip.gameObject.SetActive(false);
		}

		if (!Engine.IsStarted)
		{
			ChangeState(State.Start);
		}
		else
		{
			ChangeState(State.Downloding);
		}
	}

	void ChangeState(State state)
	{
		this.CurrentState = state;
		switch(state)
		{
			case State.Start:
				buttonDownload.SetActive(true);
				loadingBarRoot.SetActive(false);
				textMain.text = "";
				textCount.text = "";
				StartLoadEngine();
				break;
			case State.Downloding:
				buttonDownload.SetActive(false);
				StartCoroutine(CoUpdateLoading());
				break;
			case State.DownlodFinished:
				if (isOpenOnBoot)
				{
					this.Close();
					title.Open();
				}
				else
				{
					buttonDownload.SetActive(false);
					loadingBarRoot.SetActive(false);
					textMain.text = LanguageSystemText.LocalizeText(SystemText.DownloadFinished);
					textCount.text = "";
				}
				break;
		}
	}

	//スキップボタン
	public void OnTapSkip()
	{
		this.Close();
		title.Open();
	}
	
	//ｷｬｯｼｭｸﾘｱして最初のシーンを起動
	public void OnTapReDownload()
	{
		AssetFileManager.DeleteCacheFileAll();
		if (string.IsNullOrEmpty(bootSceneName))
		{
			Application.LoadLevel(0);
		}
		else
		{
			Application.LoadLevel(bootSceneName);
		}
	}

	//ローディング中の表示
	IEnumerator CoUpdateLoading()
	{
		int maxCountDownLoad = 0;
		int countDownLoading = 0;
		loadingBarRoot.SetActive(true);
		loadingBar.fillAmount = 1.0f;
		textMain.text = LanguageSystemText.LocalizeText(SystemText.Downloading);
		textCount.text = "--/--";
		while (Engine.IsWaitBootLoading) yield return 0;

		yield return 0;

		while (!AssetFileManager.IsDownloadEnd())
		{
			yield return 0;
			countDownLoading = AssetFileManager.CountDownloading();
			maxCountDownLoad = Mathf.Max(maxCountDownLoad, countDownLoading);
			textCount.text = (maxCountDownLoad - countDownLoading) + "/" + maxCountDownLoad;
			if (maxCountDownLoad > 0)
			{
				loadingBar.fillAmount = (1.0f * countDownLoading / maxCountDownLoad);
			}
		}
		loadingBarRoot.gameObject.SetActive(false);
		ChangeState(State.DownlodFinished);
	}

	//ロード開始
	void StartLoadEngine()
	{
		switch (Starter.ScenarioDataLoadType)
		{
			case AdvEngineStarter.LoadType.Local:
				Starter.LoadEngine();
				ChangeState(State.Downloding);
				break;
			case AdvEngineStarter.LoadType.Server:
				StartCoroutine(CoStartFromServer());
				break;
		}
	}


	//サーバーから起動する時にネットワークエラーをチェックする
	IEnumerator CoStartFromServer()
	{
		string url = Starter.UrlScenarioData;
		int scenarioVersion =Starter.ScenarioVersion;

		int version = scenarioVersion;

		bool isRetry = false;
		do
		{
			bool isWaiting = false;
			isRetry = false;
			version = scenarioVersion;
			//ネットワークのチェック(モバイルのみ)
			switch (Application.internetReachability)
			{
				case NetworkReachability.NotReachable:						//ネットにつながらない
					if (scenarioVersion < 0)
					{
						AssetFile file = AssetFileManager.GetFileCreateIfMissing(url);
						if(file.CacheVersion >= 0)
						{
							version = file.CacheVersion;
							if (!isAutoCacheFileLoad)
							{
								isWaiting = true;
								string text = LanguageSystemText.LocalizeText(SystemText.WarningNotOnline);
								List<ButtonEventInfo> buttons = new List<ButtonEventInfo>
										{
											new ButtonEventInfo(
												LanguageSystemText.LocalizeText(SystemText.Yes)
												, ()=>isWaiting=false
											),
											new ButtonEventInfo(
												LanguageSystemText.LocalizeText(SystemText.Retry)
												, ()=>{isWaiting=false;isRetry=true;}
											),
										};
								OnOpenDialog.Invoke(text, buttons);
							}
						}
					}
					break;
				case NetworkReachability.ReachableViaCarrierDataNetwork:	//キャリア
				case NetworkReachability.ReachableViaLocalAreaNetwork:		//Wifi
				default:
					break;
			}
			while (isWaiting)
			{
				yield return 0;
			}
		} while (isRetry);
		
		Starter.LoadEngine(version);
		ChangeState(State.Downloding);
	}

}
