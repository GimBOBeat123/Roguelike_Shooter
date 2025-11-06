using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector2Int gridPosition; // ¸Ê »ó À§Ä¡
    [Header("Doors")]
    public Transform upDoor;
    public Transform downDoor;
    public Transform leftDoor;
    public Transform rightDoor;

    public Transform GetDoor(Door.DoorDirection direction)
    {
        switch (direction)
        {
            case Door.DoorDirection.Up: return upDoor;
            case Door.DoorDirection.Down: return downDoor;
            case Door.DoorDirection.Left: return leftDoor;
            case Door.DoorDirection.Right: return rightDoor;
        }
        return null;
    }
}
