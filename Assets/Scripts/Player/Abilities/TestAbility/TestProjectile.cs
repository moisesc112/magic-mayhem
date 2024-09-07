using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectile : AbstractProjectile
{
    public override float IncomingDamage { get; set; }
    public override bool DestroyAfterCollision { get; set; } = true;
    public override Vector3 TargetDirection { get; set; } = new Vector3(1,0,0);
    public override float TargetSpeed { get; set; } = 3;

}
