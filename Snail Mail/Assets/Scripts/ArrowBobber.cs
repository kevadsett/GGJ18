using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBobber : MonoBehaviour
{
	public AnimationCurve Bob;
	private float _startTime;
	private float _initialY;

	void Start ()
	{
		_initialY = transform.position.y;
		_startTime = Time.time;

		PaperThrower.OnLetterGrabbed += OnLetterGrabbed;
	}

	void OnDestroy()
	{
		PaperThrower.OnLetterGrabbed -= OnLetterGrabbed;
	}

	void OnLetterGrabbed ()
	{
		Destroy(gameObject);
	}

	void Update ()
	{
		var newY = Bob.Evaluate(Time.time - _startTime);
		transform.position = new Vector3(transform.position.x, _initialY + newY, transform.position.z);
	}
}
