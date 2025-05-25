using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SubwayManager : MonoBehaviour
{
    public TMP_InputField inputField; // �� �� �Ұ���
    public Button generateTogglesButton;
    public Button generateTrainButton; // ����ö ���� ��ư
    public GameObject togglePrefab;
    public Transform toggleParent; // ��� �θ�

    public GameObject chairTrainPrefab; // ���� �ִ� ĭ ������
    public GameObject noChairTrainPrefab; // ���� ���� ĭ ������

    public Transform trainParent; // ���� �߰�: ���� ������Ʈ�� ���� �θ�

    public float spacing = 19.5f; // ����ö �� ĭ ���̰� 19.5m

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
                toggleObj.GetComponentInChildren<Text>().text = i + "ȣ��";
                toggles.Add(toggleObj.GetComponent<Toggle>());
            }
        }
    }

    //��� üũ �ϸ� ���� ���� ĭ / üũ ���ϸ� �Ϲ� ĭ
    //void GenerateTrainCars()
    //{
    //    //float spacing = 2.0f; // ������ ����
    //    Vector3 startPos = new Vector3(-97.5f, 2.3f, 6.24f);

    //    for (int i = 0; i < toggles.Count; i++)
    //    {
    //        GameObject obj;
    //        if (toggles[i].isOn)
    //            obj = Instantiate(noChairPrefab); //���� ���� ĭ (������ ���� cube)
    //        else
    //            obj = Instantiate(chairPrefab); // ���� �ִ� ĭ (������ �Ķ� cube)

    //        obj.transform.position = startPos + new Vector3(i * spacing, 0, 0);
    //    }
    //}

    //��� üũ �ϸ� ���� ���� ĭ / üũ ���ϸ� �Ϲ� ĭ
    //void GenerateTrainCars()
    //{
    //    // ���� ���� ����
    //    foreach (Transform child in trainParent)
    //    {
    //        Destroy(child.gameObject);
    //    }

    //    Vector3 startPos = new Vector3(87.75f, 2.4f, 6.24f); // ����ö ���� ��ġ

    //    for (int i = 0; i < toggles.Count; i++)
    //    {
    //        GameObject obj;
    //        if (toggles[i].isOn)
    //            obj = Instantiate(noChairTrainPrefab, trainParent); // �θ� ����
    //        else
    //            obj = Instantiate(chairTrainPrefab, trainParent); // �θ� ����

    //        obj.transform.position = startPos + new Vector3(-(toggles.Count - 1 - i) * spacing, 0, 0);
    //    }
    //}

    public void GenerateTrainCars()
    {
        if (trainParent == null)
        {
            Debug.LogError("Train Parent�� �������� �ʾҽ��ϴ�.");
            return;
        }

        // �ڽ� ������Ʈ���� ���� ����Ʈ�� ����
        List<GameObject> toDestroy = new List<GameObject>();

        foreach (Transform child in trainParent)
        {
            if (child != null)
            {
                toDestroy.Add(child.gameObject);
            }
        }

        // ����Ʈ�� �ִ� ������Ʈ�� ����
        foreach (GameObject go in toDestroy)
        {
            if (go != null)
                Destroy(go);
        }

        // ���� ĭ ����
        Vector3 startPos = new Vector3(87.75f, 1.5f, 1.56f); // ����ö ���� ��ġ

        for (int i = 0; i < toggles.Count; i++)
        {
            GameObject obj;
            if (toggles[i].isOn)
                obj = Instantiate(noChairTrainPrefab, trainParent); // �θ� ����
            else
                obj = Instantiate(chairTrainPrefab, trainParent); // �θ� ����

            obj.transform.position = startPos + new Vector3(-(toggles.Count - 1 - i) * spacing, 0, 0);
        }

    }


}

