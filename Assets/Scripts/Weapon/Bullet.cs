using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f;

    private float damage;         // 총알 내부에서만 저장
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // GunWeapon에서 호출해 주는 초기화 함수
    public void Initialize(float dmg, Vector2 dir, float speed)
    {
        damage = dmg;

        if (rb != null)
            rb.linearVelocity = dir * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("충돌: " + other.name);

        // Enemy 처리
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            enemy.OnHit(damage);
            Destroy(gameObject);
            return;
        }

        // Wall 레이어 충돌 시 삭제
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(gameObject);
            return;
        }
    }
}
