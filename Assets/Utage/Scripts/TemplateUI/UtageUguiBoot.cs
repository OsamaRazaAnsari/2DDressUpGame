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
/// タイトル表示のサンプル
/// </summary>
[AddComponentMenu("Utage/TemplateUI/Title")]
public class UtageUguiBoot : UguiView
{
	/// <summary>ADVエンジン</summary>
	public AdvEngine Engine { get { return this.engine ?? (this.engine = FindObjectOfType<AdvEngine>() as AdvEngine); } }
	[SerializeField]
	AdvEngine engine;

	public UguiFadeTextureStream fadeTextureStream;
	
	public UtageUguiTitle title;
	public UtageUguiMainGame maingame;
	public UtageUguiLoadWait loadWait;

	public bool isWaitDownLoad;

	///最初の画面なので自分でオープンする
	public void Start()
	{
		title.gameObject.SetActive(false);
		StartCoroutine(CoUpdate());
	}

	///
	IEnumerator CoUpdate()
	{
		if (fadeTextureStream)
		{
			fadeTextureStream.gameObject.SetActive(true);
			fadeTextureStream.Play();
			while (fadeTextureStream.IsPlaying) yield return 0;
		}
		this.Close();
		if (isWaitDownLoad && loadWait != null)
		{
			loadWait.OpenOnBoot();
		}
		else
		{
			if(Application.loadedLevelName=="Story1"){
			//	Debug.Log("Story1");

				title.Open();

			}else{

				//Debug.Log("Story2");
				
							maingame.Open();
							maingame.OpenStartGame();
			}

		}
	}
}
