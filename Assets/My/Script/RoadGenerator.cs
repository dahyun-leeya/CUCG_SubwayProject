using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RoadGenerator : MonoBehaviour
{
    private SplineContainer splineContainer;
    private Material roadMaterial;

    // 도로 너비 조절 슬라이더
    public Slider roadWidthSlider;
    private float originalWidth; //원래 너비 저장

    private float roadWidth = 2f;  // 기본값
    private int segmentCount = 100; // 도로 나눌 세그먼트 수

    private MeshFilter meshFilter;


    public void Init(SplineContainer container, Material material, Slider slider)
    {
        splineContainer = container;
        roadMaterial = material;
        roadWidthSlider = slider;

        meshFilter = GetComponent<MeshFilter>();
        var meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = roadMaterial;

        // 슬라이더 있으면 이벤트 리스너 등록하고 초기값 설정
        if (roadWidthSlider != null)
        {
            roadWidthSlider.onValueChanged.AddListener(UpdateRoadWidth); // 슬라이더 값 바뀌면 너비 업데이트
            roadWidthSlider.value = roadWidth; // 슬라이더 초기값 설정
        }
    }


    public void GenerateRoadMesh()
    {
        if (splineContainer == null) return; // 스플라인 없으면 종료

        Spline spline = splineContainer.Spline;
        List<Vector3> verts = new List<Vector3>(); // 정점 리스트
        List<int> tris = new List<int>(); // 삼각형 인덱스 리스트

        float totalLength = spline.GetLength(); // 스플라인 전체 길이 계산
        int resolution = Mathf.Max(segmentCount, Mathf.CeilToInt(totalLength * 10));

        // 각 세그먼트마다 두 개의 지점과 방향을 계산해서 정점 생성
        for (int i = 0; i < resolution; i++)
        {
            float t0 = (float)i / resolution;
            float t1 = (float)(i + 1) / resolution;

            if (t1 > 1f) break;

            Vector3 p0 = spline.EvaluatePosition(t0);
            Vector3 p1 = spline.EvaluatePosition(t1);

            // 각 지점에서의 진행 방향 (탄젠트) 계산
            Vector3 forward0 = ((Vector3)spline.EvaluateTangent(t0)).normalized;
            Vector3 forward1 = ((Vector3)spline.EvaluateTangent(t1)).normalized;

            // 진행 방향에 수직인 방향을 계산해서 도로 폭 구현
            Vector3 normal0 = Vector3.Cross(Vector3.up, forward0) * (roadWidth / 2f);
            Vector3 normal1 = Vector3.Cross(Vector3.up, forward1) * (roadWidth / 2f);

            // 도로 좌우의 정점 계산
            Vector3 v0 = p0 - normal0;
            Vector3 v1 = p0 + normal0;
            Vector3 v2 = p1 - normal1;
            Vector3 v3 = p1 + normal1;

            int idx = verts.Count;

            // 정점 리스트에 추가 (한 구간당 4개)
            verts.Add(v0);
            verts.Add(v1);
            verts.Add(v2);
            verts.Add(v3);

            // 삼각형 두 개로 한 면 구성
            tris.Add(idx);
            tris.Add(idx + 2);
            tris.Add(idx + 1);

            tris.Add(idx + 2);
            tris.Add(idx + 3);
            tris.Add(idx + 1);
        }

        // 현재 도로 너비를 저장
        originalWidth = roadWidth;
        roadWidthSlider.value = originalWidth;

        // 최종 메쉬 생성 및 설정
        Mesh mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;


    }

    // 슬라이더 값에 따라 도로 너비 변경
    private void UpdateRoadWidth(float value)
    {
        roadWidth = value;
        GenerateRoadMesh();
    }
}
