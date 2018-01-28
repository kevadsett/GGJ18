using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Addressee : ScriptableObject {
	public string forename;
	public string surname;
	public string addressline1;
	public string addressline2;

	public string Address { get {
		return addressline1 + "\n" + addressline2;
	} }
}