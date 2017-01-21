using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour 
{
	public static ScoreManager instance;

	public Text scoreText;
	public float currentComboScore;
	public int[] starThreshholds;

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

	private ComboEffect _comboEffect;

	void Awake ()
	{
		instance = this;
		_comboTricks = new List<Trick> ();
		_usedTricks = new List<Enums.TrickType> ();
		_comboEffect = GameObject.FindObjectOfType<ComboEffect> ();
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


	}

	public void Wipeout ()
	{
		currentComboScore = 0;
		_comboTricks = new List<Trick> ();
		_usedTricks = new List<Enums.TrickType> ();
	}
}
