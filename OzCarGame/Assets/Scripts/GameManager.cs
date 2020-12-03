using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Linq;
using Photon.Realtime;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GO_ID_Duo
{
 

    public PhotonView pv { get => go.GetComponent<PhotonView>(); }
    public int rank;
    public GameObject go;
    public int viewID;
    public GO_ID_Duo(GameObject _go, int vi) { viewID = vi; go = _go; rank = 0; }
}


public class GameManager : MonoBehaviourPun
{
    private int lastAssignedRank = 0;
    public GameObject endText;

    //key=a car's gameObject reference in the scene
    //value=the rank of that car
    public List<GO_ID_Duo> playerRanks;
    private int selectedCar;

    /// <summary>
    /// these are the possible event codes for all of our events that could occur in our game
    /// </summary>
    public enum raiseEventCodes
    {
        raceFinishCode=0,
        raceFinishUpdateRank=1 
    };

    public UnityEngine.UI.Text countdownTextLabel;

    /// <summary>
    /// drag-drop the standings items from the canvas into this list
    /// </summary>
    public List<PlayerStandingUIItem> standingsUIList;

    /// <summary>
    /// drag-drop the lap trigger point game objects
    /// </summary>
    public List<Transform> lapTriggerPts;

    //Lazy Singleton
    #region LAZY_SINGLETON_AND_AWAKE
    public static GameManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

    }
    #endregion


    /// <summary>
    /// drag-drop the empty game objects that denote the player start points
    /// </summary>
    public List<Transform> playerStartPts;

    /// <summary>
    /// These are prefabs from the 'Resources' folder
    /// </summary>
    public List<GameObject> playerPrefabs;

    //public int carNo = 0;

    [HideInInspector]
    public GameObject myCarInstance;

    logManager lm;

    // Use this for initialization
    void Start()
    {
        lm = GameObject.FindGameObjectWithTag("LM").GetComponent<logManager>();

        selectedCar = GameObject.FindGameObjectWithTag("carManager").GetComponent<selectCar>().carNo;
        //proceed instantiating the car only if we are connected to photon and ready
        if(!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("<color=red> Not connected to photon network. can't proceed. </color>");
            lm.updateLog(" Not connected to photon network. can't proceed.");
        }
        else
        {
            //Get the player model ID using PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue
            //and instantiate the appropriate player model based on that player model ID...
            //that is a TODO job for your team.


            ///Let's instantiate the player at an appropriate start point based on their actor number
            ///The actor number is the order in which they entered the room usually.
            
            int actorNum = PhotonNetwork.LocalPlayer.ActorNumber;
            /////actor number is 1-based. so we subtract 1 from it to get the index of the list
            Vector3 startPos = playerStartPts[actorNum - 1].position;

            ///notice we use 'PhotonNetwork.Instantiate' instead of regular 'Instantiate'
            ///Also notice we use
            myCarInstance = PhotonNetwork.Instantiate(playerPrefabs[selectedCar].name, startPos, Quaternion.identity);



        }

    }


    void CheckAndUpdateRank(int photonViewID)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        lastAssignedRank++;

        int indx=playerRanks.FindIndex(x => x.viewID == photonViewID);
        playerRanks[indx].rank = lastAssignedRank;

        PhotonNetwork.RaiseEvent((byte)GameManager.raiseEventCodes.raceFinishCode, new object[] { playerRanks[indx].viewID, lastAssignedRank } , new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }

    /// <summary>
    /// This will be called by photon when a particular game event code is triggered
    /// </summary>
    public void OnEventCallback(EventData photonEventData)
    {

        if (photonEventData.Code == (byte)raiseEventCodes.raceFinishUpdateRank)
        {
            object[] incomingData = (object[])photonEventData.CustomData;
            int viewID = (int)incomingData[1];
            CheckAndUpdateRank(viewID);
        }

        if (photonEventData.Code == (byte)raiseEventCodes.raceFinishCode)
        {
            object[] incomingData = (object[])photonEventData.CustomData;
            
            int viewID = (int)incomingData[0];
            int rank = (int)incomingData[1];

            int indx = playerRanks.FindIndex(x => x.viewID == viewID);
            //this will have no effect on the masterClient as we already have the rank updated.
            //but on every other client, this is important as those clients won't have the rank data updated until we do the following
            playerRanks[indx].rank = rank;
        
         GO_ID_Duo duo = playerRanks.Find(x => x.viewID == viewID);
            Debug.Log(duo.pv.Owner.NickName + " finished msg called in from " + viewID);
            if (duo.rank < 1) return;

            Debug.Log(duo.pv.Owner.NickName + " finished at position " + duo.rank);

            standingsUIList[duo.rank - 1].gameObject.SetActive(true);

            standingsUIList[duo.rank - 1].UpdateInfo(duo.pv.Owner.NickName, duo.rank,viewID==duo.viewID);

            endText.SetActive(true);

        }


    }


    public void finishRace()
    {
        SceneManager.LoadScene("MainLobbyV2");
        PhotonNetwork.Disconnect();

    }


    
}
