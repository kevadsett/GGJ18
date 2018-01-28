using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipient : MonoBehaviour
{
	[SerializeField] Transform Heart;
	[SerializeField] Addressee addresseeData;
	[SerializeField] float despawnTime;
	[SerializeField] float despawnAnimSpeed;


	public delegate void MessageReceivedAction(Recipient recipient, bool isCorrectRecipient);
	public static event  MessageReceivedAction OnMessageReceived;

	private Rigidbody _rb;
	private bool hasBeenMurdererdWithMail = false;
	private bool readyToAnimateOut = false;
	private float animOutTimer;

	void Start ()
	{
		_rb = GetComponent<Rigidbody>();
		CustomerSatisfaction.OnZeroSatisfaction += OnZeroSatisfaction;
	}

	void OnDestroy()
	{
		CustomerSatisfaction.OnZeroSatisfaction -= OnZeroSatisfaction;
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
				React(launchable.MyAddressse == addresseeData);
				StartCoroutine(DestroySoon(collision.gameObject));
				launchable.Impact();
			}
		}
	}

	private void React(bool happy)
	{
		ParticleSystem particles;

		particles = happy ? LoveParticles.Instance : AngryParticles.Instance;

		if (OnMessageReceived != null)
		{
			OnMessageReceived(this, happy);
		}

		particles.transform.SetParent(Heart, false);
		particles.transform.localPosition = Vector3.zero;
		particles.Play();

		Destroy(GetComponent<SideMoving>());
		_rb.constraints = new RigidbodyConstraints();

		StartCoroutine(FallOverAndDie());

		hasBeenMurdererdWithMail = true;
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

	void OnZeroSatisfaction ()
	{
		var randomOffset = new Vector3((Random.value * 2) - 1, -0.1f, (Random.value * 2) - 1) * 0.5f;
		_rb.AddExplosionForce(500, transform.position + randomOffset, 2.5f);
		React(false);
	}
}