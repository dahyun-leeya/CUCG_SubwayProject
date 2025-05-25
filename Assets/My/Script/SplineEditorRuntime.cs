//using UnityEngine;
//using UnityEngine.Splines;
//using System.Collections.Generic;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;

//public class SplineEditorRuntime : MonoBehaviour
//{
//    public Camera mainCamera;
//    public SplineContainer splineContainer;
//    public Material roadMaterial; // ���� ��Ƽ����
//    public LayerMask groundLayerMask; // ���콺 Ŭ�� �浹�� ���̾� (Ground�� ����)

//    private int selectedKnotIndex = -1; //���� ���õ� ��(==Knot)�ε���
//    private bool isDragging = false;
//    private bool isCreatingPoint = false;
//    private int currentKnotIndex = -1; // ���� �ֱٿ� ������� ���� �ε���

//    private RoadGenerator roadGenerator;
//    private float yOffset = 0.01f; // Y�� ���� ����

//    public Slider roadWidthSlider; //���� �ʺ� ���� �����̴�

//    //��� ����
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
//        roadGenerator.Init(splineContainer, roadMaterial, roadWidthSlider); // RoadGenenrator �ʱ�ȭ

//    }

//    void Update()
//    {
//        // UI �г� ���� ���콺�� �ö� ������ ���� ����
//        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

//        //���ö��� ��� Ȱ��ȭ �Ǿ��� ���� ���ö��� �׸� �� ����
//        if (isSplineMode)
//        {
//            HandleInput();
//        }

//    }

//    //���콺 �Է� ó�� �Լ�
//    void HandleInput()
//    {
//        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

//        if (Input.GetMouseButtonDown(0)) // ù Ŭ��
//        {
//            if (Physics.Raycast(ray, out RaycastHit hit, 1000, groundLayerMask)) // Ground ���̾� ���� �� �� Ŭ���� ����
//            {
//                Vector3 hitPoint = hit.point + Vector3.up * yOffset;


//                int closest = GetClosestKnot(hitPoint);
//                // ���� ���� ����� �� ã��, �Ÿ��� ������ (1.5f) ����
//                if (closest != -1 && Vector3.Distance(hitPoint, splineContainer.Spline[closest].Position) < 1.5f)
//                {
//                    selectedKnotIndex = closest;
//                    isDragging = true;
//                }
//                else // �ƴϸ� ���ο� �� ����
//                {
//                    CreateNewKnot(hitPoint);
//                }
//            }
//        }

//        if (Input.GetMouseButton(0) && isDragging && selectedKnotIndex != -1) // Ŭ���� ���¿��� �巡���ϸ� �� �̵�
//        {
//            if (Physics.Raycast(ray, out RaycastHit hit, 1000, groundLayerMask))
//            {
//                Vector3 elevatedPoint = hit.point + Vector3.up * yOffset;

//                var knot = splineContainer.Spline[selectedKnotIndex];
//                knot.Position = elevatedPoint;
//                splineContainer.Spline[selectedKnotIndex] = knot;
//                roadGenerator.GenerateRoadMesh(); // �� ��ġ ���� �� ���� �޽� ����
//            }
//        }

//        // ���콺 ��ư UP�ϸ� �巡�� false
//        if (Input.GetMouseButtonUp(0))
//        {
//            isDragging = false;
//            selectedKnotIndex = -1;
//        }

//        // �� �� ���� �� ���콺 �巡�׷� � ���� ����
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

//    // ���ο� �� ���� �Լ�
//    void CreateNewKnot(Vector3 point)
//    {
//        var newKnot = new BezierKnot(point, Vector3.zero, Vector3.zero, Quaternion.identity);
//        splineContainer.Spline.Add(newKnot);

//        currentKnotIndex = splineContainer.Spline.Count - 1;
//        isCreatingPoint = true;
//    }

//    // ���� ���콺 ��ġ���� ���� ����� �� ã�� 
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

//    // Spline ��� ���
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

