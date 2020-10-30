using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalObject : MonoBehaviour
{
    [SerializeField]
    internal Mesh mesh;

    [SerializeField]
    internal Material material;

    public Vector3 scale;

    public Vector3 position;

    public Quaternion rotation;

    protected virtual void Construct()
    {
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;
        
        this.transform.position = position;
        this.transform.rotation = rotation;
        this.transform.localScale = scale;
    }

    public void ConstructChild(Transform parentTransform)
    {
        this.transform.parent = parentTransform;
        Construct();
    }

}
