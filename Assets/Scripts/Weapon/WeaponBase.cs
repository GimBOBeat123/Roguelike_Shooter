using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public WeaponData data;
    protected float lastAttackTime;

    protected virtual void Awake()
    {
        lastAttackTime = -999f;
    }

    public virtual void Initialize(WeaponData weaponData)
    {
        data = weaponData;
    }

    public bool CanAttack()
    {
        return Time.time >= lastAttackTime + (1f / data.attackSpeed);
    }

    public virtual void Attack() { }

    protected virtual System.Collections.IEnumerator AttackMotion()
    {
        yield return null;
    }
}
