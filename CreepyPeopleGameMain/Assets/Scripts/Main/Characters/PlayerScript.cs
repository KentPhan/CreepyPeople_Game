using UnityEngine;

namespace Assets.Scripts.Main.Characters
{
    public class PlayerScript : MonoBehaviour
    {
        [SerializeField]
        private Light FlashLight;

        // Start is called before the first frame update
        void Start()
        {
            FlashLight.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("Flashlight"))
            {
                FlashLight.enabled = !FlashLight.enabled;
            }

            if (FlashLight.enabled)
            {
                // Put logic for flashlight stunning
            }
        }

        public void TurnOnFlashlight()
        {
            FlashLight.enabled = true;
        }

        public void TurnOffFlashlight()
        {
            FlashLight.enabled = false;
        }


    }
}
