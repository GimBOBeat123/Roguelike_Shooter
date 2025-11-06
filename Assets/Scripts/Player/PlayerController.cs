using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;
    private SpriteRenderer spriteRenderer;

    public float moveSpeed = 5f; // 플레이어 이동 속도
    public int maxHP = 100; // 플레이어 최대 체력
    private int currentHP; // 플레이어 현재 체력
    public Room currentRoom; // 현재 플레이어가 위치한 방

    private bool isHit = false; // 플레이어 피해 여부
    private bool isDead = false; // 플레이어 사망 여부
    private bool canMove = true; // 방 이동 중 이동 제한용

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHP = maxHP;
    }

    void Update()
    {
        if (isDead || !canMove) return;  // 이동 제한 추가

        HandleInput();
        HandleAnimation();
    }

    void FixedUpdate()
    {
        if (isDead || !canMove) return;  // 이동 제한 추가
        rb.linearVelocity = moveInput * moveSpeed;
    }

    /// <summary>
    /// 플레이어 기본 이동 구현
    /// </summary>
    void HandleInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        // 좌우 방향 전환
        if (moveX > 0)
            spriteRenderer.flipX = false;
        else if (moveX < 0)
            spriteRenderer.flipX = true;
    }

    /// <summary>
    /// 이동 애니메이션 처리
    /// </summary>
    void HandleAnimation()
    {
        animator.SetBool("isRunning", moveInput.magnitude > 0);
    }

    /// <summary>
    /// 피격 처리
    /// </summary>
    public void OnHit(int damage)
    {
        if (isDead || isHit) return;

        currentHP -= damage;
        Debug.Log($"플레이어 피해: {damage}, 현재 HP: {currentHP}");

        isHit = true;
        animator.SetBool("isHit", true);

        Invoke(nameof(ResetHit), 0.3f);

        if (currentHP <= 0)
            OnDie();
    }

    void ResetHit()
    {
        isHit = false;
        animator.SetBool("isHit", false);
    }

    /// <summary>
    /// 사망 처리
    /// </summary>
    public void OnDie()
    {
        if (isDead) return;

        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("isDie");
        Debug.Log("플레이어 사망 애니메이션 발동");
    }
    public void SetCanMove(bool value)
    {
        canMove = value;
        if (!value) rb.linearVelocity = Vector2.zero;
    }

    public void MoveToRoom(Room nextRoom, Transform spawnDoor)
    {
        StartCoroutine(MoveRoomRoutine(nextRoom, spawnDoor));
    }

    private IEnumerator MoveRoomRoutine(Room nextRoom, Transform spawnDoor)
    {
        SetCanMove(false); // 이동 잠금
        rb.linearVelocity = Vector2.zero;

        // 카메라 이동
        Camera.main.transform.position = new Vector3(nextRoom.transform.position.x, nextRoom.transform.position.y, Camera.main.transform.position.z);

        // 플레이어 위치 이동
        transform.position = spawnDoor.position;

        // 현재 방 갱신
        currentRoom = nextRoom;

        yield return new WaitForSeconds(0.2f); // 살짝 딜레이 후 이동 가능
        SetCanMove(true);
    }

}
