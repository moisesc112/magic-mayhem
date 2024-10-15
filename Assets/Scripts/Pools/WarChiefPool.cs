using UnityEngine;
using UnityEngine.Pool;

public class WarChiefPool : MonoBehaviour
{
	[SerializeField] WarChief _prefab;
	[SerializeField] int _defaultCapacity;
	[SerializeField] int _maxCapacity;
	[SerializeField] bool _collectionCheck;

	public ObjectPool<WarChief> pool => _pool;

	private void Awake()
	{
		_pool = new ObjectPool<WarChief>(
			createFunc: Create,
			actionOnGet: OnGet,
			actionOnRelease: OnRelease,
			actionOnDestroy: OnDestroyPool,
			_collectionCheck,
			_defaultCapacity,
			_maxCapacity);
	}

	private WarChief Create() => Instantiate(_prefab, Vector3.zero, Quaternion.identity, parent: null);
	private void OnGet(WarChief warChief) => warChief.gameObject.SetActive(true);
	private void OnRelease(WarChief warChief) => warChief.gameObject.SetActive(false);
	private void OnDestroyPool(WarChief warChief) => Destroy(warChief.gameObject);

	ObjectPool<WarChief> _pool;
}
