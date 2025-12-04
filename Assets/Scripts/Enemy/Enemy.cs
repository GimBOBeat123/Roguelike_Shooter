using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float maxHP = 50f;
    public float currentHP;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float detectRange = 5f;

    [Header("Target")]
    public Transform target;    // 보통 Player Transform 넣음

    Rigidbody2D rb;
    Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHP = maxHP;
    }

    void Update()
    {
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= detectRange)
        {
            ChasePlayer();   // 추적
        }
        else
        {
            StopMove();
        }
    }

    void StopMove()
    {
        rb.linearVelocity = Vector2.zero;
    }

    /// <summary>
    /// 플레이어 추적
    /// </summary>
    void ChasePlayer()
    {
        Vector2 dir = (target.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;

        // 애니메이션 X축 기준으로 방향 전환
        if (dir.x != 0)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(dir.x),
                1,
                1
            );
        }
    }

    public void OnHit(float damage)
    {
        currentHP -= damage;

        if (anim != null)
            anim.SetTrigger("Hit");

        if (currentHP <= 0)
            Die();

    }

    void Die()
    {
        if (anim != null)
            anim.SetTrigger("Dead");

        // 죽는 모션 재생 후 오브젝트 삭제
        Destroy(gameObject, 0.5f);
    }
}
