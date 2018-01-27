using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipient : MonoBehaviour
{
	public ParticleSystem Particles;
	public Transform Heart;

	private Rigidbody _rb;
	void Start ()
	{
		_rb = GetComponent<Rigidbody>();
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "LetterBall")
		{
			Destroy(GetComponent<SideMoving>());
			_rb.constraints = new RigidbodyConstraints();
			Particles.transform.SetParent(Heart, false);
			Particles.transform.localPosition = Vector3.zero;
			Particles.Play();
			StartCoroutine(DestroySoon(collision.gameObject));
		}
	}

	private IEnumerator DestroySoon(GameObject obj)
	{
		yield return new WaitForSeconds(0.1f);
		Destroy(obj);
	}
}
