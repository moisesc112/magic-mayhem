using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCone : Ability
{
    [SerializeField] AudioSource audioSource;
    private bool isAudioPaused;
    private Coroutine audioCoroutine;

    public override void Awake()
    {
        audioCoroutine = StartCoroutine(AudioFadeOut.FadeOut(audioSource, abilityInfo.despawnTime));
        StartCoroutine(DisableCollider());
        base.Awake();
    }

    IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(0.1f);
        transform.GetComponentInChildren<MeshCollider>().enabled = false;
    }

    public override void Update()
    {
        if (Time.deltaTime == 0)
        {
            audioSource.Pause();
            StopCoroutine(audioCoroutine);
            isAudioPaused = true;
        }
        else
        {
            if (isAudioPaused == true)
            {
                audioSource.UnPause();
                audioCoroutine = StartCoroutine(AudioFadeOut.FadeOut(audioSource, remainingDespawnTime));
                isAudioPaused = false;
            }
        }
        base.Update();
    }

    public override void OnTriggerEnter(Collider collision)
    {
        if (collision != null && collision.tag != "Player" && collision.GetComponent<HealthComponent>() != null)
        {
            collision.GetComponent<HealthComponent>().TakeDamage(GetAbilityDamage());
        }
    }

    public override void Despawn()
    {
        Destroy(transform.parent.gameObject);
    }

    public override void UpdateProjectileVelocity() { }
}
