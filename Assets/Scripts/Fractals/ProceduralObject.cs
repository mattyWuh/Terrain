using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralObject : MonoBehaviour
{
    [SerializeField]
    internal Mesh mesh;

    [SerializeField]
    internal Material material;

    public Vector3 scale;

    public Vector3 position;

    public Quaternion rotation;

    public virtual void Construct(Transform parentTransform)
    {
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;
        
        this.transform.parent = parentTransform;
        this.transform.position = position;
        this.transform.rotation = rotation;
        this.transform.localScale = scale;
    }

}
