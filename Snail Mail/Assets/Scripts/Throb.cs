using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throb : MonoBehaviour {
	[SerializeField] float scale;
	[SerializeField] float speed;

	float timer;

	void Update () {
		transform.localScale = Vector3.one * (1f + Mathf.Sin (timer) * scale);
		timer = Mathf.Repeat (timer + Time.deltaTime * speed, Mathf.PI * 2f);
	}
}