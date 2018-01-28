using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : MonoBehaviour {
	[SerializeField] float amount;
	[SerializeField] AnimationCurve curve;

	void FixedUpdate () {
		float n = amount * curve.Evaluate (CustomerSatisfaction.CurrentSatisfaction);

		transform.localPosition = new Vector3 (Random.Range (-n, n), Random.Range (-n, n), 0f);
	}
}
