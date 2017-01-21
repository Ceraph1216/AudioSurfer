using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour 
{
	private Transform _transform;
	private Enums.PlayerGroundState _groundedState;
	private float _currentVelocity;
	private float _previousY;

	// Use this for initialization
	void Awake () 
	{
		_transform = transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		CheckGrounded ();
		MovePlayer ();

		_previousY = WaveManager.instance.groundY;
	}

	private void CheckGrounded ()
	{
		float l_groundY = WaveManager.instance.groundY;

		// The player is below the ground
		if (_transform.position.y < l_groundY) 
		{
			if (_currentVelocity < 0) 
			{
				_currentVelocity = 0;
			}
			_currentVelocity += l_groundY - _transform.position.y;

			Vector3 l_newPosition = _transform.position;
			l_newPosition.y = l_groundY;
			_transform.position = l_newPosition;

			Land ();
			return;
		}

		if (Mathf.Abs( _previousY - l_groundY) <= Constants.GROUNDED_THRESHHOLD)
		{
			if (_currentVelocity > 0) 
			{
				Jump ();
				return;
			}
		}

		if (l_groundY < _previousY)  
		{
			Jump ();
			return;
		} 

		/*if (l_groundY == _previousY) 
		{
			if (_currentVelocity > 0) 
			{
				// Do jumping stuff
				if (_groundedState == Enums.PlayerGroundState.OnGround) 
				{
					_groundedState = Enums.PlayerGroundState.InAir;
				}
			}
		}*/

		/*if (Mathf.Abs (_transform.position.y - l_groundY) <= Constants.GROUNDED_THRESHHOLD) 
		{
			// Do landing stuff
			if (_groundedState == Enums.PlayerGroundState.InAir) 
			{
				_groundedState = Enums.PlayerGroundState.OnGround;
			}
		}
		else 
		{
			// Do jumping stuff
			if (_groundedState == Enums.PlayerGroundState.OnGround) 
			{
				_groundedState = Enums.PlayerGroundState.InAir;
			}
		}*/
	}

	private void MovePlayer ()
	{
		// Move the player by our velocity if we're in the air
		if (_groundedState != Enums.PlayerGroundState.InAir) 
		{
			return;
		}

		float l_groundY = WaveManager.instance.groundY;

		// Add gravity
		_currentVelocity -= Constants.GRAVITY * Time.deltaTime;

		// Make sure we haven't passed terminal velocity
		if (Mathf.Abs (_currentVelocity) > Constants.TERMINAL_VELOCITY) 
		{
			_currentVelocity = (Mathf.Sign (_currentVelocity) * Constants.TERMINAL_VELOCITY);
		}

		Vector3 l_newPosition = _transform.position;
		l_newPosition.y += _currentVelocity * Time.deltaTime;

		// predict ground collision and set us at ground level
		if (l_newPosition.y < l_groundY) 
		{
			l_newPosition.y = l_groundY;
		}

		_transform.position = l_newPosition;
	}

	private void Jump ()
	{
		// Do jumping stuff
		if (_groundedState == Enums.PlayerGroundState.OnGround) 
		{
			_groundedState = Enums.PlayerGroundState.InAir;
		}
	}

	private void Land ()
	{
		// Do landing stuff
		if (_groundedState == Enums.PlayerGroundState.InAir) 
		{
			_groundedState = Enums.PlayerGroundState.OnGround;
		}
	}
}
