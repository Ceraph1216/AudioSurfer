using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour 
{
	public static ScoreManager instance;

	public int currentScore;
	public float currentComboScore;

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
