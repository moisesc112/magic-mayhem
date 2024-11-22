using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

[RequireComponent(typeof(Animator))]
public class MenuCharacter : MonoBehaviour
{
	[Header("UI")]
	[SerializeField] TextMeshProUGUI _joinText;
	[SerializeField] TextMeshProUGUI _playerText;

	[Header("Config")]
	[SerializeField] int _playerIndex;
	[SerializeField] int _idlePose = 1;
	[SerializeField] InputSystemUIInputModule _inputModule;
	public InputSystemUIInputModule inputModule => _inputModule;


	public int playerIndex => _playerIndex;

	private void Awake()
	{
		_anim = GetComponent<Animator>();
	}

	private void Start()
	{
		_anim.SetInteger("Pose", _idlePose);
	}

	private void OnDestroy()
	{
		_inputModule.cancel.action.performed -= OnCancel;
	}

	public void Join()
	{
		_playerText.gameObject.SetActive(true);
		_playerText.text = $"Player {playerIndex + 1}";
		_joinText.gameObject.SetActive(false);
		_inputModule.cancel.action.performed += OnCancel;
	}

	public void BackOut()
	{
		_playerText.gameObject.SetActive(false);
		_joinText.gameObject.SetActive(true);
		_inputModule.cancel.action.performed -= OnCancel;
	}

	void OnCancel(InputAction.CallbackContext context)
	{
		BackOut();
		PlayerManager.instance.RemovePlayer(_playerIndex);
	}

	Animator _anim;
}
