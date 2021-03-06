//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// ジャンプのマネージャー
	/// </summary>
	internal class AdvJumpManager
	{
		//ジャンプ先のラベル名
		string label;
		internal string Label
		{
			get { return label; }
		}

		class RandomInfo
		{
			public string label;
			public float rate;
			public RandomInfo(string label, float rate)
			{
				this.label = label;
				this.rate = rate;
			}
		}

		List<RandomInfo> randomInfoList = new List<RandomInfo>();

		//ジャンプ先のラベルがあるか
		internal bool IsLabeled
		{
			get { return !string.IsNullOrEmpty(label); }
		}

		//ジャンプ先のラベルを登録
		internal void RegistoreLabel(string jumpLabel)
		{
			this.label = jumpLabel;
		}
		
		//ランダムラベルを登録
		internal void AddRandom(string label, float rate)
		{
			randomInfoList.Add(new RandomInfo(label, rate));
		}

		internal void Clear()
		{
			label = "";
			randomInfoList.Clear();
		}

		internal void ReserveRandomJump()
		{
			//各要素の合計値を計算
			float sum = 0;
			randomInfoList.ForEach(item => sum += item.rate );
			if(sum <= 0)
			{
				//合計値が0以下。つまりランダムジャンプは無効
				Clear();
			}
			else
			{
				//ランダム値を計算
				float rand = Random.Range(0, sum);

				//ランダムジャンプ先のラベルを取得
				string label = "";
				foreach( RandomInfo info in randomInfoList )
				{
					rand -= info.rate;
					if (rand<=0)
					{
						label = info.label;
						break;
					}
				}
	
				Clear();
				RegistoreLabel(label);
			}
		}
	}
}