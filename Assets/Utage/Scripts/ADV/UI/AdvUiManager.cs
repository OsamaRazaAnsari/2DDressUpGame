//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;

namespace Utage
{
	/// <summary>
	/// UI全般の管理
	/// </summary>
	public abstract class AdvUiManager : MonoBehaviour
	{
		[SerializeField]
		protected AdvEngine engine;

		//状態
		public UiStatus Status
		{
			get { return status; }
			set
			{
				if (this.status == value) return;
				ChangeStatus(value);

			}
		}
		public enum UiStatus
		{
			Default,			//通常
			Backlog,			//バックログ
			HideMessageWindow,	//メッセージウィンドウ非表示
		};
		protected UiStatus status;
		PointerEventData currenPointerData;

		public PointerEventData CurrenPointerData
		{
			get { return currenPointerData; }
		}
		public bool IsPointerDowned
		{
			get { return currenPointerData != null; }
		}

		public abstract void Open();

		public abstract void Close();

		protected abstract void ChangeStatus(UiStatus newStatus);

		public virtual void OnPointerDown(PointerEventData data)
		{
			currenPointerData = data;
		}
		protected virtual void LateUpdate()
		{
			currenPointerData = null;
		}

		public bool IsHide
		{
			get
			{
				return !engine.Page.IsShowingText;
			}
		}
	}
}