using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreText : MonoBehaviour {
	[SerializeField] float scale;
	[SerializeField] float speed;
	[SerializeField] AnimationCurve curve;

	float timer;

	static ScoreText instance;

	public static void Trigger () {
		if (instance != null) {
			instance.timer = 1f;
		}
	}

	void Awake () {
		instance = this;
	}

	void Update () {
		timer = Mathf.Clamp01 (timer - Time.deltaTime * speed);
		transform.localScale = Vector3.one * Mathf.Lerp (1f, scale, curve.Evaluate (timer));
	}
}
