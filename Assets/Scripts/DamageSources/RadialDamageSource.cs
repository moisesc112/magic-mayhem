using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class RadialDamageSource : MonoBehaviour
{
    
    [SerializeField] RadialDamageInfo _damageInfo;

    public HashSet<GameObject> objectsWithinRadius;

	private void Awake()
	{
        _objectsWithinRadius = new HashSet<GameObject>();
	}
	// Start is called before the first frame update
	void Start()
    {
        transform.localScale = _damageInfo.initialSize;
    }

    // Update is called once per frame
    void Update()
    {
		transform.localScale = Vector3.MoveTowards(transform.localScale, _damageInfo.finalSize, _damageInfo.expansionSpeed * Time.deltaTime);
        if ((transform.localScale - _damageInfo.finalSize).sqrMagnitude < 0.1f && !_finishedExpanding)
        {
            _finishedExpanding = true;
            StartCoroutine(nameof(FinishExpanding));
        }

        foreach (var obj in _objectsWithinRadius)
        {
            var healthComponent = obj.GetComponentInParent<HealthComponent>();
            if (healthComponent)
                healthComponent.TakeDamage(_damageInfo.damagePerSecond * Time.deltaTime);
        }
    }

	public void OnTriggerEnter(Collider other)
	{
		if (LayerMaskUtility.GameObjectIsInLayer(other.gameObject, _damageInfo.objectsToTrack))
            _objectsWithinRadius.Add(other.gameObject);
        if (!other.attachedRigidbody?.isKinematic ?? false)
        {
            var dir = other.attachedRigidbody.gameObject.transform.position - transform.position;
            other.attachedRigidbody.AddForce(dir * _damageInfo.forceStrength, ForceMode.Impulse);
        }
	}

	public void OnTriggerExit(Collider other)
	{
        if (LayerMaskUtility.GameObjectIsInLayer(other.gameObject, _damageInfo.objectsToTrack))
            _objectsWithinRadius.Remove(other.gameObject);
	}

	public void OnTriggerStay(Collider other)
	{
		if (LayerMaskUtility.GameObjectIsInLayer(other.gameObject, _damageInfo.objectsToTrack))
			_objectsWithinRadius.Add(other.gameObject);
	}

	IEnumerator FinishExpanding()
    {
        if (_damageInfo.timeBeforeDestory > 0)
        {
            yield return new WaitForSeconds(_damageInfo.timeBeforeDestory);
            Destroy(gameObject);
        }
        else
        {
            yield return new WaitForEndOfFrame();
            Destroy(gameObject);
        }
    }

    bool _finishedExpanding;
	HashSet<GameObject> _objectsWithinRadius;
    Vector3 _scale;
}
