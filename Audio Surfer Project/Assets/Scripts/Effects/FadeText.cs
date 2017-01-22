using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FadeText : MonoBehaviour 
{
	public float waitTime;
	public float fadeTime;

	private float _currentWaitTime;
	private float _currentFadeTime;

	private Text _text;

	void Awake ()
	{
		_text = GetComponent<Text> (); 
	}

	void OnEnable ()
	{
		_currentWaitTime = waitTime;
		_currentFadeTime = fadeTime;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (_currentWaitTime > 0) 
		{
			_currentWaitTime -= Time.deltaTime;
			return;
		}

		if (_currentFadeTime > 0) 
		{
			_currentFadeTime -= Time.deltaTime;
			Color l_newColor = _text.color;
			l_newColor.a = Mathf.Lerp (0, 1, _currentFadeTime / Constants.EFFECT_FADE_TIME);
			_text.color = l_newColor;
			return;
		}

		gameObject.SetActive (false);
	}
}
