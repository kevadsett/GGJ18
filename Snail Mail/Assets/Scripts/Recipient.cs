using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipient : MonoBehaviour
{
	[SerializeField] Transform Heart;
	[SerializeField] Addressee addresseeData;
	[SerializeField] float despawnTime;
	[SerializeField] float despawnAnimSpeed;

	private Rigidbody _rb;
	private bool hasBeenMurdererdWithMail = false;
	private bool readyToAnimateOut = false;
	private float animOutTimer;

	void Start ()
	{
		_rb = GetComponent<Rigidbody>();
	}

	void Update ()
	{
		if (readyToAnimateOut) {
			transform.localScale = Vector3.one * Mathf.Clamp01 (1f - animOutTimer);
			animOutTimer += Time.deltaTime * despawnAnimSpeed;
		}
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
				StartCoroutine(FallOverAndDie());

				launchable.Impact();
				hasBeenMurdererdWithMail = true;
			}
		}
	}

	private IEnumerator FallOverAndDie()
	{
		yield return new WaitForSeconds (despawnTime);

		ParticleSystem particles = RegularParticles.Instance;
		particles.transform.SetParent(Heart, false);
		particles.transform.localPosition = Vector3.zero;
		particles.Play();

		readyToAnimateOut = true;
	}

	private IEnumerator DestroySoon(GameObject obj)
	{
		yield return new WaitForSeconds(0.1f);
		Destroy(obj);
	}
}