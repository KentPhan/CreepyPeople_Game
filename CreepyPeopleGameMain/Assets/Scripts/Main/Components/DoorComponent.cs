using Assets.Scripts.Main.Managers;
using UnityEngine;

namespace Assets.Scripts.Main.Components
{
    public class DoorComponent : MonoBehaviour
    {
        private bool m_Open;
        private bool m_Locked;

        [SerializeField]
        private int m_KeyINDEX;

        // Start is called before the first frame update
        void Start()
        {
            m_Open = false;
            m_Locked = true;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void AttemptOpen(object[] i_Inventory)
        {
            // TODO should always be booleans
            if ((bool)i_Inventory[m_KeyINDEX])
            {
                gameObject.SetActive(false);
                m_Open = true;
                m_Locked = false;
                i_Inventory[m_KeyINDEX] = false;
                MainNetworkManager.Instance.PollInventoryStatus();
            }
        }
    }
}
