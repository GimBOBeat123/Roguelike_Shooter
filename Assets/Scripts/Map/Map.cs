using UnityEngine;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
    public int roomCount = 10;
    public GameObject roomPrefab;
    private Dictionary<Vector2Int, GameObject> rooms = new();

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        Vector2Int currentPos = Vector2Int.zero;
        rooms[currentPos] = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);

        for (int i = 0; i < roomCount - 1; i++)
        {
            Vector2Int nextPos = currentPos + GetRandomDirection();

            // 이미 방이 있으면 다른 방향 탐색
            int safety = 0;
            while (rooms.ContainsKey(nextPos) && safety < 10)
            {
                nextPos = currentPos + GetRandomDirection();
                safety++;
            }

            if (!rooms.ContainsKey(nextPos))
            {
                Vector3 worldPos = new Vector3(nextPos.x * 20, nextPos.y * 12, 0);
                rooms[nextPos] = Instantiate(roomPrefab, worldPos, Quaternion.identity);
                currentPos = nextPos;
            }
        }
    }

    Vector2Int GetRandomDirection()
    {
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        return dirs[Random.Range(0, dirs.Length)];
    }
}
