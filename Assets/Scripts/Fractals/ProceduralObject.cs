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

    public virtual void Construct(
        Transform parentTransform,
        Mesh iniMesh, 
        Material iniMaterial, 
        Vector3 iniScale, 
        Vector3 iniPosition, 
        Quaternion iniRotation
    ){
        gameObject.AddComponent<MeshFilter>().mesh = iniMesh;
        gameObject.AddComponent<MeshRenderer>().material = iniMaterial;
        
        this.transform.parent = parentTransform;
        this.transform.position = iniPosition;
        this.transform.rotation = iniRotation;
        this.transform.localScale = iniScale;
    }

}
