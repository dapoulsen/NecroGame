using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 0.9f, 0f);

    void LateUpdate() => transform.position = target.position + offset;
}
