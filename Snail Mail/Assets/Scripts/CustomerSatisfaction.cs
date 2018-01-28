using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerSatisfaction : MonoBehaviour
{
	public delegate void ZeroSatisfactionAction();
	public static event ZeroSatisfactionAction OnZeroSatisfaction;

	public float MaxSatisfaction;
	public float PenaltyForMiss;
	public float PenaltyForWrongRecipient;
	public float BonusForRightRecipient;
	public float SatisfactionDrainRate;
	public float SatisfactionDrainIncreaseRate;

	public TextMesh ScoreTextMesh;
	public TextMesh ScoreTextMesh2;

	private float _currentSatisfaction;
	private float _currentDrainRate;

	private bool _isGameInProgress = true;
	private int _score = 0;

	private static CustomerSatisfaction instance;

	public static float CurrentSatisfaction {
		get {
			if (instance != null) {
				return instance._currentSatisfaction;
			}
			return 1f;
		}
	}

	void Awake ()
	{
		instance = this;
	}

	void Start ()
	{
		_currentSatisfaction = MaxSatisfaction;
		_currentDrainRate = SatisfactionDrainRate;
		Recipient.OnMessageReceived += OnMessageReceived;
		Launchable.OnFailedToImpact += OnFailedToImpact;
		RestartOnCollision.OnResetGame += OnResetGame;
	}

	void OnDestroy()
	{
		Recipient.OnMessageReceived -= OnMessageReceived;
		Launchable.OnFailedToImpact -= OnFailedToImpact;
		RestartOnCollision.OnResetGame -= OnResetGame;
	}

	void Update()
	{
		if (_isGameInProgress == false)
		{
			return;
		}

		_currentSatisfaction -= _currentDrainRate * Time.deltaTime;
		_currentSatisfaction = Mathf.Clamp(_currentSatisfaction, 0, MaxSatisfaction);

		_currentDrainRate += SatisfactionDrainIncreaseRate * Time.deltaTime;

		if (_currentSatisfaction <= 0)
		{
			if (OnZeroSatisfaction != null)
			{
				OnZeroSatisfaction();
			}
			_isGameInProgress = false;
			Recipient.OnMessageReceived -= OnMessageReceived;
			Launchable.OnFailedToImpact -= OnFailedToImpact;
			_currentSatisfaction = 0;

			GameOverParticles.Instance.Play ();
		}

		Shader.SetGlobalFloat("_FillAmt", _currentSatisfaction);

		ScoreTextMesh2.text = ScoreTextMesh.text = _score.ToString ();
	}

	void OnMessageReceived (Recipient recipient, bool isCorrectRecipient)
	{
		if (isCorrectRecipient)
		{
			_currentSatisfaction += BonusForRightRecipient;
			_score++;

			ScoreText.Trigger ();
		}
		else
		{
			_currentSatisfaction -= PenaltyForWrongRecipient;
		}
	}

	void OnFailedToImpact (Addressee recipient)
	{
		_currentSatisfaction -= PenaltyForMiss;
	}

	void OnResetGame ()
	{
		_score = 0;
		_currentSatisfaction = MaxSatisfaction;
		_currentDrainRate = SatisfactionDrainRate;
		_isGameInProgress = true;
	}
}
