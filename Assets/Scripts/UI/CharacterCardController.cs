using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class CharacterCardController : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI titleText;
	[SerializeField] TextMeshProUGUI readyText;
	[SerializeField] GameObject readyIcon;
	[SerializeField] Button readyButton;

	public InputSystemUIInputModule inputSystemUIInputModule => _inputModule;
	[SerializeField] InputSystemUIInputModule _inputModule;

	public event EventHandler<PlayerReadyEventArgs> PlayerReadyStatusChanged;

	void Start()
	{
		StartCoroutine(nameof(WaitForInput));
		_inputModule.cancel.action.performed += OnCancel;
	}

	private void OnDestroy()
	{
		_inputModule.cancel.action.performed -= OnCancel;
	}

	public void SetPlayerIndex(int index)
	{
		_playerIndex = index;
		titleText.text = $"Player {(_playerIndex + 1)}";
	}

	public void Ready()
	{
		if (!_allowInput) return;

		SetReadyState(true);
	}

	public void Unready()
	{
		if (!_allowInput) return;

		SetReadyState(false);
	}

	public void BackOut()
	{
		if (!_allowInput) return;

		SetReadyState(false);
	}

	IEnumerator WaitForInput()
	{
		yield return new WaitForSeconds(0.1f);
		_allowInput = true;
	}

	void SetReadyState(bool ready)
	{
		readyIcon.SetActive(ready);
		readyText.text = ready ? "Ready!" : "Ready?";
		_isReady = ready;
		PlayerReadyStatusChanged?.Invoke(sender: this, new PlayerReadyEventArgs(ready));
	}
	
	void OnCancel(InputAction.CallbackContext context)
	{
		if (_isReady)
			Unready();
		else
			PlayerManager.instance.RemovePlayer(_playerIndex);
	}

	public class PlayerReadyEventArgs : EventArgs
	{
		public PlayerReadyEventArgs(bool isReady)
		{
			ready = isReady;
		}

		public bool ready { get; private set; }
	}

	int _playerIndex;
	bool _allowInput;
	bool _isReady;
}
