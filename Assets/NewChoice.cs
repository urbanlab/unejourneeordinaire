using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewChoice : MonoBehaviour {

	public ChoiceManager _choiceManager;

	void FireChoice()
	{
		_choiceManager.NewChoice ();
	}

	void porteevent()
	{
		_choiceManager.porte ();
	}
}
