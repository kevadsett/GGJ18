using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritterSpawner : MonoBehaviour
{
	public List<GameObject> CritterPrefabs;
	public List<Transform> SpawnPoints;
	public float MaxCritters;
	public float MaxWait;
	public float MinSpeed;
	public float MaxSpeed;


	private List<GameObject> _spawnedCritters = new List<GameObject>();
	private bool _waitingToSpawn;
	private List<string> _prefabNames = new List<string>();
	private Dictionary<string, GameObject> _namesToPrefab = new Dictionary<string, GameObject>();
	private Dictionary<string, int> _critterCounts = new Dictionary<string, int>();

	private bool _isGameInProgress = true;

	private readonly List<IEnumerator> _currentCoroutines = new List<IEnumerator>();

	void Start ()
	{
		Recipient.OnMessageReceived += OnMessageReceived;
		CustomerSatisfaction.OnZeroSatisfaction += OnZeroSatisfaction;
		RestartOnCollision.OnResetGame += OnResetGame;

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
		CustomerSatisfaction.OnZeroSatisfaction -= OnZeroSatisfaction;
		RestartOnCollision.OnResetGame -= OnResetGame;
	}

	void Update()
	{
		if (_isGameInProgress == false)
		{
			return;
		}

		for (var i = 0; i < _prefabNames.Count; i++)
		{
			string prefabName = _prefabNames[i];
			int count;
			_critterCounts.TryGetValue(prefabName, out count);
			if (count == 0)
			{
				var coroutine = WaitAndSpawn(prefabName, 0.5f);
				_currentCoroutines.Add(coroutine);
				StartCoroutine(coroutine);
			}
		}

		if (_spawnedCritters.Count < MaxCritters && !_waitingToSpawn)
		{
			var coroutine = WaitAndSpawn();
			_currentCoroutines.Add(coroutine);
			StartCoroutine(coroutine);
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

		critterObject.GetComponent<SideMoving>().Speed = MinSpeed + (Random.value * (MaxSpeed - MinSpeed));

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

	private void OnZeroSatisfaction()
	{
		_isGameInProgress = false;
		_currentCoroutines.ForEach(StopCoroutine);
	}

	private void OnResetGame()
	{
		_isGameInProgress = true;
		_spawnedCritters.Clear();
	}
}
