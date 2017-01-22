using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour 
{
	public void OnClick ()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene ("GameScreen");
	}

	void Update ()
	{
		if (Input.GetAxis ("Submit") > 0) 
		{
			OnClick ();
		}
	}
}
