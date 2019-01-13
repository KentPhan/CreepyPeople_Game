using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Main.Managers
{
    public class MainNetworkManager : MonoBehaviourPunCallbacks
    {
        private byte maxPlayersPerRoom = 2;

        public string versionName = "0.1";

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

            }

            // Only for testing
            MainCanvasManager.Instance.SetConnectionStatusText(PhotonNetwork.NetworkClientState.ToString());
        }

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
            GameObject l_playerPrefab = GameManager.Instance.GetPlayerPrefab();
            Transform l_spawn = GameManager.Instance.GetSpawnPosition();
            PhotonNetwork.Instantiate(l_playerPrefab.name, l_spawn.position, l_spawn.rotation, 0);
            Debug.Log("Joined Room");
        }

        private void OnFailedToConnectToPhoton()
        {
            Debug.Log("Disconnnected from Network...");
        }
    }
}