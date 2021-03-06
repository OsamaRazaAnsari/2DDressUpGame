//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// サウンド管理
	/// </summary>
	public class SoundManagerSystem : SoundManagerSysytemInterface
	{
		const string GameObjectNameSe = "One shot audio";

		List<string> saveStreamNameList = new List<string>();
		Dictionary<string, SoundStreamFade> streamTbl = new Dictionary<string, SoundStreamFade>();	//BGM等のストリーム
		List<AudioSource> curretFrameSeList = new List<AudioSource>();	//今のフレームで鳴らしたSEのリスト
		List<AudioSource> seList = new List<AudioSource>();				//現在管理中のSEリスト

		//サウンドマネージャー
		SoundManager SoundMangaer { get; set; }

		Transform CachedTransform { get; set; }

		public void Init(SoundManager soundMangaer, List<string> saveStreamNameList)
		{
			SoundMangaer = soundMangaer;
			CachedTransform = SoundMangaer.transform;
			this.saveStreamNameList = saveStreamNameList;
		}


		public void Play(string streamName, AssetFile file, float volume, bool isLoop, float fadeTime, bool isReplay, Func<float> callbackMasterVolume)
		{
			bool isStreaming = (file.LoadFlags & AssetFileLoadFlags.Streaming) == AssetFileLoadFlags.Streaming;
			SoundStream stream = PlaySub(streamName, file.Sound, volume, isLoop, isStreaming, fadeTime, isReplay, callbackMasterVolume);
			file.AddReferenceComponet(stream.gameObject);
		}

		public void Play(string streamName, AudioClip clip, float volume, bool isLoop, bool isStreaming, float fadeTime, bool isReplay, Func<float> callbackMasterVolume)
		{
			PlaySub (streamName, clip, volume, isLoop, isStreaming, fadeTime, isReplay, callbackMasterVolume);
		}

		SoundStream PlaySub(string streamName, AudioClip clip, float volume, bool isLoop, bool isStreaming, float fadeTime, bool isReplay, Func<float> callbackMasterVolume)
		{
			SoundStreamFade stream = GetStreamAndCreateIfMissing (streamName);
			if (isReplay || !stream.IsPlaying(clip))
			{
				return stream.Play(clip, fadeTime, volume, isLoop, isStreaming, callbackMasterVolume);
			}
			else
			{
				return stream.Current;
			}
		}

		public void Stop(string streamName, float fadeTime)
		{
			SoundStreamFade stream = GetStreamAndCreateIfMissing (streamName);
			stream.Stop (fadeTime);
		}

		public bool IsPlaying(string streamName)
		{
			SoundStreamFade stream = GetStreamAndCreateIfMissing (streamName);
			return stream.IsPlaying ();
		}

		public float GetCurrentSamplesVolume(string streamName)
		{
			SoundStreamFade stream = GetStreamAndCreateIfMissing(streamName);
			return stream.GetCurrentSamplesVolume();
		}

		public void StopAll(float fadeTime)
		{
			foreach (var stream in streamTbl.Values) 
			{
				stream.Stop(fadeTime);
			}
		}

		public void PlaySe(AssetFile file, float volume)
		{
			AudioSource audio = PlaySeSub(file.Sound, volume);
			if (audio)
			{
				file.AddReferenceComponet (audio.gameObject);
			}
		}

		public void PlaySe(AudioClip clip, float volume)
		{
			PlaySeSub(clip,volume);
		}

		// SE再生
		AudioSource PlaySeSub(AudioClip clip, float volume)
		{
			//音量0なので、鳴らさない
			if (volume <= 0) return null;
			//同一フレームで既に鳴っているので鳴らさない（多重再生防止）
			foreach (AudioSource audio in curretFrameSeList)
			{
				if (clip == audio.clip)
				{
					return null;
				}
			}

			AudioSource se = PlaySeClip(clip, volume);
			curretFrameSeList.Add(se);
			seList.Add(se);
			return se;
		}

		//オーディオクリップをSEとして再生
		AudioSource PlaySeClip(AudioClip clip, float volume)
		{
			GameObject go = UtageToolKit.AddChild(CachedTransform, new GameObject(GameObjectNameSe));
			AudioSource source = go.AddComponent<AudioSource>();
			source.clip = clip;
			source.volume = volume;
			source.Play();
			GameObject.Destroy(go, clip.length);
			return source;
		}

		//毎フレームのSeの更新
		void UpdateSe()
		{
			curretFrameSeList.Clear();
			curretFrameSeList.RemoveAll(x => (x == null));
		}

		public void LateUpdate()
		{
			UpdateSe();
		}

		public bool IsLoading
		{
			get
			{
				foreach( var stream in streamTbl.Values )
				{
					if (stream.IsLoading)
					{
						return true;
					}
				}
				return false;
			}
		}

		const int VERSION = 1;
		const int VERSION_0 = 0;
		//セーブデータ用のバイナリ書き込み
		public void WriteSaveData(BinaryWriter writer)
		{
			writer.Write(VERSION);
			writer.Write(saveStreamNameList.Count);
			foreach (string saveStreamName in saveStreamNameList)
			{
				writer.Write(saveStreamName);
				SoundStreamFade stream = GetStreamAndCreateIfMissing(saveStreamName);
				stream.WriteSaveData(writer);
			}
		}

		//セーブデータ用のバイナリ読み込み
		public void ReadSaveDataBuffer(BinaryReader reader)
		{
			int version = reader.ReadInt32();
			if (version == VERSION)
			{
				int count = reader.ReadInt32();
				for (int i = 0; i < count; ++i)
				{
					string streamName = reader.ReadString();
					SoundStreamFade stream = GetStreamAndCreateIfMissing(streamName);
					stream.ReadSaveData(reader, () => SoundMangaer.GetMasterVolume(streamName));
				}
			}
			else if (version == VERSION_0)
			{
				//BGMと環境音のみを再生
				GetStreamAndCreateIfMissing(SoundManager.IdBgm).ReadSaveData(reader, () => SoundMangaer.GetMasterVolume(SoundManager.IdBgm));
				GetStreamAndCreateIfMissing(SoundManager.IdAmbience).ReadSaveData(reader, () => SoundMangaer.GetMasterVolume(SoundManager.IdAmbience));
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, version));
			}
		}


		//指定の名前のストリームを取得。なければ作成。
		SoundStreamFade GetStreamAndCreateIfMissing(string name)
		{
			SoundStreamFade stream;
			if (!streamTbl.TryGetValue (name, out stream))
			{
				stream = UtageToolKit.AddChildGameObjectComponent<SoundStreamFade>(CachedTransform,name);
				streamTbl.Add(name,stream);
			}
			return stream;
		}
	}
}