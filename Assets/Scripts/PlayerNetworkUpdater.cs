using UnityEngine;
using System.Collections;

/// <summary>
/// THIS IS A CUSTOM OBSEVER.
/// Class that contains the OnSerializeNetworkView method that updates the player in the network.
/// This script is observed by the players networkview.
/// The same function will be called when sending or receiving information.
/// In the case of this game, one insance will always write and other instance will always read.
/// But imagine a player that both users are able to controll, the same function for both would be used to 
/// read and/or write, at the same time.
/// </summary>
public class PlayerNetworkUpdater : MonoBehaviour
{
    [SerializeField]
    private GameObject shootEffect;

    // This function is called by unity message.
    private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (stream.isWriting)
        {
            // when sending information
            Vector3 pos = transform.position;
            stream.Serialize(ref pos);
            bool shootEffectEnabled = shootEffect.activeInHierarchy;
            stream.Serialize(ref shootEffectEnabled);
        }
        else
        {
            // when receiving information
            Vector3 pos = Vector3.zero;
            bool shootEffectEnabled = false;

            stream.Serialize(ref pos);
            stream.Serialize(ref shootEffectEnabled);

            transform.position = pos;
            shootEffect.SetActive(shootEffectEnabled);
        }
    }
}
