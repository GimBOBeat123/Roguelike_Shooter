using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public WeaponBase currentWeapon; // 현재 장착된 무기

    [Header("Weapon")]
    public WeaponHolder weaponHolder;

    [Header("Player Settings")]
    public float moveSpeed = 5f;
    public int maxHP = 100;

    [Header("References")]
    public Transform playerHand;      // 손 위치

    [Header("Debug")]
    public LayerMask enemyLayer;

    [HideInInspector] public Room currentRoom;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 moveInput;
    private int currentHP;
    private bool isDead;
    private bool canMove = true;

    private float originalHandX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHP = maxHP;

        if (WeaponManager.Instance.defaultWeapon != null)
        {
            WeaponManager.Instance.AddWeapon(WeaponManager.Instance.defaultWeapon);
        }

        if (playerHand != null)
        {
            originalHandX = playerHand.localPosition.x;
        }

    }

    void Update()
    {
        if (isDead || !canMove) return;

        RotateWeaponToMouse(weaponHolder.currentWeapon);
        HandleMovementInput();
        HandleMouseDirection();
        HandleAnimation();

        // 공격 (마우스 좌 클릭)
        if (Input.GetMouseButton(0))
        {
            WeaponManager.Instance.weaponHolder.Attack();
        }

        // 무기 교체 입력 (E키)
        if (Input.GetKeyDown(KeyCode.E))
        {
            WeaponManager.Instance.SwitchWeapon();
        }

    }

    void FixedUpdate()
    {
        if (isDead || !canMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = moveInput * moveSpeed;
    }



    //============================================================
    // INPUT
    //============================================================

    private void HandleMovementInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(x, y).normalized;
    }

    private void HandleMouseDirection()
    {
        if (playerHand == null) return;

        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;

        Vector2 dir = (mouse - transform.position).normalized;

        // 캐릭터 flip
        spriteRenderer.flipX = dir.x < 0;

        // 무기 손 위치 flip (좌우 대칭)
        playerHand.localPosition = new Vector3(
            Mathf.Abs(originalHandX) * (dir.x >= 0 ? 1 : -1),
            playerHand.localPosition.y,
            0
        );
    }

    private void HandleAnimation()
    {
        animator.SetBool("isRunning", moveInput.sqrMagnitude > 0);
    }

    //============================================================
    // DAMAGE / HP
    //============================================================

    public void OnHit(int dmg)
    {
        if (isDead) return;

        currentHP -= dmg;
        animator.SetTrigger("Hit");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Die");
    }

    //============================================================
    // WEAPON
    //============================================================

    public void RotateWeaponToMouse(WeaponBase weapon)
    {
        if (weapon == null) return;

        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;

        Vector2 dir = mouse - weapon.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        weapon.transform.rotation = Quaternion.Euler(0, 0, angle);

        // 좌우 반전 고려 (필요하면)
        float scaleX = Mathf.Sign(transform.localScale.x);
        weapon.transform.localScale = new Vector3(scaleX, 1, 1);
    }

    //public void RotateWeaponToMouse()
    //{
    //    if (weaponHolder == null || weaponHolder.currentWeapon == null) return;

    //    Transform w = weaponHolder.currentWeapon.transform;

    //    Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    mouse.z = 0;

    //    Vector3 dir = mouse - w.position;
    //    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

    //    // 플레이어 좌우 반전 보정
    //    float scaleX = Mathf.Sign(transform.localScale.x);
    //    if (scaleX < 0) angle = 180 - angle;

    //    w.rotation = Quaternion.Euler(0, 0, angle);

    //    // 좌우 반전 적용
    //    w.localScale = new Vector3(scaleX, 1, 1);
    //}


    //private void RotateWeaponToMouse()
    //{
    //    if (playerHand == null) return;

    //    Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    mouse.z = 0;

    //    Vector2 dir = mouse - playerHand.position;
    //    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

    //    playerHand.rotation = Quaternion.Euler(0, 0, angle);
    //}


    //============================================================
    // MOVEMENT LOCK
    //============================================================

    public void SetCanMove(bool v)
    {
        canMove = v;
        if (!v)
            rb.linearVelocity = Vector2.zero;
    }

    //============================================================
    // ROOM 이동 루틴 (원본 그대로 유지 + 안정화)
    //============================================================

    public void MoveToRoom(Room next, Transform spawnPoint)
    {
        StartCoroutine(MoveRoomRoutine(next, spawnPoint));
    }

    private IEnumerator MoveRoomRoutine(Room nextRoom, Transform spawnDoor)
    {
        SetCanMove(false);

        rb.linearVelocity = Vector2.zero;

        // 카메라 이동
        Camera.main.transform.position = new Vector3(
            nextRoom.transform.position.x,
            nextRoom.transform.position.y,
            Camera.main.transform.position.z
        );

        // 플레이어를 방 입구로 이동
        transform.position = spawnDoor.position;

        currentRoom = nextRoom;

        yield return new WaitForSeconds(0.2f);

        SetCanMove(true);
    }

    //============================================================
    // DEBUG
    //============================================================

    private void OnDrawGizmosSelected()
    {
        if (playerHand == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerHand.position, 0.4f);
    }
}
