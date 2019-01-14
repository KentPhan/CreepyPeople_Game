using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Main.Managers
{
    public enum PhotonEventCodes
    {
        MovedPosition = 0
    }

    public class MainNetworkManager : MonoBehaviourPunCallbacks
    {
        private byte maxPlayersPerRoom = 2;

        public string versionName = "0.1";

        [SerializeField]
        private float PollRate = 2.0f;


        private float m_CurrentPollTime = 0.0f;
        private GameObject m_Player = null;

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
            if (PhotonNetwork.InRoom)
            {
                if (m_CurrentPollTime <= 0)
                {
                    object[] l_data = new object[] { m_Player.transform.position };
                    RaiseEventOptions l_raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    SendOptions l_sendOptions = new SendOptions { Reliability = true };

                    PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.MovedPosition, l_data, l_raiseEventOptions, l_sendOptions);
                    Debug.Log("Raised The Damn Event With:" + l_data[0]);


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
            GameObject l_playerPrefab = GameManager.Instance.GetPlayerPrefab();
            Transform l_spawn = GameManager.Instance.GetSpawnPosition();
            m_Player = Instantiate(l_playerPrefab, l_spawn.position, l_spawn.rotation);
            Debug.Log("Joined Room");
        }

        private void OnFailedToConnectToPhoton()
        {
            Debug.Log("Disconnected from Network...");
        }

        #endregion
    }
}
