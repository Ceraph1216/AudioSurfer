using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultsScreen : MonoBehaviour 
{
	public Text resultsText;

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
	}

	public void OnPlayClick ()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene ("GameScreen");
	}
}
