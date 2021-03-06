//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：テキスト表示（地の文）
	/// </summary>
	internal class AdvCommandText : AdvCommand
	{

		public AdvCommandText(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			//ページコントローラー
			this.pageCtrlType = AdvParser.ParseCellOptional<AdvPageController.Type>(row, AdvColumnName.PageCtrl, AdvPageController.Type.None );
			dataManager.PageContoroller.Update(this.pageCtrlType);

			//メッセージウィンドウのタイプ
			this.windowType = AdvParser.ParseCellOptional<string>(row, AdvColumnName.WindowType, "");
			
			this.text = AdvParser.ParseCell<string>(row, AdvColumnName.Text);
			if (AdvCommand.IsEditorErrorCheck)
			{
				TextData textData = new TextData(text);
				if (!string.IsNullOrEmpty(textData.ErrorMsg))
				{
					Debug.LogError(row.ToErrorString(textData.ErrorMsg));
				}
			}
			
			//ページの末端かチェック
			//ページコントローラーでテキストを表示しつづける場合は、ページ末端じゃない
			this.isPageEnd = (text.Length > 0) && !dataManager.PageContoroller.IsKeepText;
		}

		public override void DoCommand(AdvEngine engine)
		{
			//ページコントローラー処理
			engine.Page.Contoller.Update( this.pageCtrlType );

			engine.Page.ChangeMessageWindowText(text, windowType);
			if (text.Length > 0) engine.BacklogManager.Add(engine.Page.TextData.NoneMetaString);
		}

		public override bool Wait(AdvEngine engine)
		{
			return engine.Page.IsWaitText;
		}

		//ページ区切りのコマンドか
		public override bool IsTypePageEnd() { return isPageEnd; }

		protected string windowType;
		protected string text;
		protected AdvPageController.Type pageCtrlType;
		protected bool isPageEnd;
	}
}