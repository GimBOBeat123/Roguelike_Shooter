using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
    public static Map Instance { get; private set; }
    public bool CanTransition { get; private set; } = true; // 문 트리거 가능 여부

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [Header("Map Settings")]
    public int roomCount = 10;
    public GameObject roomPrefab;
    public float roomWidth = 20f;
    public float roomHeight = 12f;

    private Dictionary<Vector2Int, Room> rooms = new();
    private PlayerController player;
    private Camera mainCam;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        mainCam = Camera.main;
        GenerateMap();
    }

    void GenerateMap()
    {
        Vector2Int currentPos = Vector2Int.zero;

        Room startRoom = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity).GetComponent<Room>();
        startRoom.gridPosition = currentPos;
        rooms[currentPos] = startRoom;
        player.currentRoom = startRoom;

        for (int i = 0; i < roomCount - 1; i++)
        {
            Vector2Int nextPos = currentPos + GetRandomDirection();

            int safety = 0;
            while (rooms.ContainsKey(nextPos) && safety < 10)
            {
                nextPos = currentPos + GetRandomDirection();
                safety++;
            }

            if (!rooms.ContainsKey(nextPos))
            {
                Vector3 worldPos = new Vector3(nextPos.x * roomWidth, nextPos.y * roomHeight, 0);
                Room newRoom = Instantiate(roomPrefab, worldPos, Quaternion.identity).GetComponent<Room>();
                newRoom.gridPosition = nextPos;
                rooms[nextPos] = newRoom;

                currentPos = nextPos;
            }
        }

        // 🔥🔥🔥 방 생성 끝난 후 문 시각화 적용
        foreach (var room in rooms.Values)
            room.SetupDoorsVisual(rooms);
    }



    Vector2Int GetRandomDirection()
    {
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        return dirs[Random.Range(0, dirs.Length)];
    }

    public void MoveToNextRoom(Door.DoorDirection dir)
    {
        if (!CanTransition) return; // 이동 중이면 무시
        CanTransition = false;

        Room current = player.currentRoom;
        Vector2Int nextPos = current.gridPosition;

        switch (dir)
        {
            case Door.DoorDirection.Up: nextPos += Vector2Int.up; break;
            case Door.DoorDirection.Down: nextPos += Vector2Int.down; break;
            case Door.DoorDirection.Left: nextPos += Vector2Int.left; break;
            case Door.DoorDirection.Right: nextPos += Vector2Int.right; break;
        }

        if (!rooms.ContainsKey(nextPos))
        {
            Debug.Log("🚫 다음 방이 없습니다.");
            CanTransition = true;
            return;
        }

        Room nextRoom = rooms[nextPos];
        Transform targetDoor = nextRoom.GetDoor(Opposite(dir));

        if (targetDoor == null)
        {
            Debug.LogWarning("다음 방에 해당 방향 문이 없습니다!");
            targetDoor = nextRoom.transform; // 문 없으면 중앙
        }

        StartCoroutine(TransitionToRoom(nextRoom, targetDoor));
    }

    private IEnumerator TransitionToRoom(Room nextRoom, Transform targetDoor)
    {
        player.SetCanMove(false);

        // 1. 페이드 아웃
        if (FadeController.Instance != null)
            yield return StartCoroutine(FadeController.Instance.FadeOut(0.3f));

        // 2. 플레이어 위치 이동 (문 기준)
        player.transform.position = targetDoor.position;
        player.currentRoom = nextRoom;

        // 3. 카메라 부드럽게 이동
        Vector3 camTarget = new Vector3(nextRoom.transform.position.x, nextRoom.transform.position.y, mainCam.transform.position.z);
        yield return StartCoroutine(MoveCameraSmooth(camTarget));

        // 4. 페이드 인
        if (FadeController.Instance != null)
            yield return StartCoroutine(FadeController.Instance.FadeIn(0.3f));

        player.SetCanMove(true);
        CanTransition = true; // 다시 문 트리거 가능
    }

    private IEnumerator MoveCameraSmooth(Vector3 targetPos)
    {
        float t = 0f;
        Vector3 startPos = mainCam.transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            mainCam.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
    }

    private Door.DoorDirection Opposite(Door.DoorDirection dir)
    {
        return dir switch
        {
            Door.DoorDirection.Up => Door.DoorDirection.Down,
            Door.DoorDirection.Down => Door.DoorDirection.Up,
            Door.DoorDirection.Left => Door.DoorDirection.Right,
            Door.DoorDirection.Right => Door.DoorDirection.Left,
            _ => Door.DoorDirection.Up,
        };
    }
}
