using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour 
{
	private Transform _transform;
	private Enums.PlayerGroundState _groundedState;
	private float _currentVelocity;
	private float _previousY;

	private float _currentJumpforce;
	private int _currentJumpCount;

	// Use this for initialization
	void Awake () 
	{
		_transform = transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		CheckGrounded ();
		CheckPlayerInput ();
		MovePlayer ();
		CheckScore ();

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
			_currentVelocity += (l_groundY - _transform.position.y) * Constants.SLOPE_VELOCITY_INCREASE;

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
				Launch ();
				return;
			}
		}

		if (l_groundY < _previousY)  
		{
			Launch ();
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

	private void CheckPlayerInput ()
	{
		if (_groundedState == Enums.PlayerGroundState.OnGround) 
		{
			if (Input.GetAxis ("Jump") > 0) 
			{
				Jump ();
			}
		}
		else 
		{
			if (Input.GetAxis ("Jump") > 0) 
			{
				Jump ();
			}
		}
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
			Land ();
		}

		_transform.position = l_newPosition;
	}

	private void CheckScore ()
	{
		if (_groundedState != Enums.PlayerGroundState.InAir) 
		{
			return;
		}

		ScoreManager.instance.currentComboScore += Constants.AIR_SCORE_PER_SECOND * Time.deltaTime;
	}

	private void Launch ()
	{
		// Do jumping stuff
		if (_groundedState == Enums.PlayerGroundState.OnGround) 
		{
			_groundedState = Enums.PlayerGroundState.InAir;
		}
	}

	private void Jump ()
	{
		if (_currentJumpCount > Constants.AIR_JUMP_LIMIT) 
		{
			return;
		}

		if (_groundedState == Enums.PlayerGroundState.InAir && _currentVelocity > 0) 
		{
			return;
		}

		if (_currentVelocity < 0) 
		{
			_currentVelocity = 0;
		}

		_currentVelocity += _currentJumpforce;
		_currentJumpforce = _currentJumpforce / 2f;
		_currentJumpCount++;
		Launch ();
	}

	private void Land ()
	{
		// Do landing stuff
		if (_groundedState == Enums.PlayerGroundState.InAir) 
		{
			_groundedState = Enums.PlayerGroundState.OnGround;
			_currentJumpforce = Constants.JUMP_VELOCITY;
			_currentJumpCount = 0;
			ScoreManager.instance.CompleteCombo ();
		}
	}
}
