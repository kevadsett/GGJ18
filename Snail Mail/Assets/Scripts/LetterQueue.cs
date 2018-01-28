using System.Collections;
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

	float timer = 1f;

	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			SetupNextAdressee (gameDatabase.addressees[0]);
		}

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

	void SetupNextAdressee (Addressee addressee) {
		addresseeText.text = addressee.Name;
		addressText.text = addressee.Address;

		animPivot.localRotation = Quaternion.Euler (animFromRotation);
		timer = 0f;
	}
}
