using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour
{
    /// <summary>
    /// If this game instance(networkplayer)  has controll of this object or not.
    /// This flag will be used to allow movement, for example.
    /// </summary>
    public bool haveControl = false;

    public GameObject pcCameraAnchor;
}
