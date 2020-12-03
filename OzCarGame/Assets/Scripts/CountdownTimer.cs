using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class CountdownTimer : MonoBehaviourPun
{

    public float countdownStartValue = 5.0f;



    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        StartTimer();
    }

    void StartTimer()
    {
        timer = countdownStartValue;
    }



    //We want to make sure this function is only called on the master client.
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return; //exit this function if this script is not running in the masterclient

        #region TIMER_LOGIC
        if(timer>0.0f)
        {
            timer -= Time.deltaTime;
            photonView.RPC("UpdateTimerText", RpcTarget.All, timer);
        }
        else
        {
            photonView.RPC("StartRace", RpcTarget.All);
        }
        #endregion



    }


    [PunRPC]
    void StartRace()
    {
        GetComponent<CarMovementController>().EnableMovement();
    }


    //RPC = Remote Procedure Call.
    //When you mark a function with 'PunRPC' you denote that this function can be
    //called remotely from another client
    [PunRPC]
    void UpdateTimerText(float time)
    {
        if(time>0.0f)
        GameManager.instance.countdownTextLabel.text = Mathf.RoundToInt(time).ToString();
        else
        {
            GameManager.instance.countdownTextLabel.text = "";
            GameManager.instance.countdownTextLabel.transform.parent.gameObject.SetActive(false);
        }

    }


}
