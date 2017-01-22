using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultsScreen : MonoBehaviour 
{
	public Text resultsText;
	private ConstantMovement[] _movements;
//	private 
	private PlayerMovement _player;

	void Awake ()
	{
		_movements = GameObject.FindObjectsOfType<ConstantMovement>();
		_player = GameObject.FindObjectOfType<PlayerMovement> ();
	}

	void OnEnable ()
	{
		if (ScoreManager.instance.currentScore > ScoreManager.instance.starThreshholds [2]) 
		{
			resultsText.text = "Congratulations! You win!";
		} 
		else 
		{
			resultsText.text = "Oh no! You lost!";
		}

		for (int i = 0; i < _movements.Length; i++) 
		{
			_movements [i].enabled = false;
		}

		_player.enabled = false;
	}

	public void OnPlayClick ()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene ("GameScreen");
	}

	public void OnTitleClick ()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene ("TitleScreen");
	}
}
