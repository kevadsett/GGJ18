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

	public Text DummySatisfactionDisplay;

	private float _currentSatisfaction;

	void Start ()
	{
		_currentSatisfaction = MaxSatisfaction;
		Recipient.OnMessageReceived += OnMessageReceived;
		Launchable.OnFailedToImpact += OnFailedToImpact;
	}

	void OnDestroy()
	{
		Recipient.OnMessageReceived -= OnMessageReceived;
		Launchable.OnFailedToImpact -= OnFailedToImpact;
	}

	void Update()
	{
		_currentSatisfaction -= SatisfactionDrainRate * Time.deltaTime;

		Mathf.Clamp(_currentSatisfaction, 0, MaxSatisfaction);

		if (_currentSatisfaction <= 0)
		{
			if (OnZeroSatisfaction != null)
			{
				OnZeroSatisfaction();
			}
		}

		DummySatisfactionDisplay.text = _currentSatisfaction.ToString();

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
}
