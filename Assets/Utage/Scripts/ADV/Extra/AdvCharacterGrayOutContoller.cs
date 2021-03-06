//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Utage
{
	//喋っていないキャラクターをグレーアウトする処理
	//AdvEngineのOnPageTextChangeから呼び出す、このコンポーネントの同名メソッドを登録すると使えるようになる
	[AddComponentMenu("Utage/ADV/Extra/CharacterGrayOutContoller")]
	public class AdvCharacterGrayOutContoller : MonoBehaviour
	{
		[SerializeField]
		Color mainColor = Color.white;
		public Color MainColor
		{
			get { return mainColor; }
			set { mainColor = value; }
		}

		[SerializeField]
		Color subColor = Color.gray;
		public Color SubColor
		{
			get { return subColor; }
			set { subColor = value; }
		}

		//テキストに変更があった場合
		public void OnPageTextChange(AdvEngine engine)
		{
			List<AdvGraphicObject> graphics = engine.GraphicManager.CharacterManager.AllGraphics();
			foreach (AdvGraphicObject obj in graphics)
			{
				Color color = GetColor(engine, obj);
				obj.EffectColors.SetColor(EffectColors.Index.Color1, color);
			}
		}

		//カラーを取得
		Color GetColor( AdvEngine engine, AdvGraphicObject obj )
		{
			return ( obj.name == engine.Page.CharacterLabel ) ? MainColor : SubColor;
		}
	}
}

