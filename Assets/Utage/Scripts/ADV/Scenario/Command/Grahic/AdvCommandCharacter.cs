//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：キャラクター＆台詞表示
	/// </summary>
	internal class AdvCommandCharacter : AdvCommand
	{

		public AdvCommandCharacter(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			//名前
			string name = AdvParser.ParseCell<string>(row, AdvColumnName.Arg1);
			//パターンラベル
			string patternLabel = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Arg2, "");
			//キャラの表示情報を取得
			this.characterInfo = dataManager.CharacterSetting.GetCharacterInfo(name, patternLabel);
			AddLoadGraphic(characterInfo.Graphic);
			//表示レイヤー
			this.layerName = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Arg3, "");
			if (!string.IsNullOrEmpty(layerName) && !dataManager.LayerSetting.Contains(layerName, AdvLayerSettingData.LayerType.Character))
			{
				//表示レイヤーが見つからない
				Debug.LogError(row.ToErrorString(layerName + " is not contained in layer setting"));
			}

			//グラフィック表示処理を作成
			this.graphicOperaitonArg 
				= new AdvGraphicOperaitonArg( 
					row, 
					characterInfo.Graphic, 
					AdvParser.ParseCellOptional<float>(row, AdvColumnName.Arg6, 0.2f), 
					characterInfo.IsNonePattern );

			//メッセージウィンドウのタイプ
			this.windowType = AdvParser.ParseCellOptional<string>(row, AdvColumnName.WindowType, "");

			//ページコントローラー
			this.pageCtrlType = AdvParser.ParseCellOptional<AdvPageController.Type>(row, AdvColumnName.PageCtrl, AdvPageController.Type.None);
			dataManager.PageContoroller.Update(this.pageCtrlType);

			//テキスト関連
			this.text = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Text, "");
			if (AdvCommand.IsEditorErrorCheck)
			{
				TextData textData = new TextData(text);
				if (!string.IsNullOrEmpty(textData.ErrorMsg))
				{
					Debug.LogError(row.ToErrorString(textData.ErrorMsg));
				}
			}
			//ボイス
			string voice = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Voice, "");
			int voiceVersion = AdvParser.ParseCellOptional<int>(row, AdvColumnName.VoiceVersion, 0);

			//サウンドファイル設定
			if (!string.IsNullOrEmpty(voice))
			{
				if (AdvCommand.IsEditorErrorCheck)
				{
				}
				else
				{
					voiceFile = AddLoadFile(dataManager.SettingData.VoiceDirInfo.FileNameToPath(voice));
					if (voiceFile != null) voiceFile.Version = voiceVersion;
					//ストリーミング再生にバグがある模様。途中で無音が混じると飛ばされる？
					//				voiceFile.LoadFlags = AssetFileLoadFlags.Streaming;
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

			//キャラクター表示更新
			if (!engine.GraphicManager.IsEventMode)
			{

				//非表示なら
				if (characterInfo.IsHide)
				{
					engine.GraphicManager.CharacterManager.FadeOut( characterInfo.Label, graphicOperaitonArg.FadeTime );
				}
				else
				{
					if ( characterInfo.Graphic == null || characterInfo.Graphic.Main == null)
					{

					}
					else
					{
						//パターン指定なしで、既に同名のキャラがいるなら、表示パターンを引き継ぐ仕様は廃止？
						//if (data.CharacterInfo.IsNonePattern && IsContiansDefalutGraphic(data.Name))

						//表示する
						engine.GraphicManager.CharacterManager.DrawCharacter(layerName, characterInfo.Label, graphicOperaitonArg);
					}
				}
			}


			if (null != voiceFile)
			{
				engine.SoundManager.PlayVoice(characterInfo.Label, voiceFile);
			}
			if (text.Length > 0)
			{
				engine.Page.ChangeMessageWindowText(characterInfo.NameText, characterInfo.Label, text, windowType);
				engine.BacklogManager.Add(engine.Page.TextData.NoneMetaString, characterInfo, voiceFile);
			}
		}
		
		public override bool Wait(AdvEngine engine)
		{
			return engine.Page.IsWaitText;
		}

		//ページ区切りのコマンドか
		public override bool IsTypePageEnd() { return isPageEnd; }

		protected AdvGraphicOperaitonArg graphicOperaitonArg;

		protected AdvCharacterInfo characterInfo;
		protected string layerName;
		protected string text;
		protected AssetFile voiceFile;
		protected bool isPageEnd;
		protected AdvPageController.Type pageCtrlType;
		protected string windowType;
	}

}