//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Utage
{
	//絵文字用のデータ
	public class UguiNovelTextEmojiData : ScriptableObject
	{
		public int Size { get { return size; } }
		[SerializeField]
		int size;
		
		[SerializeField]
		List<Sprite> spriteTbl;

		Dictionary<char, Sprite> SpriteDictionary { get { if (spriteDictionary == null) MakeDictionary(); return spriteDictionary; } }
		Dictionary<char, Sprite> spriteDictionary;

		Dictionary<string, Sprite> SpriteDictionaryStringKey { get { if (spriteDictionaryStringKey == null) MakeDictionary(); return spriteDictionaryStringKey; } }
		Dictionary<string, Sprite> spriteDictionaryStringKey;

		void MakeDictionary()
		{
			spriteDictionary = new Dictionary<char, Sprite>();
			spriteDictionaryStringKey = new Dictionary<string, Sprite>();
			foreach (Sprite sprite in spriteTbl)
			{
				spriteDictionaryStringKey.Add(sprite.name, sprite);

				try
				{
					char c = System.Convert.ToChar(System.Convert.ToInt32(sprite.name, 16));
					spriteDictionary.Add(c, sprite);
				}
				catch
				{

				}
			}
		}

		public Sprite GetSprite(string key)
		{
			Sprite sprite;
			if (SpriteDictionaryStringKey.TryGetValue(key, out sprite))
			{
				return sprite;
			}
			else
			{
				return null;
			}
		}
		
		public bool Contains(char c)
		{
			return SpriteDictionary.ContainsKey(c);
		}

		public Sprite GetSprite(char c)
		{
			Sprite sprite;
			if (SpriteDictionary.TryGetValue(c, out sprite))
			{
				return sprite;
			}
			else
			{
				return null;
			}
		}
	};
}
