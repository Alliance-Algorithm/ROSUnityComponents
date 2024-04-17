using UnityEngine;

public class CloseRender : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var i in meshRenderers)
            i.enabled = false;
    }

}
