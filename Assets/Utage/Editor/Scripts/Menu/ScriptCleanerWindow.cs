//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Utage
{

	public class ScriptCleanerWindow : EditorWindow
	{
		/// <summary>
		/// セーブデータフォルダを開く
		/// </summary>
		[MenuItem(MeuToolOpen.MeuToolRoot + "DeveloperTool/Script Cleaner", priority = 50)]
		static void Open()
		{
			EditorWindow.GetWindow(typeof(ScriptCleanerWindow), false, "Script Cleaner");
		}

		void OnFocus()
		{
			rootPath = EditorPrefsUtil.LoadString(rootPathKey, "");
			if (!Directory.Exists(this.rootPath))
			{
				this.rootPath = "";
				Save();
			}
			FindAllPathes();
		}
		void Save()
		{
			EditorPrefsUtil.SaveString(rootPathKey, rootPath);
		}

		[SerializeField]
		string rootPath = "";
		string rootPathKey = UtageEditorPrefs.Key.ScriptCleanerRoot.ToString() + Application.dataPath;

		class ScriptInfo
		{
			public string path;
			public bool isEnable = true;

			public ScriptInfo(string path)
			{
				this.path = path;
			}

			public bool IsEditFile()
			{
				if (!File.Exists(path)) return false;
				return isEnable;
			}
		};

		List<ScriptInfo> infoList = new List<ScriptInfo>();
		Vector2 scrollPosition;
		void OnGUI()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( string.IsNullOrEmpty(this.rootPath) ? "Empty" : this.rootPath );
			if (GUILayout.Button("Select", GUILayout.Width(100)))
			{
				string convertPath = this.rootPath;
				string dir = string.IsNullOrEmpty(convertPath) ? "" : Path.GetDirectoryName(convertPath);
				string name = string.IsNullOrEmpty(convertPath) ? "" : Path.GetFileName(convertPath);
				string path = EditorUtility.SaveFolderPanel("Select folder", dir, name);
				if (!string.IsNullOrEmpty(path))
				{
					this.rootPath = path;
					Save();
					FindAllPathes();
				}
			}
			EditorGUILayout.EndHorizontal();

			bool isEnable = !string.IsNullOrEmpty(this.rootPath);
			if (isEnable)
			{
				if (!Directory.Exists(this.rootPath))
				{
					isEnable = false;
					this.rootPath = "";
					Save();
				}
				else
				{

					scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

					foreach (ScriptInfo info in infoList)
					{
						GUILayout.Toggle(true, info.path.Replace(this.rootPath, ".."));
					}
					EditorGUILayout.EndScrollView();
				}
			}
			EditorGUI.BeginDisabledGroup(!isEnable);
			bool isEdit = GUILayout.Button("Go!", GUILayout.Width(80f));
			EditorGUI.EndDisabledGroup();
			if (isEdit) EditAllScript();
		}

		void EditAllScript()
		{
			foreach (ScriptInfo info in infoList)
			{
				EditScript(info);
			}
			AssetDatabase.Refresh();
		}

		void FindAllPathes()
		{
			infoList.Clear();

			if (string.IsNullOrEmpty(this.rootPath)) return;

			string[] pathArray = Directory.GetFiles(this.rootPath, "*.cs", SearchOption.AllDirectories);
			foreach (string path in pathArray)
			{
				infoList.Add(new ScriptInfo(path));
			}
		}

		bool EditScript(ScriptInfo info)
		{
			if (!info.IsEditFile()) return false;

			try
			{
				string text = File.ReadAllText(info.path);
				text = text.Replace("\r\n", "\n");
				File.WriteAllText(info.path, text, System.Text.Encoding.Unicode);
				return true;
			}
			catch(System.Exception e )
			{
				Debug.LogError(e.Message);
				return false;
			}
		}
	}
}