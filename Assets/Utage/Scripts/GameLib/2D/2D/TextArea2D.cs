//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Utage
{
	/// <summary>
	/// テキストデータ（文字列のほかの色などメタデータも）
	/// </summary>
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/2D/TextArea")]
	public class TextArea2D : Node2D
	{
		/// <summary>
		/// 表示テキスト
		/// タグによるカラー指定なども可能
		/// </summary>
		public string text
		{
			get { return currentText; }
			set
			{
				if (currentText != value)
				{
					currentText = value;
					MarkAsChanged();
				}
			}
		}
		[SerializeField]
		string currentText;

		/// <summary>
		/// テキストデータ
		/// </summary>
		public TextData TextData
		{
			get { return textData; }
			set
			{
				if (textData != value)
				{
					textData = value;
					currentText = "";
					isTextDataChanged = true;
					MarkAsChanged();
				}
			}
		}
		TextData textData;
		bool isTextDataChanged = false;

		/// <summary>
		/// フォントデータ
		/// </summary>
		public FontData Font
		{
			get { return font; }
			set	{ if (font != value){font = value;MarkAsChanged();}}
		}
		[SerializeField]
		FontData font;


		/// <summary>
		/// フォントの表示サイズ
		/// </summary>
		public float Size { get { return size; } set { size = value; MarkAsChanged(); } }
		[SerializeField]
		float size = 20;

		/// <summary>
		/// フォントスプライトの拡大率
		/// </summary>
		float TextScale
		{
			get
			{
				//拡大率を取得
				float scale = 1;
				if (null != Font)
				{
					scale = 1.0f * size / Font.Size;
				}
				return scale;
			}
		}

		/// <summary>
		/// スプライトを作成する際の、座標1.0単位辺りのピクセル数
		/// </summary>
		public float PixcelsToUnits
		{
			get { return pixcelsToUnits; }
			set { pixcelsToUnits = value; MarkAsChanged(); }
		}
		[SerializeField]
		float pixcelsToUnits = 100;

		/// <summary>
		/// カーニングするかどうか
		/// </summary>
		public bool IsKerning
		{
			get { return isKerning; }
			set { isKerning = value; MarkAsChanged(); }
		}
		[SerializeField]
		bool isKerning = false;

		/// <summary>
		/// テキスト表示の最大幅（0以下は無限）
		/// </summary>
		public int MaxWidth
		{
			get { return maxWidth; }
			set { maxWidth = value; MarkAsChanged(); }
		}
		[SerializeField]
		int maxWidth = 0;

		/// <summary>
		/// テキスト表示の最大高さ（0以下は無限）
		/// </summary>
		public int MaxHeight
		{
			get { return maxHeight; }
			set { maxHeight = value; MarkAsChanged(); }
		}
		[SerializeField]
		int maxHeight = 0;

		/// <summary>
		/// スペースの幅(px)
		/// </summary>
		public int Space
		{
			get { return space; }
			set { space = value; MarkAsChanged(); }
		}
		[SerializeField]
		int space = 10;

		/// <summary>
		/// 文字間(px)
		/// </summary>
		public int LetterSpace
		{
			get { return letterSpace; }
			set { letterSpace = value; MarkAsChanged(); }
		}
		[SerializeField]
		int letterSpace = 0;

		
		/// <summary>
		/// 行間(px)
		/// </summary>
		public int LineSpace
		{
			get { return lineSpace; }
			set { lineSpace = value; MarkAsChanged(); }
		}
		[SerializeField]
		int lineSpace = 0;


		/// <summary>
		/// 禁則処理の仕方
		/// </summary>
		[System.Flags]
		public enum WordWrap
		{
			/// <summary>デフォルト（半角のみ）</summary>
			Default = 0x1,
			/// <summary>日本語の禁則処理</summary>
			JapaneseKinsoku = 0x2,
		};
		/// <summary>
		/// 行間(px)
		/// </summary>
		public WordWrap WordWrapType
		{
			get { return wordWrap; }
			set { wordWrap = value; MarkAsChanged(); }
		}
		[SerializeField]
		[EnumFlagsAttribute]
		WordWrap wordWrap = 0;

		/// <summary>表示する文字の長さ(-1なら全部表示)</summary>
		public int LengthOfView
		{ 
			get { return lengthOfView; } 
			set
			{
				if (lengthOfView != value)
				{
					lengthOfView = value;
					if (!hasChanged)
					{
						RefreshLengthOfView();
					}					
				}
			}
		}
		[SerializeField]
		int lengthOfView = -1;


		/// <summary>
		/// 表示タイプ
		/// </summary>
		public enum ViewType
		{
			/// <summary>デフォルト</summary>
			Default,

			/// <summary>影つき</summary>
			Shadow,

			/// <summary>アウトラインつき</summary>
			Outline,
		};

		/// <summary>
		/// 表示タイプ
		/// </summary>
		public ViewType Type
		{
			get { return type; }
			set { type = value; MarkAsChanged(); }
		}

		[SerializeField]
		ViewType type;

		/// <summary>
		/// 縁取りやアウトラインの色
		/// </summary>
		public Color ColorEffect
		{
			get { return colorEffect; }
			set { colorEffect = value; MarkAsChanged(); }
		}
		[SerializeField]
		Color colorEffect = Color.black;

		/// <summary>
		/// 表示のレアイウトタイプ
		/// </summary>
		public enum Layout
		{
			/// <summary>右上寄せ</summary>
			TopLeft,
			/// <summary>上寄せ</summary>
			Top,
			/// <summary>左上寄せ</summary>
			TopRight,
			/// <summary>左寄せ</summary>
			Left,
			/// <summary>中央寄せ</summary>
			Center,
			/// <summary>右寄せ</summary>
			Right,
			/// <summary>左下寄せ</summary>
			BottomLeft,
			/// <summary>下寄せ</summary>
			Bottom,
			/// <summary>右下寄せ</summary>
			BottomRight,
		}
		/// <summary>
		/// 表示のレアイウトタイプ
		/// </summary>
		public Layout LayoutType
		{
			get { return layoutType; }
			set { layoutType = value; MarkAsChanged(); }
		}
		[SerializeField]
		Layout layoutType = Layout.Center;

		[SerializeField]
		bool isDebugMode = false;

		bool egnoreRebuild;
		Transform childRoot;

		//描画情報
		class CharacterRenderInfo
		{
			public bool isAutoLineBreak;			//自動改行
			public bool isKinsokuBurasage;			//禁則処理によるぶら下げ文字
			public CharData charInfo;				//文字情報
			public FontRenderInfo fontInfo;			//フォントの描画情報
			public float x;							//表示位置X
			public float y;							//表示位置Y
			public float w;							//表示幅

			public CharacterRenderInfo(CharData charInfo, FontRenderInfo fontInfo, float x, float y, float w, bool isAutoLineBreak)
			{
				this.charInfo = charInfo;
				this.fontInfo = fontInfo;
				this.x = x;
				this.y = y;
				this.w = w;
				this.isAutoLineBreak = isAutoLineBreak;
			}

			public bool IsBr { get { return (isAutoLineBreak || charInfo.IsBr); } }

			public void AddRenderObj(GameObject obj)
			{
				renderObjList.Add(obj);
			}
			public void SetRenderActive(bool isActive)
			{
				foreach (var item in renderObjList)
				{
					if (item && item.activeSelf != isActive ) item.SetActive(isActive);
				}
			}
			List<GameObject> renderObjList = new List<GameObject>();
		};
		List<CharacterRenderInfo> renderInfoList = new List<CharacterRenderInfo>();


		void OnDestory()
		{
			Clear();
		}

		void OnDisable()
		{
			Clear();
		}

		void Clear()
		{
			DestroyRootObject();
		}

		/// <summary>
		/// データの更新をかける
		/// </summary>
		public override void RefreshCustom()
		{
			RefreshText();
		}


		/// <summary>
		/// 毎フレーム最後の更新
		/// </summary>
		protected override void LateUpdate()
		{
			base.LateUpdate();
			if (CachedTransform.hasChanged)
			{
				SyncTransform();
			}
		}

		//テキスト表示を更新
		void RefreshText()
		{
			DestroyRootObject();
			if (!this.gameObject.activeInHierarchy) return;
			if (Font == null) return;
			
			//テキストデータを取得
			if (!isTextDataChanged)
			{
				textData = new TextData(text);
			}
			string str = (textData == null) ? "" : textData.NoneMetaString;
			if(isTextDataChanged)
			{
				currentText = str;
			}
			isTextDataChanged = false;

			//フォントのアトラス作成
			egnoreRebuild = true;
			Font.MakeFontAtlas(str);
			egnoreRebuild = false;

			//描画データ作成
			renderInfoList.Clear();
			renderInfoList = CreateRenderInfo(textData);

			//描画オブジェクトを更新

			//不可視のオブジェクトとして作成（子オブジェクトにもしない。プレハブ化できなくなるから）
			CreateRootObject();

			Color mulColorEffect = ColorEffect * this.Color;
			int effectOrder = 0;
			int defaultOrder = ( type == ViewType.Default ) ? 0 : 1;
			foreach (CharacterRenderInfo renderInfo in renderInfoList)
			{
				if (renderInfo.fontInfo == null) continue;

				switch (type)
				{
					case ViewType.Shadow:
						AddCharObject(childRoot, renderInfo, mulColorEffect, effectOrder, 1, -1);
						break;
					case ViewType.Outline:
						AddCharObject(childRoot, renderInfo, mulColorEffect, effectOrder, -1, -1);
						AddCharObject(childRoot, renderInfo, mulColorEffect, effectOrder, 1, -1);
						AddCharObject(childRoot, renderInfo, mulColorEffect, effectOrder, 1, 1);
						AddCharObject(childRoot, renderInfo, mulColorEffect, effectOrder, -1, 1);
						break;
					default:
						break;
				}
				Color mulColor = ( renderInfo.charInfo.CustomInfo.IsColor  ) ? renderInfo.charInfo.CustomInfo.color * this.Color : this.Color;
				AddCharObject(childRoot, renderInfo, mulColor, defaultOrder);
			}
			RefreshLengthOfView();
		}

		//文字の表示長さを変更
		void RefreshLengthOfView()
		{
			for (int i = 0; i < renderInfoList.Count; ++i)
			{
				renderInfoList[i].SetRenderActive(i < LengthOfView || LengthOfView < 0);
			}
		}


		//ルートオブジェクト作成
		void CreateRootObject()
		{
			//不可視のオブジェクトとして作成（子オブジェクトにもしない。プレハブ化できなくなるから）
			string name = "text: " + text;
			HideFlags hideFlag = (isDebugMode) ? HideFlags.DontSave : HideFlags.HideAndDontSave;
			GameObject go;
#if UNITY_EDITOR
			go = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags(name, hideFlag);
#else
			go = new GameObject(name);
			go.hideFlags = hideFlag;
#endif
			go.layer = this.gameObject.layer;
			childRoot = go.transform;
			SyncTransform();
		}

		//ルートオブジェクト削除
		void DestroyRootObject()
		{
			if (childRoot != null)
			{
#if UNITY_EDITOR
				GameObject.DestroyImmediate(childRoot.gameObject);
#else
				GameObject.Destroy(childRoot.gameObject);
#endif
				childRoot = null;
			}
		}

		//ルートオブジェクトにTrasfomrを同期させる
		void SyncTransform()
		{
			if (childRoot != null)
			{
				if (childRoot.gameObject.layer != this.gameObject.layer)
				{
					childRoot.gameObject.layer = this.gameObject.layer;
				}
				if( childRoot.position != CachedTransform.position )
				{
					childRoot.position = CachedTransform.position;
				}
				if (childRoot.rotation != CachedTransform.rotation)
				{
					childRoot.rotation = CachedTransform.rotation;
				}
				if (childRoot.transform.localScale != CachedTransform.lossyScale)
				{
					childRoot.transform.localScale = CachedTransform.lossyScale;
				}			
				
			}
		}

		//文字表示オブジェクト追加
		void AddCharObject(Transform root, CharacterRenderInfo renderInfo, Color color, int order)
		{
			AddCharObject(root, renderInfo, color, order, 0, 0);
		}
		void AddCharObject(Transform root, CharacterRenderInfo renderInfo, Color color, int order, int x0, int y0)
		{
			Vector2 pos = Vector2.zero;
			pos.x += x0 + renderInfo.x + GetCharacterRenderWidthOffsetX(renderInfo.fontInfo);
			pos.y += y0 + renderInfo.y + renderInfo.fontInfo.OffsetY * TextScale;
			SpriteRenderer rendrer = UtageToolKit.AddChildGameObjectComponent<SpriteRenderer>(root, "" + renderInfo.fontInfo.Char, pos / PixcelsToUnits, Vector2.one * TextScale);
			rendrer.gameObject.hideFlags = HideFlags.DontSave;
			rendrer.material = Font.Font.material;
			rendrer.sprite = renderInfo.fontInfo.Sprite;
			rendrer.color = color;
			rendrer.sortingOrder = this.OrderInLayer + order;
			rendrer.sortingLayerName = this.SortingLayer;

			rendrer.transform.localEulerAngles = WrapperUnityVersion.GetFontSpriteAngles(renderInfo.fontInfo );

			renderInfo.AddRenderObj(rendrer.gameObject);
		}

		/// <summary>
		/// フォントテクスチャの再作成時に呼ばれる(システム内部で使うので、外部から呼ばないこと)
		/// </summary>
		/// <param name="font"></param>
		public void RebuildFont(FontData font)
		{
			if (egnoreRebuild) return;
			if (this.Font == font)
			{
				Clear();
				RefreshText();
			}
		}

		//各文字の描画データを作成
		List<CharacterRenderInfo> CreateRenderInfo(TextData textData)
		{

			List<CharacterRenderInfo> infoList = new List<CharacterRenderInfo>();
			if (textData == null) return infoList;

			float y = -Size;
			bool isOverHeight = false;
			while (infoList.Count < textData.Length && !isOverHeight)
			{
				float x = 0;
				while (infoList.Count < textData.Length)
				{
					CharData c = textData.CharList[infoList.Count];
					FontRenderInfo renderInfo = null;
					float w = 0;
					bool isAutoLineBreak = false;
					if (c.IsBr)
					{
					}
					else if (char.IsWhiteSpace(c.Char))
					{
						w = Space;
					}
					else
					{
						renderInfo = Font.GetRenderInfoCreateIfMissing(c.Char,pixcelsToUnits);
						if (renderInfo != null)
						{
							w = GetCharacterRenderWidth(renderInfo);
							//横幅を越えるなら自動改行
							isAutoLineBreak = IsOverMaxWidth(x, w);
						}
					}
					//文字の追加
					CharacterRenderInfo charInfo = new CharacterRenderInfo(c, renderInfo, x, y, w, isAutoLineBreak);
					infoList.Add(charInfo);
					x += w + LetterSpace;
					//改行処理
					if (charInfo.IsBr)
					{
						if (isAutoLineBreak)
						{
							//自動改行
							AutoLineBreak(infoList);
						}
						float offsetY = -(LineSpace + Size);
						//縦幅のチェック
						if (IsOverMaxHeight(y + offsetY))
						{
							isOverHeight = true;
						}
						else
						{
							y += offsetY;
						}
						break;
					}
				}
			}

			MoveLayout(y, infoList);
			return infoList;
		}

		//文字の表示幅を取得
		float GetCharacterRenderWidth(FontRenderInfo renderInfo)
		{
			return renderInfo.GetWidth(IsKerning) * TextScale;
		}

		//文字の表示位置の調整値を取得
		float GetCharacterRenderWidthOffsetX(FontRenderInfo renderInfo)
		{
			return renderInfo.GetOffsetX(IsKerning) * TextScale;
		}


		//Layoutによる位置の調整
		void MoveLayout(float y, List<CharacterRenderInfo> infoList)
		{
			//LayoutによってX位置の調整
			int index = 0;
			while (index < infoList.Count)
			{
				float w = 0;
				int begin = index;
				int end = index;
				for (int i = begin; i < infoList.Count; ++i)
				{
					CharacterRenderInfo info = infoList[i];
					w = info.x + info.w;
					end = i;
					if (info.IsBr)
					{
						break;
					}
				}
				float offsetX = ToLayoutOffsetX(w);
				for (int i = begin; i <= end; ++i)
				{
					infoList[i].x += offsetX;
				}
				index = end + 1;
			}

			float offsetY = Size / 2;			//フォントを座標の中央表示するため
			offsetY += ToLayoutOffsetY(y);		//LayoutによってY位置の調整
			foreach (var item in infoList)
			{
				item.y += offsetY;
			}
		}

		//自動改行
		//禁則などで送り出しされる文字がある場合は、改行可能な位置まで文字を削除していく
		void AutoLineBreak(List<CharacterRenderInfo> infoList)
		{
			int index = infoList.Count - 1;

			CharacterRenderInfo current = infoList[index];	//はみ出た文字
			CharacterRenderInfo prev = (index > 0) ? infoList[index - 1] : null;	//一つ前の文字

			if (null == current || null == prev)
			{	//そのまま
			}
			else if (CheckKinsokuBurasage(current))
			{	//ぶら下げ文字
				current.isKinsokuBurasage = true;
			}
			else if (prev.IsBr)
			{
				//前の文字が改行の場合、そのまま
			}
			else if (CheckWordWrap(current, prev))
			{	//禁則処理
				//改行可能な位置まで文字を削除していく
				int i = ParseWordWrap(infoList, index - 1) - 1;
				int count = (index - i);
				if (count > 0)
				{
					infoList[i].isAutoLineBreak = true;
					infoList.RemoveRange(i + 1, count);
				}
			}
			else
			{
				//前の文字を自動改行して
				prev.isAutoLineBreak = true;
				infoList.Remove(current);
			}
		}


		//WordWrap処理
		int ParseWordWrap(List<CharacterRenderInfo> infoList, int index)
		{
			CharacterRenderInfo current = infoList[index];	//はみ出た文字
			CharacterRenderInfo prev = (index > 0) ? infoList[index - 1] : null;	//一つ前の文字

			if (CheckWordWrap(current, prev))
			{	//禁則に引っかかるので、一文字前をチェック
				return ParseWordWrap(infoList, index - 1);
			}
			else
			{
				return index;
			}
		}

		//禁則処理文字のチェック
		bool CheckWordWrap(CharacterRenderInfo current, CharacterRenderInfo prev)
		{
			bool ret = false;
			if ((WordWrapType & WordWrap.Default) == WordWrap.Default)
			{
				ret |= CheckWordWrapDefaultEnd(prev) && CheckWordWrapDefaultTop(current);
			}

			if ((WordWrapType & WordWrap.JapaneseKinsoku) == WordWrap.JapaneseKinsoku)
			{
				ret |= (CheckKinsokuEnd(prev) || CheckKinsokuTop(current));
			}

			return ret;
		}

		//区切り文字
		string wordWrapSeparator = "!#%&(),-.:<=>?@[]{}";

		//英単語のワードラップチェック(行末)
		bool CheckWordWrapDefaultEnd(CharacterRenderInfo info)
		{
			if (info == null) return false;
			if (info.charInfo == null) return false;

			char c = info.charInfo.Char;
			return UtageToolKit.IsHankaku(c) && !char.IsSeparator(c) && !(wordWrapSeparator.IndexOf(c) >= 0);
		}

		//英単語のワードラップチェック(行頭)
		bool CheckWordWrapDefaultTop(CharacterRenderInfo info)
		{
			if (info == null) return false;
			if (info.charInfo == null) return false;

			char c = info.charInfo.Char;
			return UtageToolKit.IsHankaku(c) && !char.IsSeparator(c);
		}
		 


		//行頭の禁則文字
		string kinsokuTop =
			",)]｝、〕〉》」』】??〟’”?≫"
			+ "ゝゞーァィゥェォッャュョヮヵヶぁぃぅぇぉっゃゅょゎ????????????????????々?"
			+ "‐???～"
			+ "?!????"
			+ "・:;/"
			+ "。."
			+ "，）］｝＝？！：；／"	//全角を追加
			;

		//行末の禁則文字
		string kinsokuEnd =
			"([{〔〈《「『【??〝‘“?≪"
			+ "（［｛"	//全角を追加
			;

//		string kinsokuBurasage = "、。";						//ぶら下げ組み文字

		//ぶら下げ文字のチェック
		bool CheckKinsokuBurasage(CharacterRenderInfo c)
		{
			return false;
		}

		//行頭の禁則文字のチェック
		bool CheckKinsokuTop(CharacterRenderInfo info)
		{
			if (info == null) return false;
			if (info.charInfo == null) return false;
			return (kinsokuTop.IndexOf(info.charInfo.Char) >= 0);
		}
		//行末の禁則文字のチェック
		bool CheckKinsokuEnd(CharacterRenderInfo info)
		{
			if (info == null) return false;
			if (info.charInfo == null) return false;
			return (kinsokuEnd.IndexOf(info.charInfo.Char) >= 0);
		}

		//最大横幅のチェック
		bool IsOverMaxWidth(float x, float width)
		{
			if (MaxWidth <= 0) return false;

			return (x > 0) && (x + width) > MaxWidth;
		}

		//最大縦幅のチェック
		bool IsOverMaxHeight(float height)
		{
			if (MaxHeight <= 0) return false;

			return height < -MaxHeight;
		}

		//位置調整のためのXを取得
		float ToLayoutOffsetX(float width)
		{
			switch (LayoutType)
			{
				case Layout.TopLeft:
				case Layout.Left:
				case Layout.BottomLeft:
					return 0;
				case Layout.Top:
				case Layout.Center:
				case Layout.Bottom:
					return -width / 2;
				case Layout.TopRight:
				case Layout.Right:
				case Layout.BottomRight:
					return -width;
				default:
					return 0;
			}
		}

		//位置調整のためのYを取得
		float ToLayoutOffsetY(float height)
		{
			switch (LayoutType)
			{
				case Layout.TopLeft:
				case Layout.Top:
				case Layout.TopRight:
					return 0;
				case Layout.Left:
				case Layout.Center:
				case Layout.Right:
					return -height / 2;
				case Layout.BottomLeft:
				case Layout.Bottom:
				case Layout.BottomRight:
					return -height;
				default:
					return 0;
			}
		}


#if UNITY_EDITOR
		// 描画範囲をギズモで表示
		void OnDrawGizmos()
		{
			if (UnityEditor.Selection.activeGameObject != this.gameObject) return;

			float w = (float)MaxWidth / PixcelsToUnits;
			float h = (float)MaxHeight / PixcelsToUnits;

			if (w == 0 && h == 0) return;

			Vector3 pos = Vector3.zero;
			switch (LayoutType)
			{
				case Layout.TopLeft:
					pos.x += w / 2;
					pos.y -= h / 2;
					break;
				case Layout.Top:
					pos.y -= h / 2;
					break;
				case Layout.TopRight:
					pos.x -= w / 2;
					pos.y -= h / 2;
					break;
				case Layout.Left:
					pos.x += w / 2;
					break;
				case Layout.Center:
					break;
				case Layout.Right:
					pos.x -= w / 2;
					break;
				case Layout.BottomLeft:
					pos.x += w / 2;
					pos.y += h / 2;
					break;
				case Layout.Bottom:
					pos.y += h / 2;
					break;
				case Layout.BottomRight:
					pos.x -= w / 2;
					pos.y += h / 2;
					break;
				default:
					break;
			}
			Vector3 size = new Vector3(w, h, 0);
			Gizmos.matrix = CachedTransform.localToWorldMatrix;
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireCube(pos, size);
		}
#endif
	}
}