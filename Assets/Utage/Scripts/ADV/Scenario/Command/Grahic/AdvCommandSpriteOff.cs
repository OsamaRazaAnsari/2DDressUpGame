//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------


namespace Utage
{

	/// <summary>
	/// コマンド：スプライト表示
	/// </summary>
	internal class AdvCommandSpriteOff : AdvCommand
	{
		public AdvCommandSpriteOff(StringGridRow row)
			: base(row)
		{
			this.name = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Arg1, "");
			this.fadeTime = AdvParser.ParseCellOptional<float>(row, AdvColumnName.Arg6, fadeTime);
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (string.IsNullOrEmpty(name))
			{
				engine.GraphicManager.SpriteManager.FadeOutAll(fadeTime);
			}
			else
			{
				engine.GraphicManager.SpriteManager.FadeOut(name, fadeTime);
			}
		}

		string name;
		float fadeTime = 0.2f;
	}
}