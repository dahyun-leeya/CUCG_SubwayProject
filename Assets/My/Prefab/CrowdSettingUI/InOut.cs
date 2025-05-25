using UnityEngine;

public class InOut : MonoBehaviour
{

    public void SetRadius(float r){
        transform.localScale = new Vector3(r*2, transform.localScale.y, r*2);
    }

    public void SetTransparentMode(bool f)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null || renderer.material == null) return;

        Material mat = renderer.material;

        if (f)
        {
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0.3f);
        }
        else
        {
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 1.0f);
        }
    }


}
