using UnityEngine;
using System.Collections;
using UnitySampleAssets.CrossPlatformInput;

/// <summary>
/// Class that holds a Service of input.
/// This work for both pc version and android version.
/// </summary>
public class MovementInput
{
    /// <summary>
    /// Get the direction of the movement input, digital stick or a/w/s/d.
    /// </summary>
    /// <returns>The direction normalized.</returns>
    public static Vector3 GetMovementDirectionVector()
    {
#if UNITY_ANDROID
        return GuiCircleMovement.instance.GetMovementDirection();
#else
        return new Vector3()
        {
            x = Input.GetAxis("Horizontal"),
            y = 0.0f,
            z = Input.GetAxis("Vertical")
        }.normalized;
#endif
    }

}
