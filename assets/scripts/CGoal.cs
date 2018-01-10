using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGoal : MonoBehaviour
{
	bool _bGoalActived = false;
    private MAppManager _appManager;
    private int _iCurrentClip;
	private bool _bIsSelectionDone = false;

	private int _iIsShowingPanel = 0;

	[SerializeField]
	private AudioClip[] _audioClips;

	int iAudioSourcesCount;

    public bool GetGoalActive()
    {
        return _bGoalActived;
    }

	public void SelectionDone( bool a_bValue )
	{
		_bIsSelectionDone = a_bValue;
	}
	public bool IsSelectionDone( )
	{
		return _bIsSelectionDone;
	}



    public bool ExistClip()
    {
        if (_audioClips.Length > 0 && _bGoalActived)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool ExistClip(int a_Clip)
    {
        if (_audioClips.Length > a_Clip && _bGoalActived)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
	// Use this for initialization
	void Start () 
	{
        _appManager = MAppManager.Get();
		ShowPanel (false);
	}
		
	// Update is called once per frame
	void Update () 
	{
        if (_bGoalActived && Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("show panel");
            ShowPanel(true);
        }
	}

	public void Activate( bool a_bValue )
	{
		_bGoalActived = a_bValue;

		//Play 1 clip (the introduction)
        //if (_audioClips.Length > 0 && _bGoalActived) 
        //{
        //    _appManager.PlayVoice(_audioClips[0]);
        //}
						
	}

    public void PlaySound(int a_Sound)
    {
        _appManager.PlayVoice(_audioClips[a_Sound]);
    }

	public void ShowPanel( bool a_bPanelActivated )
	{
		//Activate/Desactivate children. Don't destroy parent, since we need its transform for teletransportation
		int iChildrenCount = transform.childCount;
		for (int iChild = 0; iChild < iChildrenCount; iChild++) 
		{
			transform.GetChild( iChild ).gameObject.SetActive( a_bPanelActivated );
		}

		if (a_bPanelActivated) 
		{
			_iIsShowingPanel = 1;
		}
		else 
		{
			_iIsShowingPanel = 0;
		}

	}

	public int IsShowingPanel( )
	{
		return _iIsShowingPanel;
	}

}
