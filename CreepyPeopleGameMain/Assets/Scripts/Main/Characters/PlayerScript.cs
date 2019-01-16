using UnityEngine;

namespace Assets.Scripts.Main.Characters
{
    public class PlayerScript : MonoBehaviour
    {
        [SerializeField]
        private Light FlashLight;

        [SerializeField]
        private float FlashLightStunTime = 5.0f;

        private float FlashLightStunRange = 20.0f;
        private int m_EnemyMask;

        // Start is called before the first frame update
        void Start()
        {
            FlashLight.enabled = false;
            m_EnemyMask = LayerMask.GetMask("Enemy");
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("Flashlight"))
            {
                FlashLight.enabled = !FlashLight.enabled;
            }

            // If Flashlight hits enemies
            if (FlashLight.enabled)
            {
                RaycastHit l_hitInfo;
                Ray l_FlashlightRay = new Ray(transform.position, transform.forward);
                if (Physics.Raycast(l_FlashlightRay, out l_hitInfo, FlashLightStunRange, m_EnemyMask,
                    QueryTriggerInteraction.Collide))
                {
                    l_hitInfo.collider.GetComponent<EnemyScript>().Stun(FlashLightStunTime);
                }
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
