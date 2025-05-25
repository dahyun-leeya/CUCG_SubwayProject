using System;
using UnityEngine;

namespace C2025_00 
{
    [System.Serializable]
    public class CrowdStreamParams
    {
        public Vector3 inflow_pos;
        public Vector3 inflow_normal;
        public Vector3 outflow_pos;
        public Vector3 outflow_normal;
        public float inflow_radius;
        public float outflow_radius;
        public float inflow_rate;

        static public CrowdStreamParams CreateDefault()
        {
            CrowdStreamParams param = new CrowdStreamParams();
            param.inflow_pos = new Vector3(10, 0, 10);
            param.inflow_normal = new Vector3(0, 1, 0);
            param.outflow_pos = new Vector3(-10, 0, 10);
            param.outflow_normal = new Vector3(0, 1, 0);
            param.inflow_radius = 5;
            param.outflow_radius = 5;
            param.inflow_rate = 1;
            return param;
        }
    }   



    [System.Serializable]
    public class CrowdSetting 
    {
        public CrowdStreamParams[] stream_params;

        static public CrowdSetting CreateTest00()
        {
            CrowdSetting setting = new CrowdSetting();
            setting.stream_params = new CrowdStreamParams[2];

            setting.stream_params[0] = new CrowdStreamParams();
            setting.stream_params[0].inflow_pos = new Vector3(10, 0, 10);
            setting.stream_params[0].inflow_normal = new Vector3(0, 1, 0);
            setting.stream_params[0].outflow_pos = new Vector3(-10, 0, 10);
            setting.stream_params[0].outflow_normal = new Vector3(0, 1, 0);
            setting.stream_params[0].inflow_radius = 5;
            setting.stream_params[0].outflow_radius = 5;
            setting.stream_params[0].inflow_rate = 1;

            setting.stream_params[1] = new CrowdStreamParams();
            setting.stream_params[1].inflow_pos = new Vector3(10, 0, -10);
            setting.stream_params[1].outflow_normal = new Vector3(0, 1, 0); 
            setting.stream_params[1].outflow_pos = new Vector3(-10, 0, 10);
            setting.stream_params[1].outflow_normal = new Vector3(0, 1, 0);
            setting.stream_params[1].inflow_radius = 5;
            setting.stream_params[1].outflow_radius = 5;
            setting.stream_params[1].inflow_rate = 2;

            return setting;
        }
    }

    [CreateAssetMenu(fileName = "CrowdSetting", menuName = "Settings/Crowd Setting")]
    public class CrowdSettingObject: ScriptableObject
    {
        public CrowdSetting crowd_setting;

        public Color GetStreamColor(int stream_id)
        {
            int n  = 5;
            int i = stream_id % n;
            int j = stream_id / n;
            if ( j == 0 )
                return Color.HSVToRGB((float)i / n, 1, 1);
            else
                return Color.HSVToRGB((float)i / n, Mathf.Pow(0.5f, j), 1);
        }
    }

}