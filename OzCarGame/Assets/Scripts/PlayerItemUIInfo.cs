using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemUIInfo : MonoBehaviour
{
    public Text playerName;
    public Button readyBtn;
    public Image readyImg;

    

    public void Init(int playerNum, string pName)
    {
        playerName.text = playerNum + ". " + pName;

        //We don't show the 'ready button' if the player is not the 'local player'
        //only the 'local player' can press the 'ready' button....
        if(PhotonNetwork.LocalPlayer.ActorNumber!=playerNum)
        {
            readyBtn.gameObject.SetActive(false);
        }
        else // we are a local player. so the readybutton should be active (as it is by default)
        {

        }

    }


    /// <summary>
    /// Call this on the playerListItem's 'playerReadyButton'
    /// </summary>
    public void OnReadyButtonClicked()
    {

        readyBtn.gameObject.SetActive(false);
        SetReadyState(true);

        //Transmit to photon network that our local player just pressed ready button...
        ExitGames.Client.Photon.Hashtable properties =
            new ExitGames.Client.Photon.Hashtable() { { "pReady", true } };

        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);

    }


    /// <summary>
    /// THis is called on a localplayer's instance via OnReadyButtonClicked()
    /// Also for every remote player that joins the room (and when a remote player marks as ready),
    /// this function will be called as well (from NetworkMgr) 
    /// </summary>
    /// <param name="isReady"></param>
    public void SetReadyState(bool isReady)
    {
        if(isReady)
        {
            readyImg.enabled = true;
        }

    }



}
