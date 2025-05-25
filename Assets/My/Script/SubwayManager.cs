using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SubwayManager : MonoBehaviour
{
    public TMP_InputField inputField; // 몇 량 할건지
    public Button generateTogglesButton;
    public Button generateTrainButton; // 지하철 생성 버튼
    public GameObject togglePrefab;
    public Transform toggleParent; // 토글 부모

    public GameObject chairTrainPrefab; // 의자 있는 칸 프리팹
    public GameObject noChairTrainPrefab; // 의자 없는 칸 프리팹

    public Transform trainParent; // 새로 추가: 열차 오브젝트를 담을 부모

    public float spacing = 19.5f; // 지하철 한 칸 길이가 19.5m

    private List<Toggle> toggles = new List<Toggle>();

    void Start()
    {
        generateTogglesButton.onClick.AddListener(CreateToggles);
        generateTrainButton.onClick.AddListener(GenerateTrainCars);
    }

    void CreateToggles()
    {
        foreach (Transform child in toggleParent)
        {
            Destroy(child.gameObject);
        }
        toggles.Clear();

        int count;
        if (int.TryParse(inputField.text, out count))
        {
            for (int i = 1; i <= count; i++)
            {
                GameObject toggleObj = Instantiate(togglePrefab, toggleParent);
                toggleObj.GetComponentInChildren<Text>().text = i + "호차";
                toggles.Add(toggleObj.GetComponent<Toggle>());
            }
        }
    }

    //토글 체크 하면 의자 없는 칸 / 체크 안하면 일반 칸
    //void GenerateTrainCars()
    //{
    //    //float spacing = 2.0f; // 옆으로 간격
    //    Vector3 startPos = new Vector3(-97.5f, 2.3f, 6.24f);

    //    for (int i = 0; i < toggles.Count; i++)
    //    {
    //        GameObject obj;
    //        if (toggles[i].isOn)
    //            obj = Instantiate(noChairPrefab); //의자 없는 칸 (지금은 빨간 cube)
    //        else
    //            obj = Instantiate(chairPrefab); // 의자 있는 칸 (지금은 파란 cube)

    //        obj.transform.position = startPos + new Vector3(i * spacing, 0, 0);
    //    }
    //}

    //토글 체크 하면 의자 없는 칸 / 체크 안하면 일반 칸
    //void GenerateTrainCars()
    //{
    //    // 기존 열차 삭제
    //    foreach (Transform child in trainParent)
    //    {
    //        Destroy(child.gameObject);
    //    }

    //    Vector3 startPos = new Vector3(87.75f, 2.4f, 6.24f); // 지하철 시작 위치

    //    for (int i = 0; i < toggles.Count; i++)
    //    {
    //        GameObject obj;
    //        if (toggles[i].isOn)
    //            obj = Instantiate(noChairTrainPrefab, trainParent); // 부모 설정
    //        else
    //            obj = Instantiate(chairTrainPrefab, trainParent); // 부모 설정

    //        obj.transform.position = startPos + new Vector3(-(toggles.Count - 1 - i) * spacing, 0, 0);
    //    }
    //}

    public void GenerateTrainCars()
    {
        if (trainParent == null)
        {
            Debug.LogError("Train Parent가 설정되지 않았습니다.");
            return;
        }

        // 자식 오브젝트들을 먼저 리스트로 수집
        List<GameObject> toDestroy = new List<GameObject>();

        foreach (Transform child in trainParent)
        {
            if (child != null)
            {
                toDestroy.Add(child.gameObject);
            }
        }

        // 리스트에 있는 오브젝트를 삭제
        foreach (GameObject go in toDestroy)
        {
            if (go != null)
                Destroy(go);
        }

        // 열차 칸 생성
        Vector3 startPos = new Vector3(87.75f, 1.5f, 1.56f); // 지하철 시작 위치

        for (int i = 0; i < toggles.Count; i++)
        {
            GameObject obj;
            if (toggles[i].isOn)
                obj = Instantiate(noChairTrainPrefab, trainParent); // 부모 설정
            else
                obj = Instantiate(chairTrainPrefab, trainParent); // 부모 설정

            obj.transform.position = startPos + new Vector3(-(toggles.Count - 1 - i) * spacing, 0, 0);
        }

    }


}

