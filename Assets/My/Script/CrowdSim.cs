using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace C2025_00
{


    public class CrowdSim : MonoBehaviour
    {
        [SerializeField]
        public CrowdSettingObject m_crowd_setting_object;
        CrowdSetting m_crowd_setting {
            get {
                return m_crowd_setting_object.crowd_setting;
            }
        }

        [SerializeField]
        GameObject m_agent_prefab;

        List<GameObject> m_agents;

        bool is_running = false;

        public bool IsRunning
        {
            get { return is_running; }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            ResetCrowd();

            // set timer function with interval 1 second
            InvokeRepeating("Gen", 0, 0.3f);
        }

        public void Run()
        {
            if (m_agents != null) {
                foreach (GameObject agent in m_agents) {
                    if (agent == null) {
                        continue;
                    }
                    agent.GetComponent<NavMeshAgent>().isStopped = false;
                }
                is_running = true;
            }
        }

        public void Pause()
        {
            is_running = false;
            if (m_agents == null) {
                return;
            }
            foreach (GameObject agent in m_agents) {
                if (agent == null) {
                    continue;
                }
                agent.GetComponent<NavMeshAgent>().isStopped = true;
            }
        }

        public void Toggle()
        {
            if (is_running)
            {
                Pause();
            }
            else
            {
                Run();
            }
        }

        public void ResetCrowd() {
            if ( m_agents == null ) {
                m_agents = new List<GameObject>();
            }
            else
            {
                // remove all agents
                foreach (GameObject agent in m_agents) {
                    if (agent == null) {
                        continue;
                    }
                    Destroy(agent);
                }
                m_agents.Clear();
            }

            if (m_crowd_setting_object == null) {
                return;
            }
            if ( m_crowd_setting == null ) {
                return;
            }
            if (m_crowd_setting.stream_params == null) {
                return;
            }
            if (m_crowd_setting.stream_params.Length == 0) {
                return;
            }
        }

        // Update is called once per frame
        void Update()
        {
            for ( int i=m_agents.Count-1; i>=0; i-- )
            {
                GameObject agent = m_agents[i];
                // check if agent is reached
                if ( agent.GetComponent<Agent>().isReached )
                {
                    // remove agent from list
                    Destroy(agent);
                    m_agents.RemoveAt(i);
                }
            }
        }

        void Gen()
        {
            if (!is_running) {
                return;
            }
            for (int i = 0; i < m_crowd_setting.stream_params.Length; i++)
            {
                CrowdStreamParams param = m_crowd_setting.stream_params[i];
                GameObject agent = Instantiate(m_agent_prefab);

                agent.GetComponent<Agent>().m_property.stream_id = i;
                
                // set color to agent
                agent.GetComponent<Renderer>().material.color = m_crowd_setting_object.GetStreamColor(i);

                NavMeshAgent nav_gent = agent.GetComponent<NavMeshAgent>();

                // get random position on the xy circle
                Vector2 randomCircle = Random.insideUnitCircle * param.inflow_radius;
                Vector3 pos = new Vector3(randomCircle.x, 0, randomCircle.y) + param.inflow_pos;

                if ( nav_gent.Warp(pos) ) 
                {
                    if ( nav_gent.SetDestination(param.outflow_pos) )
                    {
                        nav_gent.stoppingDistance = param.outflow_radius * 2f / 3f;
                        // start agent movement
                        nav_gent.isStopped = !is_running;

                        agent.transform.SetParent(this.transform);
                        m_agents.Add(agent);
                    }
                }

            }

        }


        // void Load
    }

}