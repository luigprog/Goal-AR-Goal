using UnityEngine;
using System.Collections;

/// <summary>
/// Class that holds the player's move functionality.
/// </summary>
public class PlayerMovement : MonoBehaviour 
{
    public float speed;
    private PlayerInfo playerInfo;

    void Awake() 
    {
        playerInfo = GetComponent<PlayerInfo>();
    }

	void Update () 
    {
        if (playerInfo.haveControl)
        {
            Vector3 camForward = GameController.instance.PlayerCamera.transform.forward;
            camForward.y = 0;
            camForward.Normalize();
            Vector3 camRight = GameController.instance.PlayerCamera.transform.right;

            if (Network.isServer)
                MovePlayer(MovementInput.GetMovementDirectionVector().x * camRight * speed, MovementInput.GetMovementDirectionVector().z * camForward * speed);
            else
                networkView.RPC("MovePlayer", RPCMode.Server, MovementInput.GetMovementDirectionVector().x * camRight * speed, MovementInput.GetMovementDirectionVector().z * camForward * speed);
        }
    }

    /// <summary>
    /// Executed only on the server.
    /// Move the player by physics.
    /// </summary>
    /// <param name="forceX"></param>
    /// <param name="forceZ"></param>
    [RPC]
    private void MovePlayer(Vector3 forceX, Vector3 forceZ) 
    { 
        rigidbody.AddForce(forceX);
        rigidbody.AddForce(forceZ);
    }

}
