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
	/// ノベル用のテキスト設定
	/// </summary>
	public class UguiNovelTextSettings : ScriptableObject
	{
		//区切り文字
		[SerializeField]
		string wordWrapSeparator = "!#%&(),-.:<=>?@[]{}";
		internal string WordWrapSeparator
		{
			get { return wordWrapSeparator; }
		}

		//行頭の禁則文字
		[SerializeField]
		string kinsokuTop =
			",)]}、〕〉》」』】〙〗〟’”｠»"
			+ "ゝゞーァィゥェォッャュョヮヵヶぁぃぅぇぉっゃゅょゎゕゖㇰㇱㇲㇳㇴㇵㇶㇷㇸㇹㇷ゚ㇺㇻㇼㇽㇾㇿ々〻"
			+ "‐゠–〜～"
			+ "?!‼⁇⁈⁉"
			+ "・:;/"
			+ "。."
			+ "，）］｝＝？！：；／"	//全角を追加
			;
		internal string KinsokuTop
		{
			get { return kinsokuTop; }
		}
		
		//行末の禁則文字
		[SerializeField]
		string kinsokuEnd =
			"([{〔〈《「『【〘〖〝‘“｟«"
			+ "（［｛"	//全角を追加
			;
		internal string KinsokuEnd
		{
			get { return kinsokuEnd; }
		}

		//同じ文字が続く場合、文字間が無視される文字
		[SerializeField]
		string ignoreLetterSpace = "…‒–—―⁓〜〰";
		internal string IgnoreLetterSpace
		{
			get { return ignoreLetterSpace; }
		}

		//		string kinsokuBurasage = "、。";						//ぶら下げ組み文字

		//禁則のチェック
		internal bool IsIgonreLetterSpace(UguiNovelTextCharacter current, UguiNovelTextCharacter next)
		{
			if (current == null || next == null) return false;

			if (current.Char == next.Char)
			{ 
				if( IgnoreLetterSpace.IndexOf(current.Char) >= 0  )
				{
					return true;
				}
			}
			return false;
		}

		//禁則のチェック
		internal bool CheckWordWrap( UguiNovelTextGenerator generator, UguiNovelTextCharacter current, UguiNovelTextCharacter prev)
		{
			//文字間無視文字は改行できない
			if (IsIgonreLetterSpace(prev, current))
			{
				return true;
			}

			//英文文字のWordWrap
			if ((generator.WordWrapType & UguiNovelTextGenerator.WordWrap.Default) == UguiNovelTextGenerator.WordWrap.Default)
			{
				if (CheckWordWrapDefaultEnd(prev) && CheckWordWrapDefaultTop(current))
				{
					return true;
				}
			}

			//日本語のWordWrap
			if ((generator.WordWrapType & UguiNovelTextGenerator.WordWrap.JapaneseKinsoku) == UguiNovelTextGenerator.WordWrap.JapaneseKinsoku)
			{
				if ((CheckKinsokuEnd(prev) || CheckKinsokuTop(current)))
				{
					return true;
				}
			}

			return false;
		}

		//英単語のワードラップチェック(行末)
		bool CheckWordWrapDefaultEnd(UguiNovelTextCharacter character)
		{
			char c = character.Char;
			return UtageToolKit.IsHankaku(c) && !char.IsSeparator(c) && !(wordWrapSeparator.IndexOf(c) >= 0);
		}

		//英単語のワードラップチェック(行頭)
		bool CheckWordWrapDefaultTop(UguiNovelTextCharacter character)
		{
			char c = character.Char;
			return UtageToolKit.IsHankaku(c) && !char.IsSeparator(c);
		}

		//ぶら下げ文字のチェック
		bool CheckKinsokuBurasage(UguiNovelTextCharacter c)
		{
			return false;
		}

		//行頭の禁則文字のチェック
		bool CheckKinsokuTop(UguiNovelTextCharacter character)
		{
			return (kinsokuTop.IndexOf(character.Char) >= 0);
		}
		//行末の禁則文字のチェック
		bool CheckKinsokuEnd(UguiNovelTextCharacter character)
		{
			return (kinsokuEnd.IndexOf(character.Char) >= 0);
		}
	}
}
