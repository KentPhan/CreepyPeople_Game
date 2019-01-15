using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Assets.Scripts.Main.Managers
{
    public enum PhotonEventCodes
    {
        MOVE_POSITION = 0,
        FLASH_LIGHT = 1
    }

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
                case (byte)PhotonEventCodes.FLASH_LIGHT:
                    {
                        object[] l_data = (object[])photonEvent.CustomData;
                        bool l_dataState = (bool)l_data[0];
                        Debug.Log($"FlashLightState: {l_dataState}");
                        break;
                    }
            }
        }

        #endregion

        #region -= RaiseEvents =-

        private void PollPlayerPosition()
        {
            object[] l_data = new object[] { GameManager.Instance.GetCurrentPlayer().transform.position };
            RaiseEventOptions l_raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions l_sendOptions = new SendOptions { Reliability = true };

            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.MOVE_POSITION, l_data, l_raiseEventOptions, l_sendOptions);
            Debug.Log("Raised The Damn Event With:" + l_data[0]);

        }

        #endregion
    }
}
