//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


namespace Utage
{

	/// <summary>
	/// イベントを受け取ってSEを鳴らす
	/// </summary>
	[AddComponentMenu("Utage/Lib/UI/ButtonSe")]
	public class UguiButtonSe : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, ISubmitHandler, IMoveHandler
	{
		Selectable Selectable { get { return selectable ?? (selectable = GetComponent<Selectable>()); } }
		Selectable selectable;

		//クリック時のSE
		public AudioClip clicked;
		//ハイライト時のSE
		public AudioClip highlited;

		// クリックイベントでSEを鳴らす
		public void OnPointerClick(PointerEventData data)
		{
		
			PlayeSe(clicked);

		}

		// ハイライトでSEを鳴らす
		public void OnPointerEnter(PointerEventData data)
		{

			PlayeSe(highlited);
		}

		// 決定でSEを鳴らす
		public void OnSubmit(BaseEventData eventData)
		{

			PlayeSe(clicked);
		}

		// キー移動でSE鳴らす
		public void OnMove(AxisEventData eventData)
		{
			if (eventData.selectedObject == this.gameObject) return;
			PlayeSe(highlited);
		}

		void PlayeSe( AudioClip clip )
		{
			if (Selectable == null) return;
			if (!Selectable.interactable) return;

			if (clip != null)
			{
				SoundManager soundManger = SoundManager.GetInstance();

				if (soundManger)
				{
					soundManger.PlaySe(clip);
				}
				else
				{
					AudioSource.PlayClipAtPoint(clip, Vector3.zero);
				}
			}
		}
	}
}