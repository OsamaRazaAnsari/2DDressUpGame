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
	/// 表示言語切り替え用のクラス
	/// </summary>
	public class LanguageManager : LanguageManagerBase
	{
		protected override void RefreshCurrentLanguage()
		{
			foreach (var item in languegeDataTbl.Values)
			{
				//システムの言語に変更
				if (!item.TryChangeCurrentLanguage(currentLanguage))
				{
					//システムの言語に対応してないので、デフォルト設定の言語を設定
					item.TryChangeCurrentLanguage(DefaultLanguage);
				}
			}
			if (!IgnoreLocalizeUiText)
			{
				{
					LegacyLocalize[] localizeTbl = GameObject.FindObjectsOfType<LegacyLocalize>();
					foreach (var item in localizeTbl)
					{
						item.OnLocalize();
					}
				}
				{
					UguiLocalize[] localizeTbl = GameObject.FindObjectsOfType<UguiLocalize>();
					foreach (var item in localizeTbl)
					{
						item.OnLocalize();
					}
				}
			}
		}
	}
}
