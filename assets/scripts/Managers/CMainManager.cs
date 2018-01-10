using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeleportState
{
    START,
    FADEIN,
    MOVING,
    FADEOUT,
    PLAYSOUND,
    SHOWCANVAS,
    END
};

public class CMainManager : MonoBehaviour 
{
    //Managers
    private static CMainManager s_oInstance = null;
	private MAppManager _appManager;
	private GameObject _goCamera;
    private CLanguageManager _languageManager;
    public GameObject _recordPlayer;
    //Fade
    private float _fFade;
    private bool _bFaded;
	private Material fadeMaterial;
	private Color _color = Color.black;
    private bool _bTeleporting = false;
    private TeleportState _enumTeleportState;
    public Light _light;

    //Goals
	[SerializeField]
	private GameObject[] _goals;

	private int _iGoalsCount = 0;
	private int _iCurrentGoal = 0;

    private AudioSource _Voice;
    private bool _bIsPlayingVoice;

    private int _iButtonLayer = 1 << 8;
    private LoadingCircle _LoadingCircle;

    private bool _bRightOpEnable = true;
    private bool _bLeftOpEnable = true;

	private Ray _ray;

    //Prefabs
    private GameObject blue;
    private GameObject orange;
    private GameObject pink;
    private GameObject _goClone;
    private GameObject _goLienSpace;
    private GameObject _goBudgetSpace;
    private GameObject _goQualiteSpace;

    //Door
    private GameObject DoorClosed;
    private GameObject DoorOpen;

	//Network WIFI
	private bool _bIsClient = false;
	private int _iIsShowingEndPanel = 0;
	private int _iIsShowingScore = 0;

    public static CMainManager Get()
    {
        return s_oInstance;
    }

    public int GetCurrentGoal()
    {
        return _iCurrentGoal;
    }

	public CGoal GetCurrentGoalScript( )
	{
		GameObject goGoal = _goals[ _iCurrentGoal ];
	
		return goGoal.GetComponent<CGoal>();
	}
    //-----------------------------------------------------
    // SOUND
    //-----------------------------------------------------
    IEnumerator WaitForSoundFinished()
    {
        while (_Voice.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }

        _bIsPlayingVoice = false;
    }

    IEnumerator WaitForSoundFinishedToGoal()
    {
        while (_Voice.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }

        _bIsPlayingVoice = false;
        //NextGoal();
    }

	public bool IsPlayingAudio( )
	{
		return _bIsPlayingVoice;
	}

    //-----------------------------------------------------
    // FADE
    //-----------------------------------------------------
    #region Fade
    IEnumerator IFadeOut()
    {
        yield return new WaitForSeconds(0.1f);
        //_goFadeSphere.SetActive(false);
        //_bFaded = true;

        _fFade = _fFade - 0.1f;

        if (_fFade > 0)
        {
			UpdateFadeMaterial (_fFade);

            StartCoroutine(IFadeOut());
        }
        else
        {
            _bFaded = true;
        }
    }

    IEnumerator IFadeIn()
    {
		yield return new WaitForSeconds(0.1f);

		_fFade = _fFade + 0.1f;

		if (_fFade < 1)
		{
			UpdateFadeMaterial (_fFade);
			StartCoroutine(IFadeIn());
		}
		else
		{
			_bFaded = true;
		}
    }

	private void UpdateFadeMaterial(float a_fValue)
	{
		_color.a = a_fValue;
		fadeMaterial.color = _color;
	}

    public void FadeOut()
    {
        StartCoroutine(IFadeOut());
    }

    public void FadeIn()
    {
        StartCoroutine(IFadeIn());
    }

    public void Teleport(int a_iGoal, TeleportState a_State)
    {
        if (a_State == TeleportState.START)
        {
            _bTeleporting = true;
            _bFaded = false;
            _fFade = 0;
            FadeIn();
            _enumTeleportState = TeleportState.FADEIN;
        }
        else if (a_State == TeleportState.MOVING)
        {
            if (_iCurrentGoal == 1)
            {
                DoorOpen.SetActive(true);
                DoorClosed.SetActive(false);
            }
            
            if (_iCurrentGoal == 9)
            {
                _light.intensity = 0;
            }
            else
            {
                _light.intensity = 1;
            }

            GoToGoal(a_iGoal);
            _bFaded = false;
			UpdateFadeMaterial (_fFade);
            FadeOut();
            _enumTeleportState = TeleportState.FADEOUT;
        }
        else if (a_State == TeleportState.PLAYSOUND)
        {
//            if(_iCurrentGoal == 3)
//            {
//                _recordPlayer.GetComponent<AudioSource>().Play();
//            }
//            else
//            {
//                _recordPlayer.GetComponent<AudioSource>().Stop();
//            }

			UpdateFadeMaterial (0);
            _bIsPlayingVoice = true;
            CGoal oGoal = _goals[_iCurrentGoal].GetComponent<CGoal>();
            if (oGoal.ExistClip())
            {
                oGoal.PlaySound(0);
                _enumTeleportState = TeleportState.PLAYSOUND;
                StartCoroutine(WaitForSoundFinished());
            }
            else
            {
                _enumTeleportState = TeleportState.PLAYSOUND;
                _bIsPlayingVoice = false;
            }
        }
        else if (a_State == TeleportState.SHOWCANVAS)
        {
            CGoal oGoal = _goals[_iCurrentGoal].GetComponent<CGoal>();
            if(oGoal.GetGoalActive())
            {
				if ( _iCurrentGoal == 10 )
				{
					_iIsShowingEndPanel = 1; //to be sended to server
                    ShowFinalScore();
					ShowEnd( );
				}
				else 
				{
					_iIsShowingEndPanel = 0; //to be sended to server
					_iIsShowingScore = 1;

					ShowScore();
					oGoal.ShowPanel(true);
				}
                _enumTeleportState = TeleportState.END;
            }
        }
        else if (a_State == TeleportState.END)
        {
            _bTeleporting = false;
        }
    }
    #endregion
    //-----------------------------------------------------
    // START
    //-----------------------------------------------------
    #region Start
    void Awake()
    {
        if (!MAppManager.IsStartSceneInit())
        {
            MAppManager.LoadLevel("Start");
            return;
        }

    }
	// Use this for initialization
	void Start () 
	{
		s_oInstance = this;
		_appManager = GameObject.Find("_app_manager").GetComponent<MAppManager>();
        _languageManager = _appManager.gameObject.GetComponent<CLanguageManager>();
        GetObjects();
        SetLanguageTexts();
        _bIsPlayingVoice = false;

		fadeMaterial = Resources.Load ("Materials/Fade") as Material;


		GameObject goCamera = Camera.main.gameObject;

		if (goCamera.GetComponent<CCameraFade> () == null) 
		{
			Camera.main.gameObject.AddComponent<CCameraFade> ();
		}
		goCamera.GetComponent<CCameraFade> ().SetMaterial (ref fadeMaterial);

        _fFade = 1.0f;
		UpdateFadeMaterial (_fFade);

        DoorOpen.SetActive(false);
        DoorClosed.SetActive(true);

		DisableAllPanels ();

		//Start position of the camera
        FadeOut();
		_goCamera.transform.SetPositionAndRotation(_goals[0].transform.position, _goals[0].transform.rotation);

		_iCurrentGoal = 0;
		_iGoalsCount = _goals.Length;
        _bTeleporting = false;
        _enumTeleportState = TeleportState.END;
        _goals[_iCurrentGoal].GetComponent<CGoal>().PlaySound(0);


		_bIsClient = GameObject.Find( "Wifi").transform.GetChild(0).gameObject.activeInHierarchy;

		_ray = new Ray();
	}

    void SetLanguageTexts()
    {

    }

    void GetObjects()
    {
		_goCamera = GameObject.Find ("Camera");
		          //  GameObject.Find ("Camera").transform.GetChild(0).gameObject.transform.GetChild(0).forward
        _Voice = GameObject.Find("Voice").GetComponent<AudioSource>();
        _LoadingCircle = GameObject.Find("Camera").GetComponentInChildren<LoadingCircle>();
        DoorClosed = GameObject.Find("Door_Closed");
        DoorOpen = GameObject.Find("Door_Open");
        //Prefabs
        blue = (GameObject)Resources.Load("Prefabs/blue");
        orange = (GameObject)Resources.Load("Prefabs/orange");
        pink = (GameObject)Resources.Load("Prefabs/pink");
    }
    #endregion

    //-----------------------------------------------------
    // FUNCTIONS
    //-----------------------------------------------------
    #region Functions
    void Update( )
	{
        //For Teleports
        if(_bTeleporting)
        {
            if(_enumTeleportState == TeleportState.FADEIN)
            {
                if(_bFaded)
                {
                    Teleport(_iCurrentGoal,TeleportState.MOVING);
                }
            }
            else if (_enumTeleportState == TeleportState.FADEOUT)
            {
                if (_bFaded)
                {
                    Teleport(_iCurrentGoal, TeleportState.PLAYSOUND);
                }
            }
            else if (_enumTeleportState == TeleportState.PLAYSOUND)
            {
                if (!_bIsPlayingVoice)
                {
                    Teleport(_iCurrentGoal, TeleportState.SHOWCANVAS);
                }
            }
            else if (_enumTeleportState == TeleportState.END)
            {
                 Teleport(_iCurrentGoal, TeleportState.END);
            }
        }
        else
        {
			if ( CRaycastHelper.isThereADoubleClick()  || Input.GetKeyDown(KeyCode.N) )
            {
                NextGoal();
            }
        }

		if ( _bIsClient ) //Cardboard & Samsung Gear are considered as clients
		{
			if ( _goCamera != null )
			{
				//Ray from the Camera
				GameObject _goTrackingSpace = _goCamera.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
				_ray.origin = _goTrackingSpace.transform.position;
				_ray.direction = _goTrackingSpace.transform.forward;

				//What hits the Ray
				RaycastHit oHit;

				if (Physics.Raycast(_ray.origin, _ray.direction, out oHit, Mathf.Infinity, _iButtonLayer) )
				{
					if ( _LoadingCircle.IsSelected() /*|| CRaycastHelper.isThereASingleClick() */)
					{											
   	                 	if (oHit.collider.gameObject.tag == "button")
    	                {
        	                oHit.collider.gameObject.GetComponent<CButton>().OnClick();
           	             	Debug.Log("<color=yellow>" + oHit.collider.gameObject.name + "</color>");
                        	_LoadingCircle.ResetCursor();
                    	}
					}
					else
					{
						if (oHit.collider.gameObject.tag == "button" && !_goals[ _iCurrentGoal ].GetComponent<CGoal>().IsSelectionDone( ) )
                    	{
                        	_LoadingCircle.IncreaseTimer();
                    	}
						else if(oHit.collider.gameObject.tag == "panel_right" )
                    	{
                       	 	_LoadingCircle.ResetCursor();

							if(_bRightOpEnable && !IsPlayingAudio() )
                        	{
                                _Voice.Stop();
                            	
                            	CGoal oGoal = _goals[_iCurrentGoal].GetComponent<CGoal>();
								if (oGoal.ExistClip(2)  )
                            	{
									_bIsPlayingVoice = true;

                               	 	_bLeftOpEnable = true;
                                	_bRightOpEnable = false;
                                	oGoal.PlaySound(2);
                                	StartCoroutine(WaitForSoundFinished());
                            	}
                            	else
                            	{
                                	_bIsPlayingVoice = false;
                            	}
                        	}
                    	}
						else if (oHit.collider.gameObject.tag == "panel_left" )
                    	{
                        	_LoadingCircle.ResetCursor();

							if (_bLeftOpEnable && !IsPlayingAudio() )
    	                    {
                                _Voice.Stop();
        	                    
            	                CGoal oGoal = _goals[_iCurrentGoal].GetComponent<CGoal>();
								if (oGoal.ExistClip(1)   )
                    	        {
									_bIsPlayingVoice = true;

                        	        _bLeftOpEnable = false;
                               	 	_bRightOpEnable = true;
                            	    oGoal.PlaySound(1);
                                	StartCoroutine(WaitForSoundFinished());
                            	}
                            	else
                            	{
                               	 	_bIsPlayingVoice = false;
                            	}
                        	}
                    	}
					}
				}
				else
				{
					_LoadingCircle.ResetCursor();
					//ResetButtons();
				}
				///////////////
			}
			else
			{
				if ( GameObject.Find("Camera") != null )
				{
					_goCamera = GameObject.Find ("Camera");
				}
			}

		}

		if ( _bIsClient ) 
		{
			SendDataToServer ();
		}


	}

    public void ResponseBeforeNextGoal(int a_Response)
    {
        if(a_Response == 1)
        {
            _Voice.Stop();
            _bIsPlayingVoice = true;
            CGoal oGoal = _goals[_iCurrentGoal].GetComponent<CGoal>();

           // oGoal.ShowPanel(false);

            if (oGoal.ExistClip(3))
            {
                _bLeftOpEnable = false;
                _bRightOpEnable = true;
                oGoal.PlaySound(3);
                StartCoroutine(WaitForSoundFinishedToGoal());
            }
            else
            {
                _bIsPlayingVoice = false;
            }
        }
        else
        {
            _Voice.Stop();
            _bIsPlayingVoice = true;
            CGoal oGoal = _goals[_iCurrentGoal].GetComponent<CGoal>();

          //  oGoal.ShowPanel(false);

            if (oGoal.ExistClip(4))
            {
                _bLeftOpEnable = false;
                _bRightOpEnable = true;
                oGoal.PlaySound(4);
                StartCoroutine(WaitForSoundFinishedToGoal());
            }
            else
            {
                _bIsPlayingVoice = false;
            }
        }
    }

	public void SendDataToServer( )
	{
		// Next goal
		string strData = "NG/" + _iCurrentGoal;

		// Timer
		int iIsTimerIncreasing = _LoadingCircle.IsIncreasing();
		strData += "/LC/" + iIsTimerIncreasing;

		// Show panel
		strData += "/SP/" + _goals[ _iCurrentGoal ].GetComponent<CGoal>().IsShowingPanel();

		// Show score
		strData += "/SS/" + _iIsShowingScore;

		// Show end
		strData += "/SE/" + _iIsShowingEndPanel;

		//Social
		strData += "/S/" + _appManager.GetLienScore( );

		//Budget
		strData += "/B/" + _appManager.GetBudgetScore( );

		//Quality
		strData += "/Q/" + _appManager.GetQualiteScore( );

		//Precarity
		strData += "/P/" + _appManager.GetPrecarityScore( );

		//FineAtHome
		strData += "/F/" + _appManager.GetFineAtHomeScore( );

		//Disturbed Environnement
		strData += "/E/" + _appManager.GetDisturbedEnvironmentScore( );

		//Debug.Log( "strData " + strData );

		CStringToServer.Get ().SendStringToServer ( strData );
	}

	public void ReceiveDataFromClient( string a_strData )  // This function is only done by server
	{
		//Debug.Log( "received data " + a_strData );

		char[] charSlashSeparator = new char[] { '/' };
		string[] aStrings = a_strData.Split( charSlashSeparator, System.StringSplitOptions.None );			
		int iNumberOfStrings = aStrings.Length;

		string strCommand = "";
		int iValue = 0;

		for (int i = 0; i < iNumberOfStrings; i++)
		{

			strCommand = aStrings [i];
			iValue = int.Parse (aStrings [i+1]);
			i++;

			switch (strCommand) 
			{
			case "NG":
				{
					int iGoalFromClient = iValue;
					if (_iCurrentGoal < iGoalFromClient ) 
					{
						NextGoal ();
					}


				}
				break;

			case "LC": //Loading circle. Does not exist
				{
					if (iValue == 1) 
					{
						//_LoadingCircle.IncreaseTimer ();  
					} 
					else 
					{
						//_LoadingCircle.ResetCursor ();
					}
				}
				break;

			case "SS": //show score
				{
					if (iValue == 1) 
					{
						if (_iIsShowingScore == 0) 
						{
							ShowScore ();
						}
					} 
					else 
					{

					}
				}
				break;

			case "SP":
				{
					CGoal oGoal = _goals [_iCurrentGoal].GetComponent<CGoal> ();

					if (iValue == 1)
					{
						if (oGoal.IsShowingPanel () == 0) 
						{
							oGoal.ShowPanel (true);
						} 
					} 
					else 
					{
						if (oGoal.IsShowingPanel () == 1) 
						{
							oGoal.ShowPanel (false);
						}
					}

				}
				break;

			case "S":
				{
					bool bValueChanged = _appManager.SetLienScore ( iValue );

					if ( bValueChanged )
					{
						UpdateScore( );
					}

				}
				break;

			case "B":
				{
					bool bValueChanged = _appManager.SetBudgetScore ( iValue );

					if ( bValueChanged )
					{
						UpdateScore( );
					}
				}
				break;

			case "Q":
				{
					bool bValueChanged = _appManager.SetQualiteScore ( iValue );

					if ( bValueChanged )
					{
						UpdateScore( );
					}
				}
				break;

			case "P":
				{
					bool bValueChanged = _appManager.SetPrecarityScore ( iValue );

					if ( bValueChanged )
					{
						UpdateScore( );
					}
				}
				break;

			case "F":
				{
					bool bValueChanged = _appManager.SetFineAtHomeScore ( iValue );

					if ( bValueChanged )
					{
						UpdateScore( );
					}
				}
				break;

			case "E":
				{
					bool bValueChanged = _appManager.SetDisturbedEnvironmentScore (iValue);

					if ( bValueChanged )
					{
						UpdateScore( );
					}
				}
				break;
			}
		}

	}



	public void NextGoal( )
	{

        if (_enumTeleportState == TeleportState.END)
        {

            if (_iCurrentGoal == 3)
            {
                _recordPlayer.GetComponent<AudioSource>().Stop();
            }

			//Desactivate current goal
            _appManager.StopVoice();
			CGoal oGoal = _goals[ _iCurrentGoal ].GetComponent< CGoal >( );
			oGoal.Activate (false);
			oGoal.ShowPanel (false);

			//Teletransport to next goal
            _iCurrentGoal++;
            if (_iCurrentGoal == _iGoalsCount) 
            { 
				//Return to start if we have done all the goals
                _iCurrentGoal = 0;
				DisableAllPanels( );
            }

            if (_iCurrentGoal != 1)
            {
                DoorOpen.SetActive(false);
                DoorClosed.SetActive(true);
            }


            Teleport(_iCurrentGoal, TeleportState.START);


        }
	}

	public void GoToGoal( int a_iGoal )
	{
		CGoal oGoal = _goals [a_iGoal].GetComponent <CGoal> ();
		oGoal.Activate (true);

		_goCamera.transform.SetPositionAndRotation(_goals[a_iGoal].transform.position, _goals[a_iGoal].transform.rotation);
    }

	public void DisableAllPanels( )
	{
		for (int iGoal = 0; iGoal < _iGoalsCount; iGoal++) 
		{
			CGoal oGoal = _goals [iGoal].GetComponent<CGoal> ();
			oGoal.ShowPanel(false);
		}

	}


	void ShowEnd( )
	{
		// Select panel
		string strPanel = "none";

		int iLienScore = _appManager.GetLienScore( );
		int iBudgetScore = _appManager.GetBudgetScore( );
		int iQuality = _appManager.GetQualiteScore( );

		int iPrecarity = _appManager.GetPrecarityScore( );
		int iFineAtHome = _appManager.GetFineAtHomeScore( );
		int iDisturbedEnvironment = _appManager.GetDisturbedEnvironmentScore( );


		if  ( iPrecarity >= iFineAtHome && iPrecarity >= iDisturbedEnvironment )
		{
			strPanel = "EndPanel_SituationPrecaire";
		}
		else if ( iFineAtHome >= iPrecarity && iFineAtHome >= iDisturbedEnvironment )
		{
			strPanel = "EndPanel_BienChezSoi";
		}
		else if ( iDisturbedEnvironment >= iPrecarity && iDisturbedEnvironment >=  iFineAtHome )
		{
			strPanel = "EndPanel_EnvironmentPerturbe";
		}


		GameObject goEndPanel = _goals[ _iCurrentGoal ].transform.Find( strPanel ).gameObject;
        GameObject goScorePanel = _goals[_iCurrentGoal].transform.Find("panel_score").gameObject;
		goEndPanel.SetActive(true);
        goScorePanel.SetActive(true);


	}


	public void UpdateScore( )
	{
		_goLienSpace = _goals[_iCurrentGoal].transform.Find("OptionsPanel/panel_score/Lien_Space").gameObject;
		_goBudgetSpace = _goals[_iCurrentGoal].transform.Find("OptionsPanel/panel_score/Budget_Space").gameObject;
		_goQualiteSpace = _goals[_iCurrentGoal].transform.Find("OptionsPanel/panel_score/Qualite_Space").gameObject;

		// Social links
		int iChildCount = _goLienSpace.transform.childCount;
		for ( int iChild = 0; iChild < iChildCount; iChild++)
		{
			GameObject goChild = _goLienSpace.transform.GetChild( iChild ).gameObject;
			goChild.SetActive( false );		
		}

		// Budget
		iChildCount = _goBudgetSpace.transform.childCount;
		for ( int iChild = 0; iChild < iChildCount; iChild++)
		{
			GameObject goChild = _goBudgetSpace.transform.GetChild( iChild ).gameObject;
			goChild.SetActive( false );		
		}

		// Quality
		iChildCount = _goQualiteSpace.transform.childCount;
		for ( int iChild = 0; iChild < iChildCount; iChild++)
		{
			GameObject goChild = _goQualiteSpace.transform.GetChild( iChild ).gameObject;
			goChild.SetActive( false );		
		}

		// Refresh buttons
		ShowScore();
	}

    void ShowFinalScore()
    {
        _iIsShowingScore = 1;

        _goLienSpace = _goals[_iCurrentGoal].transform.Find("panel_score/Lien_Space").gameObject;
        _goBudgetSpace = _goals[_iCurrentGoal].transform.Find("panel_score/Budget_Space").gameObject;
        _goQualiteSpace = _goals[_iCurrentGoal].transform.Find("panel_score/Qualite_Space").gameObject;

        if (blue != null)
        {
            for (int i = 0; i < _appManager.GetLienScore(); i++)
            {
                _goClone = (GameObject)Instantiate(blue, new Vector3(_goLienSpace.transform.position.x, _goLienSpace.transform.position.y, _goLienSpace.transform.position.z), _goLienSpace.transform.rotation);
                _goClone.name = i.ToString() + "_lien";
                _goClone.transform.SetParent(_goLienSpace.transform);
                _goClone.SetActive(true);

                _goClone.transform.localPosition = new Vector3(0 + (i * 12.2f), 0, 0);
                _goClone.transform.localScale = Vector3.one;
            }
        }

        if (pink != null)
        {
            for (int i = 0; i < _appManager.GetBudgetScore(); i++)
            {
                _goClone = (GameObject)Instantiate(pink, new Vector3(_goBudgetSpace.transform.position.x, _goBudgetSpace.transform.position.y, _goBudgetSpace.transform.position.z), _goLienSpace.transform.rotation);
                _goClone.name = i.ToString() + "_budget";
                _goClone.transform.SetParent(_goBudgetSpace.transform);
                _goClone.SetActive(true);

                _goClone.transform.localPosition = new Vector3(0 + (i * 12.2f), 0, 0);
                _goClone.transform.localScale = Vector3.one;
            }
        }

        if (orange != null)
        {
            for (int i = 0; i < _appManager.GetQualiteScore(); i++)
            {
                _goClone = (GameObject)Instantiate(orange, new Vector3(_goQualiteSpace.transform.position.x, _goQualiteSpace.transform.position.y, _goQualiteSpace.transform.position.z), _goLienSpace.transform.rotation);
                _goClone.name = i.ToString() + "_qualite";
                _goClone.transform.SetParent(_goQualiteSpace.transform);
                _goClone.SetActive(true);

                _goClone.transform.localPosition = new Vector3(0 + (i * 12.2f), 0, 0);
                _goClone.transform.localScale = Vector3.one;
            }
        }
    }

    void ShowScore()
    {
		_iIsShowingScore = 1;

        _goLienSpace = _goals[_iCurrentGoal].transform.Find("OptionsPanel/panel_score/Lien_Space").gameObject;
        _goBudgetSpace = _goals[_iCurrentGoal].transform.Find("OptionsPanel/panel_score/Budget_Space").gameObject;
        _goQualiteSpace = _goals[_iCurrentGoal].transform.Find("OptionsPanel/panel_score/Qualite_Space").gameObject;
        
        if(blue != null)
        {
            for (int i = 0; i < _appManager.GetLienScore(); i++)
            {
                _goClone = (GameObject)Instantiate(blue, new Vector3(_goLienSpace.transform.position.x, _goLienSpace.transform.position.y, _goLienSpace.transform.position.z), _goLienSpace.transform.rotation);
                _goClone.name = i.ToString() + "_lien";
                _goClone.transform.SetParent(_goLienSpace.transform);
				_goClone.SetActive(true);

                _goClone.transform.localPosition = new Vector3(0 + (i * 12.2f),0,0);
                _goClone.transform.localScale = Vector3.one;
            }
        }

        if(pink != null)
        {
            for (int i = 0; i < _appManager.GetBudgetScore(); i++)
            {
                _goClone = (GameObject)Instantiate(pink, new Vector3(_goBudgetSpace.transform.position.x, _goBudgetSpace.transform.position.y, _goBudgetSpace.transform.position.z), _goLienSpace.transform.rotation);
                _goClone.name = i.ToString() + "_budget";
                _goClone.transform.SetParent(_goBudgetSpace.transform);
				_goClone.SetActive(true);

                _goClone.transform.localPosition = new Vector3(0 + (i * 12.2f), 0, 0);
                _goClone.transform.localScale = Vector3.one;
            }
        }

        if(orange != null)
        {
            for (int i = 0; i < _appManager.GetQualiteScore(); i++)
            {
                _goClone = (GameObject)Instantiate(orange, new Vector3(_goQualiteSpace.transform.position.x, _goQualiteSpace.transform.position.y, _goQualiteSpace.transform.position.z), _goLienSpace.transform.rotation);
                _goClone.name = i.ToString() + "_qualite";
                _goClone.transform.SetParent(_goQualiteSpace.transform);
				_goClone.SetActive(true);

                _goClone.transform.localPosition = new Vector3(0 + (i * 12.2f), 0, 0);
                _goClone.transform.localScale = Vector3.one;
            }
        }
    }


    #endregion
}
