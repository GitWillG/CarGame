using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class roomListing : MonoBehaviour
{
    [SerializeField]
    Text _text;

    public void setRoomInfo(RoomInfo info)
    {
        _text.text = info.PlayerCount + " / " + info.MaxPlayers + ", ";
    }
}
