using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MagicMissileProjectile : MonoBehaviour
{
	public float damage;

	[SerializeField] LayerMask _collisionMask;
	[SerializeField] GameObject _hitPrefab;

	void OnTriggerEnter(Collider collision)
	{
		if (LayerMaskUtility.GameObjectIsInLayer(collision.gameObject, _collisionMask))
		{
			var hc = collision.gameObject.GetComponent<HealthComponent>();
			if (hc)
				hc.TakeDamage(damage);
			Instantiate(_hitPrefab, transform.position, Quaternion.identity);
			Destroy(gameObject);
		}
	}

	AudioSource _audioSource;
}
