using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideMoving : MonoBehaviour {
	public float Speed;
	public AnimationCurve Bob;

	private float _animOffset;
	private bool _goingRight;
	private float _startTime;
	private Camera _mainCamera;

	private Vector3 _viewportPoint;

	void Start()
	{
		_startTime = Time.time;
		_mainCamera = Camera.main;
		_goingRight = Random.value >= 0.5f;
		_animOffset = Random.value * 0.3f;
	}

	void Update ()
	{
		float newX = transform.position.x + (Speed * Time.deltaTime) * (_goingRight ? 1 : -1);
		float newY = Bob.Evaluate(Time.time - _startTime + _animOffset);

		_viewportPoint = _mainCamera.WorldToViewportPoint(new Vector3(newX, newY, transform.position.z));

		if (_goingRight && _viewportPoint.x >= 0.9f)
		{
			_goingRight = false;
		}

		if (!_goingRight && _viewportPoint.x <= 0.1f)
		{
			_goingRight = true;
		}

		transform.position = new Vector3(newX, newY, transform.position.z);
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(_mainCamera.ViewportToWorldPoint(_viewportPoint), 0.5f);
	}
}
