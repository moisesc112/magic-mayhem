using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Mover : MonoBehaviour
{
	[Header("Speed Settings")]
	[SerializeField] float _movementSpeed = 5.0f;

	void Awake()
	{
		_characterController = GetComponent<CharacterController>();
	}

	// Update is called once per frame
	void Update()
	{
		HandleMovement();
	}

	public void SetMovement(Vector3 movement) => _currentMovement = movement;

	void HandleMovement()
	{
		_characterController.Move(_currentMovement * _movementSpeed * Time.deltaTime);
	}

	CharacterController _characterController;
	Vector3 _currentMovement;
}
