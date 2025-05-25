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

    // ���� �ʺ� ���� �����̴�
    public Slider roadWidthSlider;
    private float originalWidth; //���� �ʺ� ����

    private float roadWidth = 2f;  // �⺻��
    private int segmentCount = 100; // ���� ���� ���׸�Ʈ ��

    private MeshFilter meshFilter;


    public void Init(SplineContainer container, Material material, Slider slider)
    {
        splineContainer = container;
        roadMaterial = material;
        roadWidthSlider = slider;

        meshFilter = GetComponent<MeshFilter>();
        var meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = roadMaterial;

        // �����̴� ������ �̺�Ʈ ������ ����ϰ� �ʱⰪ ����
        if (roadWidthSlider != null)
        {
            roadWidthSlider.onValueChanged.AddListener(UpdateRoadWidth); // �����̴� �� �ٲ�� �ʺ� ������Ʈ
            roadWidthSlider.value = roadWidth; // �����̴� �ʱⰪ ����
        }
    }


    public void GenerateRoadMesh()
    {
        if (splineContainer == null) return; // ���ö��� ������ ����

        Spline spline = splineContainer.Spline;
        List<Vector3> verts = new List<Vector3>(); // ���� ����Ʈ
        List<int> tris = new List<int>(); // �ﰢ�� �ε��� ����Ʈ

        float totalLength = spline.GetLength(); // ���ö��� ��ü ���� ���
        int resolution = Mathf.Max(segmentCount, Mathf.CeilToInt(totalLength * 10));

        // �� ���׸�Ʈ���� �� ���� ������ ������ ����ؼ� ���� ����
        for (int i = 0; i < resolution; i++)
        {
            float t0 = (float)i / resolution;
            float t1 = (float)(i + 1) / resolution;

            if (t1 > 1f) break;

            Vector3 p0 = spline.EvaluatePosition(t0);
            Vector3 p1 = spline.EvaluatePosition(t1);

            // �� ���������� ���� ���� (ź��Ʈ) ���
            Vector3 forward0 = ((Vector3)spline.EvaluateTangent(t0)).normalized;
            Vector3 forward1 = ((Vector3)spline.EvaluateTangent(t1)).normalized;

            // ���� ���⿡ ������ ������ ����ؼ� ���� �� ����
            Vector3 normal0 = Vector3.Cross(Vector3.up, forward0) * (roadWidth / 2f);
            Vector3 normal1 = Vector3.Cross(Vector3.up, forward1) * (roadWidth / 2f);

            // ���� �¿��� ���� ���
            Vector3 v0 = p0 - normal0;
            Vector3 v1 = p0 + normal0;
            Vector3 v2 = p1 - normal1;
            Vector3 v3 = p1 + normal1;

            int idx = verts.Count;

            // ���� ����Ʈ�� �߰� (�� ������ 4��)
            verts.Add(v0);
            verts.Add(v1);
            verts.Add(v2);
            verts.Add(v3);

            // �ﰢ�� �� ���� �� �� ����
            tris.Add(idx);
            tris.Add(idx + 2);
            tris.Add(idx + 1);

            tris.Add(idx + 2);
            tris.Add(idx + 3);
            tris.Add(idx + 1);
        }

        // ���� ���� �ʺ� ����
        originalWidth = roadWidth;
        roadWidthSlider.value = originalWidth;

        // ���� �޽� ���� �� ����
        Mesh mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;


    }

    // �����̴� ���� ���� ���� �ʺ� ����
    private void UpdateRoadWidth(float value)
    {
        roadWidth = value;
        GenerateRoadMesh();
    }
}
