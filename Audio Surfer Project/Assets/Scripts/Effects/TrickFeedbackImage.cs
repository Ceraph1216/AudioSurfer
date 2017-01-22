using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TrickFeedbackImage : MonoBehaviour 
{
	public Sprite[] sprites;
	public Color[] colors;

	private Image _image;

	private float _currentWaitTime;
	private float _currentFadeTime;

	void Awake ()
	{
		_image = GetComponent<Image> ();
	}

	void OnEnable ()
	{
		int l_randomIndex = Random.Range (0, sprites.Length);
		_image.sprite = sprites [l_randomIndex];
		_image.SetNativeSize ();

		/*l_randomIndex = Random.Range (0, colors.Length);
		_image.color = colors [l_randomIndex];*/
		_image.color = Color.white;

		_currentWaitTime = Constants.EFFECT_FADE_WAIT;
		_currentFadeTime = Constants.EFFECT_FADE_TIME;
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
			Color l_newColor = _image.color;
			l_newColor.a = Mathf.Lerp (0, 1, _currentFadeTime / Constants.EFFECT_FADE_TIME);
			_image.color = l_newColor;
			return;
		}

		gameObject.SetActive (false);
	}
}
