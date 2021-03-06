//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Utage
{

	/// <summary>
	/// インデックス管理できるToggledGroup
	/// </summary>
	[AddComponentMenu("Utage/Lib/UI/ToggledGroupIndexd")]
	public class UguiToggleGroupIndexed : MonoBehaviour
	{
		public Toggle[] TogglesToArray {get { return this.toggles.ToArray(); }}
		[SerializeField]
		List<Toggle> toggles;

		public int firstIndexOnAwake = 0;
		public bool ignoreValueChangeOnAwake = true;

		public bool autoToggleInteractiveOff = true;


		//シフト移動時にループするか
		public bool isLoopShift = true;

		//左にシフトするボタン
		public Button shiftLeftButton;
		//右にシフトするボタン
		public Button shiftRightButton;
		//左端にジャンプするボタン
		public Button jumpLeftEdgeButton;
		//右端にジャンプするボタン
		public Button jumpRightEdgeButton;

		//現在のインデックス
		public int CurrentIndex
		{
			get { return currentIndex; }
			set
			{
				if (value < toggles.Count)
				{
					for( int i = 0; i < toggles.Count; ++i )
					{
						bool isOn = ( i == value);
						toggles[i].isOn = isOn;
						if(autoToggleInteractiveOff)
						{
							toggles[i].interactable = !isOn;
						}						
//						Debug.Log( i  + " " + toggles[i].isOn );
					}
					if(currentIndex!=value)
					{
						currentIndex = value;
						this.OnValueChanged.Invoke(value);
					}
				}
			}
		}
		int currentIndex = -1;

		//ボタンの数
		public int Count
		{
			get { return toggles.Count; }
		}

		
		[System.Serializable]
		public class UguiTabButtonGroupEvent : UnityEvent<int> { };
		public UguiTabButtonGroupEvent OnValueChanged;

		void Awake()
		{
			for( int i = 0; i < toggles.Count; ++i )
			{
				Toggle toggle = toggles[i];
				toggle.onValueChanged.AddListener( ( bool isOn )=>OnToggleValueChanged(toggle) );
			}
			if(ignoreValueChangeOnAwake) currentIndex = firstIndexOnAwake;
			CurrentIndex = firstIndexOnAwake;

			if (shiftLeftButton) shiftLeftButton.onClick.AddListener(ShiftLeft);
			if (shiftRightButton) shiftRightButton.onClick.AddListener(ShiftRight);
			if (jumpLeftEdgeButton) jumpLeftEdgeButton.onClick.AddListener(JumpLeftEdge);
			if (jumpRightEdgeButton) jumpRightEdgeButton.onClick.AddListener(JumpRightEdge);
		}

		bool isIgnoreValueChange;
		void OnToggleValueChanged( Toggle toggle )
		{
//			Debug.Log (toggle.name + " " + toggle.isOn);
			if (isIgnoreValueChange) return;
			isIgnoreValueChange = true;
			CurrentIndex = toggles.FindIndex( (Toggle obj) => (obj == toggle) );
//			Debug.Log (CurrentIndex);
//			Debug.Log ( "Real " + toggles.FindIndex( (Toggle obj) => obj.isOn ) );
			isIgnoreValueChange = false;
		}

		public void Add( Toggle toggle)
		{
			toggles.Add (toggle);
			toggle.onValueChanged.AddListener( ( bool isOn )=>OnToggleValueChanged(toggle) );
		}

		public void ClearToggles()
		{
			toggles.Clear();
		}
/*
		public void Clear()
		{
			TabButtonList.Clear();
			UtageToolKit.DestroyChildren(toggleGroup.transform);
			SetActiveLRButtons(false);
		}
		
		public List<UguiTabButton> CreateTabButtons(int num, System.Action<GameObject, int> callbackCreateItem)
		{
			Clear();
			
			if(num >=1) SetActiveLRButtons(true);
			for (int i = 0; i < num; ++i)
			{
				GameObject go = UtageToolKit.AddChildPrefab(toggleGroup.transform, buttonPrefab);
				go.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
				UguiTabButton tabButton = go.GetComponentInChildren<UguiTabButton>();
				tabButton.Toggle.group = toggleGroup;
				//ラムダ式を使って引数を持たせる
				int index = i;
				tabButton.OnValueChanged.AddListener((bool isOn) => OnClickCategoryButton(isOn, index));
				TabButtonList.Add(tabButton);
				if (callbackCreateItem != null) callbackCreateItem(go, index);
			}
			return TabButtonList;
		}
*/
		public void SetActiveLRButtons(bool isActive)
		{
			if (shiftLeftButton) shiftLeftButton.gameObject.SetActive(isActive);
			if (shiftRightButton) shiftRightButton.gameObject.SetActive(isActive);
			if (jumpLeftEdgeButton) jumpLeftEdgeButton.gameObject.SetActive(isActive);
			if (jumpRightEdgeButton) jumpRightEdgeButton.gameObject.SetActive(isActive);
		}

		//左にシフト
		public void ShiftLeft()
		{
			if (Count <= 0) return;

			int index = CurrentIndex - 1;
			if (index < 0)
			{
				index = (isLoopShift) ? Count - 1 : 0;
			}
			CurrentIndex = index;
		}

		//右にシフト
		public void ShiftRight()
		{
			if (Count <= 0) return;

			int index = CurrentIndex + 1;
			if (index >= Count)
			{
				index = (isLoopShift) ? 0 : Count - 1;
			}
			CurrentIndex = index;
		}

		//左端に移動
		public void JumpLeftEdge()
		{
			if (Count <= 0) return;
			CurrentIndex = 0;
		}

		//右端に移動
		public void JumpRightEdge()
		{
			if (Count <= 0) return;
			CurrentIndex = Count - 1;
		}
	}
}
