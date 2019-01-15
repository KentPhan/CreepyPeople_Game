using Assets.Scripts.Main.Characters;
using UnityEngine;

namespace Assets.Scripts.Main.Managers
{
    public enum GameStates
    {
        START,
        PLAY,
        GAMEOVER
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

        }

        public void StartGame()
        {
            m_CurrentGameState = GameStates.PLAY;
            Transform l_spawn = GameManager.Instance.GetSpawnPosition();
            m_CurrentPlayer = Instantiate(PlayerPrefab, l_spawn.position, l_spawn.rotation);
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
