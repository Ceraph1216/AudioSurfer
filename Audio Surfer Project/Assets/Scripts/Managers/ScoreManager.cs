﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour 
{
	public static ScoreManager instance;

	public Text scoreText;
	public float currentComboScore;
	public GameObject wipeoutImage;
	public GameObject comboImage;
	public int[] starThreshholds;
	public Image[] starFills;

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

			if (_updateStars) 
			{
				float l_starDiff = (float)_currentStarGoal - (float)_previousStarGoal;
				_currentStar.fillAmount = (float)(_currentScore - _previousStarGoal) / l_starDiff;

				if (_currentScore >= _currentStarGoal) 
				{
					GetStar ();
				}
			}
		}
	}

	public float currentScoreMod
	{
		get 
		{
			float l_lengthMod = 1 + (Mathf.Max((_comboTricks.Count -1) * Constants.LENGTH_MODIFIER, 0));
			float l_variationMod = 1 + (Mathf.Max((_usedTricks.Count -1) * Constants.VARIATION_MODIFIER, 0));

			return l_lengthMod + l_variationMod;
		}
	}

	private List<Trick> _comboTricks;
	private List <Enums.TrickType> _usedTricks;
	private int _currentStarGoal;
	private int _previousStarGoal;
	private Image _currentStar;
	private int _currentStars;
	private bool _updateStars;

	private ComboEffect _comboEffect;

	void Awake ()
	{
		instance = this;
		_comboTricks = new List<Trick> ();
		_usedTricks = new List<Enums.TrickType> ();
		_comboEffect = GameObject.FindObjectOfType<ComboEffect> ();
		_currentStarGoal = starThreshholds [0];
		_currentStar = starFills [0];
		_updateStars = true;
	}

	public void AddTrick (Trick p_trick)
	{
		_comboTricks.Add (p_trick);
		if (!_usedTricks.Contains (p_trick.trickType)) 
		{
			_usedTricks.Add (p_trick.trickType);
		}
		currentComboScore += p_trick.score;
	}

	public void CompleteCombo ()
	{
		if (currentComboScore <= 0) 
		{
			return;
		}

		float l_lengthMod = 1 + (Mathf.Max((_comboTricks.Count -1) * Constants.LENGTH_MODIFIER, 0));
		float l_variationMod = 1 + (Mathf.Max((_usedTricks.Count -1) * Constants.VARIATION_MODIFIER, 0));

		currentComboScore = currentComboScore * (l_lengthMod + l_variationMod);
		_comboEffect.ShowTotalCombo (currentComboScore, l_lengthMod, l_variationMod);

		currentScore += (int)currentComboScore;
		currentComboScore = 0;
		_comboTricks = new List<Trick> ();
		_usedTricks = new List<Enums.TrickType> ();

		comboImage.SetActive (true);
	}

	public void Wipeout ()
	{
		currentComboScore = 0;
		_comboTricks = new List<Trick> ();
		_usedTricks = new List<Enums.TrickType> ();

		wipeoutImage.SetActive (true);
	}

	private void GetStar ()
	{
		if (_currentStars < starThreshholds.Length -1) 
		{
			_currentStars++;
			_currentStar = starFills [_currentStars];
			_previousStarGoal = _currentStarGoal;
			_currentStarGoal = starThreshholds [_currentStars];
		} 
		else 
		{
			_updateStars = false;
		}
	}
}
