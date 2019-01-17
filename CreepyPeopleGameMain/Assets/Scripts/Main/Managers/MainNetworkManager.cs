using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Assets.Scripts.Main.Managers
{
    public enum PhotonEventCodes
    {
        MOVE_POSITION = 0,
        FLASH_LIGHT_TOGGLE = 1,
        FLASH_LIGHT_POWER = 2,
        INVENTORY_STATUS = 3
    }

    // TODO Consider Communication efficiency using this method.
    public class MainNetworkManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        private byte maxPlayersPerRoom = 2;

        public string versionName = "0.1";

        [SerializeField]
        private float PollRate = 2.0f;


        private float m_CurrentPollTime = 0.0f;

        public static MainNetworkManager Instance;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            else if (Instance != this)
                Destroy(gameObject);

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


        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinOrCreateRoom("One", null, null);

            Debug.Log("Connected to Master");
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("Joined Lobby");
        }

        public override void OnJoinedRoom()
        {
            // Need to move this somewhere else later
            GameManager.Instance.StartGame();
            Debug.Log("Joined Room");
        }

        private void OnFailedToConnectToPhoton()
        {
            Debug.Log("Disconnected from Network...");
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
        private void PollPlayerPosition()
        {
            if (GameManager.Instance.GetCurrentPlayer() != null)
            {
                object[] l_data = new object[] { GameManager.Instance.GetCurrentPlayer().transform.position };
                RaiseEventOptions l_raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                SendOptions l_sendOptions = new SendOptions { Reliability = true };

                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.MOVE_POSITION, l_data, l_raiseEventOptions, l_sendOptions);
                //Debug.Log("Raised The Damn Event With:" + l_data[0]);
            }
        }

        private void PollBatteryPower()
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
        #endregion
    }
}
