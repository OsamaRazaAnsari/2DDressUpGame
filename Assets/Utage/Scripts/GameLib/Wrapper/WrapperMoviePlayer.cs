//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_WEBGL) || UNITY_EDITOR
#define USE_MOVIE_TEXTURE
#endif

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


namespace Utage
{



	/// <summary>
	/// ムービー再生処理のラッパー
	/// </summary>
	[RequireComponent(typeof(RawImage))]
	[AddComponentMenu("Utage/Lib/Wrapper/MoviePlayer")]
	public class WrapperMoviePlayer: MonoBehaviour
	{
		public static WrapperMoviePlayer GetInstance() { return instance ?? (instance = FindObjectOfType<WrapperMoviePlayer>()); }
		static WrapperMoviePlayer instance = null;

		public static void SetRenderTarget(GameObject target){ GetInstance().Target =target; }
		public static void Play(string path){ GetInstance().PlayMovie(path); }
		public static void Cancel() { GetInstance().CancelMovie(); }
		public static bool IsPlaying() { return GetInstance().isPlaying; }


		bool isPlaying;

		public Color bgColor = Color.black;
		public bool ignoreCancel = false;
		public float cancelFadeTime = 0.5f;

		public GameObject Target
		{
			set
			{
				if( renderTarget != value )
				{
#if USE_MOVIE_TEXTURE
					ClearRenderTargetTexture();
#endif
					renderTarget = value;
				}
			}
			get
			{
				if(renderTarget==null)
				{
					return this.gameObject;
				}
				else
				{
					return renderTarget;
				}
			}
		}

		GameObject renderTarget;
#if USE_MOVIE_TEXTURE
		public List<MovieTexture> movieList;
		MovieTexture movieTexture;
#endif
		void Awake()
		{
			if (null != instance)
			{
				Destroy(this.gameObject);
				return;
			}
			else
			{
				instance = this;
			}
		}


		public void PlayMovie(string path)
		{
#if USE_MOVIE_TEXTURE
			PlayMovieTextue(path);
#else
			StartCoroutine(CoPlayMobileMovie(path));
#endif
		}
		public void CancelMovie()
		{
			if (ignoreCancel) return;

#if USE_MOVIE_TEXTURE
			CancelMovieTexture();
#else
#endif
		}

#if USE_MOVIE_TEXTURE
		void PlayMovieTextue(string path)
		{
			isPlaying = true;
			string name = System.IO.Path.GetFileNameWithoutExtension(path);
			movieTexture = movieList.Find(item => (item.name == name));
			if (movieTexture)
			{
				StartCoroutine(CoPlayMovieTexture(movieTexture));
			}
			else
			{
				StartCoroutine(CoPlayMovieOGV(name));
			}
		}
		IEnumerator CoPlayMovieTexture(MovieTexture movieTexture)
		{
			PlayMovie ();
			while (movieTexture.isPlaying)
			{
				yield return 0;
			}
			StopMovieTexture();
		}

		IEnumerator CoPlayMovieOGV(string path)
		{
			//WWWでロード
			using (WWW www = new WWW(ToStreamingPath(path)))
			{
				movieTexture = www.movie;
				if (movieTexture == null )
				{
					Debug.LogError("Movie Load Error:" + path);
				}
				else
				{
					while (!movieTexture.isReadyToPlay)
					{
						if(!string.IsNullOrEmpty(www.error))
						{
							Debug.LogError("Movie Load Error : " + www.error);
							break;
						}
						yield return 0;
					}
					if (string.IsNullOrEmpty(www.error))
					{
						yield return StartCoroutine(CoPlayMovieTexture(movieTexture));
					}
				}
			}
		}

		void CancelMovieTexture()
		{
			StartCoroutine(CoCancelMovieTexture());
		}
		IEnumerator CoCancelMovieTexture()
		{
			FadeOutMovie (cancelFadeTime);
			yield return new WaitForSeconds(cancelFadeTime);
			StopMovieTexture();
		}

		void StopMovieTexture()
		{
			if(movieTexture)
			{
				movieTexture.Stop ();
				if (movieTexture.audioClip)
				{
					SoundManager.GetInstance().StopBgm();
				}
			}
			ClearRenderTargetTexture ();
			movieTexture = null;
			isPlaying = false;
			StopAllCoroutines();
		}

		void PlayMovie()
		{
			GameObject target = Target;
			RawImage rawImage = target.GetComponent<RawImage>();
			if(rawImage)
			{
				rawImage.enabled = true;
				rawImage.texture = movieTexture;
			}
			else
			{
				target.GetComponent<Renderer>().material.mainTexture = movieTexture;
			}
			movieTexture.Play();
			if (movieTexture.audioClip)
			{
				SoundManager.GetInstance().PlayBgm(movieTexture.audioClip, false);
			}
		}

		void FadeOutMovie( float fadeTime )
		{
			GameObject target = Target;
			RawImage rawImage = target.GetComponent<RawImage>();
			if(rawImage)
			{
				rawImage.CrossFadeAlpha(0, fadeTime, true);
			}
			if (movieTexture)
			{
				if (movieTexture.audioClip)
				{
					SoundManager.GetInstance().StopBgm(cancelFadeTime);
				}
			}
		}
		void ClearRenderTargetTexture()
		{
			GameObject target = Target;
			RawImage rawImage = target.GetComponent<RawImage>();
			if(rawImage)
			{
				rawImage.texture = null;
				rawImage.CrossFadeAlpha(1, 0,true);
				rawImage.enabled = false;
			}
			else
			{
				target.GetComponent<Renderer>().material.mainTexture = null;
			}
		}
#elif UNITY_WEBGL
		IEnumerator CoPlayMobileMovie(string path)
		{
			isPlaying = false;
			yield break;
		}

#else
		IEnumerator CoPlayMobileMovie(string path)
		{
			isPlaying = true;
			if (ignoreCancel)
			{
				Handheld.PlayFullScreenMovie(path, bgColor);
			}
			else
			{
				Handheld.PlayFullScreenMovie(path,bgColor,FullScreenMovieControlMode.CancelOnInput);
			}
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			isPlaying = false;
		}
#endif
		string ToStreamingPath(string path)
		{
			return ((Application.platform == RuntimePlatform.Android) ? "" : "file://") + System.IO.Path.Combine(Application.streamingAssetsPath, path);
		}
	}
}