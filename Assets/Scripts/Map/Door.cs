using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorDirection { Up, Down, Left, Right }
    public DoorDirection direction;

    private Map map;

    void Start()
    {
        map = FindObjectOfType<Map>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            map.MoveToNextRoom(direction);
        }
    }
}
