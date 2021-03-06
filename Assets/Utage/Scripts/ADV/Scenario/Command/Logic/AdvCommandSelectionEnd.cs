//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：選択肢追加終了
	/// </summary>
	internal class AdvCommandSelectionEnd : AdvCommand
	{
		public AdvCommandSelectionEnd(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			dataManager.PageContoroller.Clear();
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.Config.StopSkipInSelection();
			engine.SelectionManager.StartWaiting();
			engine.Page.SetSelectionWait();
		}

		//コマンド終了待ち
		public override bool Wait(AdvEngine engine)
		{
			if (engine.SelectionManager.IsSelected)
			{
				AdvSelection selected = engine.SelectionManager.Selected;
				string label = selected.JumpLabel;
				if (selected.Exp != null)
				{
					engine.Param.CalcExpression(selected.Exp);
				}
				engine.SelectionManager.Clear();
				engine.ScenarioPlayer.JumpManager.RegistoreLabel(label);
				return false;
			}
			return true;
		}

		//ページ区切りのコマンドか
		public override bool IsTypePageEnd() { return true; }
	}
}