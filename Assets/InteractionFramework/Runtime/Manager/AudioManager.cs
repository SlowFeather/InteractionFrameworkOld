using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace InteractionFramework.Runtime
{
	#region External Structures

	public enum MusicTransition
	{

		Swift,

		LinearFade,

		CrossFade
	}

	[Serializable]
	public struct BackgroundMusic
	{

		public AudioClip CurrentClip;

		public AudioClip NextClip;

		public MusicTransition MusicTransition;

		public float TransitionDuration;
	}

	[Serializable]
	public class SoundEffect : MonoBehaviour
	{

		private AudioSource audioSource;
		private float originalVolume;
		private float duration;
		private float time;
		private Action callback;
		private bool singleton;

		public string Name
		{
			get { return audioSource.clip.name; }
		}

		public float Length
		{
			get { return audioSource.clip.length; }
		}

		public float PlaybackPosition
		{
			get { return audioSource.time; }
		}

		public AudioSource Source
		{
			get{ return audioSource; }
			set { audioSource = value; }
		}

		public float OriginalVolume
		{
			get{ return originalVolume; }
			set { originalVolume = value; }
		}

		public float Duration
		{
			get{ return duration; }
			set { duration = value; }
		}

		public float Time
		{
			get{ return time; }
			set { time = value; }
		}

		public float NormalisedTime
		{
			get{ return Time / Duration; }
		}

		public Action Callback
		{
			get{ return callback; }
			set { callback = value; }
		}

		public bool Singleton
		{
			get{ return singleton; }
			set { singleton = value; }
		}
	}

	#endregion

	[RequireComponent(typeof(AudioSource))]
	public class AudioManager : MonoBehaviour
	{
		#region Inspector Variables

		[Header("Background Music Properties")]

		[Tooltip("Is the background music mute")]
		[SerializeField] bool _musicOn = true;

		[Tooltip("The background music volume")]
		[Range(0, 1)]
		[SerializeField] float _musicVolume = 1f;

		[Tooltip("Use the current music volume settings on initialisation start")]
		[SerializeField] bool _useMusicVolOnStart = false;

		[Tooltip("The target group for the background music to route its their signals. If none is to be used, then leave unassigned or blank")]
		[SerializeField] AudioMixerGroup _musicMixerGroup = null;

		[Tooltip("The exposed volume parameter name of the music mixer group")]
		[SerializeField] string _volumeOfMusicMixer = string.Empty;

		[Space(3)]

		[Header("Sound Effect Properties")]

		[Tooltip("The sound effects volume")]
		[SerializeField] bool _soundFxOn = true;

		[Tooltip("The sound effects volume")]
		[Range(0, 1)]
		[SerializeField] float _soundFxVolume = 1f;

		[Tooltip("Use the current sound effect volume settings on initialisation start")]
		[SerializeField] bool _useSfxVolOnStart = false;

		[Tooltip("The target group for the sound effects to route its their signals. If none is to be used, then leave unassigned or blank")]
		[SerializeField] AudioMixerGroup _soundFxMixerGroup = null;

		[Tooltip("The exposed volume parameter name of the sound effects mixer group")]
		[SerializeField] string _volumeOfSFXMixer = string.Empty;

		[Space(3)]

		[Tooltip("A list of all audio clips attached to the AudioManager")]
		[SerializeField] List<AudioClip> _playlist = new List<AudioClip>();

		#endregion

		#region Singleton Pattern

		private static AudioManager instance;

		private static object key = new object();

		private static bool alive = true;

		public static AudioManager Instance
		{

			get
			{

				if (!alive)
				{
					Debug.LogWarning(typeof(AudioManager) + "' is already destroyed on application quit.");
					return null;
				}

				if (instance == null)
				{

					instance = FindObjectOfType<AudioManager>();

					if (instance == null)
					{

						lock (key)
						{

							instance = new GameObject().AddComponent<AudioManager>();
						}
					}
				}

				return instance;
			}
		}

		private AudioManager() { }

		#endregion

		#region Public Static Getters

		public AudioClip CurrentMusicClip
		{
			get { return backgroundMusic.CurrentClip; }
		}

		public List<SoundEffect> SoundFxPool
		{
			get { return sfxPool; }
		}

		public List<AudioClip> Playlist
		{
			get { return _playlist; }
		}

		public bool IsMusicPlaying
		{
			get { return musicSource != null && musicSource.isPlaying; }
		}

		public float MusicVolume
		{
			get { return _musicVolume; }
			set { SetBGMVolume (value); }
		}

		public float SoundVolume
		{
			get { return _soundFxVolume; }
			set { SetSFXVolume (value); }
		}

		public bool IsMusicOn
		{
			get { return _musicOn; }
			set { ToggleBGMMute (value); }
		}

		public bool IsSoundOn
		{
			get { return _soundFxOn; }
			set { ToggleSFXMute (value); }
		}

		public bool IsMasterMute
		{
			get { return !_musicOn && !_soundFxOn; }
			set { ToggleMute(value); }
		}

		#endregion

		#region Private Static Variables

		List<SoundEffect> sfxPool = new List<SoundEffect>();

		static BackgroundMusic backgroundMusic;

		static AudioSource musicSource = null, crossfadeSource = null;

		static float currentMusicVol = 0, currentSfxVol = 0, musicVolCap = 0, savedPitch = 1f;

		static bool musicOn = false, sfxOn = false;

		static float transitionTime;

		static readonly string BgMusicVolKey = "BGMVol";
		static readonly string SoundFxVolKey = "SFXVol";
		static readonly string BgMusicMuteKey = "BGMMute";
		static readonly string SoundFxMuteKey = "SFXMute";

		#endregion

		#region Initialisation Functions

		void OnDestroy()
		{
			StopAllCoroutines();
			SaveAllPreferences();
		}

		void OnApplicationExit()
		{
			alive = false;
		}

		void Initialise()
		{

			gameObject.name = "AudioManager";

			_musicOn = LoadBGMMuteStatus();
			_musicVolume = _useMusicVolOnStart ? _musicVolume : LoadBGMVolume();
			_soundFxOn = LoadSFXMuteStatus();
			_soundFxVolume = _useSfxVolOnStart ? _soundFxVolume : LoadSFXVolume();

			if (musicSource == null)
			{
				musicSource = gameObject.GetComponent<AudioSource>();

				musicSource = musicSource ?? gameObject.AddComponent<AudioSource>();
			}

			musicSource = ConfigureAudioSource(musicSource);

			DontDestroyOnLoad(this.gameObject);
		}

		void Awake()
		{
			if (instance == null)
			{

				instance = this;

				Initialise();
			}
			else if (instance != this)
			{

				Destroy(this.gameObject);
			}
		}

		void Start()
		{
			if (musicSource != null)
			{

				StartCoroutine(OnUpdate());
			}
		}

		AudioSource ConfigureAudioSource(AudioSource audioSource)
		{
			audioSource.outputAudioMixerGroup = _musicMixerGroup;
			audioSource.playOnAwake = false;

			audioSource.spatialBlend = 0;
			audioSource.rolloffMode = AudioRolloffMode.Linear;

			audioSource.loop = true;

			audioSource.volume = LoadBGMVolume();

			audioSource.mute = !_musicOn;

			return audioSource;
		}

		#endregion

		#region Update Functions

		private void ManageSoundEffects()
		{

			for (int i = sfxPool.Count - 1; i >= 0; i--)
			{
				SoundEffect sfx = sfxPool[i];

				if (sfx.Source.isPlaying && sfx.Time != float.PositiveInfinity)
				{

					sfx.Time -= UnityEngine.Time.deltaTime;
					sfxPool[i] = sfx;
				}

				if (sfxPool[i].Time <= 0.09f)
				{
					sfxPool[i].Source.Stop();

					if (sfxPool[i].Callback != null)
					{
						sfxPool[i].Callback.Invoke();
					}

					Destroy(sfxPool[i].gameObject);

					sfxPool.RemoveAt(i);

					sfxPool.Sort((x, y) => x.name.CompareTo(y.name));
					break;
				}
			}
		}

		private bool IsMusicAltered()
		{

			bool flag = musicOn != _musicOn || musicOn != !musicSource.mute || currentMusicVol != _musicVolume;

			if (_musicMixerGroup != null && !string.IsNullOrEmpty(_volumeOfMusicMixer.Trim()))
			{
				float vol;

				_musicMixerGroup.audioMixer.GetFloat(_volumeOfMusicMixer, out vol);

				vol = NormaliseVolume(vol);

				return flag || currentMusicVol != vol;
			}

			return flag;
		}

		private bool IsSoundFxAltered()
		{

			bool flag = _soundFxOn != sfxOn || currentSfxVol != _soundFxVolume;

			if (_soundFxMixerGroup != null && !string.IsNullOrEmpty(_volumeOfSFXMixer.Trim()))
			{
				float vol;

				_soundFxMixerGroup.audioMixer.GetFloat(_volumeOfSFXMixer, out vol);

				vol = NormaliseVolume(vol);

				return flag || currentSfxVol != vol;
			}

			return flag;
		}

		private void CrossFadeBackgroundMusic()
		{
			if (backgroundMusic.MusicTransition == MusicTransition.CrossFade)
			{

				if (musicSource.clip.name != backgroundMusic.NextClip.name)
				{

					transitionTime -= UnityEngine.Time.deltaTime;

					musicSource.volume = Mathf.Lerp(0, musicVolCap, transitionTime / backgroundMusic.TransitionDuration);

					crossfadeSource.volume = Mathf.Clamp01(musicVolCap - musicSource.volume);

					crossfadeSource.mute = musicSource.mute;

					if (musicSource.volume <= 0.00f)
					{
						SetBGMVolume(musicVolCap);
						PlayBackgroundMusic(backgroundMusic.NextClip, crossfadeSource.time, crossfadeSource.pitch);
					}
				}
			}
		}

		private void FadeOutFadeInBackgroundMusic()
		{
			if (backgroundMusic.MusicTransition == MusicTransition.LinearFade)
			{

				if (musicSource.clip.name == backgroundMusic.NextClip.name)
				{

					transitionTime += UnityEngine.Time.deltaTime;

					musicSource.volume = Mathf.Lerp(0, musicVolCap, transitionTime / backgroundMusic.TransitionDuration);

					if (musicSource.volume >= musicVolCap)
					{
						SetBGMVolume(musicVolCap);
						PlayBackgroundMusic(backgroundMusic.NextClip, musicSource.time, savedPitch);
					}
				}

				else
				{

					transitionTime -= UnityEngine.Time.deltaTime;

					musicSource.volume = Mathf.Lerp(0, musicVolCap, transitionTime/backgroundMusic.TransitionDuration);

					if (musicSource.volume <= 0.00f)
					{
						musicSource.volume = transitionTime = 0;
						PlayMusicFromSource(ref musicSource, backgroundMusic.NextClip, 0, musicSource.pitch);
					}
				}
			}
		}

		IEnumerator OnUpdate()
		{
			while (alive)
			{
				ManageSoundEffects();

				if (IsMusicAltered())
				{

					ToggleBGMMute(!musicOn);

					if (currentMusicVol != _musicVolume)
					{
						currentMusicVol = _musicVolume;
					}

					if (_musicMixerGroup != null && !string.IsNullOrEmpty(_volumeOfMusicMixer))
					{
						float vol;
						_musicMixerGroup.audioMixer.GetFloat(_volumeOfMusicMixer, out vol);
						vol = NormaliseVolume(vol);
						currentMusicVol = vol;
					}

					SetBGMVolume(currentMusicVol);
				}

				if (IsSoundFxAltered())
				{

					ToggleSFXMute(!sfxOn);

					if (currentSfxVol != _soundFxVolume)
					{
						currentSfxVol = _soundFxVolume;
					}

					if (_soundFxMixerGroup != null && !string.IsNullOrEmpty(_volumeOfSFXMixer))
					{
						float vol;
						_soundFxMixerGroup.audioMixer.GetFloat(_volumeOfSFXMixer, out vol);
						vol = NormaliseVolume(vol);
						currentSfxVol = vol;
					}

					SetSFXVolume(currentSfxVol);
				}

				if (crossfadeSource != null)
				{
					CrossFadeBackgroundMusic();

					yield return null;
				}
				else
				{

					if (backgroundMusic.NextClip != null)
					{
						FadeOutFadeInBackgroundMusic();

						yield return null;
					}
				}

				yield return new WaitForEndOfFrame();
			}
		}

		#endregion

		#region Background Music Functions

		private void PlayMusicFromSource(ref AudioSource audio_source, AudioClip clip, float playback_position, float pitch)
		{
			try
			{

				audio_source.clip = clip;

				audio_source.time = playback_position;
				audio_source.pitch = pitch = Mathf.Clamp (pitch, -3f, 3f);
				audio_source.Play();
			}
			catch (NullReferenceException nre)
			{
				Debug.LogError(nre.Message);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}
		}

		private void PlayBackgroundMusic(AudioClip clip, float playback_position, float pitch)
		{

			PlayMusicFromSource(ref musicSource, clip, playback_position, pitch);

			backgroundMusic.NextClip = null;

			backgroundMusic.CurrentClip = clip;

			if (crossfadeSource != null)
			{
				Destroy(crossfadeSource);
				crossfadeSource = null;
			}
		}

		#region Public Background Music API 

		public void PlayBGM(AudioClip clip, MusicTransition transition, float transition_duration, float volume, float pitch, float playback_position = 0)
		{

			if (clip == null || backgroundMusic.CurrentClip == clip)
			{
				return;
			}

			if (backgroundMusic.CurrentClip == null || transition_duration <= 0)
			{
				transition = MusicTransition.Swift;
			} 

			if (transition == MusicTransition.Swift)
			{
				PlayBackgroundMusic(clip, playback_position, pitch);
				SetBGMVolume(volume);
			}
			else
			{

				if (backgroundMusic.NextClip != null)
				{
					Debug.LogWarning("Trying to perform a transition on the background music while one is still active");
					return;
				}

				backgroundMusic.MusicTransition = transition;

				transitionTime = backgroundMusic.TransitionDuration = transition_duration;

				musicVolCap = _musicVolume;

				backgroundMusic.NextClip = clip;

				if (backgroundMusic.MusicTransition == MusicTransition.CrossFade)
				{

					if (crossfadeSource != null)
					{
						Debug.LogWarning("Trying to perform a transition on the background music while one is still active");
						return;
					}

					crossfadeSource = ConfigureAudioSource(gameObject.AddComponent<AudioSource>());

					crossfadeSource.volume = Mathf.Clamp01(musicVolCap - currentMusicVol);
					crossfadeSource.priority = 0;

					PlayMusicFromSource(ref crossfadeSource, backgroundMusic.NextClip, 0, pitch);
				}
			}
		}

		public void PlayBGM(AudioClip clip, MusicTransition transition, float transition_duration, float volume)
		{
			PlayBGM(clip, transition, transition_duration, volume, 1f);
		}

		public void PlayBGM(AudioClip clip, MusicTransition transition, float transition_duration)
		{
			PlayBGM(clip, transition, transition_duration, _musicVolume, 1f);
		}

		public void PlayBGM(AudioClip clip, MusicTransition transition)
		{
			PlayBGM(clip, transition, 1f, _musicVolume, 1f);
		}

		public void PlayBGM(AudioClip clip)
		{
			PlayBGM(clip, MusicTransition.Swift, 1f, _musicVolume, 1f);
		}

		public void PlayBGM(string clip_path, MusicTransition transition, float transition_duration, float volume, float pitch, float playback_position = 0)
		{
			PlayBGM (LoadClip(clip_path), transition, transition_duration, volume, pitch, playback_position);
		}

		public void PlayBGM(string clip_path, MusicTransition transition, float transition_duration, float volume)
		{
			PlayBGM (LoadClip(clip_path), transition, transition_duration, volume, 1f);
		}

		public void PlayBGM(string clip_path, MusicTransition transition, float transition_duration)
		{
			PlayBGM(LoadClip(clip_path), transition, transition_duration, _musicVolume, 1f);
		}

		public void PlayBGM(string clip_path, MusicTransition transition)
		{
			PlayBGM(LoadClip(clip_path), transition, 1f, _musicVolume, 1f);
		}

		public void PlayBGM(string clip_path)
		{
			PlayBGM(LoadClip(clip_path), MusicTransition.Swift, 1f, _musicVolume, 1f);
		}

		public void StopBGM()
		{
			if (musicSource.isPlaying)
			{
				musicSource.Stop();
			}
		}

		public void PauseBGM()
		{
			if (musicSource.isPlaying)
			{
				musicSource.Pause();
			}
		}

		public void ResumeBGM()
		{
			if (!musicSource.isPlaying)
			{
				musicSource.UnPause();
			}
		}

		#endregion

		#endregion

		#region Sound Effect Functions

		private GameObject CreateSoundFx(AudioClip audio_clip, Vector2 location)
		{

			GameObject host = new GameObject("TempAudio");

			host.transform.position = location;

			host.transform.SetParent(transform);

			host.AddComponent<SoundEffect>();

			AudioSource audioSource = host.AddComponent<AudioSource>() as AudioSource;
			audioSource.playOnAwake = false;
			audioSource.spatialBlend = 0;
			audioSource.rolloffMode = AudioRolloffMode.Logarithmic;

			audioSource.outputAudioMixerGroup = _soundFxMixerGroup;

			audioSource.clip = audio_clip;

			audioSource.mute = !_soundFxOn;

			return host;
		}

		#region Public Sound Effect API 

		public int IndexOfSoundFxPool(string name, bool singleton = false)
		{
			int index = 0;
			while (index < sfxPool.Count)
			{
				if (sfxPool[index].Name == name && singleton == sfxPool[index].Singleton)
				{
					return index;
				}

				index++;
			}

			return -1;
		}

		public AudioSource PlaySFX(AudioClip clip, Vector2 location, float duration, float volume, bool singleton = false, float pitch = 1f, Action callback = null)
		{
			if (duration <= 0 || clip == null) 
			{
				return null;
			}

			int index = IndexOfSoundFxPool(clip.name, true);

			if (index >= 0)
			{

				SoundEffect singletonSFx = sfxPool[index];
				singletonSFx.Duration = singletonSFx.Time = duration;
				sfxPool[index] = singletonSFx;

				return sfxPool[index].Source;
			}

			GameObject host = null;
			AudioSource source = null;

			host = CreateSoundFx(clip, location);
			source = host.GetComponent<AudioSource>();
			source.loop = duration > clip.length;
			source.volume = _soundFxVolume * volume;
			source.pitch = pitch;

			SoundEffect sfx = host.GetComponent<SoundEffect>();
			sfx.Singleton = singleton;
			sfx.Source = source;
			sfx.OriginalVolume = volume;
			sfx.Duration = sfx.Time = duration;
			sfx.Callback = callback;

			sfxPool.Add(sfx);

			source.Play();

			return source;
		}

		public AudioSource PlaySFX(AudioClip clip, Vector2 location, float duration, bool singleton = false, Action callback = null)
		{
			return PlaySFX(clip, location, duration, _soundFxVolume, singleton, 1f, callback);
		}

		public AudioSource PlaySFX(AudioClip clip, float duration, bool singleton = false, Action callback = null)
		{
			return PlaySFX(clip, Vector2.zero, duration, _soundFxVolume, singleton, 1f, callback);
		}

		public AudioSource RepeatSFX(AudioClip clip, Vector2 location, int repeat, float volume, bool singleton = false, float pitch = 1f, Action callback = null)
		{
			if (clip == null) 
			{
				return null;
			}

			if (repeat != 0)
			{
				int index = IndexOfSoundFxPool(clip.name, true);

				if (index >= 0)
				{

					SoundEffect singletonSFx = sfxPool[index];
					singletonSFx.Duration = singletonSFx.Time = repeat > 0 ? clip.length * repeat : float.PositiveInfinity;
					sfxPool[index] = singletonSFx;

					return sfxPool[index].Source;
				}

				GameObject host = CreateSoundFx(clip, location);
				AudioSource source = host.GetComponent<AudioSource>();
				source.loop = repeat != 0;
				source.volume = _soundFxVolume * volume;
				source.pitch = pitch;

				SoundEffect sfx = host.GetComponent<SoundEffect>();
				sfx.Singleton = singleton;
				sfx.Source = source;
				sfx.OriginalVolume = volume;
				sfx.Duration = sfx.Time = repeat > 0 ? clip.length * repeat : float.PositiveInfinity;
				sfx.Callback = callback;

				sfxPool.Add(sfx);

				source.Play();

				return source;
			}

			return PlayOneShot(clip, location, volume, pitch, callback);
		}

		public AudioSource RepeatSFX(AudioClip clip, Vector2 location, int repeat, bool singleton = false, Action callback = null)
		{
			return RepeatSFX(clip, location, repeat, _soundFxVolume, singleton, 1f, callback);
		}

		public AudioSource RepeatSFX(AudioClip clip, int repeat, bool singleton = false, Action callback = null)
		{
			return RepeatSFX(clip, Vector2.zero, repeat, _soundFxVolume, singleton, 1f, callback);
		}

		public AudioSource PlayOneShot(AudioClip clip, Vector2 location, float volume, float pitch = 1f, Action callback = null)
		{
			if (clip == null) 
			{
				return null;
			}

			GameObject host = CreateSoundFx(clip, location);
			AudioSource source = host.GetComponent<AudioSource>();
			source.loop = false;
			source.volume = _soundFxVolume * volume;
			source.pitch = pitch;

			SoundEffect sfx = host.GetComponent<SoundEffect>();
			sfx.Singleton = false;
			sfx.Source = source;
			sfx.OriginalVolume = volume;
			sfx.Duration = sfx.Time = clip.length;
			sfx.Callback = callback;

			sfxPool.Add(sfx);

			source.Play();

			return source;
		}

		public AudioSource PlayOneShot(AudioClip clip, Vector2 location, Action callback = null)
		{
			return PlayOneShot(clip, location, _soundFxVolume, 1f, callback);
		}

		public AudioSource PlayOneShot(AudioClip clip, Action callback = null)
		{
			return PlayOneShot(clip, Vector2.zero, _soundFxVolume, 1f, callback);
		}

		public void PauseAllSFX()
		{

			foreach (SoundEffect sfx in FindObjectsOfType<SoundEffect>())
			{
				if (sfx.Source.isPlaying) sfx.Source.Pause();
			}
		}

		public void ResumeAllSFX()
		{

			foreach (SoundEffect sfx in FindObjectsOfType<SoundEffect>())
			{
				if (!sfx.Source.isPlaying) sfx.Source.UnPause();
			}
		}

		public void StopAllSFX()
		{

			foreach (SoundEffect sfx in FindObjectsOfType<SoundEffect>())
			{
				if (sfx.Source) 
				{
					sfx.Source.Stop();
					Destroy(sfx.gameObject);
				}
			}

			sfxPool.Clear();
		}

		#endregion

		#endregion

		#region Setter Functions

		public AudioClip LoadClip(string path, bool add_to_playlist = false)
		{
			AudioClip clip = Resources.Load(path) as AudioClip;
			if (clip == null)
			{
				Debug.LogError (string.Format ("AudioClip '{0}' not found at location {1}", path, System.IO.Path.Combine (Application.dataPath, "/Resources/"+path)));
				return null;
			}

			if (add_to_playlist)
			{
				AddToPlaylist(clip);
			}

			return clip;
		}

        public AudioClip LoadClipAndSavePlayList(string path,Action callback=null)
        {
            AudioClip clip = Resources.Load(path) as AudioClip;
            if (clip == null)
            {
                Debug.LogError(string.Format("AudioClip '{0}' not found at location {1}", path, System.IO.Path.Combine(Application.dataPath, "/Resources/" + path)));
                return null;
            }

            if (true)
            {
                AddToPlaylist(clip);
            }

            if (callback!=null)
            {
                callback();
            }
            return clip;
        }

        public void LoadClip(string path, AudioType audio_type, bool add_to_playlist, Action<AudioClip> callback)
		{
			StartCoroutine(LoadAudioClipFromUrl(path, audio_type, (downloadedContent) =>
				{
					if (downloadedContent != null && add_to_playlist)
					{
						AddToPlaylist(downloadedContent);
					}

					callback.Invoke(downloadedContent);
				}));
		}

		IEnumerator LoadAudioClipFromUrl(string audio_url, AudioType audio_type, Action<AudioClip> callback)
		{
			using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip(audio_url, audio_type))
			{
				yield return www.Send();

				if (www.isNetworkError)
				{
					Debug.Log(string.Format("Error downloading audio clip at {0} : ", audio_url, www.error));
				}

				callback.Invoke(UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(www));
			}
		}

		private void ToggleMute(bool flag)
		{
			ToggleBGMMute(flag);
			ToggleSFXMute(flag);
		}

		private void ToggleBGMMute(bool flag)
		{
			musicOn = _musicOn = flag;
			musicSource.mute = !musicOn;
		}

		private void ToggleSFXMute(bool flag)
		{
			sfxOn = _soundFxOn = flag;

			foreach (SoundEffect sfx in FindObjectsOfType<SoundEffect>())
			{
				sfx.Source.mute = !sfxOn;
			}

		}

		private void SetBGMVolume(float volume)
		{
			try
			{

				volume = Mathf.Clamp01(volume);

				musicSource.volume = currentMusicVol = _musicVolume = volume;

				if (_musicMixerGroup != null && !string.IsNullOrEmpty(_volumeOfMusicMixer.Trim()))
				{

					float mixerVol = -80f + (volume * 100f);

					_musicMixerGroup.audioMixer.SetFloat(_volumeOfMusicMixer, mixerVol);
				}
			}
			catch (NullReferenceException nre)
			{
				Debug.LogError(nre.Message);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}
		}

		private void SetSFXVolume(float volume)
		{
			try
			{

				volume = Mathf.Clamp01(volume);

				currentSfxVol = _soundFxVolume = volume;

				foreach (SoundEffect sfx in FindObjectsOfType<SoundEffect>())
				{
					sfx.Source.volume = _soundFxVolume * sfx.OriginalVolume;
					sfx.Source.mute = !_soundFxOn;
				}

				if (_soundFxMixerGroup != null && !string.IsNullOrEmpty(_volumeOfSFXMixer.Trim()))
				{

					float mixerVol = -80f + (volume * 100f);

					_soundFxMixerGroup.audioMixer.SetFloat(_volumeOfSFXMixer, mixerVol);
				}
			}
			catch (NullReferenceException nre)
			{
				Debug.LogError(nre.Message);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}
		}

		private float NormaliseVolume(float vol)
		{
			vol += 80f;
			vol /= 100f;
			return vol;
		}

		#endregion

		#region Player Prefs Functions

		private float LoadBGMVolume()
		{
			return PlayerPrefs.HasKey(BgMusicVolKey) ? PlayerPrefs.GetFloat(BgMusicVolKey) : _musicVolume;
		}

		private float LoadSFXVolume()
		{
			return PlayerPrefs.HasKey(SoundFxVolKey) ? PlayerPrefs.GetFloat(SoundFxVolKey) : _soundFxVolume;
		}

		private bool ToBool(int integer)
		{
			return integer == 0 ? false : true;
		}

		private bool LoadBGMMuteStatus()
		{
			return PlayerPrefs.HasKey(BgMusicMuteKey) ? ToBool(PlayerPrefs.GetInt(BgMusicMuteKey)) : _musicOn;
		}

		private bool LoadSFXMuteStatus()
		{
			return PlayerPrefs.HasKey(SoundFxMuteKey) ? ToBool(PlayerPrefs.GetInt(SoundFxMuteKey)) : _soundFxOn;
		}

		#region Public Player Prefs API

		public void SaveBGMPreferences()
		{
			PlayerPrefs.SetInt(BgMusicMuteKey, _musicOn ? 1 : 0);
			PlayerPrefs.SetFloat(BgMusicVolKey, _musicVolume);
			PlayerPrefs.Save();
		}

		public void SaveSFXPreferences()
		{
			PlayerPrefs.SetInt(SoundFxMuteKey, _soundFxOn ? 1 : 0);
			PlayerPrefs.SetFloat(SoundFxVolKey, _soundFxVolume);
			PlayerPrefs.Save();
		}

		public void ClearAllPreferences()
		{
			PlayerPrefs.DeleteKey(BgMusicVolKey);
			PlayerPrefs.DeleteKey(SoundFxVolKey);
			PlayerPrefs.DeleteKey(BgMusicMuteKey);
			PlayerPrefs.DeleteKey(SoundFxMuteKey);
			PlayerPrefs.Save();
		}

		public void SaveAllPreferences()
		{
			PlayerPrefs.SetFloat(SoundFxVolKey, _soundFxVolume);
			PlayerPrefs.SetFloat(BgMusicVolKey, _musicVolume);
			PlayerPrefs.SetInt(SoundFxMuteKey, _soundFxOn ? 1 : 0);
			PlayerPrefs.SetInt(BgMusicMuteKey, _musicOn ? 1 : 0);
			PlayerPrefs.Save();
		}

		#endregion

		#endregion

		#region Playlist Functions

		public void EmptyPlaylist()
		{
			_playlist.Clear();
		}

		public void AddToPlaylist(AudioClip clip)
		{
			if (clip != null)
			{
				_playlist.Add(clip);
			}
		}

		public void RemoveFromPlaylist(AudioClip clip)
		{
			if (clip != null && GetClipFromPlaylist(clip.name))
			{
				_playlist.Remove (clip);
				_playlist.Sort((x,y)=> x.name.CompareTo(y.name));
			}
		}

		public AudioClip GetClipFromPlaylist(string clip_name)
		{

			for(int i = 0; i < _playlist.Count; i++)
			{

				if (clip_name == _playlist[i].name)
				{
					return _playlist[i];
				}
			}

			Debug.LogWarning(clip_name +" does not exist in the playlist.");
			return null;
		}

		public void LoadPlaylist(string path, bool overwrite)
		{

			AudioClip[] clips = Resources.LoadAll<AudioClip>(path);

			if (clips != null && clips.Length > 0 && overwrite)
			{
				_playlist.Clear();
			}

			for (int i = 0; i < clips.Length; i++)
			{
				_playlist.Add(clips[i]);
			}
		}

		#endregion
	}
}