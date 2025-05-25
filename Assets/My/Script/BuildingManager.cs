//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//public class BuildingManager : MonoBehaviour
//{
//    public GameObject[] objects;
//    public GameObject pendingObject;
//    [SerializeField] private Material[] materials;

//    private Vector3 pos;

//    private RaycastHit hit;
//    [SerializeField] private LayerMask layerMask;
//    //SerializedField : private ������ inspector â�� ������Ѽ� �� ������ �� �ֵ��� ��
//    //LayerMask ����ϸ� ���ϴ� ���̾ �浹 ���� �� �� ����

//    public float rotateAmount;

//    //public float gridSize;
//    //bool gridOn = true;
//    public bool canPlace = true;
//    //[SerializeField] private Toggle gridToggle;


//    // Update is called once per frame
//    void Update()
//    {
//        if (pendingObject != null)
//        {
//            UpdateMaterials();
//            //if (gridOn)
//            //{
//            //    pendingObject.transform.position = new Vector3(
//            //        RoundToNearestGrid(pos.x),
//            //        RoundToNearestGrid(pos.y),
//            //        RoundToNearestGrid(pos.z)
//            //        );
//            //}


//            pendingObject.transform.position = pos; //������Ʈ 



//            if (Input.GetMouseButtonDown(0) && canPlace)
//            {
//                PlaceObject(); //���콺 ���� Ŭ���ϸ� ������Ʈ ��ġ
//            }
//            if (Input.GetKeyDown(KeyCode.R))
//            {
//                RotateObject();
//            }

//            //UpdateMaterials();
//        }
//    }

//    public void PlaceObject()
//    {
//        pendingObject.GetComponent<MeshRenderer>().material = materials[2];
//        pendingObject = null; //�̷��� �ϸ� ��ġ �Ŀ� ���� �Ұ���
//    }

//    public void RotateObject()
//    {
//        pendingObject.transform.Rotate(Vector3.up, rotateAmount);
//    }

//    private void FixedUpdate()
//    {
//        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //���콺 Ŀ�� �������� ī�޶� ���� �ִ� �������� ���� �߻�

//        if (Physics.Raycast(ray, out hit, 1000, layerMask)) //Ray�� ������Ʈ�� �浹�ߴ��� Ȯ�� (�ִ� Ž�� �Ÿ�: 1000), layerMask ���̾ ����
//        {
//            pos = hit.point; //�浹 ��ġ ����
//        }
//    }

//    void UpdateMaterials()
//    {
//        if (canPlace)
//        {
//            pendingObject.GetComponent<MeshRenderer>().material = materials[0];
//        }
//        else
//        {
//            pendingObject.GetComponent<MeshRenderer>().material = materials[1];
//        }
//    }

//    public void SelectObject(int index)
//    {
//        pendingObject = Instantiate(objects[index], pos, transform.rotation); //pos ��ġ�� ����
//    }

//    //public void ToggleGrid()
//    //{
//    //    if (gridToggle.isOn)
//    //    {
//    //        gridOn = true;
//    //    }
//    //    else
//    //    {
//    //        gridOn = false;
//    //    }
//    //}

//    //float RoundToNearestGrid(float pos)
//    //{
//    //    float xDiff = pos % gridSize; //�־��� ��ġ���� �׸��� ũ�⸦ ���� ������ ���
//    //    pos -= xDiff; //�׸��忡 ���� ��ġ�� ���� (�׸����� ���� ��ġ�� �̵�)
//    //    if (xDiff > (gridSize / 2)) // �������� �׸��� ũ���� ���ݺ��� ũ��
//    //    {
//    //        pos += gridSize; // ���� �׸���� �̵�
//    //    }
//    //    return pos; // ������ ��ġ ��ȯ
//    //}
//}

//using System.Collections.Generic;
//using UnityEngine;

//[System.Serializable]
//public class ObjectPlacementData
//{
//    public GameObject prefab;        // ������ ������
//    public Vector3 spawnPosition;    // ���� ��ġ
//}

//public class BuildingManager : MonoBehaviour
//{
//    public List<ObjectPlacementData> objectPlacements; // Inspector���� ����

//    [HideInInspector]
//    public GameObject pendingObject;

//    public void PlaceObjectAtIndex(int index)
//    {
//        if (index < 0 || index >= objectPlacements.Count) return;

//        ObjectPlacementData data = objectPlacements[index];
//        GameObject obj = Instantiate(data.prefab, data.spawnPosition, Quaternion.identity);
//        pendingObject = obj;
//    }
//}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class ObjectPlacementData
{
    public GameObject prefab;        // ������ ������
    public Vector3 spawnPosition;    // ���� ��ġ
    public Vector3 spawnRotation; // ���� ����
}


public class BuildingManager : MonoBehaviour
{
    public List<ObjectPlacementData> objectPlacements; // Inspector���� ����

    [HideInInspector]
    public GameObject pendingObject;

    private Vector3 pos;

    private RaycastHit hit;
    [SerializeField] private LayerMask layerMask;
    //SerializedField : private ������ inspector â�� ������Ѽ� �� ������ �� �ֵ��� ��
    //LayerMask ����ϸ� ���ϴ� ���̾ �浹 ���� �� �� ����

    public float rotateAmount = 45f;

    public bool canPlace = true;
    //[SerializeField] private Toggle gridToggle;

    private float movingObjectY; // ���� �̵� ���� ������Ʈ�� y��


    // Update is called once per frame
    void Update()
    {
        if (pendingObject != null)
        {
            pendingObject.transform.position = pos; //������Ʈ 

            if (Input.GetMouseButtonDown(0) && canPlace && !IsPointerOverUI())
            {
                PlaceObject(); //���콺 ���� Ŭ���ϸ� ������Ʈ ��ġ
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateObject();
            }
            if (Input.GetKeyUp(KeyCode.T))
            {
                RotateObjectZ();
            }
        }
    }

    public void PlaceObject()
    {
        pendingObject = null; //�̷��� �ϸ� ��ġ �Ŀ� ���� �Ұ��� (��ġ Ȯ�� �� pendingObject ����)
    }

    public void RotateObject()
    {
        if (pendingObject != null)
        {
            pendingObject.transform.Rotate(Vector3.up, rotateAmount);
        }
    }

    public void RotateObjectZ()
    {
        if (pendingObject != null)
        {
            pendingObject.transform.Rotate(Vector3.forward, rotateAmount);
        }
    }

    private void FixedUpdate()
    {
        if(pendingObject == null)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //���콺 Ŀ�� �������� ī�޶� ���� �ִ� �������� ���� �߻�

        if (Physics.Raycast(ray, out hit, 1000, layerMask)) //Ray�� ������Ʈ�� �浹�ߴ��� Ȯ�� (�ִ� Ž�� �Ÿ�: 1000), layerMask ���̾ ����
        {
            pos = hit.point; //�浹 ��ġ ����
            pos.y = movingObjectY; // �׻� ����� y������ ����
        }
    }

    // ó�� ���� ��ġ�� ������Ʈ ��ġ 
    public void PlaceObjectAtIndex(int index)
    {
        if (index < 0 || index >= objectPlacements.Count) return;

        ObjectPlacementData data = objectPlacements[index];
        Quaternion rotation = Quaternion.Euler(data.spawnRotation);
        Instantiate(data.prefab, data.spawnPosition, rotation);
    }

    // �̹� ��ġ�� ������Ʈ�� �̵� ������� ���� (Move ��ư���� ȣ��)
    public void StartMovingObject(GameObject obj)
    {
        pendingObject = obj;
        movingObjectY = obj.transform.position.y; // ������Ʈ�� ���� y�� ����
    }

    // UI ���� ���콺�� �ִ��� üũ (��ư Ŭ�� �� ��ġ ������)
    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}