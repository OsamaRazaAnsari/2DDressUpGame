//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System.Collections;

namespace Utage
{

	/// <summary>
	/// チェックボックスコンポーネント
	/// </summary>
	[AddComponentMenu("Utage/Legacy/UI/CheckBox")]
	public class LegacyUiCheckBox : LegacyUiButton
	{
		/// <summary>
		/// ラジオボタンにする場合、グループのルートオブジェクト
		/// </summary>
		public LegacyUiRadioButtonGroup RadioButtonGroup
		{
			get { return radioButtonGroup; }
		}
		[SerializeField]
		LegacyUiRadioButtonGroup radioButtonGroup;

		/// <summary>
		/// チェック時の表示スプライト
		/// </summary>
		public Sprite2D CheckSprite
		{
			get { return checkSprite; }
			set { checkSprite = value; }
		}
		[SerializeField]
		Sprite2D checkSprite;

		/// <summary>
		/// チェックがONか
		/// </summary>
		public bool IsChecked
		{
			get { return this.isChecked; }
			set
			{
				this.isChecked = value;
				if (null != CheckSprite)
				{
					CheckSprite.LocalAlpha = IsChecked ? 1f : 0f;
				}
			}
		}
		[SerializeField]
		bool isChecked = true;

		/// <summary>
		/// クリックされた
		/// </summary>
		/// <param name="touch">タッチ入力データ</param>
		protected override void OnClick(TouchData2D touch)
		{
			if (null == RadioButtonGroup)
			{
				IsChecked = !IsChecked;
			}
			else
			{
				if (!IsChecked) IsChecked = true;
			}

			base.OnClick(touch);
		}
	}

}