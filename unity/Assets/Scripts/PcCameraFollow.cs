using UnityEngine;
using System.Collections;

/// <summary>
/// Camera follow script used on the pc version.
/// </summary>
public class PcCameraFollow : MonoBehaviour
{
    public Transform target = null;
    public float smooth = 1.0f;

    void Update()
    {
        if (target != null)
            LerpTransform(transform, target, Time.deltaTime * smooth);
    }

    public void LerpTransform(Transform trans, Transform target, float t)
    {
        trans.position = Vector3.Lerp(trans.position, target.position, t);
        trans.rotation = Quaternion.Lerp(trans.rotation, target.rotation, t);
    }
}
