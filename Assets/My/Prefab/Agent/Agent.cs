using UnityEngine;
using UnityEngine.AI;

namespace C2025_00
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Agent : MonoBehaviour
    {
        public AgentProperty m_property;

        private NavMeshAgent m_nav_agent;

        public bool isReached
        {
            get
            {
                if (m_nav_agent == null)
                {
                    return false;
                }
                // current position
                Vector3 pos = transform.position;
                float dist = Vector3.Distance(pos, m_nav_agent.destination);
                if (dist > m_nav_agent.stoppingDistance)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created

        void Start()
        {
            m_nav_agent = GetComponent<NavMeshAgent>();
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
