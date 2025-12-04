using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector2Int gridPosition;

    [Header("Doors (Å¸ÀÏ¸Ê)")]
    public GameObject upDoorVisual;
    public GameObject downDoorVisual;
    public GameObject leftDoorVisual;
    public GameObject rightDoorVisual;

    [Header("Door Triggers (Collider¿ë)")]
    public Transform upDoor;
    public Transform downDoor;
    public Transform leftDoor;
    public Transform rightDoor;
    public Transform GetDoor(Door.DoorDirection direction)
    {
        return direction switch
        {
            Door.DoorDirection.Up => upDoor,
            Door.DoorDirection.Down => downDoor,
            Door.DoorDirection.Left => leftDoor,
            Door.DoorDirection.Right => rightDoor,
            _ => null
        };
    }

    public void SetupDoorsVisual(Dictionary<Vector2Int, Room> allRooms)
    {
        bool upConnected = allRooms.ContainsKey(gridPosition + Vector2Int.up);
        bool downConnected = allRooms.ContainsKey(gridPosition + Vector2Int.down);
        bool leftConnected = allRooms.ContainsKey(gridPosition + Vector2Int.left);
        bool rightConnected = allRooms.ContainsKey(gridPosition + Vector2Int.right);

        if (upDoorVisual != null) upDoorVisual.SetActive(upConnected);
        if (downDoorVisual != null) downDoorVisual.SetActive(downConnected);
        if (leftDoorVisual != null) leftDoorVisual.SetActive(leftConnected);
        if (rightDoorVisual != null) rightDoorVisual.SetActive(rightConnected);
    }
}
