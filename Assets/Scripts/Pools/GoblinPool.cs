using UnityEngine;
using UnityEngine.Pool;

public class GoblinPool : MonoBehaviour
{
    [SerializeField] Goblin _prefab;
    [SerializeField] int _defaultCapacity;
    [SerializeField] int _maxCapacity;
    [SerializeField] bool _collectionCheck;

    public ObjectPool<Goblin> pool => _pool;

	private void Awake()
	{
        _pool = new ObjectPool<Goblin>(
            createFunc: Create,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyPool,
            _collectionCheck,
            _defaultCapacity,
            _maxCapacity);
	}

    private Goblin Create() => Instantiate(_prefab, Vector3.zero, Quaternion.identity, parent: null);
    private void OnGet(Goblin goblin) => goblin.gameObject.SetActive(true);
    private void OnRelease(Goblin goblin) => goblin.gameObject.SetActive(false);
	private void OnDestroyPool(Goblin goblin) => Destroy(goblin.gameObject);

    ObjectPool<Goblin> _pool;
}
