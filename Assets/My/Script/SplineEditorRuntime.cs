//using UnityEngine;
//using UnityEngine.Splines;
//using System.Collections.Generic;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;

//public class SplineEditorRuntime : MonoBehaviour
//{
//    public Camera mainCamera;
//    public SplineContainer splineContainer;
//    public Material roadMaterial; // 도로 머티리얼
//    public LayerMask groundLayerMask; // 마우스 클릭 충돌할 레이어 (Ground로 지정)

//    private int selectedKnotIndex = -1; //현재 선택된 점(==Knot)인덱스
//    private bool isDragging = false;
//    private bool isCreatingPoint = false;
//    private int currentKnotIndex = -1; // 가장 최근에 만들어진 점의 인덱스

//    private RoadGenerator roadGenerator;
//    private float yOffset = 0.01f; // Y축 띄우는 높이

//    public Slider roadWidthSlider; //도로 너비 조절 슬라이더

//    //토글 제어
//    private bool isSplineMode = false;
//    [SerializeField] private Toggle splineToggle;

//    void Start()
//    {
//        if (mainCamera == null) mainCamera = Camera.main;
//        roadGenerator = GetComponent<RoadGenerator>();
//        if (roadGenerator == null)
//        {
//            roadGenerator = gameObject.AddComponent<RoadGenerator>();
//        }
//        roadGenerator.Init(splineContainer, roadMaterial, roadWidthSlider); // RoadGenenrator 초기화

//    }

//    void Update()
//    {
//        // UI 패널 위에 마우스가 올라가 있으면 편집 막기
//        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

//        //스플라인 모드 활성화 되었을 때만 스플라인 그릴 수 있음
//        if (isSplineMode)
//        {
//            HandleInput();
//        }

//    }

//    //마우스 입력 처리 함수
//    void HandleInput()
//    {
//        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

//        if (Input.GetMouseButtonDown(0)) // 첫 클릭
//        {
//            if (Physics.Raycast(ray, out RaycastHit hit, 1000, groundLayerMask)) // Ground 레이어 설정 된 곳 클릭할 때만
//            {
//                Vector3 hitPoint = hit.point + Vector3.up * yOffset;


//                int closest = GetClosestKnot(hitPoint);
//                // 만약 가장 가까운 점 찾고, 거리가 가까우면 (1.5f) 선택
//                if (closest != -1 && Vector3.Distance(hitPoint, splineContainer.Spline[closest].Position) < 1.5f)
//                {
//                    selectedKnotIndex = closest;
//                    isDragging = true;
//                }
//                else // 아니면 새로운 점 생성
//                {
//                    CreateNewKnot(hitPoint);
//                }
//            }
//        }

//        if (Input.GetMouseButton(0) && isDragging && selectedKnotIndex != -1) // 클릭한 상태에서 드래그하면 점 이동
//        {
//            if (Physics.Raycast(ray, out RaycastHit hit, 1000, groundLayerMask))
//            {
//                Vector3 elevatedPoint = hit.point + Vector3.up * yOffset;

//                var knot = splineContainer.Spline[selectedKnotIndex];
//                knot.Position = elevatedPoint;
//                splineContainer.Spline[selectedKnotIndex] = knot;
//                roadGenerator.GenerateRoadMesh(); // 점 위치 변경 후 도로 메시 갱신
//            }
//        }

//        // 마우스 버튼 UP하면 드래그 false
//        if (Input.GetMouseButtonUp(0))
//        {
//            isDragging = false;
//            selectedKnotIndex = -1;
//        }

//        // 새 점 생성 후 마우스 드래그로 곡선 방향 설정
//        if (isCreatingPoint && currentKnotIndex >= 0 && Input.GetMouseButton(0))
//        {
//            if (Physics.Raycast(ray, out RaycastHit hit, 1000, groundLayerMask))
//            {
//                Vector3 dragPos = hit.point + Vector3.up * yOffset;

//                BezierKnot knot = splineContainer.Spline[currentKnotIndex];
//                Vector3 direction = dragPos - new Vector3(knot.Position.x, knot.Position.y, knot.Position.z);

//                knot.TangentOut = direction;
//                knot.TangentIn = -direction;

//                splineContainer.Spline[currentKnotIndex] = knot;
//                roadGenerator.GenerateRoadMesh();
//            }
//        }
//    }

//    // 새로운 점 생성 함수
//    void CreateNewKnot(Vector3 point)
//    {
//        var newKnot = new BezierKnot(point, Vector3.zero, Vector3.zero, Quaternion.identity);
//        splineContainer.Spline.Add(newKnot);

//        currentKnotIndex = splineContainer.Spline.Count - 1;
//        isCreatingPoint = true;
//    }

//    // 현재 마우스 위치에서 가장 가까운 점 찾기 
//    int GetClosestKnot(Vector3 point)
//    {
//        float minDist = float.MaxValue;
//        int index = -1;

//        for (int i = 0; i < splineContainer.Spline.Count; i++)
//        {
//            float dist = Vector3.Distance(point, splineContainer.Spline[i].Position);
//            if (dist < minDist)
//            {
//                minDist = dist;
//                index = i;
//            }
//        }
//        return index;
//    }

//    // Spline 모드 토글
//    public void ToggleSplineMode()
//    {
//        if (splineToggle.isOn)
//        {
//            isSplineMode = true;
//        }
//        else
//        {
//            isSplineMode = false;
//        }
//    }
//}

using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SplineEditorRuntime : MonoBehaviour
{
    public Camera mainCamera;
    public SplineContainer splineContainer;
    public Material roadMaterial;
    public LayerMask groundLayerMask;
    public Terrain terrain;

    private int selectedKnotIndex = -1;
    private bool isDragging = false;
    private bool isCreatingPoint = false;
    private int currentKnotIndex = -1;

    private RoadGenerator roadGenerator;
    private float yOffset = 0.1f;

    public Slider roadWidthSlider;

    private bool isSplineMode = false;
    [SerializeField] private Toggle splineToggle;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        roadGenerator = GetComponent<RoadGenerator>();
        if (roadGenerator == null)
        {
            roadGenerator = gameObject.AddComponent<RoadGenerator>();
        }
        roadGenerator.Init(splineContainer, roadMaterial, roadWidthSlider);
    }

    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        if (isSplineMode)
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, groundLayerMask))
            {
                Vector3 hitPoint = hit.point;
                hitPoint.y = terrain.SampleHeight(hitPoint) + yOffset;

                int closest = GetClosestKnot(hitPoint);
                if (closest != -1 && Vector3.Distance(hitPoint, splineContainer.Spline[closest].Position) < 1.5f)
                {
                    selectedKnotIndex = closest;
                    isDragging = true;
                }
                else
                {
                    CreateNewKnot(hitPoint);
                }
            }
        }

        if (Input.GetMouseButton(0) && isDragging && selectedKnotIndex != -1)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, groundLayerMask))
            {
                Vector3 elevatedPoint = hit.point;
                elevatedPoint.y = terrain.SampleHeight(elevatedPoint) + yOffset;

                var knot = splineContainer.Spline[selectedKnotIndex];
                knot.Position = elevatedPoint;
                splineContainer.Spline[selectedKnotIndex] = knot;
                roadGenerator.GenerateRoadMesh();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            selectedKnotIndex = -1;
        }

        if (isCreatingPoint && currentKnotIndex >= 0 && Input.GetMouseButton(0))
        {
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, groundLayerMask))
            {
                Vector3 dragPos = hit.point;
                dragPos.y = terrain.SampleHeight(dragPos) + yOffset;

                BezierKnot knot = splineContainer.Spline[currentKnotIndex];
                // Vector3 direction = dragPos - knot.Position;
                Vector3 direction = dragPos - new Vector3(knot.Position.x, knot.Position.y, knot.Position.z);

                knot.TangentOut = direction;
                knot.TangentIn = -direction;

                splineContainer.Spline[currentKnotIndex] = knot;
                roadGenerator.GenerateRoadMesh();
            }
        }
    }

    void CreateNewKnot(Vector3 point)
    {
        point.y = terrain.SampleHeight(point) + yOffset;

        var newKnot = new BezierKnot(point, Vector3.zero, Vector3.zero, Quaternion.identity);
        splineContainer.Spline.Add(newKnot);

        currentKnotIndex = splineContainer.Spline.Count - 1;
        isCreatingPoint = true;
    }

    int GetClosestKnot(Vector3 point)
    {
        float minDist = float.MaxValue;
        int index = -1;

        for (int i = 0; i < splineContainer.Spline.Count; i++)
        {
            float dist = Vector3.Distance(point, splineContainer.Spline[i].Position);
            if (dist < minDist)
            {
                minDist = dist;
                index = i;
            }
        }
        return index;
    }

    public void ToggleSplineMode()
    {
        isSplineMode = splineToggle.isOn;
    }
}

