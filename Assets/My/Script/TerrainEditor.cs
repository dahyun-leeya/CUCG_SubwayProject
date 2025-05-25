//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;

//public class TerrainEditor : MonoBehaviour
//{
//    public Terrain terrain;
//    public float brushRadius = 20f; //�귯�� ������ ũ��
//    public float brushStrength = 0.0001f; //�귯�� ����

//    //terrain ��� ��� ����
//    private bool isTerrainEditMode = false;
//    [SerializeField] private Toggle terrainEditToggle;

//    //terrain �귯�� ����
//    public Slider brushRadiusSlider; //�귯�� ������
//    public Slider brushStrengthSlider; //�귯�� ����

//    private void Start()
//    {
//        if(brushRadiusSlider != null)
//        {
//            brushRadiusSlider.onValueChanged.AddListener(UpdateBrushRadius); //�����̴� �� �ٲ�� �귯�� ������ ������Ʈ
//            brushRadiusSlider.value = brushRadius;
//        }

//        if(brushStrengthSlider != null)
//        {
//            brushStrengthSlider.onValueChanged.AddListener(UpdateBrushStrength); //�����̴� �� �ٲ�� �귯�� ���� ������Ʈ
//            brushStrengthSlider.value = brushStrength;
//        }
//    }

//    //void Update()
//    //{
//    //    if (Mouse.current == null) return;

//    //    // UI �г� ���� ���콺�� �ö� ������ ���� ����
//    //    if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

//    //    if (isTerrainEditMode)
//    //    {
//    //        if (Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed)
//    //        {
//    //            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

//    //            if (Physics.Raycast(ray, out RaycastHit hit))
//    //            {
//    //                if (hit.collider.GetComponent<Terrain>())
//    //                {
//    //                    bool raise = Mouse.current.leftButton.isPressed;
//    //                    ModifyTerrainAtPoint(hit.point, raise);
//    //                }
//    //            }
//    //        }
//    //    }
//    //}

//    void Update()
//    {
//        if (Mouse.current == null) return;

//        // UI �г� ���� ���콺�� �ö� ������ ���� ����
//        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

//        if (isTerrainEditMode)
//        {
//            if (Mouse.current.leftButton.isPressed)
//            {
//                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

//                if (Physics.Raycast(ray, out RaycastHit hit))
//                {
//                    if (hit.collider.GetComponent<Terrain>())
//                    {
//                        // ���� Alt Ű�� ���� �ִ� ��쿡�� ���߱�
//                        bool raise = !Keyboard.current.leftAltKey.isPressed;
//                        ModifyTerrainAtPoint(hit.point, raise);
//                    }
//                }
//            }
//        }
//    }

//    void ModifyTerrainAtPoint(Vector3 point, bool raise)
//    {
//        if (terrain == null) return;

//        TerrainData terrainData = terrain.terrainData;
//        int resolution = terrainData.heightmapResolution;
//        Vector3 terrainPos = point - terrain.transform.position;

//        int centerX = Mathf.RoundToInt((terrainPos.x / terrainData.size.x) * resolution);
//        int centerY = Mathf.RoundToInt((terrainPos.z / terrainData.size.z) * resolution);

//        int radiusInSamples = Mathf.RoundToInt((brushRadius / terrainData.size.x) * resolution);
//        int startX = Mathf.Clamp(centerX - radiusInSamples, 0, resolution - 1);
//        int startY = Mathf.Clamp(centerY - radiusInSamples, 0, resolution - 1);
//        int endX = Mathf.Clamp(centerX + radiusInSamples, 0, resolution);
//        int endY = Mathf.Clamp(centerY + radiusInSamples, 0, resolution);

//        int width = endX - startX;
//        int height = endY - startY;

//        float[,] heights = terrainData.GetHeights(startX, startY, width, height);

//        for (int y = 0; y < height; y++)
//        {
//            for (int x = 0; x < width; x++)
//            {
//                int worldX = x + startX;
//                int worldY = y + startY;

//                float dx = worldX - centerX;
//                float dy = worldY - centerY;
//                float dist = Mathf.Sqrt(dx * dx + dy * dy);

//                if (dist <= radiusInSamples)
//                {
//                    float t = dist / radiusInSamples;
//                    float falloff = Mathf.Cos(t * Mathf.PI) * 0.5f + 0.5f;
//                    float delta = brushStrength * falloff;

//                    heights[y, x] += raise ? delta : -delta;
//                    heights[y, x] = Mathf.Clamp01(heights[y, x]);
//                }
//            }
//        }

//        terrainData.SetHeights(startX, startY, heights);
//    }

//    //Terrain ��� ���
//    public void ToggleTerrainMode()
//    {
//        if (terrainEditToggle.isOn)
//        {
//            isTerrainEditMode = true;
//        }
//        else
//        {
//            isTerrainEditMode = false;
//        }
//    }

//    // �����̴� ���� ���� �귯�� ũ�� Radius ����
//    private void UpdateBrushRadius(float value)
//    {
//        brushRadius = value;

//    }

//    //�����̴� ���� ���� �귯�� ���� Strength ����
//    private void UpdateBrushStrength(float value)
//    {
//        brushStrength = value;
//    }
//}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TerrainEditor : MonoBehaviour
{
    public Terrain terrain;
    public float brushRadius = 20f; // �귯�� ������ ũ��
    public float brushStrength = 0.0001f; // �귯�� ����

    // Terrain ��� ��� ����
    private bool isTerrainEditMode = false;
    [SerializeField] private Toggle terrainEditToggle;

    // Terrain �귯�� ����
    public Slider brushRadiusSlider;   // �귯�� ũ�� �����̴�
    public Slider brushStrengthSlider; // �귯�� ���� �����̴�

    // �귯�� ��� ����
    public enum BrushShape { Circle, Square }
    public BrushShape brushShape = BrushShape.Circle;
    [SerializeField] private TMP_Dropdown brushShapeDropdown;

    private void Start()
    {
        // �귯�� ������ �����̴�
        if (brushRadiusSlider != null)
        {
            brushRadiusSlider.onValueChanged.AddListener(UpdateBrushRadius);
            brushRadiusSlider.value = brushRadius;
        }

        // �귯�� ���� �����̴�
        if (brushStrengthSlider != null)
        {
            brushStrengthSlider.onValueChanged.AddListener(UpdateBrushStrength);
            brushStrengthSlider.value = brushStrength;
        }

        // �귯�� ��� ��Ӵٿ�
        if (brushShapeDropdown != null)
        {
            brushShapeDropdown.onValueChanged.AddListener(OnBrushShapeChanged);
            brushShapeDropdown.value = (int)brushShape;
        }
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        // UI ���� ���� �� ���� ����
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        if (isTerrainEditMode)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.GetComponent<Terrain>())
                    {
                        bool raise = !Keyboard.current.leftAltKey.isPressed; // Alt ������ ������
                        ModifyTerrainAtPoint(hit.point, raise);
                    }
                }
            }
        }
    }

    void ModifyTerrainAtPoint(Vector3 point, bool raise)
    {
        if (terrain == null) return;

        TerrainData terrainData = terrain.terrainData;
        int resolution = terrainData.heightmapResolution;
        Vector3 terrainPos = point - terrain.transform.position;

        int centerX = Mathf.RoundToInt((terrainPos.x / terrainData.size.x) * resolution);
        int centerY = Mathf.RoundToInt((terrainPos.z / terrainData.size.z) * resolution);

        int radiusInSamples = Mathf.RoundToInt((brushRadius / terrainData.size.x) * resolution);
        int startX = Mathf.Clamp(centerX - radiusInSamples, 0, resolution - 1);
        int startY = Mathf.Clamp(centerY - radiusInSamples, 0, resolution - 1);
        int endX = Mathf.Clamp(centerX + radiusInSamples, 0, resolution);
        int endY = Mathf.Clamp(centerY + radiusInSamples, 0, resolution);

        int width = endX - startX;
        int height = endY - startY;

        float[,] heights = terrainData.GetHeights(startX, startY, width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int worldX = x + startX;
                int worldY = y + startY;

                float dx = worldX - centerX;
                float dy = worldY - centerY;

                bool inBrush = false;
                float falloff = 1f;

                if (brushShape == BrushShape.Circle)
                {
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    if (dist <= radiusInSamples)
                    {
                        float t = dist / radiusInSamples;
                        falloff = Mathf.Cos(t * Mathf.PI) * 0.5f + 0.5f;
                        inBrush = true;
                    }
                }
                else if (brushShape == BrushShape.Square)
                {
                    if (Mathf.Abs(dx) <= radiusInSamples && Mathf.Abs(dy) <= radiusInSamples)
                    {
                        inBrush = true;
                        falloff = 1f; // �簢���� ������ ����
                    }
                }

                if (inBrush)
                {
                    float delta = brushStrength * falloff;
                    heights[y, x] += raise ? delta : -delta;
                    heights[y, x] = Mathf.Clamp01(heights[y, x]);
                }
            }
        }

        terrainData.SetHeights(startX, startY, heights);
    }

    // Terrain ��� ���
    public void ToggleTerrainMode()
    {
        isTerrainEditMode = terrainEditToggle.isOn;
    }

    // �����̴��� �귯�� ������ ����
    private void UpdateBrushRadius(float value)
    {
        brushRadius = value;
    }

    // �����̴��� �귯�� ���� ����
    private void UpdateBrushStrength(float value)
    {
        brushStrength = value;
    }

    // ��Ӵٿ����� �귯�� ��� ����
    private void OnBrushShapeChanged(int index)
    {
        brushShape = (BrushShape)index;
    }
}
