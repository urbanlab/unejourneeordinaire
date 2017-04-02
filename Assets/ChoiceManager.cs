using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceManager : MonoBehaviour {

	public GameObject _choiceCanvas;
	public UnityEngine.UI.Text _textLeft;
	public UnityEngine.UI.Text _textRight;
	public myPointDeVue _charlie;

	public Choice[] _choices;

	private int _currentChoice;

	public GvrAudioSource _audioSource;

	// Use this for initialization
	void Start () {
		_currentChoice = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void NewChoice()
	{
		PrintLog ("New choice");
		if (_currentChoice < _choices.Length) {
			Choice choice = _choices [_currentChoice];

			_audioSource.Stop ();
			_audioSource.clip = choice._amorce;
			_audioSource.Play ();
			StartCoroutine (WaitForSoundFinished());

		} else {
			PrintLog ("no more choices");
		}
	}

	IEnumerator WaitForSoundFinished()
	{
		while (_audioSource.isPlaying) {
			yield return new WaitForEndOfFrame ();
		}

		PrintLog ("Sound finished");

		Choice choice = _choices [_currentChoice];

		Vector3 lookAtPosition = _charlie.gameObject.transform.position;
		lookAtPosition.y = _choiceCanvas.transform.position.y;
		_choiceCanvas.transform.LookAt (lookAtPosition);

		_choiceCanvas.SetActive (true);
		_textLeft.text = choice._textLeft;
		_textRight.text = choice._textRight;
	}

	public void ChooseLeft()
	{
		_audioSource.Stop ();
		PrintLog ("Choose left");
		_choiceCanvas.SetActive (false);

		Choice choice = _choices [_currentChoice];
		choice._answeredLeft = true;
		if (choice._toActivate != null) {
			choice._toActivate.SetActive (true);
			PrintLog ("activate : " + choice._toActivate.name);
		} else {
			PrintLog ("to activate null");
		}
		if (choice._toUnactivate != null) {
			choice._toUnactivate.SetActive (false);
			PrintLog ("activate : " + choice._toUnactivate.name);
		} else
			PrintLog ("to unactivate null");


		PrintLog ("End choose left");

		_currentChoice++;

		if (_currentChoice < _choices.Length)
			_charlie.GotoNextPoint ();
//		else
//			Application.Quit ();
	}

	public void ChooseRight()
	{
		_audioSource.Stop ();
		PrintLog ("choose right");
		_choiceCanvas.SetActive (false);
		_currentChoice++;

		if (_currentChoice < _choices.Length)
			_charlie.GotoNextPoint ();
//		else
//			Application.Quit ();
	}
		

	public void HoverLeft()
	{
		PrintLog ("hover left");
		_audioSource.Stop ();
		_audioSource.clip = _choices [_currentChoice]._audioLeft;
		PrintLog("audio source : " + _audioSource.clip.name);
		_audioSource.Play ();
	}

	public void HoverRight()
	{
		PrintLog ("Hover right");
		_audioSource.Stop ();
		_audioSource.clip = _choices [_currentChoice]._audioRight;
		PrintLog("audio source : " + _audioSource.clip.name);
		_audioSource.Play ();
	}

	public void porte()
	{
		_charlie.portejs ();
	}


	void PrintLog(string log)
	{
		Debug.Log ("CHOICE_MANAGER " + log);
	}
}
