using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour 
{
	public ParticleSystem wipeoutParticles;

	private Enums.PlayerGroundState _groundedState;
	private float _currentVelocity;
	private float _previousY;

	private float _currentJumpforce;
	private int _currentJumpCount;
	private float _currentTrickCooldown;
	private float _currentWipeoutTimer;

	private Transform _transform;
	private Animator _animator;

	// Use this for initialization
	void Awake () 
	{
		_transform = transform;
		_animator = GetComponentInChildren<Animator> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		CheckGrounded ();
		CheckPlayerInput ();
		MovePlayer ();
//		CheckScore ();
		UpdateTrick ();

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
		if (_currentWipeoutTimer > 0) 
		{
			_currentWipeoutTimer -= Time.deltaTime;
			return;
		}

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

			if (Input.GetAxis ("Trick1") > 0) 
			{
				DoTrick (Enums.TrickType.Trick1);
			}

			if (Input.GetAxis ("Trick2") > 0) 
			{
				DoTrick (Enums.TrickType.Trick2);
			}

			if (Input.GetAxis ("Trick3") > 0) 
			{
				DoTrick (Enums.TrickType.Trick3);
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

	private void UpdateTrick ()
	{
		if (_currentTrickCooldown > 0) 
		{
			_currentTrickCooldown -= Time.deltaTime;
			return;
		}
	}

	private void Launch ()
	{
		// Do jumping stuff
		if (_groundedState == Enums.PlayerGroundState.OnGround) 
		{
			_groundedState = Enums.PlayerGroundState.InAir;
			_animator.SetBool ("Grounded", false);
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
		_currentJumpforce = _currentJumpforce / 1.3f;
		_currentJumpCount++;
		Launch ();
	}

	private void DoTrick (Enums.TrickType p_trickType)
	{
		if (_currentTrickCooldown > 0) 
		{
			return;
		}

		_animator.ResetTrigger ("Wipeout");
		Trick l_currentTrick = TrickManager.instance.GetTrick (p_trickType);

		_animator.SetTrigger (p_trickType.ToString ());

		//TODO: play animation

		ScoreManager.instance.AddTrick (l_currentTrick);
		_currentTrickCooldown = l_currentTrick.cooldown;

		Jump ();
	}

	private void Land ()
	{
		// Do landing stuff
		if (_groundedState == Enums.PlayerGroundState.InAir) 
		{
			_groundedState = Enums.PlayerGroundState.OnGround;
			_currentJumpforce = Constants.JUMP_VELOCITY;
			_currentJumpCount = 0;
			if (_currentTrickCooldown > 0) 
			{
				Wipeout ();
			} 
			else 
			{
				ScoreManager.instance.CompleteCombo ();
			}
			_animator.SetBool ("Grounded", true);
		}
	}

	private void Wipeout ()
	{
		if (_currentWipeoutTimer > 0) 
		{
			return;
		}
			
		ScoreManager.instance.Wipeout ();
		_currentWipeoutTimer = Constants.WIPEOUT_TIMER;
		_animator.SetTrigger ("Wipeout");
		wipeoutParticles.Emit (40);
	}
}
