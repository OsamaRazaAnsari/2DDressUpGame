//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using Utage;

namespace Utage
{

	/// <summary>
	/// バックログ表示
	/// </summary>
	[AddComponentMenu("Utage/ADV/UiBacklogManager")]
	public class AdvUguiBacklogManager : MonoBehaviour
	{
		/// <summary>ADVエンジン</summary>
		[SerializeField]
		AdvEngine engine;

		//バックログデータへのインターフェース
		AdvBacklogManager BacklogManager { get { return engine.BacklogManager; } }

		/// <summary>選択肢のリストビュー</summary>
		public UguiListView ListView
		{
			get { return listView; }
		}
		[SerializeField]
		UguiListView listView;

		//スクロール最下段でマウスホイール入力で閉じる入力するか
		public bool isCloseScrollWheelDown = false;

		/// <summary>
		/// 初期化。スクリプトから動的に生成する場合に
		/// </summary>
		/// <param name="engine">ADVエンジン</param>
		public void InitOnCreate(AdvEngine engine, AdvUiBacklog backlogItemPrefab)
		{
			this.engine = engine;
			this.ListView.ItemPrefab = backlogItemPrefab.gameObject;
		}


		/// <summary>開いているか</summary>
		public bool IsOpen { get { return this.gameObject.activeSelf; } }

		/// <summary>
		/// 閉じる
		/// </summary>
		public void Close()
		{
			ListView.ClearItems();
			this.gameObject.SetActive(false);
		}

		/// <summary>
		/// 開く
		/// </summary>
		public void Open()
		{
			this.gameObject.SetActive(true);
			ListView.CreateItems(BacklogManager.Backlogs.Count, CallbackCreateItem);
		}

		/// <summary>
		/// リストビューのアイテムが作られたときに呼ばれるコールバック
		/// </summary>
		/// <param name="go">作られたアイテムのGameObject</param>
		/// <param name="index">アイテムのインデックス</param>
		void CallbackCreateItem(GameObject go, int index)
		{
			AdvBacklog data = BacklogManager.Backlogs[BacklogManager.Backlogs.Count- index -1];
			AdvUguiBacklog backlog = go.GetComponent<AdvUguiBacklog>();
			backlog.Init(data, OnTapSound, index);
		}

		/// <summary>
		/// 音声再生ボタンが押された
		/// </summary>
		/// <param name="button">押されたボタン</param>
		public void OnTapSound( AdvUguiBacklog item)
		{
			BacklogManager.TryPlayVoice(engine, item.Data);
		}

		// 戻るボタンが押された
		public void OnTapBack()
		{
			Back();
		}

		// 更新
		void Update()
		{
			//閉じる入力された
			if (InputUtil.IsMousceRightButtonDown() || IsInputBottomEndScrollWheelDown() )
			{
				Back();
			}
		}

		// バックログ閉じて、メッセージウィンドウ開く
		void Back()
		{
			this.Close();
			engine.UiManager.Status = AdvUiManager.UiStatus.Default;
		}

		//スクロール最下段でマウスホイール入力で閉じる入力するチェック
		bool IsInputBottomEndScrollWheelDown()
		{
			if(isCloseScrollWheelDown && InputUtil.IsInputScrollWheelDown())
			{
				Scrollbar scrollBar = ListView.ScrollRect.verticalScrollbar;
				if(scrollBar)
				{
					return scrollBar.value <= 0;
				}
			}
			return false;
		}
	}
}
