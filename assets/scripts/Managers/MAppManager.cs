using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class MAppManager : MonoBehaviour {

    private CLanguageManager _languageManager;

    private GameObject _goCrosshair;
    [SerializeField]
    private Transform _tOccluder;
    private float _fLoadingScreenStartTime;
    [SerializeField]
    private float _fLoadingScreenMaxDuration = 15.0F;
    private bool _bIgnoreLoadingScreenMaxDuration = false;
    private CLoadingIndicator _loadingIndicator = null;
    private bool _bIsShowingError = false;
    private string _strErrorMessage;
    [SerializeField]
    private Color _cErrorPrimaryColor;
    [SerializeField]
    private Material _matErrorPrimaryColor;
    [SerializeField]
    private TextMesh _tmErrorMessage;
    private Transform _tErrorProgressBar;
    private Vector3 _v3ErrorProgressBarOriginalScale;
    private float _fErrorProgressBarHorizontalScale = 0;
    private float _fErrorProgressBarStartTime = 0;

	private bool _bIsInitialized = false;

    private AudioSource _Music;
    private AudioSource _SFX;
    private AudioSource _Voice;

    private int _iLienScore;
    private int _iBudgetScore;
    private int _iQualiteScore;

	private int _iDistubedEnvironment = 0; 
	private int _iFineAtHome = 0;  
	private int _iPrecarity = 0;    

    ///-----------------------------------------------------------------------------------
    /// 																			
    ///-----------------------------------------------------------------------------------
    private bool _bIsCardboard = false;

    public bool IsCardboard()
    {
        return _bIsCardboard;
    }

    private LoadingCircle _loadingCircle;

    public void SetLoadingCircle(LoadingCircle a_loadingCircle)
    {
        _loadingCircle = a_loadingCircle;
        AttachFPS(_loadingCircle.transform);
    }

    public void ResetCamera()
    {
        if (_bIsCardboard)
        {
            GvrViewer.Instance.Recenter();
        }
        else
        {
            OVRManager.sReset();
        }
    }

    ///-----------------------------------------------------------------------------------
    /// 																			Statics
    ///-----------------------------------------------------------------------------------
    public static bool IsStartSceneInit()
    {
        return GameObject.Find("_app_manager") != null;
    }

    public static MAppManager Get()
    {
        return GameObject.Find("_app_manager").GetComponent<MAppManager>();
    }

    #region GETS/SETS
    ///-----------------------------------------------------------------------------------
    /// 																		
    ///-----------------------------------------------------------------------------------
    public void ShowLoadingScreen(bool a_bValue)
    {
        ResetCursor();
        _bIsShowingError = false;

        if (a_bValue)
        {
            _fLoadingScreenStartTime = Time.time;
            ResetCursor();
        }

        // show loading indicator
        if (_loadingIndicator != null)
        {
            _loadingIndicator.Toggle(true);
        }
        // hide crosshair
        if (_bAllowCrosshair)
            ToggleCrosshair(!a_bValue);
    }

    private bool _bAllowCrosshair = true;
    public void ToggleCrosshair(bool a_bValue)
    {
        _loadingCircle.enabled = a_bValue;
        _goCrosshair.GetComponent<MeshRenderer>().enabled = a_bValue;
    }

    public void SetLoadingScreenMaxDuration(float a_fMaxDuration, bool a_bIgnore = false)
    {
        _fLoadingScreenMaxDuration = a_fMaxDuration;
        _bIgnoreLoadingScreenMaxDuration = a_bIgnore;
    }
    ///-----------------------------------------------------------------------------------
    /// 																		ERROR
    ///-----------------------------------------------------------------------------------
    public bool IsShowingError()
    {
        return _bIsShowingError;
    }

    public void ShowErrorMessage(string a_strXMLcode, bool a_bSecondColor = false)
    {
        //		_audioManager.PlayErrorSound ();
        _bIsShowingError = true;
        _strErrorMessage = _languageManager.GetText(a_strXMLcode);

        _tmErrorMessage.color = _cErrorPrimaryColor;
        _tErrorProgressBar.GetComponent<MeshRenderer>().material = _matErrorPrimaryColor;

        // resume texture downloads if timeout and we are not in the asset viewer
        //e.g. we tried to load an asset but if failed
        if (a_strXMLcode == "ERROR_server_timeout")
        {
            SetLoadingScreenMaxDuration(10f);
        }
    }
    ///-----------------------------------------------------------------------------------
    /// 																		SCORE
    ///-----------------------------------------------------------------------------------
	public int GetQualiteScore()
	{
		return _iQualiteScore;
	}

	public bool SetQualiteScore(int a_Score)
	{
		bool bValueChanged = true;
		if ( _iQualiteScore == a_Score )
		{
			bValueChanged = false;
		}

		_iQualiteScore = a_Score;

		return bValueChanged;
	}

	public int GetLienScore()
	{
		return _iLienScore;
	}

	public bool SetLienScore(int a_Score)
	{
		bool bValueChanged = true;
		if ( _iLienScore == a_Score )
		{
			bValueChanged = false;
		}

		_iLienScore = a_Score;

		return bValueChanged;
	}

	public int GetBudgetScore()
	{
		return _iBudgetScore;
	}

	public bool SetBudgetScore(int a_Score)
	{
		bool bValueChange = true;
		if (  _iBudgetScore == a_Score )
		{
			bValueChange = false;
		}

		_iBudgetScore = a_Score;

		return bValueChange;
	}

	public bool SetPrecarityScore(int a_iScore )
	{
		bool bValueChanged = true;

		if (_iPrecarity == a_iScore )
		{
			bValueChanged = false;
		}

		_iPrecarity = a_iScore;

		return bValueChanged;
	}

	public int GetPrecarityScore( )
	{
		return _iPrecarity;
	}

	public bool SetDisturbedEnvironmentScore( int a_iScore )
	{
		bool bValueChange = true;

		if ( _iDistubedEnvironment == a_iScore )
		{
			bValueChange = false;
		}

		_iDistubedEnvironment = a_iScore;

		return bValueChange;
	}
	public int GetDisturbedEnvironmentScore( )
	{
		return _iDistubedEnvironment;
	}

	public bool SetFineAtHomeScore( int a_iScore )
	{
		bool bValueChange = true;

		if ( _iFineAtHome == a_iScore )
		{
			bValueChange = false;
		}

		_iFineAtHome = a_iScore;

		return bValueChange;
	}
	public int GetFineAtHomeScore( )
	{
		return _iFineAtHome;
	}
    ///-----------------------------------------------------------------------------------
    #endregion

    ///-----------------------------------------------------------------------------------
    /// 																			INIT
    ///-----------------------------------------------------------------------------------
    public void Initialize()
    {
        _goCrosshair = GameObject.Find("Camera/TrackingSpace/CenterEyeAnchor/crosshair");
        if (_goCrosshair == null)
        {
            _goCrosshair = GameObject.Find("Camera/TrackingSpace/Main/crosshair");
        }

        _bIsCardboard = GameObject.Find("Camera").transform.Find("GvrViewerMain") != null;
        if (_bIsCardboard)
        {
            GvrViewer.Instance.VRModeEnabled = false;
            Camera.main.GetComponent<StereoController>().Head.trackRotation = false;
            _bAllowCrosshair = false;
            ToggleCrosshair(false);
        }

        _bIsInitialized = true;

        //FadeIn(1.0F);
    }

    ///-----------------------------------------------------------------------------------
    /// 
    ///-----------------------------------------------------------------------------------
    public bool HasLoadingCircleClicked()
    {
        return _loadingCircle.IsSelected();
    }

    public void IncreaseTimer()
    {
        _loadingCircle.IncreaseTimer();
    }

    public void ResetCursor()
    {
        _loadingCircle.ResetCursor();
    }

    ///-----------------------------------------------------------------------------------
    /// 
    ///-----------------------------------------------------------------------------------
    public void DisableAll()
    {

    }
    ///-----------------------------------------------------------------------------------
    /// 
    ///-----------------------------------------------------------------------------------
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    ///-----------------------------------------------------------------------------------
    /// 
    ///-----------------------------------------------------------------------------------
    void Start()
    {
        _languageManager = GetComponent<CLanguageManager>();

        _Music = GameObject.Find("Music").GetComponent<AudioSource>();
        _SFX = GameObject.Find("SFX").GetComponent<AudioSource>();
        _Voice = GameObject.Find("Voice").GetComponent<AudioSource>();

        _iQualiteScore = 5;
        _iLienScore = 5;
        _iBudgetScore = 5;
        Time.timeScale = 1;

        if (_bCalculateFPS)
        {
            _fNextUpdate = Time.time;
        }

        _tErrorProgressBar = _tmErrorMessage.transform.Find("progress_bar");
        _v3ErrorProgressBarOriginalScale = _tErrorProgressBar.localScale;
        _tmErrorMessage.gameObject.SetActive(false);

        LoadLevel("Main");
    }

    public bool IsInitialized()
    {
        return _bIsInitialized;
    }

    ///-----------------------------------------------------------------------------------
    /// 
    ///-----------------------------------------------------------------------------------
    void Update()
    {
        UpdateFPS();

        if (_loadingIndicator == null)
        {
			
			if (GameObject.Find("Camera") != null)
            {
				GameObject temp = GameObject.Find("___loading_indicator");
				if(temp != null)
                	_loadingIndicator = temp.GetComponent<CLoadingIndicator>();
            }
        }

        // compute occluder rendering
        if (_bIsShowingError)
        {
            if (!_tOccluder.gameObject.activeSelf)
            {
                _tOccluder.gameObject.SetActive(true);
            }
        }
        else
        {
            if (_tOccluder.gameObject.activeSelf)
            {
                _tOccluder.gameObject.SetActive(false);
            }

            if (_loadingIndicator != null)
            {
                if (_loadingIndicator.IsEnabled())
                {
                    _loadingIndicator.Toggle(false);
                }
            }
        }

        // if not showing an error
        if (!_bIsShowingError)
        {
            // hide the error text if visible
            if (_tmErrorMessage.gameObject.activeSelf)
            {
                _tmErrorMessage.gameObject.SetActive(false);
            }
        }
        // if showing error
        else
        {
            // hide loading indicator if enabled
            if (_loadingIndicator != null)
            {
                if (_loadingIndicator.IsEnabled())
                {
                    _loadingIndicator.Toggle(false);
                }
            }

            // show error if hidden
            if (!_tmErrorMessage.gameObject.activeSelf)
            {
                _tmErrorMessage.gameObject.SetActive(true);
                _tmErrorMessage.text = _strErrorMessage;

                _fErrorProgressBarStartTime = Time.time;
            }

            if (Time.time > _fErrorProgressBarStartTime + 2.0F)
            {
                ShowLoadingScreen(false);
                _tErrorProgressBar.localScale = _v3ErrorProgressBarOriginalScale;
            }
            else
            {
                _fErrorProgressBarHorizontalScale = Mathf.Lerp(_v3ErrorProgressBarOriginalScale.x, 0.0F, (Time.time - _fErrorProgressBarStartTime) * 0.5F);

            }

            _tErrorProgressBar.localScale = new Vector3(_fErrorProgressBarHorizontalScale, _v3ErrorProgressBarOriginalScale.y, _v3ErrorProgressBarOriginalScale.z);
        }

    }

    ///-----------------------------------------------------------------------------------
    /// 
    ///-----------------------------------------------------------------------------------
    void OnDestroy()
    {

    }

    ///-----------------------------------------------------------------------------------
    /// <summary> Loads an scene. </summary>
    /// <param name="level"> name of the scene to load </param>
    ///-----------------------------------------------------------------------------------
    public static void LoadLevel(string a_strLevel)
    {
#if UNITY_5_2
		            Application.LoadLevel(a_strLevel);
#else
        SceneManager.LoadScene(a_strLevel);
#endif
    }
    ///-----------------------------------------------------------------------------------
    /// 																	FPS COUNTER
    ///-----------------------------------------------------------------------------------
    [SerializeField]
    private bool _bCalculateFPS = false;
    private TextMesh _tmFPS;
    private int _iFramesPerSecond;
    private int _iFrameCount = 0;
    private float _fNextUpdate = 0.0F;

    public int GetFPS()
    {
        return _iFramesPerSecond;
    }

    private void AttachFPS(Transform _rtParent)
    {
        if (_bCalculateFPS)
        {
            GameObject _go = new GameObject();
            _tmFPS = _go.AddComponent<TextMesh>();
            _tmFPS.anchor = TextAnchor.LowerCenter;
            _tmFPS.font = Resources.Load("Fonts/Roboto_Bold") as Font;
            _tmFPS.GetComponent<MeshRenderer>().material = Resources.Load("Fonts/mRoboto_Bold") as Material;
            _tmFPS.fontSize = 30;
            _tmFPS.transform.parent = _rtParent;
            _tmFPS.transform.localPosition = -10.0F * Vector3.up;
            _tmFPS.transform.localRotation = Quaternion.identity;
            _tmFPS.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void UpdateFPS()
    {
        if (_bCalculateFPS)
        {
            _iFrameCount++;
            if (Time.time > _fNextUpdate)
            {
                _fNextUpdate += 1.0F;
                _iFramesPerSecond = (int)_iFrameCount;
                _iFrameCount = 0;
            }

            if (_tmFPS != null)
            {
                if (_iFramesPerSecond >= 60)
                {
                    _tmFPS.color = Color.green;
                }
                else if (_iFramesPerSecond >= 30)
                {
                    _tmFPS.color = Color.yellow;
                }
                else
                {
                    _tmFPS.color = Color.red;
                }
                _tmFPS.text = "\nFPS:" + _iFramesPerSecond.ToString();
            }
        }
    }
    ///-----------------------------------------------------------------------------------
    /// 																	AUDIO
    ///-----------------------------------------------------------------------------------
    public void PlaySFX(AudioClip audio)
    {
        _SFX.PlayOneShot(audio);
    }

    public void PlayVoice(AudioClip audio)
    {
		_Voice.volume = 1.0f;
        _Voice.PlayOneShot(audio);
    }

    public void PlayMusic(AudioClip audio)
    {
        _Music.Stop();
        _Music.clip = audio;
        _Music.loop = true;
        _Music.Play();
    }

    //public void PlaySubmitSFX()
    //{
    //    PlaySFX(_sfxSubmit);
    //}

    //public void PlayCancelSFX()
    //{
    //    PlaySFX(_sfxCancel);
    //}

    public void StopVoice()
    {
        _Voice.Stop();
    }

    public void PauseVoice()
    {
        _Voice.Pause();
    }

    public void StopMusic()
    {
        _Music.Stop();
    }

    public void PauseMusic()
    {
        _Music.Pause();
    }

    public void ReturnToPlay()
    {
        _Music.Play();
    }
    ///-----------------------------------------------------------------------------------
    /// 																	END
    ///-----------------------------------------------------------------------------------
}
