using UnityEngine;

public class PlayerStats : AbstractEntity
{
    public float healthRegenPerSecond;
    public float dodgeChance;
    public int gold;

    private void Update()
    {
        health += healthRegenPerSecond * Time.deltaTime;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public override void TakeDamage(float damage)
    {
        var randomDodgeChance = Random.Range(0f, 100f);
        if (randomDodgeChance < dodgeChance)
        {
            Debug.Log("Attack Dodged");
            return;
        }

        base.TakeDamage(damage);
    }
}
