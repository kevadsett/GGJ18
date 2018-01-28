using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipient : MonoBehaviour
{
	[SerializeField] Transform Heart;
	[SerializeField] Addressee addresseeData;

	private Rigidbody _rb;
	void Start ()
	{
		_rb = GetComponent<Rigidbody>();
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "LetterBall")
		{
			var particles = LoveParticles.Instance;

			Destroy(GetComponent<SideMoving>());
			_rb.constraints = new RigidbodyConstraints();
			particles.transform.SetParent(Heart, false);
			particles.transform.localPosition = Vector3.zero;
			particles.Play();
			StartCoroutine(DestroySoon(collision.gameObject));
		}
	}

	private IEnumerator DestroySoon(GameObject obj)
	{
		yield return new WaitForSeconds(0.1f);
		Destroy(obj);
	}
}