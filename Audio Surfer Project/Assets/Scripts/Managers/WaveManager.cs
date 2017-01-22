using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour 
{
	public static WaveManager instance;

	public AudioSource music;
	public GameObject resultsScreen;
	public LayerMask floorMask;

	private Transform _transform;
	private ConstantMovement _movement;


	private float _groundY;
	public float groundY
	{
		get 
		{
			return _groundY;
		}
		private set
		{
			_groundY = value;
		}
	}

	private float _currentWaitTimer;
	private bool _gameEnded;

	void Awake ()
	{
		_transform = transform;
		_movement = GameObject.FindObjectOfType<ConstantMovement> ();
		_currentWaitTimer = Constants.END_WAIT_TIME;
		instance = this;
	}
	
	// Update is called once per frame
	void Update () 
	{
		DrawRays ();
		UpdateEnd ();
	}

	private void DrawRays()
	{
		// Set the direction and distance of the ground check
		Vector2 l_downDirection = (_transform.up * -1f);
		float l_distanceGround = 15f;

		// Create a raycast for the ground check
		RaycastHit2D l_hitGround = Physics2D.Raycast(_transform.position, l_downDirection, l_distanceGround, floorMask);
		// The right sensor hit something
		if (l_hitGround != null && l_hitGround.collider != null) 
		{
			groundY = l_hitGround.point.y;
		}
	}

	private void UpdateEnd()
	{
		if (music.isPlaying) 
		{
			return;
		}

		if (_currentWaitTimer > 0) 
		{
			_currentWaitTimer -= Time.deltaTime;
			return;
		}

		if (_gameEnded) 
		{
			return;
		}

		_movement.enabled = false;
		resultsScreen.SetActive (true);
		_gameEnded = true;
	}
}
