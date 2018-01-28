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
	private List<string> _prefabNames = new List<string>();
	private Dictionary<string, GameObject> _namesToPrefab = new Dictionary<string, GameObject>();
	private Dictionary<string, int> _critterCounts = new Dictionary<string, int>();

	void Start ()
	{
		_mainCamera = Camera.main;
		Recipient.OnMessageReceived += OnMessageReceived;

		CritterPrefabs.ForEach(prefab =>
		{
			_prefabNames.Add(prefab.name);
			_namesToPrefab.Add(prefab.name, prefab);
			_critterCounts.Add(prefab.name, 0);
		});

		SpawnCritter();
	}

	void OnDestroy()
	{
		Recipient.OnMessageReceived -= OnMessageReceived;
	}

	void Update()
	{
		for (var i = 0; i < _prefabNames.Count; i++)
		{
			string prefabName = _prefabNames[i];
			int count;
			_critterCounts.TryGetValue(prefabName, out count);
			if (count == 0)
			{
				StartCoroutine(WaitAndSpawn(prefabName, 0.5f));
			}
		}

		if (_spawnedCritters.Count < MaxCritters && !_waitingToSpawn)
		{
			StartCoroutine(WaitAndSpawn());
		}
	}

	private IEnumerator WaitAndSpawn(string prefabName = null, float wait = -1)
	{
		_waitingToSpawn = true;
		var waitTime = wait < 0 ? Random.value * MaxWait : wait;

		if (string.IsNullOrEmpty(prefabName) == false)
		{
			_critterCounts[prefabName]++;
		}

		yield return new WaitForSeconds(waitTime);

		SpawnCritter(prefabName);
		_waitingToSpawn = false;
	}

	private void SpawnCritter(string prefabName = null)
	{
		GameObject selectedPrefab = null;
		if (string.IsNullOrEmpty(prefabName) == false)
		{
			selectedPrefab = _namesToPrefab[prefabName];
		}
		else
		{
			selectedPrefab = CritterPrefabs[Random.Range(0, CritterPrefabs.Count)];
			_critterCounts[selectedPrefab.name]++;
		}

		var selectedSpawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Count)];
		var critterObject = Instantiate(selectedPrefab);
		critterObject.name = selectedPrefab.name;
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
			_critterCounts[recipientGameObject.name]--;
			_spawnedCritters.Remove(recipientGameObject);
		}
	}
}
