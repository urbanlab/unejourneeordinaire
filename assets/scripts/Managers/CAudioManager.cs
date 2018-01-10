using UnityEngine;
using System.Collections;

public class CAudioManager : MonoBehaviour 
{
	private AudioSource _asGUIAudioSource;
	private AudioSource _asMusicSource;

	[SerializeField] private AudioClip _errorSound;
	
	[SerializeField] private AudioClip _clickSound;
	[SerializeField] private AudioClip _hoverSound;
	[SerializeField] private AudioClip _moveForwardSound;

	[SerializeField] private AudioClip _fadeSound;

	[SerializeField] private AudioClip _openSound;
	[SerializeField] private AudioClip _closeSound;

	[SerializeField] private AudioClip _music;
	
	// audio fade
	private bool _bMusicFadeOut = false;
	private bool _bIsMusicFading = false;
	private float _fMaxMusicVolume = 1.0F;
	private float _fMinMusicVolume = 0.0F;
	private float _fMusicVolume = 1.0F;
	private float _fMusicFadeStartTime = 0.0F;

	public static CAudioManager Get()
	{
		return GameObject.Find ("_audio_manager").GetComponent<CAudioManager>();
	}
	
	///-----------------------------------------------------------------------------------
	/// 
	///-----------------------------------------------------------------------------------
	void Awake ( )
	{
		DontDestroyOnLoad( this );
	}

	///-----------------------------------------------------------------------------------
	/// 
	///-----------------------------------------------------------------------------------	
	void Start ( ) 
	{	
		_asGUIAudioSource = GetComponent< AudioSource > ();
		_asMusicSource = gameObject.AddComponent< AudioSource > ();
		_asMusicSource.clip = _music;
		_asMusicSource.playOnAwake = true;
		_asMusicSource.loop = true;
	}

	void Update( )
	{
		FadeAudio ();
	}

	public void PlayErrorSound( )
	{
		_asGUIAudioSource.PlayOneShot (_errorSound);
	}
	
	public void PlayClickSound( )
	{	
		_asGUIAudioSource.PlayOneShot( _clickSound );
	}
	
	public void PlayHoverSound( )
	{
		_asGUIAudioSource.PlayOneShot( _hoverSound );
	}

	public void PlayFadeSound( )
	{	
		_asGUIAudioSource.PlayOneShot( _fadeSound );
	}

	public void PlayOpenSound( )
	{
		_asGUIAudioSource.PlayOneShot( _openSound );
	}

	public void PlayCloseSound( )
	{
		_asGUIAudioSource.PlayOneShot( _closeSound );
	}

	public void PlayMoveForwardPanelSound( )
	{
		_asGUIAudioSource.PlayOneShot( _moveForwardSound );
	}

	public void PlayMusic( )
	{
		if (!_asMusicSource.isPlaying )
		{
			_asMusicSource.Play ();
			AudioFadeIn( );
		}
	}
	
	public void PauseMusic( )
	{
		if(_asMusicSource.isPlaying)
		{
			AudioFadeOut( );
		}
	}
	
	///-----------------------------------------------------------------------------------
	/// <summary> starts fading in the audio </summary>
	///-----------------------------------------------------------------------------------
	private void AudioFadeIn( )
	{
		_bMusicFadeOut = false;
		_fMusicFadeStartTime = Time.time;
		_bIsMusicFading = true;
	}
	
	///-----------------------------------------------------------------------------------
	/// <summary> starts fading out the audio </summary>
	///-----------------------------------------------------------------------------------
	private void AudioFadeOut( )
	{
		_bMusicFadeOut = true;
		_fMusicFadeStartTime = Time.time;
		_bIsMusicFading = true;
	}
	
	///-----------------------------------------------------------------------------------
	/// <summary> manages the fade in/out of the video audio volume </summary>
	///-----------------------------------------------------------------------------------
	private void FadeAudio( )
	{
		if( _bIsMusicFading )
		{
			// fade in
			if( !_bMusicFadeOut )
			{
				_fMusicVolume = Mathf.SmoothStep( _fMinMusicVolume, _fMaxMusicVolume, (Time.time - _fMusicFadeStartTime ) * 1.5F );
				
				if( _fMusicVolume >= _fMaxMusicVolume )
				{
					_bIsMusicFading = false;
				}
			}
			// fade out
			else
			{
				_fMusicVolume = Mathf.SmoothStep( _fMaxMusicVolume, _fMinMusicVolume, (Time.time - _fMusicFadeStartTime )* 1.5F );
				
				if( _fMusicVolume <= _fMinMusicVolume )
				{
					_bIsMusicFading = false;
					_asMusicSource.Pause ();
				}
			}
			
			_asMusicSource.volume = _fMusicVolume;
		}
	}
}