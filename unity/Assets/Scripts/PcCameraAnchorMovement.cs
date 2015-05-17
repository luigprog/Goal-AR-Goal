using UnityEngine;
using System.Collections;

/// <summary>
/// Script used by the pc camera anchor.
/// The pc camera anchor is the PcCameraFollow target.
/// This script rotates this anchor around the player.
/// </summary>
public class PcCameraAnchorMovement : MonoBehaviour
{
    public float rotateAroundSpeed;

    void Update()
    {
        if (Input.GetKey(KeyCode.J))
            transform.RotateAround(transform.parent.transform.position, Vector3.up, -rotateAroundSpeed * Time.deltaTime);
        else if (Input.GetKey(KeyCode.L))
            transform.RotateAround(transform.parent.transform.position, Vector3.up, rotateAroundSpeed * Time.deltaTime);
    }
}
