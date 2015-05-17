using UnityEngine;
using System.Collections;

/// <summary>
/// Class that contains the OnSerializeNetworkView method that updates the player in the network.
/// This script is observed by the players networkview.
/// </summary>
public class PlayerNetworkUpdater : MonoBehaviour
{
    [SerializeField]
    private GameObject shootEffect;

    private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (stream.isWriting)
        {
            Vector3 pos = transform.position;
            stream.Serialize(ref pos);
            bool shootEffectEnabled = shootEffect.activeInHierarchy;
            stream.Serialize(ref shootEffectEnabled);
        }
        else
        {
            Vector3 pos = Vector3.zero;
            bool shootEffectEnabled = false;

            stream.Serialize(ref pos);
            stream.Serialize(ref shootEffectEnabled);

            transform.position = pos;
            shootEffect.SetActive(shootEffectEnabled);
        }
    }
}
