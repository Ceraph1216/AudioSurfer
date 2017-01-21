using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickManager : MonoBehaviour 
{
	public static TrickManager instance;

	public Trick[] tricks;

	private Dictionary<Enums.TrickType, Trick> _trickDictionary;

	void Awake ()
	{
		instance = this;
		_trickDictionary = new Dictionary<Enums.TrickType, Trick> ();
	}

	void OnEnable()
	{
		for (int i = 0; i < tricks.Length; i++) 
		{
			Trick l_currentTrick = tricks [i];
			_trickDictionary.Add (l_currentTrick.trickType, l_currentTrick);
		}
	}

	public Trick GetTrick (Enums.TrickType p_trickType)
	{
		return _trickDictionary [p_trickType];
	}
}
