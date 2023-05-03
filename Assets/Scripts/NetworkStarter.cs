using UnityEngine;
using System.Collections;

/// <summary>
/// This class deals with the network connection basics.
/// Server creation, connection to a server.
/// </summary>
public class NetworkStarter : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// Registered name. Must be unique. Used to identify the game at the MasterServer.
    /// </summary>
    private string registeredGameName = "Luigi_Garcia_AR_Golagol";

    /// <summary>
    /// Time used to refresh host list.
    /// </summary>
    private float refreshRequestLenght = 3.0f;

    private bool useNat = false;

    /// <summary>
    /// Array that holds information about hosts/sessions. Servers already created.
    /// Updated on refresh host list.
    /// </summary>
    private HostData[] hostData;

    /// <summary>
    /// Reference to the current insance network player.
    /// </summary>
    public NetworkPlayer myPlayer;

    private NetworkView myNetworkView;

    /// <summary>
    /// Status used to controll the placeholder GUI.
    /// </summary>
    private NetworkScreenStatusEnum status;

    /// <summary>
    /// Instance to this NetworkStarter. 
    /// There is only one in the scene.
    /// Is set inside the Awake method.
    /// </summary>
    public static NetworkStarter instance;
    #endregion

    #region Initiators
    private void Awake()
    {
        instance = this;
        status = NetworkScreenStatusEnum.MAIN_SCREEN;
        myNetworkView = GetComponent<NetworkView>();
    }
    #endregion

    #region GUI
    /// <summary>
    /// Draws the GUI.
    /// The networking start point.
    /// </summary>
    public void OnGUI()
    {
        switch (status)
        {
            case NetworkScreenStatusEnum.MAIN_SCREEN:
                if (!Network.isClient && !Network.isServer)
                {
                    // you are not a client or a server
                    useNat = GUI.Toggle(new Rect(200f, 25f, 150f, 30f), useNat, "Use Nat");
                    if (GUI.Button(new Rect(25f, 25f, 150f, 30f), "Start New Server"))
                    {
                        StartServer();
                    }

                    if (GUI.Button(new Rect(25f, 65, 150f, 30f), "Refresh Server List"))
                    {
                        StartCoroutine("RefreshHostList");
                    }

                    GUI.Label(new Rect(25f, 110, 500f, 30f), "Tip 1: To curve the ball, move your player to left/right after a kick.");
#if !UNITY_ANDROID
                    GUI.Label(new Rect(25f, 140f, 500f, 30f), "Tip 2: To rotate the camera, use the J and L keys.");
#endif

                    // refresh hosts and connect to host
                    if (hostData != null)
                    {
                        for (int i = 0; i < hostData.Length; i++)
                        {
                            if (GUI.Button(new Rect(Screen.width / 2, 65f + (30f * i), 300f, 30f), hostData[i].gameName))
                            {
                                Network.Connect(hostData[i]);
                            }
                        }
                    }

                }
                break;
            case NetworkScreenStatusEnum.SERVER_SCREEN:
                GUILayout.Label("Waiting the opponent...");
                break;
            case NetworkScreenStatusEnum.INGAME:
                break;
            default:
                break;
        }

    }

    #endregion

    #region StartServer/ConnectToServer

    /// <summary>
    /// Start a server, using MasterServer
    /// </summary>
    private void StartServer()
    {
        Network.InitializeServer(2, 25002, useNat);
        MasterServer.RegisterHost(registeredGameName, "Game Room", "...");
    }

    /// <summary>
    /// Coroutine that populate the current hosts.
    /// </summary>
    /// <returns></returns>
    public IEnumerator RefreshHostList()
    {
        Debug.Log("Refreshing...");
        MasterServer.RequestHostList(registeredGameName);
        float timeEnd = Time.time + refreshRequestLenght;

        while (Time.time < timeEnd)
        {
            hostData = MasterServer.PollHostList();
            yield return new WaitForEndOfFrame();
        }

        if (hostData == null || hostData.Length == 0)
        {
            Debug.Log("No active servers has been found.");
        }
        else
        {
            Debug.Log(hostData.Length + " servers has been found.");
        }

    }

    #endregion

    #region RPCs

    /// <summary>
    /// This method is always executed in the server.
    /// Clients call this via RPC.
    /// Instantiate both blue(server) and red(client) players.
    /// </summary>
    /// <param name="thisPlayer">The network player from where the method was called.</param>
    [RPC]
    private void MakePlayer(NetworkPlayer thisPlayer)
    {
        const float START_X = 0.0f;
        const float START_Y = 1.136205f;
        const float START_Z = 49.2976f;

        if (thisPlayer != myPlayer) // is the client who called this [RPC] function
        {
            GameObject go = Network.Instantiate(GameController.instance.playerRedPrefab, new Vector3(START_X, START_Y, START_Z), new Quaternion(0.0f, 1.0f, 0.0f, 0.0f), 0) as GameObject;
            // the object's parent must be the AR context.
            go.transform.parent = GameController.instance.arGame.transform;
            // call the enable method. Send the RPC call targeting thisPlayer(here it is the client networkplayer)
            myNetworkView.RPC("EnablePlayer", thisPlayer, go.GetComponent<NetworkView>().viewID);
            // the client can disable physics to improve performance
            myNetworkView.RPC("DisablePhysics", RPCMode.Others);
        }
        else // is the server who called this function
        {
            GameObject go = Network.Instantiate(GameController.instance.playerBluePrefab, new Vector3(START_X, START_Y, -START_Z), Quaternion.identity, 0) as GameObject;
            go.transform.parent = GameController.instance.arGame.transform;
            EnablePlayer(go.GetComponent<NetworkView>().viewID);
        }
    }

    /// <summary>
    /// This method decide which player the current instance can control.
    /// It also setup the cameras.
    /// Method executed in both server and client, but always called from a server.
    /// </summary>
    /// <param name="playerID"></param>
    [RPC]
    private void EnablePlayer(NetworkViewID playerID)
    {
        GameObject[] players;
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (p.GetComponent<NetworkView>().viewID == playerID)
            {
                // can controll this player
                p.GetComponent<PlayerInfo>().haveControl = true;
#if UNITY_ANDROID
#else
                GameController.instance.playerCameraPc.gameObject.SetActive(true);
                GameController.instance.playerCameraPc.gameObject.GetComponent<PcCameraFollow>().target = p.GetComponent<PlayerInfo>().pcCameraAnchor.transform;
#endif
                GameController.instance.menuCameraGO.SetActive(false);
                break;
            }
        }
    }

    /// <summary>
    /// This method disable ball and player physics, to improve performance.
    /// Always executed in the client, always called by the server.
    /// </summary>
    [RPC]
    private void DisablePhysics()
    {
        // called on the client
        GameObject[] players;
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            Destroy(p.GetComponent<Rigidbody>());
        }

        Destroy(GameController.instance.ballReference.GetComponent<Rigidbody>());
    }

    #endregion

    #region Network Callbacks

    #region Client

    private void OnConnectedToServer()
    {
        myPlayer = Network.player;
        // ask for player creation
        myNetworkView.RPC("MakePlayer", RPCMode.Server, myPlayer);
        status = NetworkScreenStatusEnum.INGAME;
    }

    // not used
    //private void OnDisconnectedFromServer(NetworkDisconnection INFO) { }
    //private void OnFailedToConnect(NetworkConnectionError error) { }

    #endregion

    #region Server

    /// <summary>
    /// Called when the host was registered in the MasterServer corretly.
    /// </summary>
    /// <param name="masterServerEvent"></param>
    private void OnMasterServerEvent(MasterServerEvent masterServerEvent)
    {
        if (masterServerEvent == MasterServerEvent.RegistrationSucceeded)
        {
            Debug.Log("Registration successful!");
        }
    }

    private void OnServerInitialized()
    {
        myPlayer = Network.player;
        // waiting for a client/opponent screen
        status = NetworkScreenStatusEnum.SERVER_SCREEN;
    }

    /// <summary>
    /// Before this method get called, the server was waiting for opponent.
    /// This method is called when a client connects.
    /// This method call the server player creation, and starts the game.
    /// </summary>
    /// <param name="player"></param>
    private void OnPlayerConnected(NetworkPlayer player)
    {
        MakePlayer(myPlayer); // now that the opponent(client) is connected, create the server player
        status = NetworkScreenStatusEnum.INGAME;
    }

    private void OnPlayerDisconnected(NetworkPlayer player)
    {
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }

    // not used
    //private void OnFailedToConnectToMasterServer(NetworkConnectionError info) { }
    //private void OnNetworkInstantiate(NetworkMessageInfo info) { }

    #endregion

    #region Both

    private void OnApplicationQuit()
    {
        if (Network.isServer)
        {
            Network.Disconnect(200);
            MasterServer.UnregisterHost();
        }
        else if (Network.isClient)
        {
            Network.Disconnect(200);
        }
    }

    #endregion

    #endregion
}