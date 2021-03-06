//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// テキストデータ（文字列のほかの色などメタデータも）
	/// </summary>
	public class TextData
	{
		public TextParser ParsedText { get { return parsedText; } }
		TextParser parsedText;

		/// <summary>
		/// 文字データリスト
		/// </summary>
		public List<CharData> CharList { get { return ParsedText.CharList; } }

		/// <summary>
		/// 表示文字数（メタデータを覗く）
		/// </summary>
		public int Length { get { return CharList.Count; } }

		/// <summary>
		/// 解析時のエラーメッセージ
		/// </summary>
		public string ErrorMsg { get { return ParsedText.ErrorMsg; } }

		/// <summary>
		/// Unityのリッチテキストフォーマットのテキスト
		/// </summary>
		public string UnityRitchText
		{
			get
			{
				//未作成なら作成する
				InitUnityRitchText();
				return unityRitchText;
			}
		}
		string unityRitchText;

		/// <summary>
		/// メタ情報なしの文字列を取得
		/// </summary>
		/// <returns>メタ情報なしの文字列</returns>
		public string NoneMetaString
		{
			get
			{
				//未作成なら作成する
				InitNoneMetaText();
				return noneMetaString;
			}
		}
		string noneMetaString;
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="text">メタデータを含むテキスト</param>
		public TextData(string text)
		{
			parsedText = new TextParser(text);
		}

		//メタ情報なしのテキストを初期化する
		void InitNoneMetaText()
		{
			//作成ずみならなにもしない
			if (!string.IsNullOrEmpty(noneMetaString)) return;

			noneMetaString = "";
			for (int i = 0; i < CharList.Count; ++i)
			{
				noneMetaString += CharList[i].Char;
			}
		}

		const string BoldEndTag = "</b>";
		const string ItalicEndTag = "</i>";
		const string ColorEndTag = "</color>";
		const string SizeEndTag = "</size>";

		/// Unityのリッチテキストフォーマットのテキストを初期化する
		public void InitUnityRitchText()
		{
			//作成ずみならなにもしない
			if (!string.IsNullOrEmpty(unityRitchText)) return;

			unityRitchText = "";
			CharData.CustomCharaInfo curentCustomInfo = new CharData.CustomCharaInfo();

			//タグの前後関係を正しくするためにStackを使う
			Stack<string> endTagStack = new Stack<string>();

			for (int i = 0; i < CharList.Count; ++i)
			{
				CharData c = CharList[i];
				if (c.CustomInfo.IsEndBold(ref curentCustomInfo)) unityRitchText += endTagStack.Pop();
				if (c.CustomInfo.IsEndItalic(ref curentCustomInfo)) unityRitchText += endTagStack.Pop();
				if (c.CustomInfo.IsEndSize(ref curentCustomInfo)) unityRitchText += endTagStack.Pop();
				if (c.CustomInfo.IsEndColor(ref curentCustomInfo)) unityRitchText += endTagStack.Pop();

				if (c.CustomInfo.IsBeginColor(ref curentCustomInfo))
				{
//					unityRitchText += "<color=#" + ColorUtil.ToColorString(c.CustomInfo.color) + ">";
					unityRitchText += "<color=" + c.CustomInfo.colorStr + ">";
					endTagStack.Push(ColorEndTag);
				}
				if (c.CustomInfo.IsBeginSize(ref curentCustomInfo))
				{
					unityRitchText += "<size=" + c.CustomInfo.size + ">";
					endTagStack.Push(SizeEndTag);
				}
				if (c.CustomInfo.IsBeginItalic(ref curentCustomInfo))
				{
					unityRitchText += "<i>";
					endTagStack.Push(ItalicEndTag);
				}
				if (c.CustomInfo.IsBeginBold(ref curentCustomInfo))
				{
					unityRitchText += "<b>";
					endTagStack.Push(BoldEndTag);
				}

				c.UnityRitchTextIndex = unityRitchText.Length;
				unityRitchText += c.Char;
				if (c.CustomInfo.IsDobleWord)
				{
					unityRitchText += " ";
				}
				curentCustomInfo = c.CustomInfo;
			}
			if (curentCustomInfo.IsBold) unityRitchText += endTagStack.Pop();
			if (curentCustomInfo.IsItalic) unityRitchText += endTagStack.Pop();
			if (curentCustomInfo.IsSize) unityRitchText += endTagStack.Pop();
			if (curentCustomInfo.IsColor) unityRitchText += endTagStack.Pop();
		}

/*
		/// <summary>
		/// NUGIフォーマットのテキストを取得
		/// </summary>
		/// <returns>NUGIフォーマットのテキスト</returns>
		public string ToNguiText()
		{
			return ToNguiText(0, CharList.Count);
		}

		/// <summary>
		/// NUGIフォーマットのテキストを取得
		/// </summary>
		/// <param name="index">文字の先頭インデックス</param>
		/// <param name="count">文字数</param>
		/// <returns>NUGIフォーマットのテキスト</returns>
		public string ToNguiText(int index, int count)
		{
			int max = Mathf.Min(index + count, CharList.Count);
			string str = "";
			Color lastColor = defaultColor;
			for (int i = index; i < max; ++i)
			{
				CharData c = CharList[i];
				if (c.Color != lastColor)
				{
					if (lastColor != defaultColor)
					{
						str += "[-]";
					}
					if (c.Color != defaultColor)
					{
						str += "[" + ColorUtil.ToNguiColorString(c.Color) + "]";
					}
				}
				str += CharList[i].Char;
				lastColor = c.Color;
			}
			if (lastColor != defaultColor)
			{
				str += "[-]";
			}
			return str;
		}
 */ 
	}
}