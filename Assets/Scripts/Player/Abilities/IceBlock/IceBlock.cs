using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class IceBlock : Ability
{
    [SerializeField] StatusEffect firstBuffStatusEffect;
    [SerializeField] StatusEffect secondBuffStatusEffect;
    [SerializeField] AudioSource audioSource;
    public float playerAnimPauseTime;

    public override void Awake()
    {
        StartCoroutine(AudioFadeOut.FadeOut(audioSource, abilityInfo.despawnTime));
        base.Awake();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Start()
    {
        if (_player != null)
        {
            _player.PlayerStats.StatusEffects.AddStatusEffect(firstBuffStatusEffect);
            _player.PlayerStats.StatusEffects.AddStatusEffect(secondBuffStatusEffect);
            _playerController = PlayerManager.instance.PlayerControllers.FirstOrDefault(x => x.playerIndex == _player.GetPlayerIndex());
            _playerInput = _playerController.playerInput;
            _playerAnim = _player.GetComponentInChildren<Animator>();
            DisablePlayerInput();
        }
    }
    
    public override void UpdateProjectileVelocity()
    {
        transform.position = _player.GetAvatarPosition();
    }

    public override void Despawn()
    {
        EnablePlayerInput();
        base.Despawn();
    }

    public void DisablePlayerInput()
    {
        _playerInput.currentActionMap.Disable();
        StartCoroutine(DisablePlayerAnimator(_playerAnim, playerAnimPauseTime));
    }

    public void EnablePlayerInput()
    {
        _playerInput.currentActionMap.Enable();
        _playerAnim.enabled = true;
    }

    public IEnumerator DisablePlayerAnimator(Animator playerAnim, float playerAnimPauseTime)
    {
        yield return new WaitForSeconds(playerAnimPauseTime);
        playerAnim.enabled = false;
    }

    PlayerController _playerController;
    PlayerInput _playerInput;
    Animator _playerAnim;
}
