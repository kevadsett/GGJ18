using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipient : MonoBehaviour
{
	[SerializeField] Transform Heart;
	[SerializeField] Addressee addresseeData;

	private Rigidbody _rb;
	private bool hasBeenMurdererdWithMail = false;

	void Start ()
	{
		_rb = GetComponent<Rigidbody>();
	}

	void OnCollisionEnter(Collision collision)
	{
		if (hasBeenMurdererdWithMail) {
			return;
		}

		if (collision.gameObject.tag == "LetterBall")
		{
			var launchable = collision.gameObject.GetComponent<Launchable> ();

			if (launchable != null && launchable.HasImpacted == false) {
				ParticleSystem particles;
				if (launchable.MyAddressse == addresseeData) {
					particles = LoveParticles.Instance;
				} else {
					particles = AngryParticles.Instance;
				}

				particles.transform.SetParent(Heart, false);
				particles.transform.localPosition = Vector3.zero;
				particles.Play();

				Destroy(GetComponent<SideMoving>());
				_rb.constraints = new RigidbodyConstraints();

				StartCoroutine(DestroySoon(collision.gameObject));

				launchable.Impact();
				hasBeenMurdererdWithMail = true;
			}
		}
	}

	private IEnumerator DestroySoon(GameObject obj)
	{
		yield return new WaitForSeconds(0.1f);
		Destroy(obj);
	}
}