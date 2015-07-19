using UnityEngine;
using System.Collections;

/// <summary>
/// Holds information about the ball.
/// Also provides infos about the ball curve functionality.
/// </summary>
public class BallInfo : MonoBehaviour
{
    /// <summary>
    /// The name of the last player who kicked the ball
    /// </summary>
    private string lastKickerName = "";

    /// <summary>
    /// The direction of the last kick.
    /// This is used to curve the ball properly.
    /// </summary>
    private Vector3 lastKickDirection = Vector3.zero;

    private Rigidbody myRigidbody;

    private NetworkView myNetworkView;

    private bool canCurve = false;

    public Rigidbody MyRigidbody
    {
        get { return myRigidbody; }
    }

    public NetworkView MyNetworkView
    {
        get { return myNetworkView; }
    }

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myNetworkView = GetComponent<NetworkView>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // only executed in the server
        if (Network.isServer && collision.gameObject.name != "ground" && collision.gameObject.name != lastKickerName)
        {
            // ball collided with something else, set the can curve to false
            myNetworkView.RPC("SetCanCurveFalse", RPCMode.All);
        }
    }

    public Vector3 GetLastKickDirection()
    {
        return lastKickDirection;
    }

    /// <summary>
    /// Method called by the player shoot.
    /// </summary>
    /// <param name="p">The player.</param>
    /// <returns>true if this player can apply curve, false if it cant.</returns>
    public bool CanApplyCurve(GameObject p)
    {
        if (p.gameObject.name == lastKickerName && canCurve)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Setup information about the last kick.
    /// RPC called by the PlayerShoot.
    /// </summary>
    /// <param name="kickerName"></param>
    /// <param name="kickDirection"></param>
    [RPC]
    public void SetLastKickInfo(string kickerName, Vector3 kickDirection)
    {
        lastKickerName = kickerName;
        lastKickDirection = kickDirection;
        canCurve = true;
    }

    [RPC]
    private void SetCanCurveFalse()
    {
        canCurve = false;
    }
}