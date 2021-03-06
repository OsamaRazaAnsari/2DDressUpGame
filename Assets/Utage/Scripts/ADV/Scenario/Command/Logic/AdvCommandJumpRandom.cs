//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：ランダムジャンプ
	/// </summary>
	internal class AdvCommandJumpRandom : AdvCommand
	{
		public AdvCommandJumpRandom(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			this.jumpLabel = AdvCommandParser.ParseScenarioLabel(row, AdvColumnName.Arg1);
			string expStr = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Arg2, "");
			if (string.IsNullOrEmpty(expStr))
			{
				this.exp = null;
			}
			else
			{
				this.exp = dataManager.DefaultParam.CreateExpressionBoolean(expStr);
				if (this.exp.ErrorMsg != null)
				{
					Debug.LogError(row.ToErrorString(this.exp.ErrorMsg));
				}
			}
			this.rate = AdvParser.ParseCellOptional<float>(row, AdvColumnName.Arg3, 1);
		}


		public override void DoCommand(AdvEngine engine)
		{
			if (IsEnable(engine.Param))
			{
				engine.ScenarioPlayer.JumpManager.AddRandom(jumpLabel,rate);
			}
		}

		bool IsEnable( AdvParamSetting param )
		{
			return (exp == null || param.CalcExpressionBoolean( exp ) );
		}

		public override string GetJumpLabel() { return jumpLabel; }
		public override bool IsContinusCommand { get { return true; } }
		
		string jumpLabel;
		ExpressionParser exp;	//ジャンプ条件式
		float rate;
	}
}