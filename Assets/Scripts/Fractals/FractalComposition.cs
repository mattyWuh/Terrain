using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalComposition : MonoBehaviour
{
    // Editor settings
    [Header("Trunk Settings")]
    [SerializeField]
    private Mesh trunkMesh;

    [SerializeField]
    private Material trunkMaterial;

    [SerializeField]
    private float meanTrunkHeight;
    
    [SerializeField]
    private float stdTrunkHeight;
    
    [SerializeField]
    private float meanTrunkDiameter;

    [SerializeField]
    private float stdTrunkDiameter;


    [Header("Branch Settings")]
    [SerializeField]
    private Mesh branchMesh;

    [SerializeField]
    private Material branchMaterial;

    [SerializeField]
    private float meanBranchHeight;
    
    [SerializeField]
    private float stdBranchHeight;
    
    [SerializeField]
    private float meanBranchDiameter;

    [SerializeField]
    private float stdBranchDiameter;

    [SerializeField]
    private int numBranches;

    
    [Header("Leaf Settings")]
    [SerializeField]
    private Mesh leafMesh;

    [SerializeField]
    private Material leafMaterial;

    [SerializeField]
    private float meanLeafWidth;
    
    [SerializeField]
    private float stdLeafWidth;

    [SerializeField]
    private float meanLeafDepth;

    [SerializeField]
    private float stdLeafDepth;
    [SerializeField]
    private int numLeaves;


    // Private variables
    private FractalObject trunk;
    private GameObject leafConnector;
    private float radius = 0f;

    void Start()
    {
        ConstructTrunk();
        for (int i = 0; i < numBranches; i++)
        {
            ConstructBranch();
        }
        for (int j = 0; j < numLeaves; j++)
        {
            ConstructLeaf();
        }
        
    }

    void ConstructTrunk()
    {
        trunk = new GameObject("Trunk").AddComponent<FractalObject>();

        trunk.mesh = trunkMesh;
        trunk.material = trunkMaterial;

        float diameter = Noise.GaussianNoise(meanTrunkDiameter, stdTrunkDiameter);

        trunk.scale = new Vector3 (
            diameter,
            Noise.GaussianNoise(meanTrunkHeight, stdTrunkHeight),
            diameter
        );
        trunk.position = this.transform.position + Vector3.up * trunk.scale.y / 2f; 
        trunk.rotation = this.transform.rotation;

        trunk.ConstructChild(this.transform);
    }

    void ConstructBranch()
    {
        GameObject branchConnector = new GameObject("BranchConnector");
        branchConnector.transform.parent = trunk.transform;
        branchConnector.transform.position = trunk.transform.position + Vector3.up * Random.Range(0f, trunk.scale.y /2f);

        FractalObject branch = new GameObject("Branch").AddComponent<FractalObject>();
        branch.mesh = branchMesh;
        branch.material = branchMaterial;

        float diameter = Mathf.Min(1.5f * trunk.scale.x,Mathf.Max(0.1f, Noise.GaussianNoise(meanBranchDiameter, stdBranchDiameter)));
        float length = Mathf.Min(1.1f*trunk.scale.y, trunk.scale.x + Noise.GaussianNoise(meanBranchHeight, stdBranchHeight));
        branch.scale = new Vector3 (
            diameter,
            length,
            diameter
        );
        branch.rotation = Quaternion.Euler(90f*Random.Range(-1f,1f),0f,90f*Random.Range(-1f,1f));
        branch.position = branchConnector.transform.position;
        

        branch.ConstructChild(branchConnector.transform);
        branch.transform.position += branch.transform.up * branch.scale.y / 2;

        if (length > radius)
            radius = length;

    }

    void ConstructLeaf()
    {
        if (leafConnector is null)
        {
            leafConnector = new GameObject("LeafConnector");
            leafConnector.transform.parent = trunk.transform;
            leafConnector.transform.position = trunk.transform.position + Vector3.up * trunk.scale.y / 2f;
        }
        

        FractalObject leaf = new GameObject("Leaf").AddComponent<FractalObject>();
        leaf.mesh = leafMesh;
        leaf.material = leafMaterial;

        leaf.scale = new Vector3 (
            Noise.GaussianNoise(meanLeafWidth, stdLeafWidth),
            0.01f,
            Noise.GaussianNoise(meanLeafDepth, stdLeafDepth)
        );

        leaf.rotation = Quaternion.Euler(
            Random.Range(0f,360f),
            Random.Range(0f,360f),
            Random.Range(0f,360f)
        );

        leaf.position = leafConnector.transform.position + new Vector3(
            Random.Range(-radius, radius),
            Random.Range(-radius / 2f, radius),
            Random.Range(-radius, radius)
        );

        leaf.ConstructChild(leafConnector.transform);

    }
}