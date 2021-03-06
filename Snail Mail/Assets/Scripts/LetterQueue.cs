﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterQueue : MonoBehaviour {
	[SerializeField] GameDatabase gameDatabase;
	[SerializeField] Vector3 animFromRotation;
	[SerializeField] Vector3 animFromScale;
	[SerializeField] float animSpeed;
	[SerializeField] AnimationCurve rotationCurve;
	[SerializeField] AnimationCurve scaleCurve;
	[SerializeField] AnimationCurve scaleUpCurve;
	[SerializeField] Transform animPivot;
	[SerializeField] Transform scalePivot;
	[SerializeField] Transform scaleUpPivot;
	[SerializeField] TextMesh addresseeText;
	[SerializeField] TextMesh addressText;

	public static Addressee CurrentAddressee { get; private set; }

	public static System.Action LetterGrabbed { get; private set; }
	public static System.Action LetterLaunched { get; private set; }

	float timer = 1f;

	void Awake () {
		LetterGrabbed = OnGrabbed;
		LetterLaunched = OnLaunched;
		OnLaunched ();
	}

	void Update () {
		if (timer >= 1f) {
			return;
		}

		float rt = rotationCurve.Evaluate (timer);
		animPivot.localRotation = Quaternion.Slerp (Quaternion.Euler (animFromRotation), Quaternion.identity, rt);

		float st = scaleCurve.Evaluate (timer);
		scalePivot.localScale = Vector3.Lerp (animFromScale, Vector3.one, st);

		float ut = scaleUpCurve.Evaluate (timer);
		scaleUpPivot.localScale = Vector3.Lerp (Vector3.zero, animFromScale, ut);

		timer += Time.deltaTime * animSpeed;
	}

	void OnGrabbed () {
		scalePivot.gameObject.SetActive (false);
	}

	void OnLaunched () {
		scalePivot.gameObject.SetActive (true);

		Addressee selected = CurrentAddressee;
		while (selected == CurrentAddressee) {
			selected = gameDatabase.addressees [Random.Range (0, gameDatabase.addressees.Length)];
		}

		SetupNextAdressee (selected);
	}

	void SetupNextAdressee (Addressee addressee) {
		CurrentAddressee = addressee;

		addresseeText.text = addressee.Name;
		addressText.text = addressee.Address;

		animPivot.localRotation = Quaternion.Euler (animFromRotation);
		timer = 0f;
	}
}
