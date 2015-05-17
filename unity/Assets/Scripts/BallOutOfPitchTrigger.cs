using UnityEngine;
using System.Collections;

/// <summary>
/// Component class attached to triggers.
/// This is intended to position the ball in the pitch again, when the physics fail(walls).
/// </summary>
public class BallOutOfPitchTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        // only on server
        if (Network.isServer && other.gameObject.tag == "Ball")
        {
            // decide if will position the ball to the blue player or the red player
            other.gameObject.transform.localPosition = new Vector3(0, 8.3f, (other.gameObject.transform.localPosition.z < 0 ? -60.6f : 60.6f));
            other.gameObject.rigidbody.velocity = Vector3.zero;
        }
    }
}
