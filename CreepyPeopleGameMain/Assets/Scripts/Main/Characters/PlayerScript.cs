using Assets.Scripts.Main.Components;
using Assets.Scripts.Main.Managers;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.Scripts.Main.Characters
{
    public enum PhoneStates
    {
        ON,
        OFF
    }

    public enum PlayerStates
    {
        ALIVE,
        DEAD
    }

    public class PlayerScript : MonoBehaviour
    {
        // TODO. All this code is shit for prototyping. Rebuild if we make it. Switch to a more modular component system. Magic Strings

        // Camera
        [SerializeField] private Camera CameraReference;

        // Interact shit
        [SerializeField] private float InteractRange = 20.0f;


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


        // Life Shit
        private PlayerStates m_CurrentPlayerState;


        // Cached Shit
        private int m_EnemyMask;
        private int m_InteractableMask;
        private object[] m_CurrentInventory;
        private FirstPersonController m_FirstPersonController;

        // Start is called before the first frame update
        void Start()
        {
            // Initalize
            m_CurrentPhoneState = PhoneStates.ON;
            m_CurrentInventory = new object[5];
            for (int i = 0; i < m_CurrentInventory.Length; i++)
            {
                m_CurrentInventory[i] = false;
            }
            m_CurrentPlayerState = PlayerStates.ALIVE;
            m_CurrentBatteryPower = MaxBatteryPower;

            MainNetworkManager.Instance.PollInventoryStatus();


            // Cache
            FlashLight.enabled = false;
            m_EnemyMask = LayerMask.GetMask("Enemy");
            m_InteractableMask = LayerMask.GetMask("Item", "Interactable");
            m_FirstPersonController = GetComponent<FirstPersonController>();
        }

        public void RestartPlayer()
        {
            m_CurrentPhoneState = PhoneStates.ON;
            m_CurrentInventory = new object[5];
            for (int i = 0; i < m_CurrentInventory.Length; i++)
            {
                m_CurrentInventory[i] = false;
            }
            m_CurrentPlayerState = PlayerStates.ALIVE;
            m_CurrentBatteryPower = MaxBatteryPower;

            //m_FirstPersonController.enabled = true;
            FlashLight.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {

            if (m_CurrentPlayerState == PlayerStates.DEAD)
                return;

            float l_deltaTime = Time.deltaTime;

            // Phone Flashlight stuff
            if (m_CurrentPhoneState == PhoneStates.ON)
            {
                if (Input.GetButtonDown("DieBattery"))
                {
                    m_CurrentBatteryPower = 10.0f;
                }

                // Check if Phone is Off
                if (m_CurrentBatteryPower <= 0)
                {
                    m_CurrentPhoneState = PhoneStates.OFF;
                    FlashLight.enabled = false;
                    MainNetworkManager.Instance.PollBatteryPower();
                    return;
                }

                // Keyboard Input for TEMP
                if (Input.GetButtonDown("Flashlight"))
                {
                    FlashLight.enabled = !FlashLight.enabled;
                }

                // If Flashlight hits enemies
                if (FlashLight.enabled)
                {
                    RaycastHit l_hitInfo;
                    Ray l_FlashlightRay = new Ray(CameraReference.transform.position, CameraReference.transform.forward);
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
            else
            {
                if (m_CurrentBatteryPower > 0)
                {
                    m_CurrentPhoneState = PhoneStates.ON;
                    MainNetworkManager.Instance.PollBatteryPower();
                }
            }

            // Recharge Logic can go here.

            if (Input.GetButtonDown("Interact"))
            {
                Debug.Log("Checking Raycase");
                RaycastHit l_hitInfo;
                Ray l_InteractRay = new Ray(CameraReference.transform.position, CameraReference.transform.forward);
                if (Physics.Raycast(l_InteractRay, out l_hitInfo, InteractRange, m_InteractableMask,
                    QueryTriggerInteraction.Collide))
                {
                    GameObject l_Interactable = l_hitInfo.collider.gameObject;
                    if (l_Interactable.tag.Equals("Item"))
                    {
                        // Add to inventory
                        m_CurrentInventory[l_Interactable.GetComponent<ItemComponent>().INDEX] = true;
                        MainNetworkManager.Instance.PollInventoryStatus();

                        // TODO Consider if we need to respawn items
                        l_Interactable.GetComponent<MeshRenderer>().enabled = false;
                        //l_Interactable.GetComponent<BoxCollider>().enabled = false;
                    }
                    else if (l_hitInfo.collider.tag.Equals("Door"))
                    {
                        // Check Key
                        l_Interactable.GetComponent<DoorComponent>().AttemptOpen(m_CurrentInventory);
                    }
                }
            }
        }

        public void KillPlayer()
        {
            m_CurrentPlayerState = PlayerStates.DEAD;
            m_FirstPersonController.enabled = false;
            GameManager.Instance.TriggerGameOver();
        }

        public void TurnOnFlashlight()
        {
            FlashLight.enabled = true;
        }

        public void TurnOffFlashlight()
        {
            FlashLight.enabled = false;
        }

        public bool IsFlashLightOn()
        {
            return FlashLight.enabled;
        }

        // TODO for future reference. Consolidate battery drain logic on either mobile or pc side. Currently on PC side, consider mobile side. Costs and benefits for either
        public float GetBatteryRatio()
        {
            return (m_CurrentBatteryPower / MaxBatteryPower);
        }

        public object[] GetCurrentInventoryStatus()
        {
            return m_CurrentInventory;
        }

        public void ResetInventory()
        {
            if (m_CurrentInventory == null)
                return;
            for (int i = 0; i < m_CurrentInventory.Length; i++)
            {
                m_CurrentInventory[i] = false;
            }
        }

        public void OnTriggerEnter(Collider i_collider)
        {
            if (i_collider.gameObject.CompareTag("Win"))
            {
                GameManager.Instance.TriggerWin();
            }
        }
    }
}
