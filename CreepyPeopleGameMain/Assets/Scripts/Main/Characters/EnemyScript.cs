using System;
using Assets.Scripts.Main.Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Main.Characters
{
    public enum EnemyStates
    {
        ACTIVE,
        STUNNED
    }

    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyScript : MonoBehaviour
    {
        //[SerializeField] private float EnemyRange = 30.0f;

        private PlayerScript m_Player;
        private NavMeshAgent m_Agent;
        private EnemyStates m_CurrentState;
        private float m_CurrentStunTime;


        // Start is called before the first frame update
        void Start()
        {
            m_Agent = GetComponent<NavMeshAgent>();
            m_Player = GameManager.Instance.GetCurrentPlayer();
        }

        // Update is called once per frame
        void Update()
        {
            switch (m_CurrentState)
            {
                case EnemyStates.ACTIVE when m_Player != null:
                    // Calculate Range from Player
                    m_Agent.destination = m_Player.transform.position;
                    break;
                case EnemyStates.ACTIVE:
                    m_Player = GameManager.Instance.GetCurrentPlayer();
                    break;
                case EnemyStates.STUNNED:
                    m_CurrentStunTime -= Time.deltaTime;
                    if (m_CurrentStunTime <= 0.0f)
                    {
                        m_CurrentState = EnemyStates.ACTIVE;
                        m_Agent.enabled = true;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public void Stun(float i_StunTime)
        {
            m_CurrentState = EnemyStates.STUNNED;
            m_CurrentStunTime = i_StunTime;
            m_Agent.enabled = false;
            return;
        }
    }
}

