//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CheckPlacement : MonoBehaviour
//{
//    BuildingManager buildingManager;

//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        buildingManager = GameObject.Find("BuildingManager").GetComponent<BuildingManager>();
//    }

//    // 트리거: 충돌 영역 (트리거 영역 활성화 -> Collider의 Is Trigger 옵션 체크하기)
//    private void OnTriggerEnter(Collider other) // 트리거 모드로 설정된 collider끼리 충돌할 때 호출됨
//    {
//        if (other.gameObject.CompareTag("Object"))
//        {
//            buildingManager.canPlace = false;
//        }
//    }

//    private void OnTriggerExit(Collider other) // 트리거 모드로 설정된 collider끼리 충돌이 끝날 때 호출됨
//    {
//        if (other.gameObject.CompareTag("Object"))
//        {
//            buildingManager.canPlace = true;
//        }
//    }

//}
