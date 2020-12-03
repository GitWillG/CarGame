using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class NetworkMgr : MonoBehaviourPunCallbacks
{
    private string _playerName;
    logManager lm;

    GameObject item;

    public string playerName
    {

        set { _playerName = value; }
        get { return _playerName; }
    }

    /****** Setup a dynamic property to pass the room name to a roomName dynamic property here******/
    //This part of Lab 1 In-class today.
    private string _roomName;

    public string roomName
    {

        set { _roomName = value; }
        get { return _roomName; }
    }




    public GameObject waitingToConnectPanel;
    public GameObject gamelobbyOptionsPanel;

    /// <summary>
    /// The panel to create a room by entering room name and selecting game mode/level
    /// Drag-drop the reference in the inspector
    /// </summary>
    public GameObject createRoomPanel;
    /// <summary>
    /// This is the transition panel that shows a 'creating room' message 
    /// while the room is being created on photonserver..
    /// drag/drop the panel reference in the inspector
    /// </summary>
    public GameObject creatingRoomPanel;

    /// <summary>
    /// This is the panel that shows all users in the room and the option to select the car before we start the game..
    /// </summary>
    public GameObject roomUserPanel;

    /// <summary>
    /// This is the panel that shows when we are waiting for Photon to confirm whether we are joining a random room 
    /// Drag-drop the 'JoiningRoomPanel' gameObject to this field in the inspector
    /// </summary>
    public GameObject joiningRoomPanel;

    //drag-drop the RoomInfoText field here...
    public UnityEngine.UI.Text roomInfoTxt;

    /// <summary>
    /// drag-drop the 'PlayerList' (from RoomUserPanel) to this field
    /// </summary>
    public Transform playerListHolder;

    /// <summary>
    /// Drag-drop 'playerListItem' from prefabs folder, to this field
    /// </summary>
    public GameObject playerListItemPrefab;

    /// <summary>
    /// Drag-drop the 'startGameButton' from 'roomUserPanle' in hierarchy to this field
    /// </summary>
    public UnityEngine.UI.Button startGameBtn;


    //saved via the 'SetGameMode()' function below
    private string _gameMode;


    /// <summary>
    /// Keep track of all playerList GOs instantiated so that we can
    /// remove it from the display when a player leaves the room.
    /// </summary>
    private Dictionary<int, GameObject> playerDictGOs;
   

    #region CONNECT_TO_SERVER

    //Master Client - > Is the one who creates a room 
    //Client -> all the other clients are the ones who join an existing room created by a master client

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        lm = GameObject.FindGameObjectWithTag("LM").GetComponent<logManager>();
    }

    /// <summary>
    /// My custom function that will connect to the server
    /// </summary>
    void Connect()
    {
        //if we are not connected to PhotonNetwork, then let's connect
        if(!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
            lm.updateLog("Connected.");
        }
    }


    /// <summary>
    /// This will be called when the login button is pressed in the UI
    /// </summary>
    public void OnLoginButtonPressed()
    {
        if(string.IsNullOrEmpty(playerName))
        {
            Debug.Log("<color=red> Player name not entered. Can't connect to server without it.</color>");
            lm.updateLog("Player name not entered. Cannot connect to server without it.");
            return;
        }
       // else
        Connect();

    }

    public override void OnConnected()
    {
        Debug.Log("Connection established with Photon");
        lm.updateLog("Connection established with Photon");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " got connected");
        lm.updateLog(PhotonNetwork.LocalPlayer.NickName + " got connected");
        //lm.updateLog()
        waitingToConnectPanel.SetActive(false);
        gamelobbyOptionsPanel.SetActive(true);

        PhotonNetwork.JoinLobby();

    }

    #endregion



    #region CREATE_ROOM

    /// <summary>
    /// This is called from the 'CreateRoomPanel'. A two letter 'game mode' code is passed depending on
    /// which game mode checkbox is selected.
    /// Race Mode: rm
    /// Death Mode: dm
    /// </summary>
    /// <param name="gameMode"></param>
    public void SetGameMode(string gameMode)
    {
        _gameMode = gameMode;
    }

    /// <summary>
    /// Called when we click 'CreateRoom' button under 'CreateRoomPanel' in the UI.
    /// </summary>
    public void OnCreateRoomButtonPressed()
    {

        //if roomName is not entered, then print debug log message and return..
        if(string.IsNullOrEmpty(roomName))
        {
            Debug.Log("<color=red> Room name not entered. Cannot create a room. </color>");
            lm.updateLog("Room name not entered. Cannot create a room.");
            return;
        }
        if(string.IsNullOrEmpty(_gameMode))
        {
            Debug.Log("<color=red> Game mode not selected.. cannot create a room. </color>");
            lm.updateLog("Game mode not selected.. cannot create a room.");
            return;
        }

        createRoomPanel.SetActive(false);
        creatingRoomPanel.SetActive(true);

        CreateRoom();
    }


    private void CreateRoom()
    {
        Photon.Realtime.RoomOptions ro = new Photon.Realtime.RoomOptions();
        ro.MaxPlayers = 5;

        //we use 'm' as a short-hand property for denoting our gameMode. 

        ro.CustomRoomPropertiesForLobby = new string[] { "m" };

        ExitGames.Client.Photon.Hashtable gameRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            {"m", _gameMode}
        };
        
        ro.CustomRoomProperties = gameRoomProperties;

        PhotonNetwork.CreateRoom(roomName + "_" + UnityEngine.Random.Range(100000, 999999), ro);
    }


    public override void OnCreatedRoom()
    {

        Debug.Log("<color=green> Room created successfully...</color>");
        lm.updateLog("Room created successfully...");
        creatingRoomPanel.SetActive(false);
        roomUserPanel.SetActive(true);

    }

    public override void OnJoinedRoom()
    {
        joiningRoomPanel.SetActive(false);
        roomUserPanel.SetActive(true);

        Debug.Log("<color=cyan> User:  " + PhotonNetwork.LocalPlayer.NickName + " joined " + PhotonNetwork.CurrentRoom.Name + " </color>");
        lm.updateLog("User:  " + PhotonNetwork.LocalPlayer.NickName + " joined " + PhotonNetwork.CurrentRoom.Name);
        //We need to print certain info in the room.. such as room name, room game mode type, and list of players currently in room...

        roomInfoTxt.text = PhotonNetwork.CurrentRoom.Name + " ||| Players:  " + PhotonNetwork.CurrentRoom.PlayerCount + "/" +
             PhotonNetwork.CurrentRoom.MaxPlayers;


        ///If playerDictGOs is null, create the object for it
        if (playerDictGOs == null)
            playerDictGOs = new Dictionary<int, GameObject>();

        //Populate a list of players in the current room (populate in the playerList UI)
        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
            CreatePlayerListItem(p);

        // Room game mode type..
        //Without an OUT parameter....
        // if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("m"))
        //   {
                //   object gameModeName = PhotonNetwork.CurrentRoom.CustomProperties["m"];
          //}
        
        //Using an out parameter... and the alternative for the above commented out code
        object gameModeName = "";

            if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("m",out gameModeName))
            {
                Debug.Log("<color=magenta> Room's game mode is:  " + gameModeName + "</color>");
            }

        

    }



    private void CreatePlayerListItem(Player newPlayer)
    {
        item = Instantiate(playerListItemPrefab, playerListHolder);
        
        item.GetComponent<PlayerItemUIInfo>().Init(newPlayer.ActorNumber, newPlayer.NickName);

        playerDictGOs.Add(newPlayer.ActorNumber, item);


        object _isRemotePlayerReady;


        if (newPlayer.CustomProperties.TryGetValue("pReady", out _isRemotePlayerReady))
        {
            item.GetComponent<PlayerItemUIInfo>().SetReadyState((bool) _isRemotePlayerReady);
        }



    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        CreatePlayerListItem(newPlayer);
        roomInfoTxt.text = PhotonNetwork.CurrentRoom.Name + " | Players:  " + PhotonNetwork.CurrentRoom.PlayerCount + "/" +
        PhotonNetwork.CurrentRoom.MaxPlayers;

    }


    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("<color=red> Create room failed: error code: " + returnCode + "\n msg=" + message);
        lm.updateLog("Create room failed: error code: " + returnCode + "\n msg=" + message);
    }


 
    #endregion


    #region UPDATE_PLAYER_PROPERTIES

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        object _isRemotePlayerReady;

        if (changedProps.TryGetValue("pReady", out _isRemotePlayerReady))
        {
            playerDictGOs[targetPlayer.ActorNumber].GetComponent<PlayerItemUIInfo>().SetReadyState((bool) _isRemotePlayerReady);
        }

        startGameBtn.interactable = IsGameReadyToStart();
    }

    #endregion  



    #region JOIN_RANDOM_ROOM


    /// <summary>
    /// Call this on the two buttons 'RacingGameMode' and 'DeathRaceGameMode' under 'JoinRandomRoomPanel->Background->GameModes
    /// for RaceGameMode, the code is rm
    /// for DeathGameMode, the code is dm
    /// </summary>
    /// <param name="gameModeCode"></param>
    public void OnJoinRandomRoomGameModeTypeClicked(string gameModeCode)
    {


        Debug.Log("<color=orange> Trying to find a random room of gameMode type=" + gameModeCode + " </color>");
        lm.updateLog("Trying to find a random room of gameMode type=" + gameModeCode);

        ExitGames.Client.Photon.Hashtable expectedProperties = new ExitGames.Client.Photon.Hashtable
        {
            {"m", gameModeCode}
        };

        PhotonNetwork.JoinRandomRoom(expectedProperties, 0);

     //   _gameMode = gameModeCode;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {

        Debug.Log("<color=red>Join random room failed with message = " + message+"</color>");
        lm.updateLog("Join random room failed with message = " + message);

        Transform failedPanelTrans= joiningRoomPanel.transform.Find("FailedPanel");
        if (failedPanelTrans != null)
        {
            failedPanelTrans.gameObject.SetActive(true);
            failedPanelTrans.GetComponent<ErrorMessageDisplay>().DisplayMessage(
                "Couldn't join a random room. \n Error Message: " + message, CallOnRandomRoomFailedAfterDelay);
        }

        //Let's make the user go back to the 'GameOptions' panel...
        //gamelobbyOptionsPanel.SetActive(true);
        //joiningRoomPanel.SetActive(false);

        //Alternatively, if you wish to create a room, then you do this:::
        //Let's create a room instead with the gameMode that the user tried to join for...
         //    CreateRoom();
        //  Debug.Log("<color=lightgreen>New room created. Mode: " + _gameMode + "</color>");
    }

    void CallOnRandomRoomFailedAfterDelay()
    {
        Invoke("OnRandomRoomFailed", 3.0f);
    }

    void OnRandomRoomFailed()
    {
        joiningRoomPanel.SetActive(false);
        gamelobbyOptionsPanel.SetActive(true);
    }


    #endregion


    #region GAME_START_FUNCTION

    /// <summary>
    /// A private method that returns true if the game is ready to start
    /// </summary>
    /// <returns></returns>
    private bool IsGameReadyToStart()
    {
        //THis check should only happen on the MasterClient's side.
        if (!PhotonNetwork.IsMasterClient) return false;

         foreach(Player p in PhotonNetwork.PlayerList)
        {

            object isRemotePlayerReady;

            if (p.CustomProperties.TryGetValue("pReady", out isRemotePlayerReady))
            {
                if (!(bool)isRemotePlayerReady)
                    return false;
            }
            else
            { ///error -.... can't find pReady property .. may be we mis-spelled either here or in PlayerItemUIInfo. both should match....
                return false;
            }
        }

        return true;

    }


    #endregion


    #region START_GAME

    /// <summary>
    /// Hook this up to the 'StartGameButton' in 'RoomUserPanel'
    /// </summary>
    public void OnStartGameButtonClicked()
    {

        object gameModeCode;

        if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("m",out gameModeCode))
        {
            if ((string)gameModeCode == "rm")
                PhotonNetwork.LoadLevel("RaceModeLevel");
            else if ((string)gameModeCode == "dm")
                PhotonNetwork.LoadLevel("DeathModeLevel");
            else
            {
                Debug.Log("Didn't recognize the game mode code: " + gameModeCode);
                lm.updateLog("Didn't recognize the game mode code: " + gameModeCode);
                //For your project, you will show the errors/warnings in a proper dialog window (like we did for the error message in 'JoiningRoom' panel.
            }


        }
        else
        {
            Debug.Log("Can't find 'm' property in the room. ");
            lm.updateLog("Can't find 'm' property in the room.");
        }


    }

    #endregion

    public void leaveServer()
    {

        PhotonNetwork.LeaveRoom();
        PhotonNetwork.CurrentRoom.IsOpen = false;

        foreach(GameObject t in playerListHolder)
        {
            Destroy(t);
        }

        lm.updateLog("Left the server. Server has closed.");
        
    }

}
