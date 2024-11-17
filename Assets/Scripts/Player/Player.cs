using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerHitVisualizer))]
public class Player : MonoBehaviour
{
	public bool isControlled => _playerIndex >= 0;
	public Vector3 velocity => _velocity;
	private AbstractTrap detectedTrap;
	public bool playerInShopRange => _shopTrigger != null && _shopTrigger.playerInShop;
	public PlayerController owningController => _owningController;
	public AbilitySlotsComponent abilitySlotsComponent => _abilitySlotsComponent;
	public Color playerColor => _playerColor;
	public Shop shop => _shop;

	[SerializeField] GameObject _avatar;
	[SerializeField] Renderer _indicatorRenderer;
	[SerializeField] AudioClip onDeathSound;

	void Awake()
	{
		_mover = GetComponentInChildren<Mover>();
		_mover.SetPlayer(this);
		_castingComponent = GetComponentInChildren<CastingComponent>();
		_playerStats = GetComponent<PlayerStats>();
		_ragdoll = _avatar.GetComponent<RagdollComponent>();
		_playerStats.onDeath += HealthComp_OnDeath;
		_inGameMenu = FindObjectOfType<InGameMenu>();
		_shopTrigger = FindObjectOfType<ShopTrigger>();
		_abilitySlotsComponent = _avatar.GetComponent<AbilitySlotsComponent>();
		_audioSource = GetComponentInChildren<AudioSource>();
		UpdateHitRenderers();
	}

	void Start()
	{
		_previousPos = GetAvatarPosition();
		_canPlayDeathSound = true;
	}

	void LateUpdate()
	{
		var newPos = GetAvatarPosition();
		_velocity = Vector3.MoveTowards(_velocity, (newPos - _previousPos) / Time.deltaTime, Time.deltaTime);
		_previousPos = newPos;
	}

	public Vector3 GetAvatarPosition() => _avatar.transform.position;
	public Transform GetAvatarTransform() => _avatar.transform;
	public Vector3 GetAvatarVelocity() => _velocity;
	public Vector2 GetAimDirection() => _mover.GetAimDirection();

	public int GetPlayerIndex() => _playerIndex;

	public void SetShop(Shop shop) => _shop = shop;

	public void Possess(PlayerController playerController, Color color)
	{
		_owningController = playerController;
		_playerIndex = playerController.playerIndex;

		if (playerController.usingMK)
			_mover.SetFollowCursor();

		_playerColor = color;
		_indicatorRenderer.material.color = _playerColor;
	}

	public void ResetPlayer(Transform spawnPoint)
	{
		transform.position = spawnPoint.position;
		transform.rotation = spawnPoint.rotation;
		_avatar.transform.localPosition = Vector3.zero;
		_avatar.transform.localRotation = Quaternion.identity;
		_playerStats.health = _playerStats.maxHealth;
		_ragdoll.DisableRagdoll();

		_mover.enabled = true;
		_castingComponent.enabled = true;
		gameObject.tag = "AlivePlayers";
	}

	public void Release()
	{
		_owningController = null;
		_playerIndex = -1;
	}

	public void MovePlayer(Vector2 input)
	{
		var movement = Vector3.zero;
		movement.x = input.x;
		movement.z = input.y;
		_mover.SetMovement(movement);
	}

	public void SelectAbility(int slotNumber)
	{
		//TODO unify casting animations with abilities and direction
		_castingComponent.SetSelectedAbility(slotNumber);
	}

	public void SelectAbilityByDirection(SelectAbilityDirection direction)
	{
		_castingComponent.SetSelectedAbilityByDirection(direction);
	}

	public void UpdateCasting(bool isCasting)
	{
		_castingComponent.UpdateCasting(isCasting);
	}

	public void SetAiming(bool isAiming, Vector2 aimingDir, bool useMouse)
	{
		_mover.SetAiming(isAiming, aimingDir, useMouse);
	}

	public void ToggleShopUI(bool isEnabled)
	{
		_shop.ToggleShopUI(isEnabled);
	}

	public void SwapShopTab()
	{
		_shop.ToggleShopPage();
	}

	public void OnRoll()
	{
		_mover.OnRoll();
	}

	// Set the reference trap from AvatarTrapActivation
	// that the player is currently colliding with
	public void SetDetectedTrap(AbstractTrap trap)
	{
		detectedTrap = trap;
	}

	public void SetBellTower(BellTower bellTower)
	{
		_bellTower = bellTower;
	}

	public void ActivateTrap(bool isActivated)
	{
		// If player is on a trap and has chosen to activate it and has
		// enough gold then minus the gold and then activate the trap 
		if (isActivated && detectedTrap != null && _playerStats.gold >= detectedTrap.trapInfo.trapCost)
		{
			_playerStats.gold -= detectedTrap.trapInfo.trapCost;
			detectedTrap.ActivateTrap();
		}
		else if (isActivated && _bellTower != null && _bellTower.isActivatable)
		{
			WaveManager.instance.SkipShopPhase();
			SetBellTower(null);
		}
	}

	public bool GameOver()
	{
		return _inGameMenu.gameOver;
	}

	// Same methodology as the enemy death
	void HealthComp_OnDeath(object sender, EventArgs e)
	{
		HandleDeath();
	}

	/// <summary>
	/// If we decide to include a character creator, 
	/// the renderer material values may change which requires calling this method.
	/// </summary>
	private void UpdateHitRenderers()
	{
		var activeRenderers = _avatar.GetComponentsInChildren<Renderer>(includeInactive: false);
		GetComponent<PlayerHitVisualizer>().SetRenderers(activeRenderers);
	}

	void HandleDeath()
	{
		// Enable ragdoll and disable movement, casting and asign tag
		_ragdoll.EnableRagdoll();
		_mover.enabled = false;
		_castingComponent.enabled = false;
		gameObject.tag = "DeadPlayers";
		_audioSource.PlayOneShot(onDeathSound);

		GameStateManager.instance.HandlePlayerDied(this);
	}

	private void OnDeathSound()
	{
		if (_canPlayDeathSound)
		{
			_audioSource.PlayOneShot(onDeathSound);
			_canPlayDeathSound = false;
			StartCoroutine(HandleDeathSound());
		}
	}

	IEnumerator HandleDeathSound()
	{
		yield return new WaitForSeconds(1.0f);
		_canPlayDeathSound = true;
	}

	public PlayerStats PlayerStats => GetComponent<PlayerStats>();

	PlayerStats _playerStats;
	Mover _mover;
	CastingComponent _castingComponent;
	Shop _shop;
	int _playerIndex = -1;
	Vector3 _velocity;
	Vector3 _previousPos;
	RagdollComponent _ragdoll;
	InGameMenu _inGameMenu;
	BellTower _bellTower;
	ShopTrigger _shopTrigger;
	PlayerController _owningController;
	AbilitySlotsComponent _abilitySlotsComponent;
	Color _playerColor;
	AudioSource _audioSource;
	bool _canPlayDeathSound;
}
