using Assets.Scripts.Main.Characters;
using Assets.Scripts.Main.Components;
using System;
using UnityEngine;

namespace Assets.Scripts.Main.Managers
{
    public enum GameStates
    {
        NOT_IN_ROOM = 0,
        START = 1,
        PLAY = 2,
        GAMEOVER = 3,
        WIN,

    }

    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private PlayerScript PlayerPrefab;
        [SerializeField]
        private GameObject SpawnPosition;

        public static GameManager Instance;

        private PlayerScript m_CurrentPlayer;
        private GameStates m_CurrentGameState;

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
            m_CurrentGameState = GameStates.START;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }

            switch (m_CurrentGameState)
            {
                case GameStates.START:
                    if (Input.GetButtonDown("Submit"))
                    {
                        StartGame();
                    }
                    break;
                case GameStates.PLAY:
                    break;
                case GameStates.GAMEOVER:
                    if (Input.GetButtonDown("Submit"))
                        RestartGame();
                    break;
                case GameStates.WIN:
                    if (Input.GetButtonDown("Submit"))
                        RestartGame();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void StartGame()
        {
            GameObject[] l_Players = GameObject.FindGameObjectsWithTag("Player");

            if (l_Players.Length > 1)
                Debug.LogError("More than 1 player detected on start game");

            m_CurrentGameState = GameStates.PLAY;
            MainNetworkManager.Instance.PollGameState();
            Transform l_spawn = GameManager.Instance.GetSpawnPosition();

            if (m_CurrentPlayer == null)
            {
                if (l_Players.Length == 1)
                    m_CurrentPlayer = l_Players[0].GetComponent<PlayerScript>();
                else
                    m_CurrentPlayer = Instantiate(PlayerPrefab, l_spawn.position, l_spawn.rotation);
            }

            // Poll initial Flashlight
            MainNetworkManager.Instance.PollInitialFlashLight(m_CurrentPlayer.IsFlashLightOn());


            // UI Shit
            MainCanvasManager l_Canvas = MainCanvasManager.Instance;
            l_Canvas.SetRoomNameText(MainNetworkManager.Instance.GetRoomName());
            l_Canvas.ShowPlayMode();
        }

        public void TriggerGameOver()
        {
            MainCanvasManager.Instance.ShowGameOver();
            m_CurrentGameState = GameStates.GAMEOVER;
            MainNetworkManager.Instance.PollGameState();
        }

        public void TriggerWin()
        {
            m_CurrentGameState = GameStates.WIN;
            MainCanvasManager.Instance.ShowWinScreen();
        }

        public void RestartGame()
        {
            // Player. Destroying and Recreating player because the First Person Controller is Fucking with me
            Destroy(m_CurrentPlayer.gameObject);

            // Enemies
            foreach (GameObject l_enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                l_enemy.GetComponent<EnemyScript>().RestartEnemy();
            }

            // Trigger Areas
            foreach (GameObject l_triggerArea in GameObject.FindGameObjectsWithTag("TriggerArea"))
            {
                l_triggerArea.GetComponent<TriggerAreaScript>().Reset();
            }


            // Items
            foreach (GameObject l_item in GameObject.FindGameObjectsWithTag("Item"))
            {
                //Debug.Log(l_item);
                if (l_item.GetComponentInChildren<MeshRenderer>() != null)
                    l_item.GetComponentInChildren<MeshRenderer>().enabled = true;
                //if (l_item.GetComponentInChildren<BoxCollider>() != null)
                //    l_item.GetComponentInChildren<BoxCollider>().enabled = true;
            }


            // Doors
            // Items
            foreach (GameObject l_door in GameObject.FindGameObjectsWithTag("Door"))
            {
                if (l_door.GetComponent<DoorComponent>() != null)
                    l_door.GetComponent<DoorComponent>().Reset();
            }

            // UI
            MainCanvasManager.Instance.Reset();

            // Reset Mobile
            m_CurrentGameState = GameStates.START;
            MainNetworkManager.Instance.PollGameState();
        }

        public PlayerScript GetCurrentPlayer()
        {
            return m_CurrentPlayer;
        }

        public Transform GetSpawnPosition()
        {
            return SpawnPosition.transform;
        }

        public GameStates GetCurrentGameState()
        {
            return m_CurrentGameState;
        }
    }
}
