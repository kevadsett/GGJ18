using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipient : MonoBehaviour
{
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
		}
	}
}
