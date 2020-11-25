using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySceleton : EnemyControllerBase
{
    public override void TakeDamage(int damage, DamageType type = DamageType.Casual, Transform player = null)
    {
        if (type != DamageType.Projectile)
            return;

        base.TakeDamage(damage, type, player);
    }
}
