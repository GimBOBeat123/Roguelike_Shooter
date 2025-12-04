using UnityEngine;

public class GunWeapon : WeaponBase
{
    [Header("총구 위치(Optional)")]
    public Transform firePoint;

    public override void Attack()
    {
        if (!CanAttack()) return;

        lastAttackTime = Time.time;

        if (data.bulletPrefab == null)
        {
            Debug.LogError("bulletPrefab이 WeaponData에 없습니다.");
            return;
        }

        // 총구 위치
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;

        // 총알 생성
        GameObject bulletObj = Instantiate(
            data.bulletPrefab,
            spawnPos,
            transform.rotation
        );

        Bullet bullet = bulletObj.GetComponent<Bullet>();

        if (bullet != null)
        {
            // bullet.Initialize(데미지, 방향, 속도)
            bullet.Initialize(
                data.damage,
                transform.right,
                data.bulletSpeed
            );
        }
        else
        {
            Debug.LogError("bulletPrefab에 Bullet 스크립트가 없습니다!");
        }
    }
}
