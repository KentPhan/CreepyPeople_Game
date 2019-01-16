using UnityEngine;

namespace Assets.Scripts.Main.Characters
{
    public enum PhoneStates
    {
        ON,
        OFF
    }
    public class PlayerScript : MonoBehaviour
    {
        // TODO. All this code is shit for prototyping. Rebuild if we make it. Switch to a more modular component system
        // Flash light shit
        [SerializeField]
        private Light FlashLight;

        [SerializeField]
        private float FlashLightStunTime = 5.0f;
        [SerializeField]
        private float FlashLightStunRange = 20.0f;
        [SerializeField]
        private float FlashLightBatteryDrainRate = 1.0f;

        // Phone Shit
        [SerializeField] private float MaxBatteryPower = 100.0f;
        [SerializeField] private float PhoneBatteryDrainRate = 1.0f;
        private float m_CurrentBatteryPower = 90.0f;
        private PhoneStates m_CurrentPhoneState;



        // Cached Shit
        private int m_EnemyMask;

        // Start is called before the first frame update
        void Start()
        {
            // Initalize
            m_CurrentPhoneState = PhoneStates.ON;

            // Cache
            FlashLight.enabled = false;
            m_EnemyMask = LayerMask.GetMask("Enemy");
        }

        // Update is called once per frame
        void Update()
        {
            float l_deltaTime = Time.deltaTime;
            if (m_CurrentPhoneState == PhoneStates.ON)
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
                    m_CurrentBatteryPower -= (FlashLightBatteryDrainRate * l_deltaTime);
                }

                m_CurrentBatteryPower -= (PhoneBatteryDrainRate * l_deltaTime);
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

        // TODO for future reference. Consolidate battery drain logic on either mobile or pc side. Currently on PC side, consider mobile side. Costs and benefits for either
        public float GetBatteryRatio()
        {
            return (m_CurrentBatteryPower / MaxBatteryPower);
        }
    }
}
