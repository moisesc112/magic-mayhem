using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectile : AbstractProjectile
{
    public override float IncomingDamage { get; set; }
    public override bool DestroyAfterCollision { get; set; } = true;

    public override void UpdateProjectileVelocity()
    {
        transform.position += new Vector3(3, 0, 0) * Time.deltaTime;
    }
}
