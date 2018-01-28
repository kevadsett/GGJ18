using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (ParticleSystem))]
public class LoveParticles : MonoBehaviour {
	public static ParticleSystem Instance { get; private set; }

	private void Awake () {
		Instance = GetComponent<ParticleSystem> ();
	}
}