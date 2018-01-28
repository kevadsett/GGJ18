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

	private float _currentSatisfaction;
	private float _currentDrainRate;

	private bool _isGameInProgress = true;

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
		}

		Shader.SetGlobalFloat("_FillAmt", _currentSatisfaction);

	}

	void OnMessageReceived (Recipient recipient, bool isCorrectRecipient)
	{
		if (isCorrectRecipient)
		{
			_currentSatisfaction += BonusForRightRecipient;
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
		_currentSatisfaction = MaxSatisfaction;
		_currentDrainRate = SatisfactionDrainRate;
		_isGameInProgress = true;
	}
}
