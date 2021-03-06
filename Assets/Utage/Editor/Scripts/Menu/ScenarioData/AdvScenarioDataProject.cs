//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utage
{

	//「Utage」のシナリオデータ用のエクセルファイルのプロジェクトデータ
	public class AdvScenarioDataProject : ScriptableObject
	{
		/// <summary>
		/// エクセルのリスト
		/// </summary>
		public List<Object> ExcelList
		{
			get { return excelList; }
			set { excelList = value; }
		}
		[SerializeField]
		List<Object> excelList = new List<Object>();

		/// <summary>
		/// エクセルのパスリスト
		/// </summary>
		public List<string> ExcelPathList
		{
			get { return UtageEditorToolKit.AssetsToPathList(excelList); }
		}


		/// <summary>
		/// コンバート先のパス
		/// </summary>
		[SerializeField]
		string convertPath;
		public string ConvertPath
		{
			get { return convertPath; }
			set { convertPath = value; }
		}

		/// <summary>
		/// コンバートファイルのバージョン
		/// </summary>
		[SerializeField]
		int convertVersion = 0;
		public int ConvertVersion
		{
			get { return convertVersion; }
			set { convertVersion = value; }
		}

		/// <summary>
		/// コンバート後にバージョンを自動で更新するか
		/// </summary>
		[SerializeField]
		bool isAutoUpdateVersionAfterConvert = false;
		public bool IsAutoUpdateVersionAfterConvert
		{
			get { return isAutoUpdateVersionAfterConvert; }
			set { isAutoUpdateVersionAfterConvert = value; }
		}


		/// <summary>
		/// インポート時に自動でコンバートをするか
		/// </summary>
		[SerializeField]
		bool isAutoConvertOnImport = false;
		public bool IsAutoConvertOnImport
		{
			get { return isAutoConvertOnImport; }
			set { isAutoConvertOnImport = value; }
		}

		public bool IsEnableImport
		{
			get
			{
				bool isEnableImport = false;
				foreach (Object asset in excelList)
				{
					if (null != asset)
					{
						isEnableImport = true;
						break;
					}
				}
				return isEnableImport;
			}
		}

		public bool IsEnableConvert
		{
			get { return IsEnableImport && !string.IsNullOrEmpty(ConvertPath); }
		}

		public void AddExcelAsset( Object asset )
		{
			excelList.Add(asset);
		}

		/// <summary>
		/// 宴用のカスタムインポート設定を強制するSpriteフォルダassetのリスト
		/// </summary>
		[SerializeField]
		List<Object> customInportSpriteFolders = new List<Object>();
		public List<Object> CustomInportSpriteFolders
		{
			get { return customInportSpriteFolders; }
		}

		///宴用のカスタムインポート設定を強制するSpriteフォルダassetのリストを追加
		public void AddCustomImportSpriteFloders(List<Object> assetList)
		{
			CustomInportSpriteFolders.AddRange(assetList);
		}

		/// <summary>
		/// 宴用のカスタムインポート設定を強制するAudioフォルダassetのリスト
		/// </summary>
		[SerializeField]
		List<Object> customInportAudioFolders = new List<Object>();
		public List<Object> CustomInportAudioFolders
		{
			get { return customInportAudioFolders; }
		}
		///宴用のカスタムインポート設定を強制するSpriteフォルダassetのリストを追加
		public void AddCustomImportAudioFloders( List<Object> assetList )
		{
			CustomInportAudioFolders.AddRange(assetList);
		}

		/// <summary>
		/// 宴用のカスタムインポート設定を強制するMovieフォルダassetのリスト
		/// </summary>
		[SerializeField]
		List<Object> customInportMovieFolders = new List<Object>();
		public List<Object> CustomInportMovieFolders
		{
			get { return customInportMovieFolders; }
		}

		///宴用のカスタムインポート設定を強制するMovieフォルダassetのリストを追加
		public void AddCustomImportMovieFloders(List<Object> assetList)
		{
			CustomInportMovieFolders.AddRange(assetList);
		}

		/// <summary>
		/// リソースのパス
		/// </summary>
		[SerializeField]
		Object resourcesRoot;
		public Object ResourcesRoot
		{
			get { return resourcesRoot; }
			set { resourcesRoot = value; }
		}

		/// <summary>
		/// リソースの出力先のパス
		/// </summary>
		[SerializeField]
		string outputResourcesPath;
		public string OutputResourcesPath
		{
			get { return outputResourcesPath; }
			set { outputResourcesPath = value; }
		}

		/// <summary>
		/// リソースを暗号化して出力するか
		/// </summary>
		[SerializeField]
		bool isResoucesCopyNewerOnly;
		public bool IsResoucesCopyNewerOnly
		{
			get { return isResoucesCopyNewerOnly; }
			set { isResoucesCopyNewerOnly = value; }
		}

		/// <summary>
		/// 宴用のカスタムインポート設定を強制するAudioアセットかチェック
		/// </summary>
		public bool IsCustomImportAudio(AssetImporter importer)
		{
			string assetPath = importer.assetPath;
			foreach( Object folderAsset in CustomInportAudioFolders)
			{
				if (assetPath.StartsWith(AssetDatabase.GetAssetPath(folderAsset)))
				{
					return true;
				}
			}
			return false;
		}

		public enum TextureType
		{
			Unknown,
			Character,
			Sprite,
			BG,
			Event,
			Thumbnail,
		};
		/// <summary>
		/// 宴用のカスタムインポート設定を強制するSpriteアセットかチェック
		/// </summary>
		public TextureType ParseCustomImportTextureType(AssetImporter importer)
		{
			string assetPath = importer.assetPath;
			foreach (Object folderAsset in CustomInportSpriteFolders)
			{
				string floderPath = AssetDatabase.GetAssetPath(folderAsset);
				if (assetPath.StartsWith(floderPath))
				{
					string name = System.IO.Path.GetFileName( floderPath );
					TextureType type;
					if (UtageToolKit.TryParaseEnum<TextureType>(name, out type))
					{
						return type;
					}
					return TextureType.Unknown;
				}
			}
			return TextureType.Unknown;
		}

		/// <summary>
		/// 宴用のカスタムインポート設定を強制するMovieアセットかチェック
		/// </summary>
		public bool IsCustomImportMovie(AssetImporter importer)
		{
			string assetPath = importer.assetPath;
			foreach (Object folderAsset in CustomInportMovieFolders)
			{
				if (assetPath.StartsWith(AssetDatabase.GetAssetPath(folderAsset)))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 管理対象のリソースを再インポート
		/// </summary>
		public void ReImportResources()
		{
			ImportAssetOptions options = ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive;
			foreach( Object folder in CustomInportSpriteFolders )
			{
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(folder), options);
			}
			foreach (Object folder in CustomInportAudioFolders)
			{
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(folder), options);
			}
			AssetDatabase.Refresh();
		}
/*
		/// <summary>
		/// 管理対象のリソースを再インポート
		/// </summary>
		public void ReImportResources()
		{
			foreach (Object folder in CustomInportSpriteFolders)
			{
				ReImportFolder(AssetDatabase.GetAssetPath(folder));
			}
			foreach (Object folder in CustomInportSpriteFolders)
			{
				ReImportFolder(AssetDatabase.GetAssetPath(folder));
			}
			AssetDatabase.Refresh();
		}

		/// <summary>
		/// フォルダ以下のアセットを再インポート
		/// </summary>
		void ReImportFolder(string folderPath)
		{
			Debug.Log(folderPath);
			ImportAssetOptions options = ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive;
			foreach (Object asset in AssetDatabase.LoadAllAssetRepresentationsAtPath(folderPath))
			{
				string path = AssetDatabase.GetAssetPath(asset);
				Debug.Log(path);
				AssetDatabase.ImportAsset(path, options);
			}
		}
*/
	}
}