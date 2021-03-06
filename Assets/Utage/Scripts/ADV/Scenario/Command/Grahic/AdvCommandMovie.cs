//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：ムービー再生
	/// </summary>
	internal class AdvCommandMovie : AdvCommand
	{
		public AdvCommandMovie(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			this.label = AdvParser.ParseCell<string>(row, AdvColumnName.Arg1);
		}

		public override void DoCommand(AdvEngine engine)
		{
			WrapperMoviePlayer.Play (label);
		}

		public override bool Wait(AdvEngine engine)
		{
			if (engine.UiManager.IsPointerDowned)
			{
				WrapperMoviePlayer.Cancel();
			}
			bool isWait = WrapperMoviePlayer.IsPlaying();
			return isWait;
		}
		string label;
	}
}