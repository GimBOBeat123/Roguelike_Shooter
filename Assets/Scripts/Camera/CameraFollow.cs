using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public IEnumerator MoveToRoom(Vector3 target)
    {
        Vector3 start = transform.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 3f;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }
    }
}
