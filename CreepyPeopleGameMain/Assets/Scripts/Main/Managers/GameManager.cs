using Assets.Scripts.Main.Characters;
using System;
using UnityEngine;

namespace Assets.Scripts.Main.Managers
{
    public enum GameStates
    {
        START = 0,
        PLAY = 1,
        GAMEOVER = 2
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
            switch (m_CurrentGameState)
            {
                case GameStates.START:
                    break;
                case GameStates.PLAY:
                    break;
                case GameStates.GAMEOVER:
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
        }

        public void TriggerGameOver()
        {
            MainCanvasManager.Instance.ShowGameOver();
            m_CurrentGameState = GameStates.GAMEOVER;
            MainNetworkManager.Instance.PollGameState();
        }

        public void RestartGame()
        {
            // Player. Destroying and Recreating player because the First Person Controller is Fucking with me
            Destroy(m_CurrentPlayer.gameObject);
            Transform l_spawn = GameManager.Instance.GetSpawnPosition();
            m_CurrentPlayer = Instantiate(PlayerPrefab, l_spawn.position, l_spawn.rotation);

            // Enemies
            foreach (GameObject l_enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                l_enemy.GetComponent<EnemyScript>().RestartEnemy();
            }

            // UI
            MainCanvasManager.Instance.Reset();

            // Reset Mobile
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
