using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartOnCollision : MonoBehaviour
{
	public delegate void RestartGameAction();
	public static event RestartGameAction OnResetGame;

	void Start ()
	{
		CustomerSatisfaction.OnZeroSatisfaction += OnZeroSatisfaction;
		gameObject.SetActive(false);
	}

	void OnDestroy()
	{
		CustomerSatisfaction.OnZeroSatisfaction -= OnZeroSatisfaction;
	}

	void OnZeroSatisfaction ()
	{
		gameObject.SetActive(true);
	}

	void OnCollisionEnter(Collision collision) {

		if (collision.gameObject.tag == "LetterBall")
		{
			var launchable = collision.gameObject.GetComponent<Launchable> ();

			if (launchable != null && launchable.HasImpacted == false) {
				launchable.Impact ();
				Destroy(launchable.gameObject);
			}

			gameObject.SetActive(false);

			if (OnResetGame != null)
			{
				OnResetGame();
			}
		}
	}
}
