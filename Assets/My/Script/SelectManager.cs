using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    public GameObject selectedObject;
    public TextMeshProUGUI objNameText;
    private BuildingManager buildingManager;
    public GameObject objUI;

    // 크기 조절 변수
    public Slider scaleSlider_x;
    public Slider scaleSlider_y;
    public Slider scaleSlider_z;
    public Slider uniformScaleSlider;

    private Vector3 originalScale;
    private Vector3 currentBaseScale = Vector3.one; // 사용자가 조절한 비율 기준값

    void Start()
    {
        buildingManager = GameObject.Find("BuildingManager").GetComponent<BuildingManager>();

        scaleSlider_x.onValueChanged.AddListener(UpdateScaleX);
        scaleSlider_y.onValueChanged.AddListener(UpdateScaleY);
        scaleSlider_z.onValueChanged.AddListener(UpdateScaleZ);
        uniformScaleSlider.onValueChanged.AddListener(UpdateUniformScale);

        scaleSlider_x.gameObject.SetActive(false);
        scaleSlider_y.gameObject.SetActive(false);
        scaleSlider_z.gameObject.SetActive(false);
        uniformScaleSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000))
            {
                if (hit.collider.gameObject.CompareTag("Object"))
                {
                    Select(hit.collider.gameObject);
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && selectedObject != null)
        {
            Deselect();
        }
    }

    private void Select(GameObject obj)
    {
        if (obj == selectedObject)
        {
            return;
        }

        if (selectedObject != null)
        {
            Deselect();
        }

        Outline outline = obj.GetComponent<Outline>();
        if (outline == null)
        {
            obj.AddComponent<Outline>();
        }
        else
        {
            outline.enabled = true;
        }

        objNameText.text = obj.name;
        objUI.SetActive(true);
        selectedObject = obj;

        originalScale = selectedObject.transform.localScale;
        currentBaseScale = originalScale;

        scaleSlider_x.value = originalScale.x;
        scaleSlider_y.value = originalScale.y;
        scaleSlider_z.value = originalScale.z;

        scaleSlider_x.gameObject.SetActive(true);
        scaleSlider_y.gameObject.SetActive(true);
        scaleSlider_z.gameObject.SetActive(true);
        uniformScaleSlider.gameObject.SetActive(true);
        uniformScaleSlider.value = 1f; // uniform 조절 초기값
    }

    private void Deselect()
    {
        objUI.SetActive(false);

        scaleSlider_x.gameObject.SetActive(false);
        scaleSlider_y.gameObject.SetActive(false);
        scaleSlider_z.gameObject.SetActive(false);
        uniformScaleSlider.gameObject.SetActive(false);

        selectedObject.GetComponent<Outline>().enabled = false;
        selectedObject = null;
    }

    public void Move()
    {
        buildingManager.StartMovingObject(selectedObject);
    }

    public void Delete()
    {
        GameObject objToDestroy = selectedObject;
        Deselect();
        Destroy(objToDestroy);
    }

    private void UpdateScaleX(float value)
    {
        if (selectedObject != null)
        {
            Vector3 scale = selectedObject.transform.localScale;
            scale.x = value;
            selectedObject.transform.localScale = scale;

            currentBaseScale = scale;
        }
    }

    private void UpdateScaleY(float value)
    {
        if (selectedObject != null)
        {
            Vector3 scale = selectedObject.transform.localScale;
            scale.y = value;
            selectedObject.transform.localScale = scale;

            currentBaseScale = scale;
        }
    }

    private void UpdateScaleZ(float value)
    {
        if (selectedObject != null)
        {
            Vector3 scale = selectedObject.transform.localScale;
            scale.z = value;
            selectedObject.transform.localScale = scale;

            currentBaseScale = scale;
        }
    }

    private void UpdateUniformScale(float value)
    {
        if (selectedObject != null)
        {
            Vector3 newScale = currentBaseScale * value;
            selectedObject.transform.localScale = newScale;
        }
    }

}
