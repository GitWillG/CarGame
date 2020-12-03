using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
/// Make sure this script is placed on each car's prefab
/// </summary>
public class LapController : MonoBehaviourPun
{

     /// <summary>
    /// Detect which trigger we passed through
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

        //Check if the 'other' collider object is 
        //one of our lap trigger game objects.
        //we are only interested in finding if our car
        //passed through one of the lap triggers...
        if(GameManager.instance.lapTriggerPts.Contains(other.transform))
        {
            //check if we reached the finish lap trigger
            if(other.name=="FinishTrigger")
            {
                Debug.Log("Car reached the finish point...");
                EndRace();
            }
            else //we passed through one of the lap triggers
            {
                //TO DO::
                //Send an update to refresh the car standings UI
                
            }



        }

    }


    /// <summary>
    /// Called via OnTriggerEnter above when a car enters the finish trigger
    /// </summary>
    void EndRace()
    {
        //Step 1: When the race ends, we detach the camera from the car
   if(photonView.IsMine)
            GetComponentInChildren<Camera>().transform.parent = null;
        //Step 2: disable the car movement script so that the player controlling the car can no longer control it once it crosses the finish line
        GetComponent<CarMovementController>().enabled = false;

 
        //construct the send data array
        //[0]: string for our playerName
        //[1]: int for our rank
        //[2]: int for the photon's ViewID
        string currentPlayerNickName = photonView.Owner.NickName;

        object[] data = new object[] { currentPlayerNickName,photonView.ViewID };

        //How to raise event?
        RaiseEventOptions reo = new RaiseEventOptions();
      //  reo.CachingOption = EventCaching.AddToRoomCache;
        reo.Receivers = ReceiverGroup.All; //send the data to all clients, including the masterclient

        //construct sendOptions

        SendOptions so = new SendOptions();
        so.Reliability = false; ///set this to false so that we overwrite any previous data

        PhotonNetwork.RaiseEvent((byte)GameManager.raiseEventCodes.raceFinishUpdateRank, data, new RaiseEventOptions()
        { Receivers=ReceiverGroup.MasterClient}, so);


 
        Debug.Log("End Race was called on "+photonView.ViewID);

    }
 
     private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += GameManager.instance.OnEventCallback;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= GameManager.instance.OnEventCallback;
    }

}
