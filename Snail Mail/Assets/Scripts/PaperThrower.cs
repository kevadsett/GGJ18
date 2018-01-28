using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperThrower : MonoBehaviour
{
	public delegate void LetterGrabbedAction();
	public static event LetterGrabbedAction OnLetterGrabbed;

	public GameObject BallPrefab;
	public float YLimitMin = -2f;
	public float YLimitMax = 2f;
	public float TargetDistanceMax = 10;
	public float TargetDistanceMin = 1;
	public float Spread = 3;
	public GameObject TargetObject;

	public float LoadViewRadius;
	public Vector3 LoadViewPoint;

	public float GrabViewRadius;
	public Vector3 GrabViewPoint;

	private enum State { NothingGrabbed, Loading, Aiming, SettingPower };
	private State _state;

	private Camera _mainCamera;
	private GameObject _grabbedObject;
	private Vector3 _initialMousePosition;

	private Vector3 _targetPosition;

	private static PaperThrower instance;

	public static GameObject LoadedObject { 
		get {
			if (instance != null && instance._state == State.Aiming) {
				return instance._grabbedObject;
			}
			return null;
		}
	}

	void Awake ()
	{
		instance = this;
	}

	void Start ()
	{
		_state = State.NothingGrabbed;
		_mainCamera = Camera.main;
	}
	
	void Update ()
	{
		switch (_state)
		{
		case State.NothingGrabbed:
			UpdateNothingGrabbed();
			break;
		case State.Loading:
			UpdateLoading();
			break;
		case State.Aiming:
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

		Vector3 vpos = LoadViewPoint;
		vpos.z = mvpos.z;

		if (Input.GetMouseButtonDown(0) && Vector3.Distance (mvpos, GrabViewPoint) < GrabViewRadius)
		{
			if (OnLetterGrabbed != null)
			{
				OnLetterGrabbed();
			}
			OnThingGrabbed (Instantiate(BallPrefab));
		}
	}

	private void UpdateLoading()
	{
		Vector3 mpos = HandController.ConstrainedMousePos ();
		Vector3 mvpos = _mainCamera.ScreenToViewportPoint (mpos);
		mvpos.x = mvpos.x * 2f - 1f;
		mvpos.y = mvpos.y * 2f - 1f;

		Vector3 vpos = LoadViewPoint;
		vpos.z = mvpos.z;

		if (Input.GetMouseButton(0) && Vector3.Distance (mvpos, LoadViewPoint) < LoadViewRadius)
		{
			OnShotLoaded();
		}

		Vector3 finalPosition = GetFinalPosition();

		_grabbedObject.transform.position = finalPosition;

		if (Input.GetMouseButtonUp(0))
		{
			Vector3 fallPos = finalPosition;
			fallPos.y = 0f;

			_grabbedObject.GetComponent<Launchable>().Launch(fallPos, 0f);
			_grabbedObject = null;
			_state = State.NothingGrabbed;
			TargetObject.SetActive(false);
			return;
		}
	}

	private void UpdatePositioning()
	{
		Vector3 finalPosition = GetFinalPosition();

		_grabbedObject.transform.position = finalPosition;

		var yStrength = (finalPosition.y - YLimitMin) / (YLimitMax - YLimitMin);

		_targetPosition = new Vector3(-finalPosition.x * Spread, 0, TargetDistanceMin + (yStrength * (TargetDistanceMax - TargetDistanceMin)));

		TargetObject.transform.position = _targetPosition;

		if (Input.GetMouseButtonUp(0))
		{
			// TODO if you let go too soon after grabbing, don't penalise
			_grabbedObject.GetComponent<Launchable>().Launch(_targetPosition, yStrength);
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
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(mpos.x, mpos.y, _initialMousePosition.z));
		curPosition.x *= 0.8f;
		curPosition.y *= 0.9f;
		curPosition.y += 0.2f;
		return curPosition;
	}

	private void OnThingGrabbed(GameObject gameObject)
	{
		TargetObject.SetActive(true);

		_grabbedObject = gameObject;

		Destroy(_grabbedObject.GetComponent<Grabbable>());

		_initialMousePosition = new Vector3 (LoadViewPoint.x * 0.5f + 0.5f * Screen.width,
			LoadViewPoint.y * 0.5f + 0.5f * Screen.height, 0f);

		_initialMousePosition.z = _mainCamera.WorldToScreenPoint(_grabbedObject.transform.position).z;

		_grabbedObject.transform.position = GetFinalPosition();

		LetterQueue.LetterGrabbed ();

		_state = State.Loading;
	}

	private void OnShotLoaded()
	{
		TargetObject.SetActive(true);

		_grabbedObject.transform.position = GetFinalPosition();

		_state = State.Aiming;
	}
}
