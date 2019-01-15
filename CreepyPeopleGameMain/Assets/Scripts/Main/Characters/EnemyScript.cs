using Assets.Scripts.Main.Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Main.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyScript : MonoBehaviour
    {
        //[SerializeField] private float EnemyRange = 30.0f;
        [SerializeField] private float StunTime = 5.0f;

        private PlayerScript m_Player;
        private NavMeshAgent m_Agent;

        // Start is called before the first frame update
        void Start()
        {
            m_Agent = GetComponent<NavMeshAgent>();
            m_Player = GameManager.Instance.GetCurrentPlayer();
        }

        // Update is called once per frame
        void Update()
        {
            if (m_Player != null)
            {
                // Calculate Range from Player

                m_Agent.destination = m_Player.transform.position;
            }
            else
            {
                m_Player = GameManager.Instance.GetCurrentPlayer();
            }
        }
    }
}
