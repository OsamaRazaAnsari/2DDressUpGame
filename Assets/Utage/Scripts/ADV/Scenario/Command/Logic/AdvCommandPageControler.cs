//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：ページ制御
	/// </summary>
	internal class AdvCommandPageControler : AdvCommand
	{
		public AdvCommandPageControler(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
//			dataManager.PageContoroller.IsKeepText = true;
		}

		public override void DoCommand(AdvEngine engine)
		{
//			engine.Page.Contoller.IsKeepText = true;
		}
	}
}