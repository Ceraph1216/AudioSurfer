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

	private List<Trick> _comboTricks;
	private List <Enums.TrickType> _usedTricks;

	public int[] starThreshholds;

	void Awake ()
	{
		instance = this;
		_comboTricks = new List<Trick> ();
		_usedTricks = new List<Enums.TrickType> ();
	}

	public void CompleteCombo ()
	{
		float l_lengthMod = 1f + (_comboTricks.Count / 10f);
		float l_variationMod = 1f + _usedTricks.Count;

		currentComboScore = currentComboScore * l_lengthMod * l_variationMod;

		currentScore += (int)currentComboScore;
		currentComboScore = 0;
		_comboTricks = new List<Trick> ();
		_usedTricks = new List<Enums.TrickType> ();
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
}
