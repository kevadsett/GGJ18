using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSpawner : MonoBehaviour
{
	public List<GameObject> GrassPrefabs;
	public int GrassCount;
	public float NearLimit = 5f;
	public float FarLimit = 100f;

	private Camera _mainCamera;
	private List<GameObject> _spawnedGrass = new List<GameObject>();

	void Start ()
	{
		_mainCamera = Camera.main;
		SpawnGrass();
	}

	void SpawnGrass()
	{
		_spawnedGrass.ForEach(Destroy);
		_spawnedGrass.Clear();
		while (_spawnedGrass.Count < GrassCount)
		{
			var selectedPrefab = GrassPrefabs[Random.Range(0, GrassPrefabs.Count)];
			var x = Random.value;
			var y = Random.value;
			var instance = Instantiate(selectedPrefab);
			Vector3 newPos = new Vector3(x, y, NearLimit + Random.value * (FarLimit - NearLimit));
			newPos = _mainCamera.ViewportToWorldPoint(newPos);
			newPos.y = 0;
			instance.transform.position = newPos;
			_spawnedGrass.Add(instance);
		}
	}
}
