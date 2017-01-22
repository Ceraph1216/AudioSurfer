using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingBG : MonoBehaviour 
{
	public Camera BGcamera;
	public float moveSpeed;
	public float width;

	private Transform _transform;

	void Awake ()
	{
		_transform = transform;
	}

	
	// Update is called once per frame
	void Update () 
	{
		Move ();
		CheckWrap ();
	}

	private void Move ()
	{
		Vector3 l_newPosition = _transform.localPosition;
		l_newPosition.x += moveSpeed * Time.deltaTime;
		_transform.localPosition = l_newPosition;
	}

	private void CheckWrap ()
	{
		Vector3 l_screenPosition = BGcamera.WorldToScreenPoint (_transform.position);
		if (l_screenPosition.x < 0) 
		{
			Vector3 l_newPosition = _transform.position;
			l_newPosition.x += width;
			_transform.position = l_newPosition;
		}
	}
}
