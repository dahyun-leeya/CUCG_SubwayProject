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

//    // Ʈ����: �浹 ���� (Ʈ���� ���� Ȱ��ȭ -> Collider�� Is Trigger �ɼ� üũ�ϱ�)
//    private void OnTriggerEnter(Collider other) // Ʈ���� ���� ������ collider���� �浹�� �� ȣ���
//    {
//        if (other.gameObject.CompareTag("Object"))
//        {
//            buildingManager.canPlace = false;
//        }
//    }

//    private void OnTriggerExit(Collider other) // Ʈ���� ���� ������ collider���� �浹�� ���� �� ȣ���
//    {
//        if (other.gameObject.CompareTag("Object"))
//        {
//            buildingManager.canPlace = true;
//        }
//    }

//}
