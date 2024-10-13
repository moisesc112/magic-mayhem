using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingAura : Ability
{
    [SerializeField] StatusEffect buffStatusEffect;
    [SerializeField] AudioSource audioSource;

    public override void Awake()
    {
        StartCoroutine(AudioFadeOut.FadeOut(audioSource, abilityInfo.despawnTime));
        base.Awake();
    }

    public override void OnTriggerEnter(Collider collision)
    {
        if (collision != null && collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponentInParent<PlayerStats>().StatusEffects.AddStatusEffect(buffStatusEffect);
        }       
    }

    public void OnTriggerExit(Collider collision)
    {
        if (collision != null && collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponentInParent<PlayerStats>().StatusEffects.RemoveStatusEffectsByName(buffStatusEffect.name);
        }
    }

    public override void Despawn()
    {
        _player.PlayerStats.StatusEffects.RemoveStatusEffectsByName(buffStatusEffect.name);
        base.Despawn();
    }

    public override void UpdateProjectileVelocity() { }
}
