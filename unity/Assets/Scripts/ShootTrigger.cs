using UnityEngine;
using System.Collections;

/// <summary>
/// Component class attached to the shootEffect objet in both players.
/// This will trigger and send a message to the player that the kick area hit the ball.
/// </summary>
public class ShootTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        // only in the server
        if (Network.isServer && other.gameObject.tag == "Ball")
            transform.parent.gameObject.SendMessage("OnShootEffectTriggerEnter", other);
    }
}
