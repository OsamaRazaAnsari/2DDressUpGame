//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// シーン回想のデータ
	/// </summary>
	public partial class AdvSceneGallerySettingData : StringGridContainerKeyValue
	{
		/// <summary>
		/// シナリオラベル
		/// </summary>
		public string ScenarioLabel { get { return this.Key; } }
		
		/// <summary>
		/// タイトル
		/// </summary>
		public string Title { get { return this.title; } }
		string title;

		/// <summary>
		/// カテゴリ名
		/// </summary>
		public string Category { get { return this.category; } }
		string category;

		/// <summary>
		/// サムネイル用ファイル名
		/// </summary>
		string thumbnailName;

		/// <summary>
		/// サムネイル用ファイルパス
		/// </summary>
		public string ThumbnailPath { get { return this.thumbnailPath; } }
		string thumbnailPath;

		/// <summary>
		/// サムネイル用ファイルのバージョン
		/// </summary>
		public int ThumbnailVersion { get { return this.thumbnailVersion; } }
		int thumbnailVersion;

		/// <summary>
		/// StringGridの一行からデータ初期化
		/// </summary>
		/// <param name="row">初期化するためのデータ</param>
		/// <returns>成否</returns>
		public override bool InitFromStringGridRow(StringGridRow row)
		{
			string key = AdvCommandParser.ParseScenarioLabel(row, AdvColumnName.ScenarioLabel);
			InitKey(key);
			this.title = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Title,"");
			this.thumbnailName = AdvParser.ParseCell<string>(row, AdvColumnName.Thumbnail);
			this.thumbnailVersion = AdvParser.ParseCellOptional<int>(row, AdvColumnName.ThumbnailVersion, 0);
			this.category = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Categolly, "");
			return true;
		}

		/// <summary>
		/// 起動時の初期化
		/// </summary>
		/// <param name="settingData">設定データ</param>
		public void BootInit(AdvBootSetting settingData)
		{
			thumbnailPath = settingData.ThumbnailDirInfo.FileNameToPath(thumbnailName);
		}
	}

	/// <summary>
	/// シーン回想のデータ
	/// </summary>
	[System.Serializable]
	public partial class AdvSceneGallerySetting : StringGridContainer<AdvSceneGallerySettingData>
	{
		/// <summary>
		/// 起動時の初期化
		/// </summary>
		/// <param name="settingData">設定データ</param>
		public void BootInit(AdvBootSetting settingData)
		{
			BootInit();
			foreach (AdvSceneGallerySettingData data in List)
			{
				//初期化
				data.BootInit(settingData);
				//ファイルマネージャーにバージョンの登録
				AssetFile file = AssetFileManager.GetFileCreateIfMissing(data.ThumbnailPath);
				if(file!=null) file.Version = data.ThumbnailVersion;
			}
		}

		/// <summary>
		/// 全てのリソースをダウンロード
		/// </summary>
		public void DownloadAll()
		{
			//ファイルマネージャーにバージョンの登録
			foreach (AdvSceneGallerySettingData data in List)
			{
				AssetFileManager.Download(data.ThumbnailPath);
			}
		}

		/// <summary>
		/// ギャラリー用のデータを取得
		/// </summary>
		/// <param name="category">カテゴリ</param>
		public List<AdvSceneGallerySettingData> CreateGalleryDataList(string category)
		{
			List<AdvSceneGallerySettingData> list = new List<AdvSceneGallerySettingData>();
			foreach (var item in List)
			{
				if (item.Category == category)
				{
					list.Add(item);
				}
			}
			return list;
		}

		/// <summary>
		/// カテゴリのリストを取得
		/// </summary>
		public List<string> CreateCategoryList()
		{
			List<string> list = new List<string>();
			foreach (var item in List)
			{
				if (string.IsNullOrEmpty(item.ThumbnailPath)) continue;
				if (!list.Contains(item.Category))
				{
					list.Add(item.Category);
				}
			}
			return list;
		}
	}
}