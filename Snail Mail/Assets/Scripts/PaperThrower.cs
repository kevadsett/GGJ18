using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperThrower : MonoBehaviour
{
	public float MotionLimitMax = 0.01f;
	public float MotionLimitMin = 0.001f;
	public float ZLimit = -8f;

	public float TargetDistanceMax = 10;
	public float TargetDistanceMin = 1;

	private Camera _mainCamera;
	private GameObject _grabbedObject;
	private Vector3 _startScreenSpace;
	private Vector3 _offset;
	private float _currentMotionLimit;

	private Vector3 _targetPosition;

	void Start ()
	{
		_mainCamera = Camera.main;
		_currentMotionLimit = MotionLimitMax;
		Grabbable.ThingGrabbedEvent += OnThingGrabbed;
	}

	void OnDestroy()
	{
		Grabbable.ThingGrabbedEvent -= OnThingGrabbed;
	}
	
	void Update ()
	{
		if (_grabbedObject == null)
		{
			return;
		}

		if (Input.GetMouseButtonUp(0))
		{
			//TEMP - we'll want to see it drop later
			// also, if you let go too soon after grabbing, don't penalise
//			Destroy(_grabbedObject);
			_grabbedObject = null;
			return;
		}

		Vector3 currentScreenSpace = new Vector3(_startScreenSpace.x, _startScreenSpace.y, Input.mousePosition.y * _currentMotionLimit); 

		Vector3 curPosition = Camera.main.ScreenToWorldPoint(currentScreenSpace) + _offset;

		curPosition.z = Mathf.Clamp(curPosition.z, ZLimit, 0);

		Debug.Log(curPosition.z / ZLimit);
		var relativePosition = Mathf.Abs(curPosition.z / ZLimit);
		_currentMotionLimit = MotionLimitMin + ((1 - relativePosition) * (MotionLimitMax - MotionLimitMin));

		_grabbedObject.transform.position = new Vector3(_grabbedObject.transform.position.x, _grabbedObject.transform.position.y, curPosition.z);

		_targetPosition = new Vector3(0, 0, TargetDistanceMin + (relativePosition * (TargetDistanceMax - TargetDistanceMin)));
		Debug.Log(_targetPosition);
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawSphere(_targetPosition, 1);
	}

	private void OnThingGrabbed(Grabbable thing)
	{
		_grabbedObject = thing.gameObject;

		_startScreenSpace = _mainCamera.WorldToScreenPoint(_grabbedObject.transform.position);
		_offset = _grabbedObject.transform.position - _mainCamera.ScreenToWorldPoint(new Vector3(_startScreenSpace.x, _startScreenSpace.y, Input.mousePosition.y * _currentMotionLimit));
	}
}
