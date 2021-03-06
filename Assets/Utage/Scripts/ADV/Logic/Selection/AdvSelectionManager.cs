//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Utage
{

	/// <summary>
	/// 選択肢管理
	/// </summary>
	[AddComponentMenu("Utage/ADV/Internal/SelectionManager")]
	public class AdvSelectionManager : MonoBehaviour
	{
		/// <summary>ADVエンジン</summary>
		public AdvEngine Engine { get { return this.engine ?? (this.engine = GetComponent<AdvEngine>()); } }
		AdvEngine engine;

		/// <summary>
		/// 選択肢データ
		/// </summary>
		public List<AdvSelection> Selections { get { return selections; } }
		List<AdvSelection> selections = new List<AdvSelection>();

		/// <summary>
		/// スプライト選択肢データ
		/// </summary>
		public List<AdvSelection> SpriteSelections { get { return spriteSelections; } }
		List<AdvSelection> spriteSelections = new List<AdvSelection>();
		
		/// <summary>
		/// 選択待ち状態か
		/// </summary>
		public bool IsWaitSelect { get { return this.isWaitSelect; } }
		bool isWaitSelect = false;

		/// <summary>
		/// 選択されたか
		/// </summary>
		public bool IsSelected { get { return selected != null; } }

		/// <summary>
		/// 選択されたデータ
		/// </summary>
		public AdvSelection Selected
		{
			get { return selected; }
		}
		AdvSelection selected = null;

		/// <summary>
		/// 選択
		/// </summary>
		/// <param name="index">選択肢のインデックス</param>
		public void Select(int index)
		{
			Select(selections[index]);
		}

		/// <summary>
		/// 選択
		/// </summary>
		/// <param name="selected">選んだ選択肢</param>
		public void Select( AdvSelection selected)
		{
			this.selected = selected;
			isWaitSelect = false;
		}

		/// <summary>
		/// 選択肢追加
		/// </summary>
		/// <param name="label">ジャンプ先のラベル</param>
		/// <param name="text">表示テキスト</param>
		/// <param name="exp">選択時に実行する演算式</param>
		public void AddSelection(string label, string text, ExpressionParser exp, StringGridRow row )
		{
			selections.Add(new AdvSelection(label, text, exp, row));
		}

		/// <summary>
		/// 選択肢追加
		/// </summary>
		/// <param name="label">ジャンプ先のラベル</param>
		/// <param name="name">クリックを有効にするオブジェクト名</param>
		/// <param name="isPolygon">ポリゴンコライダーを使うか</param>
		/// <param name="exp">選択時に実行する演算式</param>
		public void AddSelectionClick(string label, string name, bool isPolygon, ExpressionParser exp, StringGridRow row)
		{
			AdvSelection select = new AdvSelection(label, name, isPolygon, exp, row);
			spriteSelections.Add(select);
			AddSpriteClickEvent(select);
		}

		void AddSpriteClickEvent(AdvSelection select)
		{
			Engine.GraphicManager.AddClickEvent(select.SpriteName, select.IsPolygon, select.RowData, (eventData) => OnSpriteClick(eventData, select));
		}

		void OnSpriteClick(BaseEventData eventData, AdvSelection select)
		{
			Select(select);
		}


		/// <summary>
		/// 選択の入力待ち開始
		/// </summary>
		public void StartWaiting()
		{
			isWaitSelect = true;
			selected = null;
		}

		/// <summary>
		/// 選択肢データをクリア
		/// </summary>
		public void Clear()
		{
			isWaitSelect = false;
			selected = null;
			selections.Clear();
			foreach (AdvSelection selection in spriteSelections)
			{
				Engine.GraphicManager.RemoveClickEvent(selection.SpriteName);
			}
			spriteSelections.Clear();
		}

		/// <summary>
		/// セーブデータ用のバイナリを取得
		/// </summary>
		/// <returns>セーブデータのバイナリ</returns>
		public byte[] ToSaveDataBuffer()
		{
			using (MemoryStream stream = new MemoryStream())
			{
				//バイナリ化
				using (BinaryWriter writer = new BinaryWriter(stream))
				{
					Write(writer);
				}
				return stream.ToArray();
			}
		}

		/// <summary>
		/// セーブデータを読みこみ
		/// </summary>
		/// <param name="buffer">セーブデータのバイナリ</param>
		public void ReadSaveDataBuffer(byte[] buffer)
		{
			using (MemoryStream stream = new MemoryStream(buffer))
			{
				using (BinaryReader reader = new BinaryReader(stream))
				{
					Read(reader);
				}
			}
		}

		const int VERSION = 1;
		const int VERSION_0 = 0;
		//バイナリ書き込み
		void Write(BinaryWriter writer)
		{
			writer.Write(VERSION);
			writer.Write(Selections.Count);
			foreach (var selection in Selections)
			{
				selection.Write(writer);
			}
			writer.Write(SpriteSelections.Count);
			foreach (var selection in SpriteSelections)
			{
				selection.Write(writer);
			}
		}
		//バイナリ読み込み
		void Read(BinaryReader reader)
		{
			this.Clear();
			int version = reader.ReadInt32();
			if (version == VERSION)
			{
				int count = reader.ReadInt32();
				for (int i = 0; i < count; i++)
				{
					AdvSelection selection = new AdvSelection(reader, engine);
					selections.Add(selection);
				}
				count = reader.ReadInt32();
				for (int i = 0; i < count; i++)
				{
					AdvSelection selection = new AdvSelection(reader, engine);
					spriteSelections.Add(selection);
					AddSpriteClickEvent(selection);
				}
			}
			else if (version == VERSION_0)
			{
				int count = reader.ReadInt32();
				for (int i = 0; i < count; i++)
				{
					AdvSelection selection = new AdvSelection(reader, engine);
					selections.Add(selection);
				}
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, version));
			}
		}
	}
}