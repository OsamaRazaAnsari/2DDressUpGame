//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------


namespace Utage
{

	/// <summary>
	/// コマンド：イベントCG表示OFF
	/// </summary>
	internal class AdvCommandBgEventOff : AdvCommand
	{
		public AdvCommandBgEventOff(StringGridRow row)
			: base(row)
		{
			this.fadeTime = AdvParser.ParseCellOptional<float>(row, AdvColumnName.Arg6, 0.2f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.GraphicManager.BgManager.FadeOutAll(fadeTime);
		}

		float fadeTime;
	}
}