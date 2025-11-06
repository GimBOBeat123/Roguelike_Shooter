using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerController player;
    public CameraFollow cam;

    public float roomWidth = 20f;
    public float roomHeight = 12f;

    private bool isTransitioning = false;

    private Dictionary<Vector2Int, Room> roomMap = new();
    private Vector2Int currentRoomPos = Vector2Int.zero;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 모든 방 등록
        Room[] rooms = FindObjectsOfType<Room>();
        foreach (Room room in rooms)
        {
            Vector2Int gridPos = new(
                Mathf.RoundToInt(room.transform.position.x / roomWidth),
                Mathf.RoundToInt(room.transform.position.y / roomHeight)
            );
            roomMap[gridPos] = room;
        }
    }

    public void EnterNewRoom(Door door)
    {
        if (isTransitioning) return;

        StartCoroutine(RoomTransition(door.direction));
    }

    private IEnumerator RoomTransition(Door.DoorDirection dir)
    {
        isTransitioning = true;
        player.SetCanMove(false);

        Vector2Int nextPos = currentRoomPos;
        switch (dir)
        {
            case Door.DoorDirection.Up: nextPos += Vector2Int.up; break;
            case Door.DoorDirection.Down: nextPos += Vector2Int.down; break;
            case Door.DoorDirection.Left: nextPos += Vector2Int.left; break;
            case Door.DoorDirection.Right: nextPos += Vector2Int.right; break;
        }

        if (!roomMap.ContainsKey(nextPos))
        {
            Debug.LogWarning("다음 방이 없음!");
            player.SetCanMove(true);
            isTransitioning = false;
            yield break;
        }

        Room nextRoom = roomMap[nextPos];
        Room currentRoom = roomMap[currentRoomPos];

        // 이동 오프셋 계산
        Vector3 offset = new Vector3(
            (nextPos.x - currentRoomPos.x) * roomWidth,
            (nextPos.y - currentRoomPos.y) * roomHeight,
            0
        );

        // 카메라 이동
        Vector3 targetCamPos = cam.transform.position + offset;
        yield return StartCoroutine(cam.MoveToRoom(targetCamPos));

        // 플레이어 위치를 다음 방의 반대 문 근처로 이동
        Door.DoorDirection oppositeDir = GetOppositeDirection(dir);
        player.transform.position = nextRoom.GetDoor(oppositeDir).position;

        currentRoomPos = nextPos;

        yield return new WaitForSeconds(0.1f);
        player.SetCanMove(true);
        isTransitioning = false;
    }

    private Door.DoorDirection GetOppositeDirection(Door.DoorDirection dir)
    {
        return dir switch
        {
            Door.DoorDirection.Up => Door.DoorDirection.Down,
            Door.DoorDirection.Down => Door.DoorDirection.Up,
            Door.DoorDirection.Left => Door.DoorDirection.Right,
            Door.DoorDirection.Right => Door.DoorDirection.Left,
            _ => dir
        };
    }
}
