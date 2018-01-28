using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {
	[SerializeField] float collideBounceRadius;

	void OnCollisionEnter(Collision collision) {

		if (collision.gameObject.tag == "LetterBall")
		{
			var launchable = collision.gameObject.GetComponent<Launchable> ();

			if (launchable != null && launchable.HasImpacted == false) {
				launchable.Impact ();
				launchable.DropItLikeItsHot (collision.contacts[0].point - new Vector3 (0f, 0f, collideBounceRadius));
			}
		}
	}
}
