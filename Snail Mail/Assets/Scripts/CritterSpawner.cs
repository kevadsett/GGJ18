using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritterSpawner : MonoBehaviour
{
	public List<GameObject> CritterPrefabs;
	public List<Transform> SpawnPoints;
	public float MaxCritters;
	public float MaxWait;

	private Camera _mainCamera;
	private List<GameObject> _spawnedCritters = new List<GameObject>();
	private bool _waitingToSpawn;

	void Start ()
	{
		_mainCamera = Camera.main;
		Recipient.OnMessageReceived += OnMessageReceived;
		SpawnCritter();
	}

	void Update()
	{
		if (_spawnedCritters.Count < MaxCritters && !_waitingToSpawn)
		{
			StartCoroutine(WaitAndSpawn());
		}
	}

	private IEnumerator WaitAndSpawn()
	{
		_waitingToSpawn = true;
		var waitTime = Random.value * MaxWait;
		yield return new WaitForSeconds(waitTime);

		SpawnCritter();
		_waitingToSpawn = false;
	}

	private void SpawnCritter()
	{
		var selectedPrefab = CritterPrefabs[Random.Range(0, CritterPrefabs.Count)];
		var selectedSpawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Count)];
		var critterObject = Instantiate(selectedPrefab);
		critterObject.transform.position = new Vector3(
			selectedSpawnPoint.position.x,
			selectedSpawnPoint.position.y,
			selectedSpawnPoint.position.z + Random.value * 2
		);

		_spawnedCritters.Add(critterObject);
	}

	private void OnMessageReceived(Recipient recipient, bool isCorrectRecipient)
	{
		var recipientGameObject = recipient.gameObject;

		if (_spawnedCritters.Contains(recipientGameObject))
		{
			_spawnedCritters.Remove(recipientGameObject);
		}
	}
}
