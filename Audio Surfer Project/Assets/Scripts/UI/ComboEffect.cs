using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboEffect : MonoBehaviour 
{
	public Text comboText;
	public GameObject totalDisplay;
	public Text finishedComboText;

	private float _currentTimer;

	// Update is called once per frame
	void Update () 
	{
		if (ScoreManager.instance.currentComboScore <= 0) 
		{
			comboText.text = "";
		}
		else
		{
			comboText.text = ((int)ScoreManager.instance.currentComboScore) + "x " + ScoreManager.instance.currentScoreMod;
		}

		if (_currentTimer > 0) 
		{
			_currentTimer -= Time.deltaTime;
			return;
		}

		if (totalDisplay.activeSelf) 
		{
			totalDisplay.SetActive (false);
		}
	}

	public void ShowTotalCombo(float p_combo, float p_lengthMod, float p_varietyMod)
	{
		totalDisplay.SetActive (true);
		finishedComboText.text = p_combo + "\nLength Mod: " + p_lengthMod + "\nVariety Mod: " + p_varietyMod;

		_currentTimer = Constants.COMBO_FADE_TIMER;
	}
}
