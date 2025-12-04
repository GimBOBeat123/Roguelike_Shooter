using UnityEngine;
using System.Collections;

public class MeleeWeapon : WeaponBase
{
    private Vector3 originalLocalPos;
    private Transform model;

    // 칼끝(찌르는 지점) 오프셋(로컬 좌표 기준)
    public Vector3 hitOffset = new Vector3(1.0f, 0f, 0f);
    public float hitRadius = 0.25f;
    public LayerMask hitMask;

    private void Start()
    {
        model = transform;
        originalLocalPos = model.localPosition;
    }

    public void Awake()
    {
        hitMask = LayerMask.GetMask("Enemy");
    }

    private bool isAttacking = false;
    public override void Attack()
    {
        Debug.Log("MeleeWeapon.Attack called, canAttack=" + CanAttack());
        if (!CanAttack() || isAttacking) return;

        lastAttackTime = Time.time;
        StartCoroutine(AttackMotion());
    }

    //protected override IEnumerator AttackMotion()
    //{
    //    float duration = 0.25f; // 전체 시간
    //    float half = duration / 2f;
    //    float elapsed = 0f;

    //    Vector3 startPos = originalLocalPos;
    //    // 로컬 X 방향(무기 프리팹이 +X를 앞 방향으로 향하도록 설정되어 있어야 함)
    //    Vector3 thrustPos = startPos + hitOffset;

    //    // 앞으로 찌르기
    //    elapsed = 0f;
    //    while (elapsed < half)
    //    {
    //        elapsed += Time.deltaTime;
    //        float t = elapsed / half;
    //        model.localPosition = Vector3.Lerp(startPos, thrustPos, t);
    //        yield return null;
    //    }

    //    // 히트 판정(최대로 나갔을 때 한번만 체크하길 원하면 여기서 체크)
    //    DoHitCheck();

    //    // 원위치로 복귀
    //    elapsed = 0f;
    //    while (elapsed < half)
    //    {
    //        elapsed += Time.deltaTime;
    //        float t = elapsed / half;
    //        model.localPosition = Vector3.Lerp(thrustPos, startPos, t);
    //        yield return null;
    //    }

    //    model.localPosition = startPos; // 보정
    //}
    protected override IEnumerator AttackMotion()
    {
        isAttacking = true;
        float duration = 0.25f;
        float half = duration / 2f;
        float elapsed = 0f;

        Vector3 startLocalPos = model.localPosition;

        // 마우스 방향 계산 (로컬 기준)
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector3 dirWorld = (mouseWorld - model.position).normalized;
        Vector3 dirLocal = model.parent.InverseTransformDirection(dirWorld); // 부모 기준 로컬 방향

        // 찌르기 목표 위치 (로컬)
        Vector3 targetLocalPos = startLocalPos + dirLocal * data.attackDistance;

        // 앞으로 찌르기
        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / half;
            model.localPosition = Vector3.Lerp(startLocalPos, targetLocalPos, t);
            yield return null;
        }

        // 히트 체크
        DoHitCheck();

        // 원위치 복귀
        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / half;
            model.localPosition = Vector3.Lerp(targetLocalPos, startLocalPos, t);
            yield return null;
        }

        model.localPosition = startLocalPos; // 최종 보정
        isAttacking = false;
    }

    private void DoHitCheck()
    {
        // 모델의 로컬 hitOffset 위치를 월드 좌표로 변환
        Vector3 worldHitPos = model.TransformPoint(hitOffset);
        Collider2D[] hits = Physics2D.OverlapCircleAll(worldHitPos, hitRadius, hitMask);

        foreach (var c in hits)
        {
            Debug.Log("Melee hit: " + c.name + worldHitPos);
            c.GetComponent<Enemy>()?.OnHit(data.damage);
        }


#if UNITY_EDITOR
        // 에디터용 디버그 원 그려주기
        Debug.DrawLine(worldHitPos + Vector3.up * 0.02f, worldHitPos - Vector3.up * 0.02f, Color.red, 0.5f);
        Debug.DrawLine(worldHitPos + Vector3.right * 0.02f, worldHitPos - Vector3.right * 0.02f, Color.red, 0.5f);
#endif
    }

    private void OnDrawGizmosSelected()
    {
        if (model == null) model = transform;
        Vector3 worldHitPos = model.TransformPoint(hitOffset);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(worldHitPos, hitRadius);
    }
}
