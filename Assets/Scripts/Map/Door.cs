using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorDirection { Up, Down, Left, Right }
    public DoorDirection direction;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Map.Instance != null && Map.Instance.CanTransition)
        {
            Map.Instance.MoveToNextRoom(direction);
        }
    }
}
