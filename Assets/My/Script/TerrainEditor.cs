//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;

//public class TerrainEditor : MonoBehaviour
//{
//    public Terrain terrain;
//    public float brushRadius = 20f; //브러쉬 반지름 크기
//    public float brushStrength = 0.0001f; //브러쉬 강도

//    //terrain 모드 토글 관련
//    private bool isTerrainEditMode = false;
//    [SerializeField] private Toggle terrainEditToggle;

//    //terrain 브러쉬 관련
//    public Slider brushRadiusSlider; //브러쉬 사이즈
//    public Slider brushStrengthSlider; //브러쉬 강도

//    private void Start()
//    {
//        if(brushRadiusSlider != null)
//        {
//            brushRadiusSlider.onValueChanged.AddListener(UpdateBrushRadius); //슬라이더 값 바뀌면 브러쉬 사이즈 업데이트
//            brushRadiusSlider.value = brushRadius;
//        }

//        if(brushStrengthSlider != null)
//        {
//            brushStrengthSlider.onValueChanged.AddListener(UpdateBrushStrength); //슬라이더 값 바뀌면 브러쉬 강도 업데이트
//            brushStrengthSlider.value = brushStrength;
//        }
//    }

//    //void Update()
//    //{
//    //    if (Mouse.current == null) return;

//    //    // UI 패널 위에 마우스가 올라가 있으면 편집 막기
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

//        // UI 패널 위에 마우스가 올라가 있으면 편집 막기
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
//                        // 왼쪽 Alt 키가 눌려 있는 경우에는 낮추기
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

//    //Terrain 모드 토글
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

//    // 슬라이더 값에 따라 브러쉬 크기 Radius 변경
//    private void UpdateBrushRadius(float value)
//    {
//        brushRadius = value;

//    }

//    //슬라이더 값에 따라 브러쉬 강도 Strength 변경
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
    public float brushRadius = 20f; // 브러쉬 반지름 크기
    public float brushStrength = 0.0001f; // 브러쉬 강도

    // Terrain 모드 토글 관련
    private bool isTerrainEditMode = false;
    [SerializeField] private Toggle terrainEditToggle;

    // Terrain 브러쉬 관련
    public Slider brushRadiusSlider;   // 브러쉬 크기 슬라이더
    public Slider brushStrengthSlider; // 브러쉬 강도 슬라이더

    // 브러쉬 모양 관련
    public enum BrushShape { Circle, Square }
    public BrushShape brushShape = BrushShape.Circle;
    [SerializeField] private TMP_Dropdown brushShapeDropdown;

    private void Start()
    {
        // 브러쉬 반지름 슬라이더
        if (brushRadiusSlider != null)
        {
            brushRadiusSlider.onValueChanged.AddListener(UpdateBrushRadius);
            brushRadiusSlider.value = brushRadius;
        }

        // 브러쉬 강도 슬라이더
        if (brushStrengthSlider != null)
        {
            brushStrengthSlider.onValueChanged.AddListener(UpdateBrushStrength);
            brushStrengthSlider.value = brushStrength;
        }

        // 브러쉬 모양 드롭다운
        if (brushShapeDropdown != null)
        {
            brushShapeDropdown.onValueChanged.AddListener(OnBrushShapeChanged);
            brushShapeDropdown.value = (int)brushShape;
        }
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        // UI 위에 있을 때 편집 막기
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
                        bool raise = !Keyboard.current.leftAltKey.isPressed; // Alt 누르면 내리기
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
                        falloff = 1f; // 사각형은 균일한 영향
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

    // Terrain 모드 토글
    public void ToggleTerrainMode()
    {
        isTerrainEditMode = terrainEditToggle.isOn;
    }

    // 슬라이더로 브러쉬 반지름 변경
    private void UpdateBrushRadius(float value)
    {
        brushRadius = value;
    }

    // 슬라이더로 브러쉬 강도 변경
    private void UpdateBrushStrength(float value)
    {
        brushStrength = value;
    }

    // 드롭다운으로 브러쉬 모양 변경
    private void OnBrushShapeChanged(int index)
    {
        brushShape = (BrushShape)index;
    }
}
