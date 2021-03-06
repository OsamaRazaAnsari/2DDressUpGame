//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// シェイクアニメーションのデータ
	/// </summary>
	public class AdvEffectDataShake : AdvEffectDataTween
	{
		public AdvEffectDataShake(StringGridRow row)
		{
			this.targetName = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Arg1, "");
			if (!UtageToolKit.TryParaseEnum<TargetType>(this.targetName, out this.targetType))
			{
				this.targetType = TargetType.Default;
			}
			this.targetName = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Arg2, this.targetName);
			string defaultStr = " x=30 y=30";
			string arg = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Arg3, defaultStr);
			if (!arg.Contains("x=") && !arg.Contains("y="))
			{
				arg += defaultStr;
			}
			string easeType = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Arg4, "");
			string loopType = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Arg5, "");
			this.tweenData = new iTweenData(iTweenType.ShakePosition.ToString(), arg, easeType, loopType);

			this.waitType = AdvParser.ParseCellOptional<WaitType>(row, AdvColumnName.Arg6, WaitType.Wait);
			
			if (!string.IsNullOrEmpty(tweenData.ErrorMsg))
			{
				Debug.LogError(row.ToErrorString(tweenData.ErrorMsg));
			}
		}
	}
}
