using UnityEngine;

public class BowWeapon : WeaponBase
{
    private bool isCharging = false;
    private float chargeTime = 0f;

    public override void Attack()
    {
        if (!isCharging)
        {
            isCharging = true;
            chargeTime = 0f;
        }
    }

    private void Update()
    {
        if (isCharging)
        {
            chargeTime += Time.deltaTime;
        }

        if (isCharging && Input.GetMouseButtonUp(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (!CanAttack())
        {
            isCharging = false;
            return;
        }

        lastAttackTime = Time.time;

       // GameObject arrow = Instantiate(data.projectilePrefab, transform.position, transform.rotation);
        //Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();

        float power = Mathf.Clamp(chargeTime, 0.2f, 1.5f);

        //rb.linearVelocity = transform.right * data.projectileSpeed * power;

        isCharging = false;
        chargeTime = 0f;
    }
}
