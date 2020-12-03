using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roomManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Transform _content;
    [SerializeField]
    roomListing _roomListing;
    // Update is called once per frame
    void Update()
    {
          
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomInfo r in roomList)
        {
            roomListing listing = Instantiate(_roomListing, _content);
            if(listing != null)
            {
                listing.setRoomInfo(r);
            }
        }
    }

 
}
