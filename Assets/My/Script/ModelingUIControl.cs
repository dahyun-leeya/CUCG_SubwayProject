using UnityEngine;

public class ModelingUIController : MonoBehaviour
{
    public BuildingManager buildingManager;

    public void OnClickPlaceObject(int index)
    {
        buildingManager.PlaceObjectAtIndex(index);
    }
}
