using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Main.Managers
{
    public class MainCanvasManager : MonoBehaviour
    {
        [SerializeField] private RectTransform StartScreen;
        [SerializeField] private Camera StartCamera;
        [SerializeField] private RectTransform GameOverScreen;
        [SerializeField] private RectTransform WinScreen;

        [SerializeField] private Text ConnectionText;
        [SerializeField] private Text TransformText;
        [SerializeField] private Text RoomNameText;

        public static MainCanvasManager Instance;

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
            GameOverScreen.gameObject.SetActive(false);
            WinScreen.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetConnectionStatusText(string i_text)
        {
            ConnectionText.text = i_text;
        }

        public void SetTransformText(string i_text)
        {
            TransformText.text = i_text;
        }

        public void SetRoomNameText(string i_text)
        {
            RoomNameText.text = i_text;
        }

        public void ShowPlayMode()
        {
            StartScreen.gameObject.SetActive(false);
            StartCamera.gameObject.SetActive(false);
        }

        public void ShowGameOver()
        {
            GameOverScreen.gameObject.SetActive(true);
        }

        public void ShowWinScreen()
        {
            WinScreen.gameObject.SetActive(true);
        }

        public void Reset()
        {
            GameOverScreen.gameObject.SetActive(false);
            StartScreen.gameObject.SetActive(true);
            StartCamera.gameObject.SetActive(true);
            WinScreen.gameObject.SetActive(false);
        }
    }
}
