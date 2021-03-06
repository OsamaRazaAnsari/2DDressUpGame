//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Utage
{

	/// <summary>
	/// グラフィックオブジェクトのデータ
	/// </summary>
	public abstract class AdvGraphicObject : MonoBehaviour
	{
		public AdvGraphicLayer Layer { get { return layer; } }
		AdvGraphicLayer layer;

		public AdvEngine Engine { get { return Layer.Manager.Engine; } }
		public GraphicInfoList Graphic { get; protected set; }

		public bool IsLoading { get; protected set; }

		//エフェクトによるカラー値
		public EffectColors EffectColors { get { return effectColors; } }
		EffectColors effectColors = new EffectColors();

		public float PixelsToUnits { get { return Layer.Manager.PixelsToUnits; } }

		public bool IsDefault { get { return (Layer.DefaultObject == this); }  }

		internal virtual void Init(AdvGraphicLayer layer)
		{
			this.layer = layer;
			EffectColors.OnValueChanged = OnEffectColorsChange;
			IsLoading = true;
			OnInit();
		}

		//現在のメインのオブジェクト（フェードなどのエフェクトなどの抜きにしたもの）
		public abstract GameObject CurrentObject { get; }

		internal virtual void OnInit() { }

		//グラフィック情報の設定
		internal abstract void OnDraw(GraphicInfoList graphic, float fadeTime);
	
		//描画時の引数を適用
		internal virtual void OnDrawArg(AdvGraphicOperaitonArg arg)
		{
			if(arg.IsPostionArgs)
			{
				Vector3 pos = transform.localPosition;
				if (arg.X != null) pos.x = (float)arg.X / PixelsToUnits;
				if (arg.Y != null) pos.y = (float)arg.Y / PixelsToUnits;
				transform.localPosition = pos;
			}
			OnDrawArgCustom(arg);
		}

		//描画時の引数を適用をさらにカスタム
		internal virtual void OnDrawArgCustom(AdvGraphicOperaitonArg arg)
		{
		}

		//クリア処理
		internal virtual void OnClear() { }
		//フェードアウト処理
		internal abstract void OnFadeOut(float fadeTime);

		//エフェクト用の色が変化したとき
		internal virtual void OnEffectColorsChange(EffectColors colors)
		{
			if (CurrentObject)
			{
				foreach (Renderer renderer in CurrentObject.GetComponentsInChildren<Renderer>())
				{
					renderer.material.color = colors.MulColor;
				}
			}
		}

		internal virtual void Draw(AdvGraphicOperaitonArg arg)
		{
			StartCoroutine( CoLoadGraphic(arg.Graphic, ()=>OnLoadGraphicComplete(arg) ));
		}

		protected IEnumerator CoLoadGraphic(GraphicInfoList graphic, Action OnComplete )
		{
			IsLoading = true;
			//直前のファイルがあればそれを削除
			foreach( var item in this.GetComponents<AssetFileReference>() )
			{
				Destroy(item);
			}

			foreach (var item in graphic.InfoList)
			{
				AssetFile file = AssetFileManager.Load(item.File, this);
				file.AddReferenceComponet(this.gameObject);
				file.Unuse(this);
			}

			while (!graphic.IsLoadEnd) yield return 0;

			OnComplete();
			IsLoading = false;
		}

		void OnLoadGraphicComplete(AdvGraphicOperaitonArg arg)
		{
			float time = GetCurrentFadeTime(arg.FadeTime);
			if(!arg.IsNoPtattern || Graphic ==null)
			{
				OnDraw(arg.Graphic, time);
				Graphic = arg.Graphic;
			}
			OnDrawArg(arg);
		}

		internal virtual void Clear()
		{
			OnClear();
			GameObject.Destroy(gameObject);
		}

		internal virtual void FadeOut(float fadeTime)
		{
			float time = GetCurrentFadeTime(fadeTime);
			OnFadeOut(time);
			GameObject.Destroy(gameObject, time);
		}

		internal virtual float GetCurrentFadeTime( float fadeTime )
		{
			return Engine.Page.CheckSkip() ? fadeTime / Engine.Config.SkipSpped : fadeTime;
		}

		protected List<Component> eventColliders = new List<Component>();
		protected List<EventTrigger> eventTriggers = new List<EventTrigger>();

		/// <summary>
		/// クリックイベントを設定
		/// </summary>
		internal virtual void AddClickEvent(bool isPolygon, StringGridRow row, UnityAction<BaseEventData> action)
		{
			GameObject go = CurrentObject;
			List<Component> colliders = AddEventColliders(go, isPolygon);

			//イベントトリガーの追加
			foreach (var item in colliders )
			{
				EventTrigger eventTrigger = item.gameObject.AddComponent<EventTrigger>();
				UtageToolKit.AddEventTriggerEntry(eventTrigger, (eventData) => OnClick(eventData,row,action), EventTriggerType.PointerClick);
				eventTriggers.Add(eventTrigger);
			}
		}

		/// <summary>
		/// クリック時の処理
		/// 通常は登録されたアクションを呼ぶだけ
		/// 必要に応じてoverrideして処理を書き換える
		/// </summary>
		internal virtual void OnClick(BaseEventData eventData, StringGridRow row, UnityAction<BaseEventData> action)
		{
			action.Invoke(eventData);
		}

		/// <summary>
		/// クリックイベントを削除
		/// </summary>
		internal virtual void RemoveClickEvent()
		{
			eventColliders.ForEach(x => UnityEngine.Object.Destroy(x));
			eventColliders.Clear();
			eventTriggers.ForEach(x => UnityEngine.Object.Destroy(x));
			eventTriggers.Clear();
		}
		
		/// <summary>
		/// コライダーを設定
		/// </summary>
		internal virtual List<Component> AddEventColliders(GameObject go, bool isPolygon)
		{
			//コライダーが既に設定済みなら、なにもしない
			Collider[] colliders = go.GetComponentsInChildren<Collider>();
			Collider2D[] colliders2D = go.GetComponentsInChildren<Collider2D>();
			if (colliders.Length > 0 || colliders2D.Length > 0)
			{
				List<Component> components = new List<Component>();
				components.AddRange(colliders);
				components.AddRange(colliders2D);
				return components;
			}

			return AddNewEventColliders(go, isPolygon);
		}

		/// <summary>
		/// コライダーを新しく設定
		/// </summary>
		internal virtual List<Component> AddNewEventColliders(GameObject go, bool isPolygon)
		{
			//レンダラーにコライダーをつける
			bool is2D = false;
			Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in renderers)
			{
				SpriteRenderer sprite = renderer as SpriteRenderer;
				if (sprite)
				{
					is2D = true;
					break;
				}
			}

			if (isPolygon&&is2D)
			{
				foreach (Renderer renderer in renderers)
				{
					SpriteRenderer sprite = renderer as SpriteRenderer;
					if (sprite) AddEventCollider<PolygonCollider2D>(renderer.gameObject);
				}
			}
			else
			{
				foreach (Renderer renderer in renderers)
				{
					AddEventCollider<BoxCollider>(renderer.gameObject);
				}
			}

			return eventColliders;
		}
		
		/// <summary>
		/// コライダーを設定
		/// </summary>
		internal virtual T AddEventCollider<T>(GameObject target) where T : Component
		{
			T collider = target.AddComponent<T>();
			if (!collider) Debug.LogError("Failed Add Collider to " + target.name);
			eventColliders.Add(collider);
			return collider;
		}

		protected const int Version = 0;
		internal virtual void Write(BinaryWriter writer)
		{
			writer.Write(Version);
			UtageToolKit.WriteLocalTransform(this.transform, writer);
			this.EffectColors.Write(writer);

			//無限ループのTweenがある場合は、Tween情報を書き込む
			AdvTweenPlayer[] tweenArray = this.gameObject.GetComponents<AdvTweenPlayer>() as AdvTweenPlayer[];
			int tweenCount = 0;
			foreach (var tween in tweenArray)
			{
				if (tween.IsEndlessLoop) ++tweenCount;
			}
			writer.Write(tweenCount);
			foreach (var tween in tweenArray)
			{
				if (tween.IsEndlessLoop) tween.Write(writer);
			}
			OnWrite(writer);
		}

		internal virtual void Read(GraphicInfoList graphic, BinaryReader reader)
		{
			int version = reader.ReadInt32();
			if (version == Version)
			{
				UtageToolKit.ReadLocalTransform(this.transform, reader);
				this.EffectColors.Read(reader);

				//Tweenがある場合は、Tween情報を読み込む
				int tweenCount = reader.ReadInt32();
				for (int i = 0; i < tweenCount; ++i)
				{
					AdvTweenPlayer tween = this.gameObject.AddComponent<AdvTweenPlayer>() as AdvTweenPlayer;
					tween.Read(reader, PixelsToUnits);
				}
				OnRead(graphic, reader);
			}
		}


		protected const int BaseVersion = 0;
		/// セーブデータ書き込み処理
		internal virtual void OnWrite(BinaryWriter writer)
		{
			writer.Write(BaseVersion);
		}

		/// セーブデータの読みこみ
		internal virtual void OnRead(GraphicInfoList graphic, BinaryReader reader)
		{
			int version = reader.ReadInt32();
			if (version >= BaseVersion)
			{
				StartCoroutine(CoLoadGraphic(graphic, () => OnReadComplete(graphic)));
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, version));
			}
		}

		internal virtual void OnReadComplete(GraphicInfoList graphic)
		{
			OnDraw(graphic, 0);
			Graphic = graphic;
			EffectColors.OnValueChanged.Invoke(EffectColors);
		}
	}
}
