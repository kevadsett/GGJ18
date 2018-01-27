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
		if (Input.GetMouseButtonDown(0))
		{
			OnThingGrabbed(Instantiate(BallPrefab));
		}
	}

	private void UpdatePositioning()
	{
		Vector3 finalPosition = GetFinalPosition();

		_grabbedObject.transform.position = finalPosition;

		_zStrength = Mathf.Abs((ZLimitMin + finalPosition.z) / (ZLimitMax - ZLimitMin));
		Debug.Log(_zStrength);

		_targetPosition = new Vector3(-finalPosition.x * 2, 0, TargetDistanceMin + (_zStrength * (TargetDistanceMax - TargetDistanceMin)));

		if (Input.GetMouseButtonUp(0))
		{
			// TODO if you let go too soon after grabbing, don't penalise
			_grabbedObject.GetComponent<Launchable>().Launch(_targetPosition, _zStrength);
			_grabbedObject = null;
			_state = State.NothingGrabbed;
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
		var yDiff = _initialMousePosition.y - Input.mousePosition.y;
		Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, _initialMousePosition.y - yDiff, _initialMousePosition.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(currentScreenSpace) + _offset;
		Vector3 finalPosition = new Vector3(curPosition.x, _grabbedObject.transform.position.y, curPosition.y);
		finalPosition.z = Mathf.Clamp(finalPosition.z, ZLimitMin, ZLimitMax);
		return finalPosition;
	}

	private void OnThingGrabbed(GameObject gameObject)
	{
		_grabbedObject = gameObject;

		Destroy(_grabbedObject.GetComponent<Grabbable>());

		_initialMousePosition = Input.mousePosition;

		_initialMousePosition.z = _mainCamera.WorldToScreenPoint(_grabbedObject.transform.position).z;

		_offset = _grabbedObject.transform.position - _mainCamera.ScreenToWorldPoint(_initialMousePosition);

		_grabbedObject.transform.position = GetFinalPosition();

		_state = State.Positioning;
	}
}
