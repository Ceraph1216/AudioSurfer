using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboEffect : MonoBehaviour 
{
	public Text comboText;
	
	// Update is called once per frame
	void Update () 
	{
		comboText.text = ((int)ScoreManager.instance.currentComboScore).ToString ();
	}
}
