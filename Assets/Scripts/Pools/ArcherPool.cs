using UnityEngine;
using UnityEngine.Pool;

public class ArcherPool : MonoBehaviour
{
	[SerializeField] Archer _prefab;
	[SerializeField] int _defaultCapacity;
	[SerializeField] int _maxCapacity;
	[SerializeField] bool _collectionCheck;

	public ObjectPool<Archer> pool => _pool;

	private void Awake()
	{
		_pool = new ObjectPool<Archer>(
			createFunc: Create,
			actionOnGet: OnGet,
			actionOnRelease: OnRelease,
			actionOnDestroy: OnDestroyPool,
			_collectionCheck,
			_defaultCapacity,
			_maxCapacity);
	}

	private Archer Create() => Instantiate(_prefab, Vector3.zero, Quaternion.identity, parent: null);
	private void OnGet(Archer archer) => archer.gameObject.SetActive(true);
	private void OnRelease(Archer archer) => archer.gameObject.SetActive(false);
	private void OnDestroyPool(Archer archer) => Destroy(archer.gameObject);

	ObjectPool<Archer> _pool;
}
