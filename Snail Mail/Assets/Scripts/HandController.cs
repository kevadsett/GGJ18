using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour {
	[SerializeField] Camera cam;
	[SerializeField] Transform target;
	[SerializeField] Transform source;
	[SerializeField] GameObject open;
	[SerializeField] GameObject closed;
	[SerializeField] float sourceMoveMultiplier;
	[SerializeField] [Range (0f, 1f)] float yScreenSpaceLimit;

	static HandController instance;

	void Awake () {
		instance = this;
		Cursor.visible = false;
	}

	public static Vector3 ConstrainedMousePos () {
		if (instance != null) {
			return instance.GetConstrainedMousePosition ();
		}

		return Input.mousePosition;
	}

	Vector3 GetConstrainedMousePosition () {

		Vector3 mpos = Input.mousePosition;
		// mpos.x = Mathf.Clamp (mpos.x, 0f, Screen.width); <.<
		mpos.y = Mathf.Clamp (mpos.y, 0f, Screen.height * yScreenSpaceLimit);

		return mpos;
	}

	void Update () {
		Vector3 sourceViewPos = cam.WorldToViewportPoint (source.position);

		Vector3 mpos = GetConstrainedMousePosition ();
		mpos.z = sourceViewPos.z;

		Vector3 targetWorldPos = cam.ScreenToWorldPoint (mpos);

		source.position = new Vector3 (targetWorldPos.x * sourceMoveMultiplier, 0f, 0f);

		Vector3 handVec = (targetWorldPos - source.position).normalized;

		bool isClosed = Input.GetMouseButton (0);
		open.SetActive (!isClosed);
		closed.SetActive (isClosed);

		target.transform.position = targetWorldPos;
		target.transform.up = handVec;

		if (Input.GetKeyDown (KeyCode.Tab)) {
			Cursor.visible = !Cursor.visible;
		}
	}
}
