using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launchable : MonoBehaviour {
	public AnimationCurve YTrajectory;
	public float MaxDuration = 1f;
	public float MinDuration = 0.3f;
	public float MaxHeight = 10f;
	public float MinHeight = 2f;

	private bool _inFlight = false;
	private Vector3 _startPos;
	private Vector3 _target;
	private float _launchTime;
	private float _launchDuration;
	private float _launchHeight;

	public Addressee MyAddressse { get; private set; }
	public bool HasImpacted { get; private set; }

	void Update ()
	{
		if (_inFlight == false)
		{
			return;
		}
		float timeSinceLaunch = Time.time - _launchTime;
		float progress = timeSinceLaunch / _launchDuration;

		if (progress >= 1)
		{
			if (HasImpacted == false) {
				FailToImpact();
			}

			_inFlight = false;
			transform.position = _target;
			Destroy(GetComponent<CapsuleCollider>());
			return;
		}

		float normalisedHeight = YTrajectory.Evaluate(progress);

		float xPos = _startPos.x + (progress * (_target.x - _startPos.x));

		float currentLowY = _startPos.y + (progress * (_target.y - _startPos.y));

		float yPos = currentLowY + (normalisedHeight * (_launchHeight - currentLowY));
		float zPos = _startPos.z + (progress * (_target.z - _startPos.z));

		transform.position = new Vector3(xPos, yPos, zPos);
	}

	void FailToImpact()
	{

	}

	public void Impact()
	{
		HasImpacted = true;
	}

	public void DropItLikeItsHot(Vector3 colPoint)
	{
		_startPos.x = _target.x = colPoint.x;
		_startPos.z = _target.z = colPoint.z;
	}

	public void Launch(Vector3 target, float strength)
	{
		MyAddressse = LetterQueue.CurrentAddressee;
		LetterQueue.LetterLaunched ();

		_startPos = transform.position;
		_target = target;
		_inFlight = true;
		_launchTime = Time.time;
		_launchDuration = MinDuration + (strength * (MaxDuration - MinDuration));
		_launchHeight = MinHeight + (strength * (MaxHeight - MinHeight));
	}
}
