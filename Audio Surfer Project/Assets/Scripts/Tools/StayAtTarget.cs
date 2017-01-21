using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayAtTarget : MonoBehaviour 
{
	public Transform target;

	private Transform _transform;

	void Awake ()
	{
		_transform = transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		_transform.position = target.position;
	}
}
