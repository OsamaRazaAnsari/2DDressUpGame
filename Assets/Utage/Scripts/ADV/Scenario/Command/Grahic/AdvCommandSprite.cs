//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：スプライト表示
	/// </summary>
	internal class AdvCommandSprite : AdvCommand
	{
		public AdvCommandSprite(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			this.spriteName = AdvParser.ParseCell<string>(row, AdvColumnName.Arg1);
			string fileName = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Arg2, spriteName);

			if (!dataManager.TextureSetting.ContainsLabel(fileName))
			{
				Debug.LogError(row.ToErrorString(fileName + " is not contained in file setting"));
			}

			this.graphic = dataManager.TextureSetting.LabelToGraphic(fileName);
			AddLoadGraphic(graphic);
			this.layerName = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Arg3, "");
			if (!string.IsNullOrEmpty(layerName) && !dataManager.LayerSetting.Contains(layerName, AdvLayerSettingData.LayerType.Sprite))
			{
				Debug.LogError(row.ToErrorString( layerName + " is not contained in layer setting"));
			}

			//グラフィック表示処理を作成
			this.graphicOperaitonArg =  new AdvGraphicOperaitonArg(row, graphic, AdvParser.ParseCellOptional<float>(row, AdvColumnName.Arg6, 0.2f) );
		}

		public override void DoCommand(AdvEngine engine)
		{
			//表示する
			engine.GraphicManager.SpriteManager.Draw(layerName, spriteName, graphicOperaitonArg);
		}

		protected GraphicInfoList graphic;
		protected string layerName;
		protected string spriteName;
		protected AdvGraphicOperaitonArg graphicOperaitonArg;
	}
}