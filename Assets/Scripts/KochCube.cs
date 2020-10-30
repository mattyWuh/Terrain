using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KochCube : MonoBehaviour
{
    [SerializeField]
    private int maxDepth;

    [SerializeField]
    private float childScale;

    [SerializeField]
    private float spawnProbability;

    [SerializeField]
    private Mesh mesh;

    [SerializeField]
    private Material material;

    private int depth;

    void Start()
    {
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;

        if (depth < maxDepth)
        {
            StartCoroutine(CreateChildren());
        }
            
    }

    // void Update() 
    // {
    //     transform.Rotate(0f, 30f * Time.deltaTime, 0f);
    // }



    private static Vector3[] childDirections = {
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };

    private static Quaternion[] childOrientations = {
        Quaternion.identity,
        Quaternion.Euler(0f, 0f, -90f),
        Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0f),
        Quaternion.Euler(-90f, 0f, 0f),
        Quaternion.Euler(0f, 0f, -90f),
        Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0f),
        Quaternion.Euler(-90f, 0f, 0f)
        // Quaternion.Euler(0f, 0f, -90f),
        // Quaternion.Euler(0f, 0f, 90f),
        // Quaternion.Euler(0f, 0f, -90f),
        // Quaternion.Euler(0f, 0f, 90f)
    };

    private IEnumerator CreateChildren()
    {
        for (int i = 0; i < childOrientations.Length; i++)
        {
            if (Random.value < spawnProbability){
                yield return new WaitForSeconds(0f);
                new GameObject("KochCube Child").AddComponent<KochCube>().Initialize(this, i);
            }
            
        }
    }

    private void Initialize (KochCube parent, int childIndex)
    {
        mesh = parent.mesh;
        material = parent.material;
        maxDepth = parent.maxDepth;
        depth = parent.depth + 1;
        childScale = parent.childScale;
        spawnProbability = parent.spawnProbability;

        float adjScale = 0.5f + 0.5f * childScale;
        float perpScale = 0.5f + 1.5f * childScale;
        Vector3 relevel = Vector3.zero; //Vector3.down * 0.5f * childScale;

        Vector3[] childPositions = new[] {
            Vector3.up * adjScale,
            Vector3.right * adjScale,
            Vector3.left * adjScale,
            Vector3.forward * adjScale,
            Vector3.back * adjScale,
            Vector3.right * perpScale + relevel,
            Vector3.left * perpScale + relevel,
            Vector3.forward * perpScale + relevel,
            Vector3.back * perpScale + relevel
            // Vector3.right * perpScale + Vector3.forward*perpScale + relevel,
            // Vector3.left * perpScale + Vector3.forward*perpScale + relevel,
            // Vector3.right * perpScale + Vector3.back*perpScale + relevel,
            // Vector3.left * perpScale + Vector3.back*perpScale + relevel
        };

        
        transform.parent = parent.transform;
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = childPositions[childIndex];
        transform.localRotation = childOrientations[childIndex];
    }

}
