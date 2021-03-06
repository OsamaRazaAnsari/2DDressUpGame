//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// 2D用のソートデータ
	/// </summary>
	public class Node2DSortData : ScriptableObject
	{
		/// <summary>
		/// シングルトンなインスタンスの取得
		/// </summary>
		/// <returns></returns>
		public static Node2DSortData Instance
		{
			get
			{
				if (instance == null)
				{
					if (CustomProjectSetting.Instance)
					{
						instance = CustomProjectSetting.Instance.Node2DSortData;
					}
				}
				return instance;
			}
		}
		static Node2DSortData instance;

		//ソートデータを使わないというキーこのキーはユーザで設定できない
		public const string KeyNone = "None";

		/// <summary>
		/// データのDictionary
		/// </summary>
		public DictionarySortData2D Dictionary { get { return dataTbl; } }
		[SerializeField]
		DictionarySortData2D dataTbl;

		/// <summary>
		/// キーからデータ取得
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="data">データ</param>
		/// <returns>成否。設定されてないキーならfalseを返す</returns>
		public bool TryGetValue(string key, out SortData2D data)
		{
			DictionaryKeyValueSortData2D keyValue;
			bool ret = dataTbl.TryGetValue(key, out keyValue);
			data = (ret) ? keyValue.value : null;
			return ret;
		}

		/// <summary>
		/// インスペクターから値が変更された場合
		/// </summary>
		void OnValidate()
		{
			dataTbl.RefreshDictionary();
		}

		/// <summary>
		/// ソート用のデータ
		/// </summary>
		[System.Serializable]
		public class SortData2D
		{
			/// <summary>
			/// ソート用のレイヤー名
			/// </summary>
			public string sortingLayer;

			/// <summary>
			/// レイヤーの描画順
			/// </summary>
			public int orderInLayer;

			/// <summary>
			/// Z(ローカル座標)
			/// </summary>
			public float z;
		}

		/// <summary>
		/// 2Dソートデータのキー・バリュー用の定義
		/// </summary>
		[System.Serializable]
		public class DictionaryKeyValueSortData2D : SerializableDictionaryKeyValue
		{
			/// <summary>
			/// 値
			/// </summary>
			public SortData2D value;

			/// <summary>
			/// 初期化
			/// </summary>
			/// <param name="key">キー</param>
			/// <param name="value">値</param>
			public void Init(string key, SortData2D value)
			{
				InitKey(key);
				this.value = value;
			}
		};

		/// <summary>
		/// 2DソートデータのDictionary用の定義
		/// </summary>
		[System.Serializable]
		public class DictionarySortData2D : SerializableDictionary<DictionaryKeyValueSortData2D>
		{
		}
	}
}