using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

using System.Linq;

public class PlayerSync : MonoBehaviourPunCallbacks
{
    public UnityEngine.UI.Text playerNameLabel;

    private CarMovementController cmController;
    private Camera carCam;

    // Start is called before the first frame update
    void Start()
    {
        cmController = GetComponentInChildren<CarMovementController>();
        carCam = GetComponentInChildren<Camera>();

        /*
     if(photonView.IsMine)
        {
            cmController.enabled = true;
            carCam.enabled = true;
        }
     else
        {
            cmController.enabled = false;
            carCam.enabled = false;
        }
        */

        //the above block of code can be re-written in two lines as follows:
        cmController.enabled = photonView.IsMine;
        carCam.gameObject.SetActive(photonView.IsMine);
        //enable lap controller only if it belongs to our photonView.
        GetComponent<LapController>().enabled = photonView.IsMine;
 
      if (GameManager.instance.playerRanks == null) GameManager.instance.playerRanks = new List<GO_ID_Duo>();

       GO_ID_Duo duo = new GO_ID_Duo(gameObject, photonView.ViewID);
        if (!GameManager.instance.playerRanks.Exists(x=>x.viewID==photonView.ViewID))
            GameManager.instance.playerRanks.Add(duo);



        if (playerNameLabel != null)
        {
            playerNameLabel.text = photonView.Owner.NickName;

            if (photonView.IsMine)
                playerNameLabel.gameObject.SetActive(false);

        }

    }
 
}
