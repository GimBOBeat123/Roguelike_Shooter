using UnityEngine;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
    public int roomCount = 10;
    public GameObject roomPrefab;
    private Dictionary<Vector2Int, Room> rooms = new();
    public float roomWidth = 20f;
    public float roomHeight = 12f;

    private PlayerController player;
    private Camera mainCam;

    void Start()
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
    }

    Vector2Int GetRandomDirection()
    {
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        return dirs[Random.Range(0, dirs.Length)];
    }

    public void MoveToNextRoom(Door.DoorDirection dir)
    {
        Room current = player.currentRoom;
        Vector2Int nextPos = current.gridPosition;

        switch (dir)
        {
            case Door.DoorDirection.Up: nextPos += Vector2Int.up; break;
            case Door.DoorDirection.Down: nextPos += Vector2Int.down; break;
            case Door.DoorDirection.Left: nextPos += Vector2Int.left; break;
            case Door.DoorDirection.Right: nextPos += Vector2Int.right; break;
        }

        if (rooms.ContainsKey(nextPos))
        {
            Room nextRoom = rooms[nextPos];
            player.SetCanMove(false);

            // 방 전환 애니메이션처럼 살짝 텀 줌
            player.transform.position = nextRoom.GetDoor(Opposite(dir)).position;
            player.currentRoom = nextRoom;

            // 카메라도 방 중심으로 이동
            mainCam.transform.position = new Vector3(nextRoom.transform.position.x, nextRoom.transform.position.y, mainCam.transform.position.z);

            // 이동 제한 해제
            player.Invoke(nameof(EnableMove), 0.2f);
        }
        else
        {
            Debug.Log("다음 방이 없습니다");
        }
    }

    Door.DoorDirection Opposite(Door.DoorDirection dir)
    {
        switch (dir)
        {
            case Door.DoorDirection.Up: return Door.DoorDirection.Down;
            case Door.DoorDirection.Down: return Door.DoorDirection.Up;
            case Door.DoorDirection.Left: return Door.DoorDirection.Right;
            case Door.DoorDirection.Right: return Door.DoorDirection.Left;
        }
        return Door.DoorDirection.Up;
    }

    void EnableMove()
    {
        player.SetCanMove(true);
    }
}
