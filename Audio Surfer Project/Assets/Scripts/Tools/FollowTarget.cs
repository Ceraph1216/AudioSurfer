using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour 
{
	public Transform target;
	public bool followX;
	public bool followY;
	public bool followZ;

	private Vector3 previousVector;
	private Vector3 deltaVector;

	private Transform myTransform;

	// Use this for initialization
	void Awake()
	{
		myTransform = transform;
	}

	void Start ()
	{
		Initialize();
	}

	void OnEnable()
	{
		Initialize();
	}

	void OnDisable()
	{
	}

	public void Initialize() 
	{
		if(target != null)
		{
			previousVector = target.position;
		}
	}

	// Update is called once per frame
	void Update ()
	{
		deltaVector = target.position - previousVector;
		if(!followX)
		{
			deltaVector.x = 0;	
		}

		if(!followY)
		{
			deltaVector.y = 0;	
		}

		if(!followZ)
		{
			deltaVector.z = 0;	
		}

		myTransform.Translate(deltaVector, Space.World);
		previousVector = target.position;
	}
}