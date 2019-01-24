using Assets.Scripts.Main.Characters;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Main.Managers
{
    public enum PhotonEventCodes
    {
        PLAYER_TRANSFORM = 0,
        FLASH_LIGHT_TOGGLE = 1,
        FLASH_LIGHT_POWER = 2,
        INVENTORY_STATUS = 3,
        GAME_STATE = 4,
        ENEMY_POSITIONS = 5
    }

    // TODO Consider Communication efficiency using this method.
    public class MainNetworkManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        private byte maxPlayersPerRoom = 2;

        public string versionName = "0.1";

        [SerializeField] private float PollRate = 2.0f;


        private float m_CurrentPollTime = 0.0f;

        public static MainNetworkManager Instance;

        public string[] m_RoomNames;

        private string m_RoomName;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            else if (Instance != this)
                Destroy(gameObject);

            m_RoomNames = new string[]
            {
                "Ugh",
                "Bleh",
                "Boop",
                "Brah",
                "Scream",
                "Run",
                "Hide",
                "Light",
                "Crap"
            };

            m_RoomName = m_RoomNames[Random.Range(0, m_RoomNames.Length - 1)];

            DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            ConnectToNetwork();
        }

        // Update is called once per frame
        void Update()
        {
            if (PhotonNetwork.InRoom && GameManager.Instance.GetCurrentGameState() == GameStates.PLAY)
            {
                if (m_CurrentPollTime <= 0)
                {
                    // Raise 
                    PollPlayerPosition();
                    PollBatteryPower();
                    m_CurrentPollTime = PollRate;
                }

                m_CurrentPollTime -= Time.deltaTime;
            }

            // Only for testing
            MainCanvasManager.Instance.SetConnectionStatusText(PhotonNetwork.NetworkClientState.ToString());
        }

        #region -= ConnectionToRoom =-

        public void ConnectToNetwork()
        {
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Connecting to Network...");
        }


        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            base.OnRoomListUpdate(roomList);
        }


        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinOrCreateRoom(m_RoomName, null, null);
            //PhotonNetwork.GetCustomRoomList();
            Debug.Log("Connected to Master");
        }

        public string GetRoomName()
        {
            return m_RoomName;
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("Joined Lobby");
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Created Room");
            base.OnCreatedRoom();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogWarning($"Failed to Create Room {message}");
            base.OnCreateRoomFailed(returnCode, message);
        }

        public override void OnJoinedRoom()
        {
            // Need to move this somewhere else later
            //GameManager.Instance.StartGame();
            MainCanvasManager.Instance.SetRoomNameText(m_RoomName);
            Debug.Log("Joined Room");
        }

        private void OnFailedToConnectToPhoton()
        {
            Debug.Log("Disconnected from Network...");
        }

        public override void OnDisconnected(DisconnectCause i_cause)
        {
            Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}.", i_cause);
            //PhotonNetwork.ReconnectAndRejoin();
        }

        #endregion

        #region -= Events =-

        public override void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case (byte)PhotonEventCodes.FLASH_LIGHT_TOGGLE:
                    {
                        object[] l_data = (object[])photonEvent.CustomData;
                        bool l_dataState = (bool)l_data[0];
                        if (l_dataState)
                            GameManager.Instance.GetCurrentPlayer().TurnOnFlashlight();
                        else
                            GameManager.Instance.GetCurrentPlayer().TurnOffFlashlight();
                        Debug.Log($"FlashLightState: {l_dataState}");
                        break;
                    }
            }
        }

        #endregion

        #region -= RaiseEvents =-

        // TODO Logic below look like shit. Consolidate more if you can
        public void PollInitialFlashLight(bool i_newState)
        {
            object[] l_content = new object[] { i_newState };
            RaiseEventOptions l_eventOptions = new RaiseEventOptions() { Receivers = ReceiverGroup.All };
            SendOptions l_sendOptions = new SendOptions() { Reliability = true };
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.FLASH_LIGHT_TOGGLE, l_content, l_eventOptions, l_sendOptions);
        }


        private void PollPlayerPosition()
        {
            if (GameManager.Instance.GetCurrentPlayer() != null)
            {
                PlayerScript l_Player = GameManager.Instance.GetCurrentPlayer();
                object[] l_data = new object[] { l_Player.transform.position, l_Player.transform.rotation };
                RaiseEventOptions l_raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                SendOptions l_sendOptions = new SendOptions { Reliability = true };

                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PLAYER_TRANSFORM, l_data, l_raiseEventOptions, l_sendOptions);
                //Debug.Log("Raised The Damn Event With:" + l_data[0]);
            }
        }

        public void PollBatteryPower()
        {
            if (GameManager.Instance.GetCurrentPlayer() != null)
            {

                object[] l_data = new object[] { GameManager.Instance.GetCurrentPlayer().GetBatteryRatio() };
                RaiseEventOptions l_raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                SendOptions l_sendOptions = new SendOptions { Reliability = true };

                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.FLASH_LIGHT_POWER, l_data, l_raiseEventOptions, l_sendOptions);
                //Debug.Log("Raised The Damn Event With:" + l_data[0]);
            }
        }


        //TODO Consolidate logic to be more consistent in publics and privates here.
        public void PollInventoryStatus()
        {
            if (GameManager.Instance.GetCurrentPlayer() != null)
            {

                object[] l_data = GameManager.Instance.GetCurrentPlayer().GetCurrentInventoryStatus();
                RaiseEventOptions l_raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                SendOptions l_sendOptions = new SendOptions { Reliability = true };

                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.INVENTORY_STATUS, l_data, l_raiseEventOptions, l_sendOptions);
                //Debug.Log("Sent The Damn Item:" + l_data[0]);
            }
        }

        public void PollGameState()
        {
            if (GameManager.Instance.GetCurrentPlayer() != null)
            {

                object[] l_data = new object[] { GameManager.Instance.GetCurrentGameState() };
                RaiseEventOptions l_raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                SendOptions l_sendOptions = new SendOptions { Reliability = true };

                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.GAME_STATE, l_data, l_raiseEventOptions, l_sendOptions);
            }
        }
        #endregion
    }
}
