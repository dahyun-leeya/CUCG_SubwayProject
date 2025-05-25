using UnityEngine;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.Rendering.Universal;
using System;


namespace C2025_00 
{
    public class CrowdSettingUI : MonoBehaviour
    {
        private string m_data_dir = "Data";
        // ui dropdown
        private TMP_Dropdown m_crowd_dropdown;
        private TMP_Dropdown m_stream_dropdown;


        [SerializeField]
        private CrowdSettingObject m_crowd_setting_object;
        [SerializeField]
        private CrowdSim m_crowd_sim;
        private CrowdSetting m_current_crowd {
            set {
                m_crowd_setting_object.crowd_setting = value;
            }
            get {
                return m_crowd_setting_object.crowd_setting;
            }
        }
        private string[] m_crowd_names;
        private int current_crowd_id = -1;
        private int selected_stream_id = -1;

        private GameObject[] in_area_marks;
        private GameObject[] out_area_marks;


        public Camera mainCamera;
        public Material m_transparent_material;


        private bool save_current_crowd_required = false;

        private bool panel_opened = false;

        private bool edit_mode = false;

        string current_setting_name
        {
            get { return m_crowd_names[current_crowd_id]; }
        }

        [SerializeField]
        GameObject in_mark_prefab;

        [SerializeField]
        GameObject out_mark_prefab;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

            if ( m_crowd_sim == null)
            {
                m_crowd_sim = FindFirstObjectByType<CrowdSim>();
                if (m_crowd_sim == null)
                {
                    Debug.LogError("CrowdSim not found in the scene.");
                    return;
                }

                m_crowd_setting_object = m_crowd_sim.m_crowd_setting_object;

                if (m_crowd_setting_object == null)
                {
                    Debug.LogError("CrowdSettingObject not found in the CrowdSim.");
                    return;
                }
            }

            // absolute data dir
            if (!Directory.Exists(Path.Combine(Application.dataPath, m_data_dir)))
            {
                Directory.CreateDirectory(Path.Combine(Application.dataPath, m_data_dir));
            }

            // camera
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            // transparent material
            if (m_transparent_material==null)
            {
                // find the transparent material "TrasparentURP_Mat"
                m_transparent_material = Resources.Load<Material>("TrasparentURP_Mat");
            }

            m_crowd_dropdown = GameObject.Find("CrowdDropdown").GetComponent<TMP_Dropdown>();
            m_stream_dropdown = GameObject.Find("StreamDropdown").GetComponent<TMP_Dropdown>();
            Button run_toggle_btn = GameObject.Find("RunToggleBtn").GetComponent<Button>();
            Button sim_reset_btn = GameObject.Find("SimResetBtn").GetComponent<Button>();

            m_crowd_dropdown.onValueChanged.AddListener(delegate {
                CrowdDropdownValueChanged(m_crowd_dropdown);
            });

            m_stream_dropdown.onValueChanged.AddListener(delegate {
                StreamDropdownValueChanged(m_stream_dropdown);
            });

            run_toggle_btn.onClick.AddListener(delegate {
                m_crowd_sim.Toggle();
            });

            sim_reset_btn.onClick.AddListener(delegate {
                m_crowd_sim.ResetCrowd();
            });

            InitSettingPanel();
            CloseSettingPanel();
            OnEditMode(false);
        }

        void InitSettingPanel()
        {
            LoadCrowdNames();
            ResetCrowdDropdown();

            if (m_crowd_names.Length > 0)
            {
                m_crowd_dropdown.value = 0;
                SelectCrowd(0);
                ResetUIWithCurrentCrowd();

                if (m_current_crowd.stream_params.Length > 0)
                {
                    m_stream_dropdown.value = 0;
                    SelectStream(m_stream_dropdown.value);
                    ResetUIWithSelectedStream();
                }
            }
        }

        // event keydown

        public void OnOffPanel()
        {
            // find "SettingPanel" in Children of the current GameObject
            GameObject settingPanel = transform.Find("SettingPanel").gameObject;
            if (panel_opened) 
            {
                CloseSettingPanel();

                // OpenButton text
                GameObject openButton = transform.Find("OpenButton").gameObject;
                if (openButton != null)
                {
                    TextMeshProUGUI openButtonText = openButton.GetComponentInChildren<TextMeshProUGUI>();
                    if (openButtonText != null)
                    {
                        openButtonText.text = "▼︎";
                    }
                }
            }
            else
            {
                OpenSettingPanel();

                GameObject openButton = transform.Find("OpenButton").gameObject;
                if (openButton != null)
                {
                    TextMeshProUGUI openButtonText = openButton.GetComponentInChildren<TextMeshProUGUI>();
                    if (openButtonText != null)
                    {
                        openButtonText.text = "▲";
                    }
                }
            }
        }

        void OpenSettingPanel()
        {
            // if first time to open the setting panel
            if (m_crowd_names == null)
            {
                InitSettingPanel();
            }

            // find "SettingPanel" in Children of the current GameObject
            GameObject settingPanel = transform.Find("SettingPanel").gameObject;
            settingPanel.SetActive(true);
            SelectStream(-1);
            ResetUIWithSelectedStream();
            panel_opened = true;
            OnEditMode(false);
        }   

        void CloseSettingPanel()
        {
            // find "SettingPanel" in Children of the current GameObject
            GameObject settingPanel = transform.Find("SettingPanel").gameObject;
            settingPanel.SetActive(false);
            DisableXRayMode();
            SelectStream(-1);
            ResetUIWithSelectedStream();
            panel_opened = false;
            OnEditMode(false);
        }

        string CrowdNameToPath(string name)
        {
            return Path.Combine(Application.dataPath, m_data_dir, name + ".crowdset.json");
        }

        void LoadCrowdNames()
        {
            // find all files with the postfix "crowdset.json" in the data directory
            string[] setting_filepaths = Directory.GetFiles(Path.Combine(Application.dataPath, m_data_dir), "*crowdset.json", SearchOption.AllDirectories);

            m_crowd_names = new string[setting_filepaths.Length];
            for (int i = 0; i < setting_filepaths.Length; i++)
            {
                m_crowd_names[i] = Path.GetFileNameWithoutExtension(setting_filepaths[i]).Replace(".crowdset", "");
            }
        }

        void ResetCrowdDropdown()
        {
            m_crowd_dropdown.ClearOptions();
            m_crowd_dropdown.AddOptions(new List<string>(m_crowd_names));
        }

        void SelectCrowd(int id)
        {
            current_crowd_id = id;
            m_current_crowd = LoadCrowdFromJSON(id);
        }

        CrowdSetting LoadCrowdFromJSON(int id)
        {
            string filepath = CrowdNameToPath(m_crowd_names[id]);
            CrowdSetting setting = JsonUtility.FromJson<CrowdSetting>(File.ReadAllText(filepath));
            return setting;
        }

        void ResetUIWithCurrentCrowd()
        {
            // update UI with the current setting
            m_stream_dropdown.ClearOptions();
            string[] stream_names = new string[m_current_crowd.stream_params.Length];
            for (int i = 0; i < m_current_crowd.stream_params.Length; i++)
            {
                stream_names[i] = "Stream " + i;
            }
            m_stream_dropdown.AddOptions(new List<string>(stream_names));

            // clear marks
            if (in_area_marks != null)
            {
                foreach (GameObject mark in in_area_marks)
                {
                    Destroy(mark);
                }
            }
            if (out_area_marks != null)
            {
                foreach (GameObject mark in out_area_marks)
                {
                    Destroy(mark);
                }
            }

            // create marks
            in_area_marks = new GameObject[m_current_crowd.stream_params.Length];
            for (int i = 0; i < m_current_crowd.stream_params.Length; i++)
            {
                // create a instance of the prefab called "InMark"
                in_area_marks[i] = Instantiate(in_mark_prefab);
                in_area_marks[i].layer = LayerMask.NameToLayer("Ignore Raycast");
                in_area_marks[i].GetComponent<InOut>().SetRadius(m_current_crowd.stream_params[i].inflow_radius);
                in_area_marks[i].GetComponent<InOut>().SetTransparentMode(true);
                in_area_marks[i].transform.position = m_current_crowd.stream_params[i].inflow_pos;
                in_area_marks[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, m_current_crowd.stream_params[i].inflow_normal);
                in_area_marks[i].GetComponent<Renderer>().material.color = m_crowd_setting_object.GetStreamColor(i);

                TextMeshPro text = in_area_marks[i].GetComponentInChildren<TextMeshPro>();
                if (text != null)
                {
                    text.text = "In\n" + i;
                }

            }
            out_area_marks = new GameObject[m_current_crowd.stream_params.Length];
            for (int i = 0; i < m_current_crowd.stream_params.Length; i++)
            {
                out_area_marks[i] = Instantiate(out_mark_prefab);
                out_area_marks[i].layer = LayerMask.NameToLayer("Ignore Raycast");
                out_area_marks[i].GetComponent<InOut>().SetRadius(m_current_crowd.stream_params[i].outflow_radius);
                out_area_marks[i].GetComponent<InOut>().SetTransparentMode(true);
                out_area_marks[i].transform.position = m_current_crowd.stream_params[i].outflow_pos;
                out_area_marks[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, m_current_crowd.stream_params[i].outflow_normal);
                out_area_marks[i].GetComponent<Renderer>().material.color = m_crowd_setting_object.GetStreamColor(i);

                TextMeshPro text = out_area_marks[i].GetComponentInChildren<TextMeshPro>();
                if (text != null)
                {
                    text.text = "Out\n" + i;
                }
            }

        }

        void SelectStream(int id)
        {
            selected_stream_id = id;
        }

        void ResetUIWithSelectedStream()
        {
            if (selected_stream_id < 0 || selected_stream_id >= m_current_crowd.stream_params.Length)
            {
                for (int i = 0; i < m_current_crowd.stream_params.Length; i++)
                {
                    in_area_marks[i].layer = LayerMask.NameToLayer("UI");
                    out_area_marks[i].layer = LayerMask.NameToLayer("UI");
                    in_area_marks[i].GetComponent<InOut>().SetTransparentMode(true);
                    out_area_marks[i].GetComponent<InOut>().SetTransparentMode(true);
                }
                return;
            }

            for (int i = 0; i < m_current_crowd.stream_params.Length; i++)
            {
                if (i == selected_stream_id)
                {
                    in_area_marks[i].layer = LayerMask.NameToLayer("UI");
                    out_area_marks[i].layer = LayerMask.NameToLayer("UI");
                    in_area_marks[i].GetComponent<InOut>().SetTransparentMode(false);
                    out_area_marks[i].GetComponent<InOut>().SetTransparentMode(false);
                }
                else
                {
                    in_area_marks[i].layer = LayerMask.NameToLayer("Ignore Raycast");
                    out_area_marks[i].layer = LayerMask.NameToLayer("Ignore Raycast");
                    in_area_marks[i].GetComponent<InOut>().SetTransparentMode(true);
                    out_area_marks[i].GetComponent<InOut>().SetTransparentMode(true);
                }
            }
        }

        void CrowdDropdownValueChanged(TMP_Dropdown change)
        {
            if (save_current_crowd_required)
            {
                SaveCurrentCrowdToJSON();
                save_current_crowd_required = false;
            }
            SelectCrowd(change.value);
            ResetUIWithCurrentCrowd();

            if (m_current_crowd.stream_params.Length > 0)
            {
                m_stream_dropdown.value = 0;
                SelectStream(0);
                ResetUIWithSelectedStream();
            }
        }

        void StreamDropdownValueChanged(TMP_Dropdown change)
        {
            if (save_current_crowd_required)
            {
                SaveCurrentCrowdToJSON();
                save_current_crowd_required = false;
            }
            SelectStream(change.value);
            ResetUIWithSelectedStream();
        }

        public void AddStream()
        {
            if (m_current_crowd == null)
            {
                return;
            }

            CrowdStreamParams[] new_stream_params = new CrowdStreamParams[m_current_crowd.stream_params.Length + 1];
            for (int i = 0; i < m_current_crowd.stream_params.Length; i++)
            {
                new_stream_params[i] = m_current_crowd.stream_params[i];
            }
            new_stream_params[m_current_crowd.stream_params.Length] = CrowdStreamParams.CreateDefault();

            m_current_crowd.stream_params = new_stream_params;
            SaveCurrentCrowdToJSON();

            ResetUIWithCurrentCrowd();
            m_stream_dropdown.value = m_current_crowd.stream_params.Length - 1;
            SelectStream(m_current_crowd.stream_params.Length - 1);
            ResetUIWithSelectedStream();

            save_current_crowd_required = true;
        }

        public void RemoveStream()
        {
            if (m_current_crowd == null || m_current_crowd.stream_params.Length == 0)
            {
                return;
            }

            if (selected_stream_id >= 0)
            {
                Destroy(in_area_marks[selected_stream_id]);
                Destroy(out_area_marks[selected_stream_id]);
            }
            else 
            {
                return;
            }

            CrowdStreamParams[] new_stream_params = new CrowdStreamParams[m_current_crowd.stream_params.Length - 1];
            for (int i = 0; i < m_current_crowd.stream_params.Length - 1; i++)
            {
                if ( i >= selected_stream_id)
                {
                    new_stream_params[i] = m_current_crowd.stream_params[i + 1];
                }
                else
                {
                    new_stream_params[i] = m_current_crowd.stream_params[i];
                }
            }

            m_current_crowd.stream_params = new_stream_params;
            SaveCurrentCrowdToJSON();
            ResetUIWithCurrentCrowd();

            if (m_current_crowd.stream_params.Length > 0)
            {
                selected_stream_id = (selected_stream_id >= m_current_crowd.stream_params.Length) ? m_current_crowd.stream_params.Length - 1 : selected_stream_id;
                if (selected_stream_id >= 0)
                {
                    m_stream_dropdown.value = selected_stream_id;
                    SelectStream(selected_stream_id);
                    ResetUIWithSelectedStream();
                }
            }

            save_current_crowd_required = true;
        }


        // m_original_renderers: the renderers that are not in the in_area_marks and out_area_marks
        private List<Renderer> m_original_renderers = new List<Renderer>();
        private Dictionary<Renderer, Material[]> m_originalMaterials = new Dictionary<Renderer, Material[]>();

        
        public void EnableXRayMode()
        {
            DisableXRayMode();

            m_original_renderers.Clear();
            m_originalMaterials.Clear();
            Renderer[] allRenderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
            foreach (Renderer renderer in allRenderers)
            {
                bool is_area_ui = false;
                foreach ( GameObject mark in in_area_marks)
                {
                    if (renderer.gameObject == mark || renderer.gameObject.transform.IsChildOf(mark.transform))
                    {
                        is_area_ui = true;
                        break;
                    }
                }

                if (!is_area_ui)
                {
                    foreach (GameObject mark in out_area_marks)
                    {
                        if (renderer.gameObject == mark || renderer.gameObject.transform.IsChildOf(mark.transform))
                        {
                            is_area_ui = true;
                            break;
                        }
                    }
                }

                if (is_area_ui)
                {
                    continue;
                }

                m_originalMaterials[renderer] = renderer.materials;
                Material[] transparentMats = new Material[renderer.materials.Length];
                for (int i = 0; i < transparentMats.Length; i++)
                {
                    transparentMats[i] = m_transparent_material;
                }
                renderer.materials = transparentMats;
                m_original_renderers.Add(renderer);
            }
        }


        public void DisableXRayMode()
        {
            foreach (var kvp in m_originalMaterials)
            {
                if (kvp.Key != null)
                    kvp.Key.materials = kvp.Value;
            }
            m_original_renderers.Clear();
            m_originalMaterials.Clear();
        }

        void OnEditMode(bool f)
        {
            if (f)
            {
                edit_mode = true;
                EnableXRayMode();
            }
            else
            {
                edit_mode = false; 
                DisableXRayMode();
            }
        }

        private Vector3 mouse_screen_pos_offset = Vector3.zero;
        private int picked_in_or_out = -1; // 0: inflow, 1: outflow

        void Update()
        {
            if (!panel_opened)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                OnEditMode(true);
            }
            
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                OnEditMode(false);
            }

            if (edit_mode)
            {
                UpdateInteractiveEdit();
            }
        }

        void UpdateInteractiveEdit()
        {
            LayerMask ui_mask = 1 << LayerMask.NameToLayer("UI");

            // select an in_area_mark or out_area_mark, and set the mouse screen pos offset
            if ( !Input.GetMouseButton (0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
            {
                // mouse ray
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 10000, ui_mask))
                {
                    // detect the in_area_mark or out_area_mark
                    int hovered_in_or_out = -1;
                    int hovered_stream_id = -1;

                    for (int i = 0; i < in_area_marks.Length; i++)
                    {
                        if (hit.collider.gameObject == in_area_marks[i])
                        {
                            hovered_in_or_out = 0;
                            hovered_stream_id = i;
                            break;
                        }
                        else if (hit.collider.gameObject == out_area_marks[i])
                        {
                            hovered_in_or_out = 1;
                            hovered_stream_id = i;
                            break;
                        }
                    }


                    // select
                    if (selected_stream_id != hovered_stream_id 
                        || picked_in_or_out != hovered_in_or_out)
                    {
                        selected_stream_id = hovered_stream_id;
                        m_stream_dropdown.value = selected_stream_id;
                        SelectStream(selected_stream_id);
                        ResetUIWithSelectedStream();

                        picked_in_or_out = hovered_in_or_out;
                    }

                    // set the mouse screen pos offset
                    if (selected_stream_id != -1)
                    { 
                        Vector3 screen_center_of_mark = Input.mousePosition;
                        if (picked_in_or_out == 0)
                        {
                            screen_center_of_mark = mainCamera.WorldToScreenPoint(in_area_marks[selected_stream_id].transform.position);
                        }
                        else if (picked_in_or_out == 1)
                        {
                            screen_center_of_mark = mainCamera.WorldToScreenPoint(out_area_marks[selected_stream_id].transform.position);
                        }
                        mouse_screen_pos_offset = screen_center_of_mark - Input.mousePosition;
                        mouse_screen_pos_offset.z = 0;
                    }
                }
                else
                {
                    selected_stream_id = -1;
                    m_stream_dropdown.value = -1;
                    SelectStream(-1);
                    ResetUIWithSelectedStream();
                }
            }

            // move the in_area_mark or out_area_mark
            else if (Input.GetMouseButton(0) 
                    && selected_stream_id>=0 && selected_stream_id < m_current_crowd.stream_params.Length)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition + mouse_screen_pos_offset);
                RaycastHit hit;

                LayerMask surfaceMask = 1 << LayerMask.NameToLayer("Ground");  //원래 default 레이어로 되어 있었는데, 내가 terrain을 ground 레이어로 바꿔서 작동 안 됐었음

                // set the position of the in_area_mark
                if (Physics.Raycast(ray, out hit, 10000, surfaceMask))
                {
                    if (picked_in_or_out == 0) 
                    {
                        in_area_marks[selected_stream_id].transform.position = hit.point;
                        m_current_crowd.stream_params[selected_stream_id].inflow_pos = hit.point;

                        in_area_marks[selected_stream_id].transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                        m_current_crowd.stream_params[selected_stream_id].inflow_normal = hit.normal;
                    }
                    else if (picked_in_or_out == 1)
                    {
                        out_area_marks[selected_stream_id].transform.position = hit.point;
                        m_current_crowd.stream_params[selected_stream_id].outflow_pos = hit.point;

                        out_area_marks[selected_stream_id].transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                        m_current_crowd.stream_params[selected_stream_id].outflow_normal = hit.normal;
                    }

                    save_current_crowd_required = true;
                }
            }


            // adjust the radius of the in_area_mark or out_area_mark by mouse scroll
            float scroll_delta = Input.mouseScrollDelta.y == 0 ? Input.mouseScrollDelta.x : Input.mouseScrollDelta.y;
            if ( scroll_delta != 0 
                    && selected_stream_id>=0 && selected_stream_id < m_current_crowd.stream_params.Length)
            {
                if (picked_in_or_out == 0)
                {
                    float r = m_current_crowd.stream_params[selected_stream_id].inflow_radius + 0.5f*scroll_delta;
                    if (r < 0)
                        r = 3;
                    m_current_crowd.stream_params[selected_stream_id].inflow_radius = r;
                    in_area_marks[selected_stream_id].GetComponent<InOut>().SetRadius(r);
                    save_current_crowd_required = true;
                }
                else if (picked_in_or_out == 1)
                {
                    float r = m_current_crowd.stream_params[selected_stream_id].outflow_radius + 0.5f*scroll_delta;
                    if (r < 0)
                        r = 3;
                    m_current_crowd.stream_params[selected_stream_id].outflow_radius = r;
                    out_area_marks[selected_stream_id].GetComponent<InOut>().SetRadius(r);
                    save_current_crowd_required = true;
                }
            }
        }

        void OnDisable()
        {
            if (save_current_crowd_required)
            {
                SaveCurrentCrowdToJSON();
                save_current_crowd_required = false;
            }
        }

        public void SaveCurrentCrowdToJSON()
        {

            // Serialize setting object to JSON string
            string jsonString = JsonUtility.ToJson(m_current_crowd, true);

            // Write JSON string to file
            string filepath = Path.Combine(Application.dataPath, m_data_dir, current_setting_name + ".crowdset.json");
            File.WriteAllText(filepath, jsonString);
        }

        // Update is called once per frame
    }

}