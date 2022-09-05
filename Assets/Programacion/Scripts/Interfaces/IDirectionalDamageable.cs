using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDirectionalDamageable
{
    void TakeDamageDirectional(int damage, Transform hitFrom);
}
