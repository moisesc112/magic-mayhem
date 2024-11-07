using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class HyperBeam : Ability
{
    [SerializeField] private Transform beamTransform;
    private HashSet<Collider> hitEnemies = new HashSet<Collider>();
    private Vector3 beamDirection;
    private Transform avatarTransform;
    public float damageCooldown = .5f;

    public override void Awake()
    {
        beamDirection = SetDirectionToPlayerAimDirection();
        base.Awake();
    }

    public override void Start()
    {
        if (_player != null)
        {
            _playerController = PlayerManager.instance.PlayerControllers.FirstOrDefault(x => x.playerIndex == _player.GetPlayerIndex());
            _playerInput = _playerController.playerInput;
            DisableMovement();
        }
    }

    public override void Update()
    {
        if (!_player.isPlayerCasting)
        {
            Despawn();
            return;
        }
        
        avatarTransform = _player.GetAvatarTransform();
        UpdateProjectileVelocity();

        remainingDespawnTime -= Time.deltaTime;
        if (remainingDespawnTime <= 0)
        {
            Despawn();
        }
        if (beamTransform != null)
        {
            beamTransform.forward = avatarTransform.forward;
        }
    }

    public override void OnTriggerEnter(Collider collision)
    {
        if (collision != null && collision.gameObject.tag != "Player" && collision.GetComponent<HealthComponent>() != null)
        {
            collision.GetComponent<HealthComponent>().TakeDamage(GetAbilityDamage());
        }       
    }

    public void OnTriggerStay(Collider collision)
    {
        if (collision != null && collision.gameObject.tag != "Player" && collision.GetComponent<HealthComponent>() != null)
        {
            if (!hitEnemies.Contains(collision))
            {
                hitEnemies.Add(collision);
                StartCoroutine(UseHyperBeamCoroutine(collision));
            }
        }
    }

    public void OnTriggerExit(Collider collision)
    {
        if (hitEnemies.Contains(collision))
        {
            hitEnemies.Remove(collision);
            StopCoroutine(UseHyperBeamCoroutine(collision));
        }
    }

    public virtual IEnumerator UseHyperBeamCoroutine(Collider collision)
    {
        while (collision != null && collision.GetComponent<HealthComponent>() != null)
        {
            yield return new WaitForSeconds(damageCooldown);
            Debug.Log("go");
            collision.GetComponent<HealthComponent>().TakeDamage(GetAbilityDamage());
        }
        hitEnemies.Remove(collision);
    }

    public override void Despawn()
    {
        EnableMovement();
        base.Despawn();
        hitEnemies.Clear();
    }

    public void DisableMovement()
    {
        _playerInput.actions.FindActionMap("GamePlay").FindAction("Move").Disable();
    }

    public void EnableMovement()
    {
        _playerInput.actions.FindActionMap("GamePlay").FindAction("Move").Enable();
    }

    PlayerController _playerController;
    PlayerInput _playerInput;
}