using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Main.Managers
{
    public class MainCanvasManager : MonoBehaviour
    {
        [SerializeField] private RectTransform StartScreen;
        [SerializeField] private RectTransform GameOverScreen;

        [SerializeField] private Text ConnectionText;
        [SerializeField] private Text TransformText;

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

        public void ShowGameOver()
        {
            GameOverScreen.gameObject.SetActive(true);
        }

        public void Reset()
        {
            GameOverScreen.gameObject.SetActive(false);
        }
    }
}
