using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchThing : MonoBehaviour {
	public AnimationCurve YTrajectory;
	public float MaxDuration = 1f;
	public float MinDuration = 0.3f;
	public float MaxHeight = 10f;
	public float MinHeight = 2f;

	private bool _inFlight = false;
	private Vector3 _target;
	private float _launchTime;
	private float _launchDuration;
	private float _launchHeight;

	void Update ()
	{
		if (_inFlight == false)
		{
			return;
		}
		float timeSinceLaunch = Time.time - _launchTime;

		if (timeSinceLaunch >= _launchDuration)
		{
			_inFlight = false;
			return;
		}

		float yPos = YTrajectory.Evaluate(timeSinceLaunch / _launchDuration) * _launchHeight;
		float zPos = timeSinceLaunch / _launchDuration * _target.z;

		transform.position = new Vector3(transform.position.x, yPos, zPos);
	}

	public void Launch(Vector3 target, float strength)
	{
		_target = target;
		_inFlight = true;
		_launchTime = Time.time;
		_launchDuration = MinDuration + (strength * (MaxDuration - MinDuration));
		_launchHeight = MinHeight + (strength * (MaxHeight - MinHeight));
	}
}
