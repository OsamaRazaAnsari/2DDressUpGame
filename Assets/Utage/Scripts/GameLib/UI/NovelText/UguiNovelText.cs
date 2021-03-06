//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


namespace Utage
{

	/// <summary>
	/// ノベル用に、禁則処理などを含めたテキスト表示
	/// </summary>
	[RequireComponent(typeof(UguiNovelTextGenerator))]
	[AddComponentMenu("Utage/Lib/UI/NovelText")]
	public class UguiNovelText : Text
	{
		public TextData TextData
		{
			get { return TextGenerator.TextData; }
		}

		public int LengthOfView
		{
			get { return TextGenerator.LengthOfView; }
			set { TextGenerator.LengthOfView = value; }
		}

		public UguiNovelTextGenerator TextGenerator { get { return textGenerator ?? (textGenerator = GetComponent<UguiNovelTextGenerator>()); } }
		UguiNovelTextGenerator textGenerator;

		//文字送りをしない場合の文字の最後の座標
		public Vector3 EndPosition { get { return TextGenerator.EndPosition; } }

		//文字送りをする場合の文字の最後の座標
		public Vector3 CurrentEndPosition { get { TextGenerator.RefleshEndPosition(); return TextGenerator.EndPosition; } }


		//頂点情報を作成
		/// <summary>
		/// Draw the Text.
		/// </summary>

		protected override void OnFillVBO(List<UIVertex> vbo)
		{
			if (font == null)
				return;

			if (TextGenerator.IsRequestingCharactersInTexture)
			{
				return;
			}

			//フォントの再作成によるものであればその旨を通知
			if (!isDirtyVerts)
			{
				TextGenerator.IsRebuidFont = true;
			}

			IList<UIVertex> verts = TextGenerator.CreateVertex();
			vbo.AddRange(verts);
			isDirtyVerts = false;
		}

		protected override void Start()
		{
			UnityAction onDirtyVertsCallback = OnDirtyVerts;
			m_OnDirtyVertsCallback += onDirtyVertsCallback;
		}
		
		protected override void OnEnable()
		{
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		public override void SetAllDirty()
		{
			TextGenerator.HasChanged = true;
			base.SetAllDirty();
		}

		void OnDirtyVerts()
		{
			TextGenerator.HasChanged = true;
			isDirtyVerts = true;
		}
		bool isDirtyVerts = false;
	}
}

