using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour 
{
	public static ScoreManager instance;

	public Text scoreText;
	public float currentComboScore;

	private int _currentScore;
	public int currentScore
	{
		get 
		{
			return _currentScore;
		}
		set 
		{
			_currentScore = value;
			scoreText.text = _currentScore.ToString ();
		}
	}

	public int[] starThreshholds;

	void Awake ()
	{
		instance = this;
	}

	public void CompleteCombo ()
	{
		currentScore += (int)currentComboScore;
		currentComboScore = 0;
	}
}
