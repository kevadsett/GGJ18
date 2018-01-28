using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElasticString : MonoBehaviour {
	[SerializeField] Transform defaultTarget;
	[SerializeField] float stretchOffset;

	void Update () {
		Vector3 targetPos;

		if (PaperThrower.GrabbedObject != null) {
			targetPos = PaperThrower.GrabbedObject.transform.position + Vector3.right * stretchOffset;
		} else {
			targetPos = defaultTarget.position;
		}

		Vector3 between = (targetPos - transform.position);
		transform.up = between;
		transform.localScale = new Vector3 (1f, between.magnitude * 0.5f, 1f);
	}
}
