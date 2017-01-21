using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantMovement : MonoBehaviour 
{
	public Vector3 moveSpeed;

	private Transform _transform;

	void Awake ()
	{
		_transform = transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		_transform.Translate (moveSpeed * Time.deltaTime);
	}
}
