//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;

namespace Utage
{

	/// <summary>
	/// ページ制御
	/// </summary>
	public class AdvPageController
	{
		public enum Type
		{
			None,
			KeepText,
			WaitClick,
		};

		//テキスト表示を続けたままにするか
		bool isKeepText;
		public bool IsKeepText
		{
			get { return isKeepText; }
		}

		//クリック待ちする
		bool isWaitClick;
		public bool IsWaitClick
		{
			get { return isWaitClick; }
		}

		public void Update( Type type )
		{
			switch (type)
			{
			case Type.None:
				isKeepText = false;
				isWaitClick = true;
				break;
			case Type.KeepText:
				isKeepText = true;
				isWaitClick = false;
				break;
			case Type.WaitClick:
				isKeepText = true;
				isWaitClick = true;
				break;
			}
		}

		public void Clear()
		{
			isKeepText = false;
		}
	}
}
