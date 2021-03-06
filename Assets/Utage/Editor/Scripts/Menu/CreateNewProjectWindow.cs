//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Utage
{

	//「Utage」のシナリオデータ用のエクセルファイルの管理エディタウイドウ
	public class CreateNewProjectWindow : EditorWindow
	{
		enum Type
		{
			CreateNewAdvScene,			//ADV用新規シーンを作成
			AddToCurrentScene,			//現在のシーンに追加
			CreateScenarioAssetOnly,	//シナリオ用プロジェクトファイルのみ作成
		};
		Type createType;
		string newProjectName = "";
		string newProjectDir = "";
		const string TemplateName = "Template";
		const string TemplateAssetsDir = "Assets/Utage/" + TemplateName;

		int gameScreenWidth = 800;
		int gameScreenHeight = 600;

		void OnGUI()
		{
			UtageEditorToolKit.BeginGroup("Create New Project");
			GUIStyle style = new GUIStyle();
			GUILayout.Space(4f);
			GUILayout.Label("<b>" + "Input New Project Name" + "</b>", style, GUILayout.Width(80f));
			newProjectName = GUILayout.TextField(newProjectName);

			GUILayout.Space(4f);
			GUILayout.Label("<b>" + "Select Create Type" + "</b>", style, GUILayout.Width(80f));
			Type type = (Type)EditorGUILayout.EnumPopup("Type", createType);
			if (createType != type)
			{
				createType = type;
			}
			UtageEditorToolKit.EndGroup();

			bool isGameSizeEnable = (createType != Type.CreateScenarioAssetOnly);
			EditorGUI.BeginDisabledGroup(!isGameSizeEnable);
			GUILayout.Space(4f);
			UtageEditorToolKit.BeginGroup("Game Screen Size");
			int width = EditorGUILayout.IntField("Width", gameScreenWidth);
			if (gameScreenWidth != width && width > 0)
			{
				gameScreenWidth = width;
			}
			int height = EditorGUILayout.IntField("Hegiht", gameScreenHeight);
			if (gameScreenHeight != height && height > 0)
			{
				gameScreenHeight = height;
			}
			UtageEditorToolKit.EndGroup();
			EditorGUI.EndDisabledGroup();

			bool isProjectNameEnable = IsEnableProjcetName(newProjectName);
			EditorGUI.BeginDisabledGroup(!isProjectNameEnable);
			bool isCreate = GUILayout.Button("Create", GUILayout.Width(80f));
			EditorGUI.EndDisabledGroup();
			if(isCreate) Create();
		}

		//新たなプロジェクトを作成
		void Create()
		{
			switch (createType)
			{
				case Type.CreateNewAdvScene:
					if (!EditorApplication.SaveCurrentSceneIfUserWantsTo()) return;
					break;
				default:
					break;
			}

			newProjectDir = ToProjectDir(newProjectName);

			Profiler.BeginSample("CopyTemplate");
			//テンプレートをコピー
			CopyTemplate();
			Profiler.EndSample();

			//プロジェクトファイルを作成
			string path = FileUtil.GetProjectRelativePath(newProjectDir +  newProjectName + ".project.asset");
			AdvScenarioDataProject ProjectData = UtageEditorToolKit.CreateNewUniqueAsset<AdvScenarioDataProject>(path);

			//プロジェクトにエクセルファイルを設定
			ProjectData.AddExcelAsset( UtageEditorToolKit.LoadAssetAtPath<Object>(GetExcelRelativePath() ));
			//プロジェクトにカスタムインポートフォルダを設定
			ProjectData.AddCustomImportAudioFloders(LoadAudioFloders());
			ProjectData.AddCustomImportSpriteFloders(LoadSpriteFloders());
			ProjectData.AddCustomImportMovieFloders(LoadMovieFloders());
			//プロジェクトファイルを設定してインポート
			AdvScenarioDataBuilderWindow.ProjectData = ProjectData;
			AdvScenarioDataBuilderWindow.Import();

			Profiler.BeginSample("SceneEdting");
			switch (createType)
			{
				case Type.CreateNewAdvScene:
					//ADV用新規シーンを作成
					CreateNewAdvScene();
					break;
				case Type.AddToCurrentScene:
					//テンプレートシーンをコピー
					AddToCurrentScene();
					break;
				case Type.CreateScenarioAssetOnly:
					AssetDatabase.DeleteAsset(GetSceneRelativePath());
					break;
			}
			Profiler.EndSample();
		}

		//オーディオフォルダを取得
		List<Object> LoadAudioFloders()
		{
			List<Object> assetList = new List<Object>();

			string rootDir = GetProjectRelativeDir() + "/Resources/" + newProjectName + "/Sound/";
			assetList.Add(UtageEditorToolKit.LoadAssetAtPath<Object>(rootDir + "Ambience"));
			assetList.Add(UtageEditorToolKit.LoadAssetAtPath<Object>(rootDir + "BGM"));
			assetList.Add(UtageEditorToolKit.LoadAssetAtPath<Object>(rootDir + "SE"));
			assetList.Add(UtageEditorToolKit.LoadAssetAtPath<Object>(rootDir + "Voice"));

			return assetList;
		}

		//スプライトフォルダを取得
		List<Object> LoadSpriteFloders()
		{
			List<Object> assetList = new List<Object>();

			string rootDir = GetProjectRelativeDir() + "/Resources/" + newProjectName + "/Texture/";
			assetList.Add(UtageEditorToolKit.LoadAssetAtPath<Object>(rootDir + "BG"));
			assetList.Add(UtageEditorToolKit.LoadAssetAtPath<Object>(rootDir + "Character"));
			assetList.Add(UtageEditorToolKit.LoadAssetAtPath<Object>(rootDir + "Event"));
			assetList.Add(UtageEditorToolKit.LoadAssetAtPath<Object>(rootDir + "Sprite"));
			assetList.Add(UtageEditorToolKit.LoadAssetAtPath<Object>(rootDir + "Thumbnail"));

			return assetList;
		}

		//ムービーフォルダを取得
		List<Object> LoadMovieFloders()
		{
			List<Object> assetList = new List<Object>();

			//			assetList.Add(UtageEditorToolKit.LoadAssetAtPath<Object>(GetProjectRelativeDir() + "/Resources/" + newProjectName + "/Movie"));
			return assetList;
		}

		//ADV用新規シーンを作成
		void CreateNewAdvScene()
		{
			//シーンを開く
			string scenePath = GetSceneRelativePath();
			EditorApplication.OpenScene(scenePath);

			//「宴」エンジンの初期化
			InitUtageEngine();

			//シーンをセーブ
			EditorApplication.SaveScene();
			Selection.activeObject = AssetDatabase.LoadAssetAtPath(scenePath, typeof(Object));
		}

		void AddToCurrentScene()
		{
			//シーンを開く
			string scenePath = GetSceneRelativePath();
			EditorApplication.OpenSceneAdditive(scenePath);

			//余分なオブジェクトを削除
			UtageUguiTitle title = UtageEditorToolKit.FindComponentAllInTheScene<UtageUguiTitle>();
			GameObject.DestroyImmediate(title.transform.root.gameObject);
			SystemUi systemUi = UtageEditorToolKit.FindComponentAllInTheScene<SystemUi>();
			GameObject.DestroyImmediate(systemUi.gameObject);

			//シーンのアセットを削除
			AssetDatabase.DeleteAsset(scenePath);

			//「宴」エンジンの初期化
			InitUtageEngine();

			//エンジン休止状態に
			AdvEngine engine = GameObject.FindObjectOfType<AdvEngine>();
			engine.gameObject.SetActive(false);

			Selection.activeObject = AssetDatabase.LoadAssetAtPath(scenePath, typeof(Object));
		}

		//シーン内のAdvエンジンの初期設定
		void InitUtageEngine()
		{
			//シナリオデータの設定
			AdvEngine engine = GameObject.FindObjectOfType<AdvEngine>();
			AdvEngineStarter starter = GameObject.FindObjectOfType<AdvEngineStarter>();

			AdvSettingDataManager settingDataAsset = UtageEditorToolKit.LoadAssetAtPath<AdvSettingDataManager>(GetSettingAssetRelativePath());
			AdvScenarioDataExported exportedScenarioAsset = UtageEditorToolKit.LoadAssetAtPath<AdvScenarioDataExported>(GetScenarioAssetRelativePath());
			AdvScenarioDataExported[] exportedScenarioDataTbl = { exportedScenarioAsset };
			starter.InitOnCreate( engine, settingDataAsset, exportedScenarioDataTbl, newProjectName);

			UguiLetterBoxCamera[] cameras = GameObject.FindObjectsOfType<UguiLetterBoxCamera>();
			foreach (UguiLetterBoxCamera camera in cameras)
			{
				camera.Width = camera.MaxWidth = gameScreenWidth;
				camera.Height = camera.MaxHeight = gameScreenHeight;
			}

			//セーブファイルの場所の設定
			AdvSaveManager saveManager = GameObject.FindObjectOfType<AdvSaveManager>();
			saveManager.DirectoryName = "Save" + newProjectName;

			AdvSystemSaveData systemSaveData = GameObject.FindObjectOfType<AdvSystemSaveData>();
			systemSaveData.DirectoryName = "Save" + newProjectName;

			//シーン内の全てのテンプレートアセットをクローンアセットに置き換える
			ReplaceAssetsFromTempleateToCloneInSecne();
		}


		bool IsEnableProjcetName(string name)
		{
			if( string.IsNullOrEmpty(name) ) return false;
			if (System.IO.Directory.Exists(ToProjectDir(name))) return false;
			return true;
		}
		string ToProjectDir(string name)
		{
			return Application.dataPath + "/" + name + "/";
		}

		string GetProjectRelativeDir()
		{
			return "Assets/" + newProjectName;
		}
		string GetProjectRelativePath()
		{
			return GetProjectRelativeDir() + "/" + newProjectName;
		}
		string GetExcelRelativePath()
		{
			return GetProjectRelativePath() + ".xls";
		}
		string GetSceneRelativePath()
		{
			return GetProjectRelativePath() + ".unity";
		}
		string GetSettingAssetRelativePath()
		{
			return GetProjectRelativePath() + AdvExcelImporter.SettingAssetExt;
		}
		string GetScenarioAssetRelativePath()
		{
			return GetProjectRelativePath() + AdvExcelImporter.ScenarioAssetExt;
		}


		void CopyTemplate()
		{
//			FileUtil.CopyFileOrDirectory(TemplateAssetsDir, GetProjectRelativeDir() );
			AssetDatabase.CopyAsset(TemplateAssetsDir, GetProjectRelativeDir());
			//リフレッシュ必須
			AssetDatabase.Refresh();
			//Templateというファイル名をリネーム
			foreach (string filePath in System.IO.Directory.GetFiles(newProjectDir, "*", SearchOption.AllDirectories))
			{
				if (Path.GetFileNameWithoutExtension(filePath) == TemplateName && Path.GetExtension(filePath) != ".meta")
				{
					string src = FileUtil.GetProjectRelativePath(filePath).Replace("\\", "/");
					string error = AssetDatabase.RenameAsset(src, newProjectName);
					if (!string.IsNullOrEmpty(error))
					{
						Debug.LogError(src + " " + error);
					}
				}
			}

			//Templateというフォルダ名をリネーム
			foreach (string dirPath in System.IO.Directory.GetDirectories(newProjectDir, "*", SearchOption.AllDirectories))
			{
				if (Path.GetFileName(dirPath) == TemplateName)
				{
					string src = FileUtil.GetProjectRelativePath(dirPath).Replace("\\", "/");
					string error = AssetDatabase.RenameAsset(src, newProjectName);
					if (!string.IsNullOrEmpty(error))
					{
						Debug.LogError(src + " " + error);
					}
				}
			}
			//アセットの再設定
			RebuildAssets();
		}
		//アセットの再設定
		void RebuildAssets()
		{
			//いったんアセットをリフレッシュ
			AssetDatabase.Refresh();
			//アセットの編集開始
			AssetDatabase.StartAssetEditing();

			Debug.Log("RebuildAssets･･･");
			RebuildAssetsSub();
			Debug.Log("...End RebuildAssets");

			//アセットの編集終了
			AssetDatabase.StopAssetEditing();
			//アセットのセーブ
			AssetDatabase.SaveAssets();
			//いったんアセットをリフレッシュ
			AssetDatabase.Refresh();
		}

		//テンプレートからコピーしたアセットの
		Dictionary<Object, Object> cloneAssetPair = new Dictionary<Object, Object>();		
		//アセットの再設定
		void RebuildAssetsSub()
		{
			cloneAssetPair = new Dictionary<Object, Object>();
			//クローンしたアセットにパッキングタグなどの必要な処置をして
			//テンプレートのアセットとのペアでリスト化する
			List<string> pathList = UtageEditorToolKit.GetAllAssetPathList(newProjectDir);
			foreach (string assetpath in pathList)
			{
				if (Path.GetExtension(assetpath) == ".unity") continue;

				//テンプレート（クローン元）のアセット
				string templatePath = assetpath.Replace(GetProjectRelativeDir() + "/", TemplateAssetsDir + "/");
				//クローンしたアセット
				foreach (Object clone in AssetDatabase.LoadAllAssetsAtPath(assetpath))
				{
					if (PrefabUtility.GetPrefabType(clone) == PrefabType.Prefab)
					{
						//プレハブの場合
						Object prefab = PrefabUtility.FindRootGameObjectWithSameParentPrefab(clone as GameObject) as Object;
						Object template = AssetDatabase.LoadAssetAtPath(templatePath, prefab.GetType());
						if (template != null)
						{
							if (cloneAssetPair.ContainsKey(template))
							{
								Debug.LogError(templatePath + " is already contains");
							}
							else
							{
								cloneAssetPair.Add(template, prefab);
							}
						}
						else
						{
							Debug.LogError(templatePath + " not found");
						}
						break;
					}
					else
					{
						Sprite sprite = clone as Sprite;
						if (sprite != null)
						{
							//スプライトの場合のみ
							//インポーターのパッキングタグを変える
							TextureImporter importer = AssetImporter.GetAtPath(assetpath) as TextureImporter;
							if (importer != null)
							{
								importer.spritePackingTag = newProjectName + "_UI";
								AssetDatabase.ImportAsset(assetpath);
								EditorUtility.SetDirty(importer);
							}
						}
						Object template = AssetDatabase.LoadAssetAtPath(templatePath, clone.GetType());
						if (template != null)
						{
							if (cloneAssetPair.ContainsKey(template))
							{
								Debug.LogError(templatePath + " is already contains");
							}
							else
							{
								cloneAssetPair.Add(template, clone);
							}
						}
					}
				}
			}

			//クローンしたプレハブやScriptableObject内にテンプレートアセットへのリンクがあったら、クローンアセットのものに変える
			foreach( Object obj in cloneAssetPair.Values )
			{
				bool isReplaced = false;
				if (PrefabUtility.GetPrefabType(obj) == PrefabType.Prefab)
				{
					//プレハブの場合
					GameObject go = obj as GameObject;
					foreach (Component component in go.GetComponentsInChildren<Component>(true))
					{
						isReplaced |= UtageEditorToolKit.ReplaceSerializedProperties(new SerializedObject(component), cloneAssetPair);
					}
				}
				else if (UtageEditorToolKit.IsScriptableObject(obj))
				{
					//ScriptableObjectの場合
					isReplaced |= UtageEditorToolKit.ReplaceSerializedProperties(new SerializedObject(obj), cloneAssetPair);
				}

				if (isReplaced)
				{
					EditorUtility.SetDirty(obj);
				}
			}
		}
		
		//シーン内の全てのテンプレートアセットをクローンアセットに置き換える
		void ReplaceAssetsFromTempleateToCloneInSecne()
		{
			Debug.Log(System.DateTime.Now + " プレハブインスタンスを検索");
			//プレハブインスタンスを検索
			List<GameObject> prefabInstanceList = new List<GameObject>();
			foreach (GameObject go in UtageEditorToolKit.GetAllObjectsInScene())
			{
				if (PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance)
				{
					GameObject prefabInstance = PrefabUtility.FindRootGameObjectWithSameParentPrefab(go);
					if(!prefabInstanceList.Contains(prefabInstance))
					{
						prefabInstanceList.Add(prefabInstance);
					}
				}
			}
			Debug.Log(System.DateTime.Now + " prefabInstanceList");
			//プレハブインスタンスはいったん削除して、クローンプレハブからインスタンスを作って置き換える
			foreach (GameObject go in prefabInstanceList)
			{
				GameObject prefabAsset = PrefabUtility.GetPrefabParent(go) as GameObject;
				Object clonePrefabAsset;
				if (cloneAssetPair.TryGetValue(prefabAsset, out clonePrefabAsset))
				{
					GameObject cloneInstance = PrefabUtility.InstantiatePrefab(clonePrefabAsset) as GameObject;
					cloneInstance.transform.SetParent(go.transform.parent);
					GameObject.DestroyImmediate(go);
				}
				else
				{
					Debug.LogError( go.name + " Not Find Clone Prefab.");
				}
			}

			Debug.Log(System.DateTime.Now + "ReplaceSerializedProperties");
			//オブジェクト内のコンポーネントからのリンクを全て、テンプレートからクローンに置き換える
			UtageEditorToolKit.ReplaceSerializedPropertiesAllComponentsInSene(cloneAssetPair);
			Debug.Log(System.DateTime.Now);
		}

		/*
		//テクスチャ・スプライトの再設定
		void RebuildTextureAndSpriteAssets()
		{
			RebuildSpriteAssets(newProjectDir + "Textures/");
			RebuildSpriteAssets(newProjectDir + "Textures/SpriteUI");
			RebuildTextureAssets(newProjectDir + "Textures/TextureUI");
		}

		//テクスチャ・スプライトの再設定
		void RebuildSpriteAssets(string root)
		{
			List<string> pathList = GetAllAssetPathList(root);
			foreach (string assetpath in pathList)
			{
				TextureImporter importer = AssetImporter.GetAtPath(assetpath) as TextureImporter;
				if (importer != null)
				{
					importer.spritePackingTag = newProjectName + "_UI";
					AssetDatabase.ImportAsset(assetpath);
				}
				Sprite sprite = AssetDatabase.LoadAssetAtPath(assetpath, typeof(Sprite)) as Sprite;
				if (sprite != null)
				{
					Sprite originalSprite = LoadTemelateAssetFromNewProjectAssetPath<Sprite>(assetpath);
					cloneAssetPair.Add(originalSprite, sprite);
					EditorUtility.SetDirty(sprite);
				}
			}
		}

		//テクスチャ・スプライトの再設定
		void RebuildTextureAssets(string root)
		{
			List<string> pathList = GetAllAssetPathList(root);
			foreach (string assetpath in pathList)
			{
				Texture texture = AssetDatabase.LoadAssetAtPath(assetpath, typeof(Texture)) as Texture;
				if (texture != null)
				{
					Texture originalTexture = LoadTemelateAssetFromNewProjectAssetPath<Texture>(assetpath);
					cloneAssetPair.Add(originalTexture, texture);
					EditorUtility.SetDirty(texture);
				}
			}
		}


		//オーディの再設定
		void RebuildAudioClipAssets()
		{
			List<string> pathList = GetAllAssetPathList(newProjectDir + "Audio");
			foreach (string assetpath in pathList)
			{
				AudioClip audio = AssetDatabase.LoadAssetAtPath(assetpath, typeof(AudioClip)) as AudioClip;
				if (audio != null)
				{
					AudioClip originalAudio = LoadTemelateAssetFromNewProjectAssetPath<AudioClip>(assetpath);
					cloneAssetPair.Add(originalAudio, audio);
				}
			}
		}

		//ScriptableObjectの再設定
		void RebuildScriptableObject()
		{
			List<string> pathList = GetAllAssetPathList(newProjectDir + "ScriptableObject");
			foreach (string assetpath in pathList)
			{
				ScriptableObject obj = AssetDatabase.LoadAssetAtPath(assetpath, typeof(ScriptableObject)) as ScriptableObject;
				if (obj != null)
				{
					ScriptableObject original = LoadTemelateAssetFromNewProjectAssetPath<ScriptableObject>(assetpath);
					cloneAssetPair.Add(original, obj);
				}
			}
			foreach (ScriptableObject obj in cloneAssetPair.Values)
			{
				CustomProjectSetting customProjectSetting = obj as CustomProjectSetting;
				if (customProjectSetting!=null)
				{
					SwapCopiedScriptabelObject(customProjectSetting);
					EditorUtility.SetDirty(customProjectSetting);
				}
			}
		}


		//プレハブの再設定
		void RebuildPrefabs()
		{
			List<string> pathList = GetAllAssetPathList(newProjectDir + "Prefabs");
			foreach (string assetpath in pathList)
			{
				GameObject prefab = AssetDatabase.LoadAssetAtPath(assetpath,typeof(GameObject)) as GameObject;
				if (prefab!=null)
				{
					if (PrefabUtility.GetPrefabType(prefab) == PrefabType.Prefab)
					{
						//使用するスプライトを差し替えてプレハブを更新する
						SwapCopiedSprite(prefab.transform);
						SwapCopiedTexture(prefab.transform);
						SwapCopiedAudio(prefab.transform);
						EditorUtility.SetDirty(prefab);
						copiedPrefabAssetPair.Add(LoadTemelateAssetFromNewProjectAssetPath<GameObject>(assetpath), prefab);
					}
				}
			}
		}

		//シーン内の全てのテンプレートアセットをクローンアセットに置き換える
		void ReplaceAssetsFromTempleateToCloneInSecne()
		{
/*
			//プレハブをテンプレートからクローンアに置き換える
			foreach( GameObject go in UtageEditorToolKit.GetAllObjectsInScene() )
			{
				if (PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance)
				{
					GameObject prefabInstance = PrefabUtility.FindRootGameObjectWithSameParentPrefab(go);
					GameObject prefabAsset = PrefabUtility.GetPrefabParent(prefabInstance) as GameObject;
					Object copiedPrefabAsset;
					if (cloneAssetPair.TryGetValue(prefabAsset, out copiedPrefabAsset))
					{
						GameObject go = PrefabUtility.InstantiatePrefab(copiedPrefabAsset) as GameObject;
						go.transform.SetParent(prefabInstance.transform.parent);
						GameObject.DestroyImmediate(prefabInstance);
					}
					else
					{
						Debug.LogError( go.name + "Not Find Clone Prefab.");
					}
				}
			}

			//テンプレートからクローンに置き換える
			foreach (GameObject go in UtageEditorToolKit.GetAllObjectsInScene())
			{
				foreach( Component component in go.GetComponents<Component>() )
				{
					foreach (KeyValuePair<Object, Object> keyValue in cloneAssetPair)
					{
						UtageEditorToolKit.ReplaceSerializedProperties(component, keyValue.Key, keyValue.Value);
					}
				}
			}
		}

/*
		// UI等のテンプレートアセットの再設定
		void InitCloneAsset(bool isAddtiveScene)
		{
			List<Transform> rootList = FindRootObject(isAddtiveScene);
			foreach (Transform root in rootList)
			{
				SwapCopiedSprite(root);
				SwapCopiedSprie2D(root);
				SwapCopiedTexture(root);
				SwapCopiedAudio(root);
				SwapCopiedPrefab(root);
			}
			SwapCopiedScriptabelObject(GameObject.FindObjectOfType<BootCustomProjectSetting>());
			SwapCopiedEmojiData();
			SwapCopiedTextSettings();
		}

		// UI等のテンプレートアセットの再設定
		List<Transform> FindRootObject(bool isAddtiveScene)
		{
			List<Transform> rootList = new List<Transform>();
			AdvEngine engine = GameObject.FindObjectOfType<AdvEngine>();
			rootList.Add(engine.transform.root);
			BootCustomProjectSetting managers = GameObject.FindObjectOfType<BootCustomProjectSetting>();
			rootList.Add(managers.transform.root);
			if (!isAddtiveScene)
			{
				UtageUguiTitle title = UtageEditorToolKit.FindComponentAllInTheScene<UtageUguiTitle>();
				rootList.Add(title.transform.root);
				SystemUi systemUi = UtageEditorToolKit.FindComponentAllInTheScene<SystemUi>();
				rootList.Add(systemUi.transform.root);
			}
			return rootList;
		}
		
		//コピーしたスプライトに差し替える
		void SwapCopiedSprite( Transform root )
		{
			foreach (Image item in root.GetComponentsInChildren<Image>(true))
			{
				Sprite copiedSprite;
				if (copiedSpriteAssetPair.TryGetValue(item.sprite, out copiedSprite))
				{
					item.sprite = copiedSprite;
				}
			}
		}

		//コピーしたスプライトに差し替える
		void SwapCopiedSprie2D(Transform root)
		{
			foreach (Sprite2D item in root.GetComponentsInChildren<Sprite2D>(true))
			{
				Sprite copiedSprite;
				if (copiedSpriteAssetPair.TryGetValue(item.Sprite, out copiedSprite))
				{
					item.Sprite = copiedSprite;
				}
			}
		}

		//コピーしたテクスチャに差し替える
		void SwapCopiedTexture(Transform root)
		{
			foreach (RawImage item in root.GetComponentsInChildren<RawImage>(true))
			{
				Texture copiedTexture;
				if (copiedTextureAssetPair.TryGetValue(item.texture, out copiedTexture))
				{
					item.texture = copiedTexture;
				}
			}
		}


		//シーン内にアタッチされているプレハブをコピーしたプレハブに差し替える
		void SwapCopiedPrefab(Transform root)
		{
			SwapListViewCopiedPrefab(root);
			SwapGridPageCopiedPrefab(root);
			SwapCategoryGirdPageCopiedPrefab(root);
		}

		//リストビューのアイテムプレハブをコピーしたプレハブに差し替える
		void SwapListViewCopiedPrefab(Transform root)
		{
			foreach (UguiListView item in root.GetComponentsInChildren<UguiListView>(true))
			{
				GameObject copiedPrefab;
				if (copiedPrefabAssetPair.TryGetValue(item.ItemPrefab, out copiedPrefab))
				{
					item.ItemPrefab = copiedPrefab;
				}
				SwapChildPrefabInstance(item.Content);
			}
		}

		//グリッドページのアイテムプレハブをコピーしたプレハブに差し替える
		void SwapGridPageCopiedPrefab(Transform root)
		{
			foreach (UguiGridPage item in root.GetComponentsInChildren<UguiGridPage>(true))
			{
				GameObject copiedPrefab;
				if (copiedPrefabAssetPair.TryGetValue(item.itemPrefab, out copiedPrefab))
				{
					item.itemPrefab = copiedPrefab;
				}
				if (copiedPrefabAssetPair.TryGetValue(item.pageCarouselPrefab, out copiedPrefab))
				{
					item.pageCarouselPrefab = copiedPrefab;
				}
				SwapChildPrefabInstance(item.grid.transform);
				SwapChildPrefabInstance(item.pageCarouselAlignGroup.transform);
			}
		}

		//カテゴリグリッドページのアイテムプレハブをコピーしたプレハブに差し替える
		void SwapCategoryGirdPageCopiedPrefab(Transform root)
		{
			foreach (UguiCategoryGirdPage item in root.GetComponentsInChildren<UguiCategoryGirdPage>(true))
			{
				GameObject copiedPrefab;
				if (copiedPrefabAssetPair.TryGetValue(item.categoryPrefab, out copiedPrefab))
				{
					item.categoryPrefab = copiedPrefab;
				}
				SwapChildPrefabInstance(item.categoryAlignGroup.CachedRectTransform);
			}
		}
	
		//プレハブのリンクを書き換え
		void SwapChildPrefabInstance( Transform root )
		{
			List<GameObject> prefabInstanceList = new List<GameObject>();
			foreach (Transform child in root.GetComponentsInChildren<Transform>(true) )
			{
				if (PrefabUtility.GetPrefabType(child.gameObject) == PrefabType.PrefabInstance)
				{
					GameObject prefabInstance = PrefabUtility.FindRootGameObjectWithSameParentPrefab(child.gameObject);
					if( !prefabInstanceList.Contains(prefabInstance) )
					{
						prefabInstanceList.Add(child.gameObject);
					}
				}
			}
			foreach (GameObject prefabInstance in prefabInstanceList)
			{
				GameObject prefabAsset = PrefabUtility.GetPrefabParent(prefabInstance) as GameObject;
				GameObject copiedPrefabAsset;
				if (copiedPrefabAssetPair.TryGetValue(prefabAsset, out copiedPrefabAsset))
				{
					GameObject go = PrefabUtility.InstantiatePrefab(copiedPrefabAsset) as GameObject;
					go.transform.SetParent(prefabInstance.transform.parent);
					GameObject.DestroyImmediate(prefabInstance);
				}
			}
		}


		//SEのサウンドをコピーしたサウンドに差し替える
		void SwapCopiedAudio(Transform root)
		{
			foreach (UguiButtonSe item in root.GetComponentsInChildren<UguiButtonSe>(true))
			{
				AudioClip copiedAudio;
				if (copiedAudioClipAssetPair.TryGetValue(item.clicked, out copiedAudio))
				{
					item.clicked = copiedAudio;
				}
				if (copiedAudioClipAssetPair.TryGetValue(item.highlited, out copiedAudio))
				{
					item.highlited = copiedAudio;
				}
			}
		}

		//ScriptabelObject差し替え
		void SwapCopiedScriptabelObject(BootCustomProjectSetting bootSetting)
		{
			ScriptableObject copiedObj;
			if (copiedScriptableObjectAssetPair.TryGetValue(bootSetting.CustomProjectSetting, out copiedObj))
			{
				bootSetting.CustomProjectSetting = (CustomProjectSetting)copiedObj;
			}
		}
		void SwapCopiedScriptabelObject(CustomProjectSetting customProjectSetting)
		{
			ScriptableObject copiedObj;
			if (copiedScriptableObjectAssetPair.TryGetValue(customProjectSetting.Language, out copiedObj))
			{
				customProjectSetting.Language = (LanguageManager)copiedObj;
			}
			if (copiedScriptableObjectAssetPair.TryGetValue(customProjectSetting.Node2DSortData, out copiedObj))
			{
				customProjectSetting.Node2DSortData = (Node2DSortData)copiedObj;
			}
		}

		//絵文字データ差し替え
		void SwapCopiedEmojiData()
		{
			foreach(UguiNovelTextGenerator text in UtageEditorToolKit.FindComponentsAllInTheScene<UguiNovelTextGenerator>())
			{
				SwapCopiedEmojiData(text);
			}
		}
		void SwapCopiedEmojiData(UguiNovelTextGenerator novelText)
		{
			ScriptableObject copiedObj;
			if (copiedScriptableObjectAssetPair.TryGetValue(novelText.EmojiData, out copiedObj))
			{
				novelText.EmojiData = (UguiNovelTextEmojiData)copiedObj;
			}

			GameObject prefabInstance = PrefabUtility.FindPrefabRoot(novelText.gameObject);
			var prefab = PrefabUtility.GetPrefabParent(prefabInstance) as GameObject;
			if (prefab == null)
			{
				return;
			}
			PrefabUtility.ReplacePrefab(prefabInstance, prefab, ReplacePrefabOptions.ConnectToPrefab);
		}

		//テキスト設定データ差し替え
		void SwapCopiedTextSettings()
		{
			foreach (UguiNovelTextGenerator text in UtageEditorToolKit.FindComponentsAllInTheScene<UguiNovelTextGenerator>())
			{
				SwapCopiedTextSettings(text);
			}
		}
		void SwapCopiedTextSettings(UguiNovelTextGenerator novelText)
		{
			ScriptableObject copiedObj;
			if (copiedScriptableObjectAssetPair.TryGetValue(novelText.TextSettings, out copiedObj))
			{
				novelText.TextSettings = (UguiNovelTextSettings)copiedObj;
			}

			GameObject prefabInstance = PrefabUtility.FindPrefabRoot(novelText.gameObject);
			var prefab = PrefabUtility.GetPrefabParent(prefabInstance) as GameObject;
			if (prefab == null)
			{
				return;
			}
			PrefabUtility.ReplacePrefab(prefabInstance, prefab, ReplacePrefabOptions.ConnectToPrefab);
		}
*/	}
}
