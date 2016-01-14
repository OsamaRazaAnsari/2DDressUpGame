//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Utage
{

	/// <summary>
	/// キャラクタのテクスチャ設定（名前や表情と、テクスチャ名の対応）
	/// </summary>
	public partial class AdvCharacterSettingData : StringGridContainerKeyValue
	{
		//独自にカスタムしたいファイルタイプの、ルートディレクトリを指定
		public delegate void ParseCustomFileTypeRootDir(string fileType, ref string rootDir);
		public static ParseCustomFileTypeRootDir CallbackParseCustomFileTypeRootDir;

		//独自にカスタムしたいファイルタイプの、ルートディレクトリを指定
		public delegate void ParseCustomFileTypeLoadComplete(string fileType, ref AssetFileLoadComplete onLoadCmplete);
		public static ParseCustomFileTypeLoadComplete CallbackParseCustomFileTypeLoadComplete;

		/// <summary>
		/// キャラ名
		/// </summary>
		public string Name { get { return this.name; } }
		string name;

		/// <summary>
		/// 表示パターンターン
		/// </summary>
		public string Pattern { get { return this.pattern; } }
		string pattern;
	
		/// <summary>
		/// 表示名のテキスト
		/// </summary>
		public string NameText { get { return this.nameText; } }
		string nameText;

		//グラフィックの情報
		public GraphicInfoList Graphic { get { return this.graphic; } }
		GraphicInfoList graphic;

		/// <summary>
		/// StringGridの一行からデータ初期化
		/// ただし、このクラスに限っては未使用
		/// </summary>
		/// <param name="row">初期化するためのデータ</param>
		/// <returns>成否</returns>
		public override bool InitFromStringGridRow(StringGridRow row)
		{
			Debug.LogError("Not Use");
			return false;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="key">キー(キャラ名を)</param>
		/// <param name="fileName">ファイルネーム</param>
		public void Init(string name, string pattern, string nameText, StringGridRow row )
		{
			this.name = name;
			this.pattern = pattern;
			this.InitKey( AdvCharacterSetting.ToDataKey(name, pattern));
			this.nameText = nameText;
			this.graphic = new GraphicInfoList( AdvGraphicInfoParser.TypeCharacter, Key , AdvParser.ParseCellOptional<string>(row, AdvColumnName.FileType, ""));
			if (AdvParser.ParseCellOptional<string>(row, AdvColumnName.FileName, "").Length > 0)
			{
				AddGraphicInfo(row);
			}
		}

		/// <summary>
		/// 起動時の初期化
		/// </summary>
		/// <param name="settingData">設定データ</param>
		public void BootInit(AdvBootSetting settingData)
		{
			Graphic.BootInit((fileName) => FileNameToPath(fileName, settingData));
						
			//特定のファイルタイプなら、ロード終了時の処理をする
			if (CallbackParseCustomFileTypeLoadComplete != null && !AssetFileManager.IsEditorErrorCheck)
			{
				AssetFileLoadComplete onLoadComplete = null;
				CallbackParseCustomFileTypeLoadComplete(this.graphic.FileType, ref onLoadComplete);
				if (onLoadComplete != null)
				{
					foreach (GraphicInfo info in Graphic.InfoList)
					{
						info.File.OnLoadComplete += onLoadComplete;
					}
				}
			}
		}

		string FileNameToPath(string fileName, AdvBootSetting settingData)
		{
			string root = null;
			if (CallbackParseCustomFileTypeRootDir!=null)
			{
				CallbackParseCustomFileTypeRootDir(this.graphic.FileType, ref root);
			}
			if (root!=null)
			{
				return settingData.ResorceDir + root + "/" +  fileName;
			}
			else
			{
				return settingData.CharacterDirInfo.FileNameToPath(fileName);
			}
		}


		internal void AddGraphicInfo(StringGridRow row)
		{
			Graphic.Add(row);
		}
	};

	/// <summary>
	/// キャラクタのテクスチャ設定（名前や表情と、テクスチャ名の対応）
	/// </summary>
	[System.Serializable]
	public partial class AdvCharacterSetting : StringGridContainer<AdvCharacterSettingData>
	{
		/// <summary>
		/// 各キャラのデフォルト表情のキーの一覧
		/// </summary>
		DictionaryString defaultKey = new DictionaryString();

		//連続するデータとして追加できる場合はする。
		protected override bool TryParseContiunes(AdvCharacterSettingData last, StringGridRow row)
		{
			if (last == null) return false;

			//キャラ名
			string name = AdvParser.ParseCellOptional<string>(row, AdvColumnName.CharacterName,"");
			//表示パターン
			string pattern = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Pattern, "");

			//キャラ名と表示パターンが空白なら、グラフィック情報のみを追加
			if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(pattern))
			{
				last.AddGraphicInfo(row);
				return true;
			}
			else
			{
				return false;
			}
		}

		//連続するデータとして追加できる場合はする。基本はしない
		protected override AdvCharacterSettingData ParseFromStringGridRow(AdvCharacterSettingData last, StringGridRow row)
		{
			//キャラ名
			string name = AdvParser.ParseCellOptional<string>(row, AdvColumnName.CharacterName,"");
			//表示パターン
			string pattern = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Pattern, "");
			//表示名
			string nameText = AdvParser.ParseCellOptional<string>(row, AdvColumnName.NameText, "");

			//キャラ名が空白なら、直前と同じキャラ名を使う
			if(string.IsNullOrEmpty(name) )
			{
				if( last == null )
				{
					Debug.LogError(row.ToErrorString("Not Found Chacter Name"));
					return null;
				}
				name = last.Name;
			}

			//表示名が空で、直前のデータとキャラ名が同じならその名前を使う
			if(string.IsNullOrEmpty(nameText))
			{
				if(last!=null && (name == last.Name) )
				{
					nameText = last.NameText;
				}
				else
				{
					nameText = name;
				}
			}
 
			AdvCharacterSettingData data = new AdvCharacterSettingData();
			data.Init( name, pattern, nameText, row);

			if (!Dictionary.ContainsKey(data.Key))
			{
				Add(data);
				if (!defaultKey.ContainsKey(name))
				{
					defaultKey.Add(name, data.Key);
				}
				return data;
			}
			else
			{
				string errorMsg = "";
				errorMsg += row.ToErrorString(ColorUtil.AddColorTag(data.Key, Color.red) + "  is already contains");
				Debug.LogError(errorMsg);
			}
			return null;
		}

/*
		/// <summary>
		/// StringGridのデータ解析
		/// </summary>
		/// <param name="grid">解析するデータ</param>
		protected override void ParseFromStringGrid(StringGrid grid)
		{
			defaultKey.Clear();
			string name = "";
			string nameText = "";
			foreach (StringGridRow row in grid.Rows)
			{
				if (row.RowIndex < grid.DataTopRow) continue;			//データの行じゃない
				if (row.IsEmpty) continue;								//データがない
				//名前は空白なら、直前のものと同じ
				string newName = AdvParser.ParseCellOptional<string>(row,AdvColumnName.CharacterName, name);
				if(newName!=name)
				{
					name = newName;
					nameText = "";
				}
				//表示パターン
				string pattern = ToDataKey(name, AdvParser.ParseCellOptional<string>(row, AdvColumnName.Pattern, ""));

				//表示名は空白なら、直前のものと同じ
				nameText = AdvParser.ParseCellOptional<string>(row, AdvColumnName.NameText, nameText);

				//データ追加
				AdvCharacterSettingData data = new AdvCharacterSettingData();
				data.Init(pattern,
					string.IsNullOrEmpty(nameText) ? name : nameText,
					row);
				Add(data);
				if (!defaultKey.ContainsKey(name))
				{
					defaultKey.Add(name, pattern);
				}
			}
		}
*/
		/// <summary>
		/// 起動時の初期化
		/// </summary>
		/// <param name="settingData">設定データ</param>
		public void BootInit(AdvBootSetting settingData)
		{
			defaultKey.Clear();
			BootInit();
			foreach (AdvCharacterSettingData data in List)
			{
				data.BootInit(settingData);
			}
		}

		/// <summary>
		/// 全てのリソースをダウンロード
		/// </summary>
		public void DownloadAll()
		{
			foreach (AdvCharacterSettingData data in List)
			{
				data.Graphic.DownloadAll();
			}
		}


		/// <summary>
		/// 指定のキャラ名の立ち絵があるか
		/// </summary>
		/// <param name="name">キャラ名</param>
		/// <returns>ファイルパス</returns>
		public bool Contains(string name)
		{
			return defaultKey.ContainsKey(name);
		}
/*
		/// <summary>
		/// 指定のキャラ名の立ち絵があるか
		/// </summary>
		/// <param name="name">キャラ名</param>
		/// <param name="label">ラベル</param>
		/// <returns>ファイルパス</returns>
		public bool Contains(string name, string label)
		{
			if (!defaultKey.ContainsKey(name))
			{
				return false;
			}
			else
			{
				string key = ToDataKey(name, label);
				return this.ContainsKey(key);
			}
		}
*/
/*
		/// <summary>
		/// キャラのデフォルトファイルパスを取得
		/// </summary>
		/// <param name="name">キャラ名</param>
		/// <returns>ファイルパス</returns>
		public string GetDefaultPath(string name )
		{
			if (!defaultKey.ContainsKey(name))
			{
				Debug.LogError("Not found default texture :" + name );
				return "";
			}
			string key = defaultKey.Get(name);
			return FindData(key).FilePath;
		}
*/
/*
		/// <summary>
		/// ラベルからテクスチャ情報を取得
		/// </summary>
		/// <param name="label">ラベル</param>
		/// <returns>スプライト情報</returns>
		public GraphicInfo LabelToTextureInfo(string name, string label)
		{
			//既に絶対URLならそのまま
			if (UtageToolKit.IsAbsoluteUri(label))
			{
				//ラベルをそのままファイル名扱いに
				return new GraphicInfo(label);
			}
			else
			{
				string key = ToFileKey(name, label);
				AdvCharacterSettingData data = FindData(key);
				if (data == null)
				{
					//名前をキーに
					data = FindData(ToFileKey(name, ""));
					if( data == null )
					{
						//ラベルをそのままファイル名扱いに
						return new GraphicInfo(label);
					}
					else
					{
						return data.GraphicInfo;
					}
				}
				else
				{
					return data.GraphicInfo;
				}
			}
		}
*/
		/// <summary>
		/// キーからグラフィック情報を取得
		/// </summary>
		/// <param name="label">ラベル</param>
		/// <returns>スプライト情報</returns>
		public GraphicInfoList KeyToGraphicInfo(string key)
		{
			//既に絶対URLならそのまま
			if (UtageToolKit.IsAbsoluteUri(key))
			{
				//ラベルをそのままファイル名扱いに
				return new GraphicInfoList(key);
			}
			else
			{
				AdvCharacterSettingData data = FindData(key);
				if (data == null)
				{
					//ラベルをそのままファイル名扱いに
					return new GraphicInfoList(key);
				}
				else
				{
					return data.Graphic;
				}
			}
		}

/*
		/// <summary>
		/// ラベルからファイルパスを取得
		/// </summary>
		/// <param name="name">キャラ名</param>
		/// <param name="label">ラベル</param>
		/// <returns>ファイルパス</returns>
		public string LabelToPath(string name, string label)
		{
			//既に絶対URLならそのまま
			if (UtageToolKit.IsAbsoluteUri(label))
			{
				return label;
			}
			else
			{
				string key = ToFileKey(name, label);
				AdvCharacterSettingData data = FindData(key);
				if (data == null)
				{
					//ラベルをそのままファイル名扱いに
					return label;
				}
				else
				{
					return data.FilePath;
				}
			}
		}
*/

		public AdvCharacterInfo GetCharacterInfo(string nameText, string patternLabel)
		{
			string characterTag = "";
			bool isHide = false;
			Func<string, string, bool> callbackTagParse = (tagName, arg) =>
			{
				switch (tagName)
				{
					case "Off":
						isHide = true;
						return true;
					case "Character":
						characterTag = arg;
						return true;
					default:
						return false;
				}
			};

			patternLabel = ParserUtil.ParseTagTextToString(patternLabel, callbackTagParse);
			return GetCharacterInfoSub(nameText, characterTag, patternLabel, isHide);
		}

		AdvCharacterInfo GetCharacterInfoSub(string nameText, string characterTag, string patternLabel, bool isHide)
		{
			string characterLabel = string.IsNullOrEmpty(characterTag) ? nameText : characterTag;
			AdvCharacterInfo info = new AdvCharacterInfo(characterLabel, isHide, string.IsNullOrEmpty(patternLabel));
			info.NameText = nameText;
			if (!Contains(characterLabel))
			{
				return info;
			}

			if (!isHide)
			{
				//デフォルトパターン
				AdvCharacterSettingData data = FindData(defaultKey.Get(characterLabel));

				//既に絶対URLならそのまま
				if (UtageToolKit.IsAbsoluteUri(patternLabel))
				{
					info.Graphic = new GraphicInfoList(patternLabel);
				}
				else
				{
					AdvCharacterSettingData patternData = info.IsNonePattern ? data : FindData(ToDataKey(characterLabel, patternLabel));

					if (patternData == null)
					{
						if (data.Graphic.IsDefaultFileType)
						{
							info.ErrorMsg = characterLabel + ", " + patternLabel + " is not contained in file setting";
							//ラベルをそのままファイル名扱いに
							info.Graphic = new GraphicInfoList(patternLabel);
						}
						else
						{
							info.Graphic = data.Graphic;
						}
					}
					else
					{
						data = patternData;
						info.Graphic = patternData.Graphic;
					}
				}

				if (string.IsNullOrEmpty(characterTag) && !string.IsNullOrEmpty(data.NameText))
				{
					info.NameText = data.NameText;
				}
			}
			return info;
		}


		//キーからファイルデータを取得
		AdvCharacterSettingData FindData(string key)
		{
			AdvCharacterSettingData data;
			if (!TryGetValue(key, out data))
			{
				return null;
			}
			else
			{
				return data;
			}
		}

		//キーの変更
		static internal string ToDataKey(string name, string label)
		{
			//名前とラベルからキーを
			string key = string.Format(
				"{0},{1}",
				name,
				label
				);
			return key;
		}

		internal GraphicInfoList FindFromPath(string texturePath )
		{
			foreach (var item in Dictionary.Values)
			{

				if (item.Graphic.FindFromPath(texturePath) != null)
				{
					return item.Graphic;
				}
			}
			return null;
		}
	}
}