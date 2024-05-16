using UnityEngine;

public class FollowCameraRotation : MonoBehaviour
{
    Transform target;

    void Start()
    {
        if (target == null)
        {
            target = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + target.forward);
    }
}
