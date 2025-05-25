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
//    //SerializedField : private 변수도 inspector 창에 노출시켜서 값 조정할 수 있도록 함
//    //LayerMask 사용하면 원하는 레이어만 충돌 감지 할 수 있음

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


//            pendingObject.transform.position = pos; //오브젝트 



//            if (Input.GetMouseButtonDown(0) && canPlace)
//            {
//                PlaceObject(); //마우스 왼쪽 클릭하면 오브젝트 배치
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
//        pendingObject = null; //이렇게 하면 배치 후에 수정 불가능
//    }

//    public void RotateObject()
//    {
//        pendingObject.transform.Rotate(Vector3.up, rotateAmount);
//    }

//    private void FixedUpdate()
//    {
//        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //마우스 커서 기준으로 카메라가 보고 있는 방향으로 광선 발사

//        if (Physics.Raycast(ray, out hit, 1000, layerMask)) //Ray가 오브젝트에 충돌했는지 확인 (최대 탐색 거리: 1000), layerMask 레이어만 감지
//        {
//            pos = hit.point; //충돌 위치 저장
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
//        pendingObject = Instantiate(objects[index], pos, transform.rotation); //pos 위치에 복제
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
//    //    float xDiff = pos % gridSize; //주어진 위치에서 그리드 크기를 나눈 나머지 계산
//    //    pos -= xDiff; //그리드에 맞춰 위치를 보정 (그리드의 시작 위치로 이동)
//    //    if (xDiff > (gridSize / 2)) // 나머지가 그리드 크기의 절반보다 크면
//    //    {
//    //        pos += gridSize; // 다음 그리드로 이동
//    //    }
//    //    return pos; // 보정된 위치 반환
//    //}
//}

//using System.Collections.Generic;
//using UnityEngine;

//[System.Serializable]
//public class ObjectPlacementData
//{
//    public GameObject prefab;        // 생성할 프리팹
//    public Vector3 spawnPosition;    // 생성 위치
//}

//public class BuildingManager : MonoBehaviour
//{
//    public List<ObjectPlacementData> objectPlacements; // Inspector에서 설정

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
    public GameObject prefab;        // 생성할 프리팹
    public Vector3 spawnPosition;    // 생성 위치
    public Vector3 spawnRotation; // 생성 각도
}


public class BuildingManager : MonoBehaviour
{
    public List<ObjectPlacementData> objectPlacements; // Inspector에서 설정

    [HideInInspector]
    public GameObject pendingObject;

    private Vector3 pos;

    private RaycastHit hit;
    [SerializeField] private LayerMask layerMask;
    //SerializedField : private 변수도 inspector 창에 노출시켜서 값 조정할 수 있도록 함
    //LayerMask 사용하면 원하는 레이어만 충돌 감지 할 수 있음

    public float rotateAmount = 45f;

    public bool canPlace = true;
    //[SerializeField] private Toggle gridToggle;

    private float movingObjectY; // 현재 이동 중인 오브젝트의 y값


    // Update is called once per frame
    void Update()
    {
        if (pendingObject != null)
        {
            pendingObject.transform.position = pos; //오브젝트 

            if (Input.GetMouseButtonDown(0) && canPlace && !IsPointerOverUI())
            {
                PlaceObject(); //마우스 왼쪽 클릭하면 오브젝트 배치
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
        pendingObject = null; //이렇게 하면 배치 후에 수정 불가능 (위치 확정 후 pendingObject 해제)
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

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //마우스 커서 기준으로 카메라가 보고 있는 방향으로 광선 발사

        if (Physics.Raycast(ray, out hit, 1000, layerMask)) //Ray가 오브젝트에 충돌했는지 확인 (최대 탐색 거리: 1000), layerMask 레이어만 감지
        {
            pos = hit.point; //충돌 위치 저장
            pos.y = movingObjectY; // 항상 저장된 y값으로 유지
        }
    }

    // 처음 지정 위치에 오브젝트 배치 
    public void PlaceObjectAtIndex(int index)
    {
        if (index < 0 || index >= objectPlacements.Count) return;

        ObjectPlacementData data = objectPlacements[index];
        Quaternion rotation = Quaternion.Euler(data.spawnRotation);
        Instantiate(data.prefab, data.spawnPosition, rotation);
    }

    // 이미 배치된 오브젝트를 이동 대상으로 지정 (Move 버튼에서 호출)
    public void StartMovingObject(GameObject obj)
    {
        pendingObject = obj;
        movingObjectY = obj.transform.position.y; // 오브젝트의 현재 y값 저장
    }

    // UI 위에 마우스가 있는지 체크 (버튼 클릭 시 배치 방지용)
    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}