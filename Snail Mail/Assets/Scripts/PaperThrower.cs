using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperThrower : MonoBehaviour
{
	public GameObject BallPrefab;
	public float MotionLimitMax = 0.01f;
	public float MotionLimitMin = 0.001f;
	public float ZLimitMin = -2f;
	public float ZLimitMax = 2f;
	public float TargetDistanceMax = 10;
	public float TargetDistanceMin = 1;
	public float ArrowScaleMin = 4;
	public float ArrowScaleMax = 10;
	public float Spread = 3;
	public GameObject ArrowObject;
	public GameObject TargetObject;
	public float GrabViewRadius;
	public Vector3 GrabViewPoint;

	private enum State { NothingGrabbed, Positioning, SettingPower };
	private State _state;

	private Camera _mainCamera;
	private GameObject _grabbedObject;
	private Vector3 _initialMousePosition;
	private Vector3 _offset;
	private float _currentMotionLimit;
	private float _zStrength;

	private float _timePositioningStarted;

	private Vector3 _targetPosition;

	void Start ()
	{
		_state = State.NothingGrabbed;
		_mainCamera = Camera.main;
		_currentMotionLimit = MotionLimitMax;
	}
	
	void Update ()
	{
		switch (_state)
		{
		case State.NothingGrabbed:
			UpdateNothingGrabbed();
			break;
		case State.Positioning:
			UpdatePositioning();
			break;
		}
	}

	private void UpdateNothingGrabbed()
	{
		Vector3 mpos = HandController.ConstrainedMousePos ();
		Vector3 mvpos = _mainCamera.ScreenToViewportPoint (mpos);
		mvpos.x = mvpos.x * 2f - 1f;
		mvpos.y = mvpos.y * 2f - 1f;

		Vector3 vpos = GrabViewPoint;
		vpos.z = mvpos.z;

		Debug.Log (mvpos);

		if (Input.GetMouseButtonDown(0) && Vector3.Distance (mvpos, GrabViewPoint) < GrabViewRadius)
		{
			Debug.Log ("GRAB!!! " + mvpos);

			OnThingGrabbed(Instantiate(BallPrefab));
		}
	}

	private void UpdatePositioning()
	{
		Vector3 finalPosition = GetFinalPosition();

		_grabbedObject.transform.position = finalPosition;

		_zStrength = Mathf.Abs((ZLimitMin + finalPosition.z) / (ZLimitMax - ZLimitMin));

		_targetPosition = new Vector3(-finalPosition.x * Spread, 0, TargetDistanceMin + (_zStrength * (TargetDistanceMax - TargetDistanceMin)));

		Vector3 relativePos = _targetPosition - _grabbedObject.transform.position;
		Quaternion rotation = Quaternion.LookRotation(relativePos);
		ArrowObject.transform.rotation = rotation;
		ArrowObject.transform.localScale = new Vector3(ArrowObject.transform.localScale.x, ArrowObject.transform.localScale.y, ArrowScaleMin + (_zStrength * (ArrowScaleMax - ArrowScaleMin)));

		TargetObject.transform.position = _targetPosition;

		if (Input.GetMouseButtonUp(0))
		{
			// TODO if you let go too soon after grabbing, don't penalise
			_grabbedObject.GetComponent<Launchable>().Launch(_targetPosition, _zStrength);
			_grabbedObject = null;
			_state = State.NothingGrabbed;
			TargetObject.SetActive(false);
			return;
		}
	}

	void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			Gizmos.DrawSphere(_targetPosition, 1);
		}
	}

	private Vector3 GetFinalPosition()
	{
		Vector3 mpos = HandController.ConstrainedMousePos ();

		var yDiff = _initialMousePosition.y - mpos.y;
		Vector3 currentScreenSpace = new Vector3(mpos.x, _initialMousePosition.y - yDiff, _initialMousePosition.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(currentScreenSpace) + _offset;
		Vector3 finalPosition = new Vector3(curPosition.x, _grabbedObject.transform.position.y, curPosition.y);
		finalPosition.z = Mathf.Clamp(finalPosition.z, ZLimitMin, ZLimitMax);
		return finalPosition;
	}

	private void OnThingGrabbed(GameObject gameObject)
	{
		TargetObject.SetActive(true);

		_grabbedObject = gameObject;

		Destroy(_grabbedObject.GetComponent<Grabbable>());

		_initialMousePosition = HandController.ConstrainedMousePos ();

		_initialMousePosition.z = _mainCamera.WorldToScreenPoint(_grabbedObject.transform.position).z;

		_offset = _grabbedObject.transform.position - _mainCamera.ScreenToWorldPoint(_initialMousePosition);

		_grabbedObject.transform.position = GetFinalPosition();

		_state = State.Positioning;
	}
}
