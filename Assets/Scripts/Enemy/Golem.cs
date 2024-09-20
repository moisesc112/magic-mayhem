using UnityEngine;

public class Golem : MonoBehaviour
{
	[Header("FX")]
	[SerializeField] ParticleSystem _dustParticleSystem;
	[SerializeField] AudioClip _stompAudio;

	[Header("Attack")]
	[SerializeField] float _attackRange;
	[SerializeField] RadialDamageSource _stompSource;

	void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
		_animator = GetComponent<Animator>();
		_navPoller = GetComponent<NavPollerComponent>();
	}

	void Update()
	{
		var distanceToPlayer = _navPoller.DistanceToPlayer;
		if (distanceToPlayer <= _attackRange)
			AttackPlayer();

	}

	public void Stomp()
    {
		_animator.SetBool("IsAttacking", false);
		_dustParticleSystem.Play();
		_audioSource.PlayOneShot(_stompAudio);

		// Particle system is attacked to golem's foot
		var hitPoint = _dustParticleSystem.transform.position;
		Instantiate(_stompSource, hitPoint, Quaternion.identity);
	}

	public void AttackPlayer()
	{
		_animator.SetBool("IsAttacking", true);
	}
	
	AudioSource _audioSource;
	Animator _animator;
	NavPollerComponent _navPoller;
}
