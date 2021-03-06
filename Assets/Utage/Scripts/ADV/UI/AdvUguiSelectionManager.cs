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
	/// 選択肢表示のサンプル
	/// </summary>
	[AddComponentMenu("Utage/ADV/UiSelectionManager")]
	public class AdvUguiSelectionManager : MonoBehaviour
	{
		/// <summary>ADVエンジン</summary>
		[SerializeField]
		AdvEngine engine;

		AdvSelectionManager SelectionManager { get { return engine.SelectionManager; } }

		bool isInit;

		/// <summary>選択肢のリストビュー</summary>
		public UguiListView ListView
		{
			get { return listView ?? (listView = GetComponent<UguiListView>()); }
		}
		UguiListView listView;


		/// <summary>
		/// 初期化。スクリプトから動的に生成する場合に
		/// </summary>
		/// <param name="engine">ADVエンジン</param>
		public void InitOnCreate(AdvEngine engine, AdvUguiSelection selectionItemPrefab)
		{
			this.engine = engine;
			this.ListView.ItemPrefab = selectionItemPrefab.gameObject;
		}


		/// <summary>開く</summary>
		public void Open()
		{
			this.gameObject.SetActive(true);
		}

		/// <summary>閉じる</summary>
		public void Close()
		{
			ClearAll();
			this.gameObject.SetActive(false);
		}

		void Start()
		{
			ClearAll();
		}

		void Update()
		{
			//選択肢入力待ちなら、初期化して表示
			//そうでないなら非表示
			if (SelectionManager.IsWaitSelect)
			{
				if (!isInit)
				{
					Init();
				}
			}
			else
			{
				if (isInit) ClearAll();
			}
		}

		//全てクリア
		void ClearAll()
		{
			ListView.ClearItems();
			isInit = false;
		}

		//初期化
		void Init()
		{
			ListView.CreateItems(SelectionManager.Selections.Count, CallbackCreateItem);
			isInit = true;
		}

		//リストビューのアイテムが作成されるときに呼ばれるコールバック
		void CallbackCreateItem(GameObject go, int index)
		{
			AdvSelection data = SelectionManager.Selections[index];
			AdvUguiSelection selection = go.GetComponentInChildren<AdvUguiSelection>();
			selection.Init(data, OnTap);

		}

		//選択肢が押された
		void OnTap(AdvUguiSelection item)
		{
			SelectionManager.Select(item.Data );


			ClearAll();
		}
	}
}
