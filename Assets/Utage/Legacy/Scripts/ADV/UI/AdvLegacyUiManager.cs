//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;

namespace Utage
{
	/// <summary>
	/// UI全般の管理
	/// </summary>
	[AddComponentMenu("Utage/Legacy/ADV/UiManager")]
	public class AdvLegacyUiManager : AdvUiManager
	{
		[SerializeField]
		AdvUiMessageWindow messageWindow;

		[SerializeField]
		AdvUiSelectionManager selection;

		[SerializeField]
		AdvUiBacklogManager backLog;

		/// <summary>
		/// 初期化。スクリプトから動的に生成する場合に
		/// </summary>
		/// <param name="engine">ADVエンジン</param>
		public void InitOnCreate(AdvEngine engine, AdvUiMessageWindow messageWindow, AdvUiSelectionManager selection, AdvUiBacklogManager backLog)
		{
			this.engine = engine;
			this.messageWindow = messageWindow;
			this.selection = selection;
			this.backLog = backLog;
		}

		public override void Open()
		{
			ChangeStatus(UiStatus.Default);
			this.gameObject.SetActive(true);
		}

		public override void Close()
		{
			this.gameObject.SetActive(false);
			if (messageWindow != null) messageWindow.Close();
			if (selection != null) selection.Close();
			if (backLog != null) backLog.Close();
		}

		protected override void ChangeStatus(UiStatus newStatus)
		{
			switch (newStatus)
			{
				case UiStatus.Backlog:
					if (backLog == null) return;

					if (messageWindow != null) messageWindow.Close();
					if (selection != null) selection.Close();
					if (backLog != null) backLog.Open();
					engine.Config.IsSkip = false;
					break;
				case UiStatus.HideMessageWindow:
					if (messageWindow != null) messageWindow.Close();
					if (selection != null) selection.Close();
					if(backLog != null) backLog.Close();
					engine.Config.IsSkip = false;
					break;
				case UiStatus.Default:
					if (messageWindow != null) messageWindow.Open();
					if (selection != null) selection.Open();
					if (backLog != null) backLog.Close();
					break;
			}
			this.status = newStatus;
		}

		//ウインドウ閉じるボタンが押された
		void OnTapCloseWindow()
		{
			Status = UiStatus.HideMessageWindow;
		}
	}
}