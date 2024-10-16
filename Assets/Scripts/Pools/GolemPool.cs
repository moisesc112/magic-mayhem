using UnityEngine;
using UnityEngine.Pool;

public class GolemPool : MonoBehaviour
{
	[SerializeField] Golem _prefab;
	[SerializeField] int _defaultCapacity;
	[SerializeField] int _maxCapacity;
	[SerializeField] bool _collectionCheck;

	public ObjectPool<Golem> pool => _pool;

	private void Awake()
	{
		_pool = new ObjectPool<Golem>(
			createFunc: Create,
			actionOnGet: OnGet,
			actionOnRelease: OnRelease,
			actionOnDestroy: OnDestroyPool,
			_collectionCheck,
			_defaultCapacity,
			_maxCapacity);
	}

	private Golem Create() => Instantiate(_prefab, Vector3.zero, Quaternion.identity, parent: null);
	private void OnGet(Golem golem) => golem.gameObject.SetActive(true);
	private void OnRelease(Golem golem) => golem.gameObject.SetActive(false);
	private void OnDestroyPool(Golem golem) => Destroy(golem.gameObject);

	ObjectPool<Golem> _pool;
}
