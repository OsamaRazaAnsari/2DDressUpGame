using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// デバッグ用に、マウスダウン、アウップでUnityエディタをポーズさせる
	/// </summary>
	[AddComponentMenu("Utage/Lib/System UI/DebugPauseEditor")]
	public class DebugPauseEditor : MonoBehaviour
	{
		public bool isPuaseOnMouseDown = false;
		public bool isPuaseOnMouseUp = false;
		 [Range(0.00001f, 10)] 
		public float timeScale = 1.0f;

#if UNITY_EDITOR
		void Start()
		{
			timeScale = Time.timeScale;
		}

		void Update()
		{
			if ( (isPuaseOnMouseDown && Input.GetButtonDown("Fire1") )
				||(isPuaseOnMouseUp && Input.GetButtonUp("Fire1") ) )
			{
				PauseEditor();
			}
		}

		public void PauseEditor()
		{
			UnityEditor.EditorApplication.isPaused = true;
		}

		void OnValidate()
		{
			if ( !Mathf.Approximately( Time.timeScale,timeScale) ) Time.timeScale = timeScale;
		}
#endif
	}
}