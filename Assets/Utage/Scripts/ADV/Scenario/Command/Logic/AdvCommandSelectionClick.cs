//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：クリックによる選択肢表示
	/// </summary>
	internal class AdvCommandSelectionClick : AdvCommand
	{

		public AdvCommandSelectionClick(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			this.jumpLabel = AdvCommandParser.ParseScenarioLabel(row, AdvColumnName.Arg1);
			string expStr = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Arg2, "");
			if( string.IsNullOrEmpty(expStr) )
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

			string selectedExpStr = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Arg3, "");
			if (string.IsNullOrEmpty(selectedExpStr))
			{
				this.selectedExp = null;
			}
			else
			{
				this.selectedExp = dataManager.DefaultParam.CreateExpression(selectedExpStr);
				if (this.selectedExp.ErrorMsg != null)
				{
					Debug.LogError(row.ToErrorString(this.selectedExp.ErrorMsg));
				}
			}

			this.name = AdvParser.ParseCell<string>(row, AdvColumnName.Arg4 );
			this.isPolygon = AdvParser.ParseCellOptional<bool>(row, AdvColumnName.Arg5, true );
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (exp == null || engine.Param.CalcExpressionBoolean(exp))
			{
				engine.SelectionManager.AddSelectionClick(jumpLabel, name, isPolygon, selectedExp, RowData );
			}
		}

		public override string GetJumpLabel() { return jumpLabel; }
		public override bool IsContinusCommand { get { return true; } }

		string name;						//表示物の名前
		bool isPolygon;						//ポリゴンコライダーを使うか
		string jumpLabel;					//ジャンプ先のラベル
		ExpressionParser exp;				//選択肢表示条件式
		ExpressionParser selectedExp;		//選択後に実行する演算式
	}
}