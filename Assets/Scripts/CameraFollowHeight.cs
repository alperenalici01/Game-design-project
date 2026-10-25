using UnityEngine;

public class CameraFollowHeight : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float extraHeight = -2f;

    private float heightOffset;

    void Start()
    {
        if (target == null) return;
        heightOffset = transform.position.y - target.position.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 pos = transform.position;
        pos.y = target.position.y + heightOffset + extraHeight;
        transform.position = pos;
    }
}