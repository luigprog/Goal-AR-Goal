using UnityEngine;
using System.Collections;
using UnitySampleAssets.CrossPlatformInput;

/// <summary>
/// Class that deals with all the player's shoot functionality.
/// </summary>
public class PlayerShoot : MonoBehaviour
{
    #region Fields
    // inspector
    public float minPower;

    public float maxPower;

    public float curve;

    public GameObject shootEffect;
    // others
    private GameObject ball;

    private BallInfo ballInfo; // ball component info

    /// <summary>
    /// Slider used to measure the shoot's power.
    /// These crazy values is set while debugging, this is not meant to change.
    /// </summary>
    private Slider sliderDistance = new Slider(4.111784f, 6.408222f);

    private PlayerInfo playerInfo;

    private NetworkView myNetworkView;

    #endregion

    #region Initiators

    private void Awake()
    {
        playerInfo = GetComponent<PlayerInfo>();
        myNetworkView = GetComponent<NetworkView>();
    }

    private void Start()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
        ballInfo = ball.GetComponent<BallInfo>();
    }

    #endregion

    #region Update

    void Update()
    {
        if (playerInfo.haveControl)
        {
            #region Update Shoot Code

            // shoot code, for android and pc \/
            // turn shoot on and off
#if UNITY_ANDROID
            if (CrossPlatformInputManager.GetButtonDown("Jump"))
            {
                if(Network.isServer)
                {
                    ShootOn();
                } 
                else 
                {
                    myNetworkView.RPC("ShootOn", RPCMode.Server);
                }
            }

            if (CrossPlatformInputManager.GetButtonUp("Jump"))
            {
                if (Network.isServer)
                {
                    ShootOff();
                }
                else
                {
                    myNetworkView.RPC("ShootOff", RPCMode.Server);
                }
            }
#else
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Network.isServer)
                {
                    ShootOn();
                }
                else
                {
                    myNetworkView.RPC("ShootOn", RPCMode.Server);
                }
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (Network.isServer)
                {
                    ShootOff();
                }
                else
                {
                    myNetworkView.RPC("ShootOff", RPCMode.Server);
                }
            }
#endif

            #endregion

            #region Update Apply Curve Code
            // apply curve code \/
            if (ballInfo.CanApplyCurve(gameObject))
            {
                // setup the curve direcion, crossing the kick direction with the up vector
                // we can also think about this curve direction vector, as the right vector of the "kick"

                Vector3 curveDirection = Vector3.zero;
                if (MovementInput.GetMovementDirectionVector().x > 0.0f)
                {
                    curveDirection = Vector3.Cross(ballInfo.GetLastKickDirection(), Vector3.up);
                }
                else if (MovementInput.GetMovementDirectionVector().x < 0.0f)
                {
                    curveDirection = -Vector3.Cross(ballInfo.GetLastKickDirection(), Vector3.up);
                }

                if (curveDirection != Vector3.zero)
                {
                    if (Network.isServer)
                    {
                        ApplyCurve(curveDirection * curve);
                    }
                    else
                    {
                        myNetworkView.RPC("ApplyCurve", RPCMode.Server, curveDirection * curve);
                    }
                }
            }
            #endregion
        }
    }

    /// <summary>
    /// Method called by message. The ShootTrigger script send that message.
    /// </summary>
    /// <param name="other"></param>
    public void OnShootEffectTriggerEnter(Collider other)
    {
        ball = other.gameObject; // refresh the ball reference

        // get the distance between the player and the ball
        float currDistance = Vector3.Distance(new Vector3(transform.position.x, 0.0f, transform.position.z),
                                   new Vector3(ball.transform.position.x, 0.0f, ball.transform.position.z));
        // calculate the shoot's power based on this distance
        float powerPoint = sliderDistance.GetPoint(currDistance);
        float powerValue = new Slider(maxPower, minPower).GetValue(powerPoint);

        Vector3 shootDirection = (ball.transform.position - gameObject.transform.position).normalized;

        ballInfo.MyRigidbody.AddForce(shootDirection * powerValue);
        // set the kick information
        ballInfo.MyNetworkView.RPC("SetLastKickInfo", RPCMode.All, gameObject.name, shootDirection);
    }

    #endregion

    #region RPC's

    /// <summary>
    /// Curves the ball.
    /// Executed always in the server.
    /// Called from server and client.
    /// </summary>
    /// <param name="curveForce"></param>
    [RPC]
    private void ApplyCurve(Vector3 curveForce)
    {
        ballInfo.MyRigidbody.AddForce(curveForce);
    }

    /// <summary>
    /// Turn shooteffect on.
    /// Executed always in the server.
    /// Called from server and client.
    /// </summary>
    [RPC]
    private void ShootOn()
    {
        shootEffect.SetActive(true);
    }

    /// <summary>
    /// Turn shoot effecti off.
    /// Executed always in the server.
    /// Called from server and client.
    /// </summary>
    [RPC]
    private void ShootOff()
    {
        shootEffect.SetActive(false);
    }

    #endregion

}
