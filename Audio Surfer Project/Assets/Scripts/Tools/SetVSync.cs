using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVSync : MonoBehaviour 
{
	public int vSyncCount;

	void OnEnable ()
	{
		QualitySettings.vSyncCount = 0;
	}
}
