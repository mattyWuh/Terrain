using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralComposition : MonoBehaviour
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

    [SerializeField]
    private AggregateLeafShapes aggregateLeafShape;


    // Private variables
    private ProceduralObject trunk;
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

    /// <summary>
    /// Generates a connector GameObject with localScale (1,1,1).
    /// </summary>
    /// <param name="name">Object name.</param>
    /// <param name="parentTransform">Parent object transform.</param>
    /// <param name="localPosition">Position relative to parent position.</param>
    /// <returns>
    /// The connector GameObject.
    /// </returns>
    private GameObject _ConstructConnector(
        string name, Transform parentTransform, Vector3 localPosition
    ){
        GameObject connector = new GameObject(name);
        connector.transform.parent = parentTransform;
        connector.transform.position = parentTransform.position + localPosition;
        return connector;
    }

    /// <summary>
    /// Generates ProceduralObject as a child to parent object.
    /// </summary>
    /// <param name="name">Object name.</param>
    /// <param name="parentTransform">Parent object transform.</param>
    /// <param name="mesh">Object mesh.</param>
    /// <param name="material">Object material.</param>
    /// <param name="scale">Object dimensions.</param>
    /// <param name="position">Position relative to parent position.</param>
    /// <param name="rotation">Object rotation.</param>
    /// <returns>
    /// The new ProceduralObject.
    /// </returns>
    private ProceduralObject _ConstructObject(
        string name, 
        Transform parentTransform, 
        Mesh mesh, 
        Material material, 
        Vector3 scale, 
        Vector3 position, 
        Quaternion rotation
    )
    {
        ProceduralObject newObject = new GameObject(name).AddComponent<ProceduralObject>();
        newObject.Construct(parentTransform, mesh, material, scale, position, rotation);
        return newObject;
    }

    void ConstructTrunk()
    {
        // Use diameter for X and Z scale, height for Y scale.
        float diameter = Mathf.Max(
            0.1f, Noise.GaussianNoise(meanTrunkDiameter, stdTrunkDiameter)
        );
        float height = Noise.GaussianNoise(meanTrunkHeight, stdTrunkHeight);

        // Make trunk with bottom at master GameObject position.
        trunk = _ConstructObject(
            "Trunk", 
            this.transform, 
            trunkMesh, 
            trunkMaterial, 
            new Vector3(diameter, height, diameter), 
            this.transform.position + Vector3.up * height / 2.0f, 
            this.transform.rotation
        );
    }

    void ConstructBranch()
    {
        // Reference trunk height and radius.
        float trunkHeight = trunk.transform.localScale.y;
        float trunkRadius = trunk.transform.localScale.x / 2.0f;

        // Make connector on top half of trunk.
        GameObject branchConnector = _ConstructConnector(
            "BranchConnector", 
            trunk.transform, 
            Vector3.up * Random.Range(0.0f, trunkHeight / 2.0f)
        );

        // Branch diameter is between 0.1 and the trunk radius.
        float diameter = Mathf.Min(
            trunkRadius,
            Mathf.Max(
                0.1f, 
                Noise.GaussianNoise(meanBranchDiameter, stdBranchDiameter)
            )
        );

        // Branch length is at most 1.1 * trunk height.
        float length = Mathf.Min(
            1.1f*trunkHeight, 
            trunkRadius + Noise.GaussianNoise(meanBranchHeight, stdBranchHeight)
        );

        // Make branch with any rotation into the Y-plane.
        ProceduralObject branch = _ConstructObject(
            "Branch", 
            branchConnector.transform, 
            branchMesh, 
            branchMaterial, 
            new Vector3(diameter, length, diameter), 
            branchConnector.transform.position, 
            Quaternion.Euler(
                90.0f*Random.Range(-1.0f,1.0f),
                0.0f,
                90.0f*Random.Range(-1.0f,1.0f)
            )
        );

        // Make sure branch end is at trunk centerline.
        branch.transform.position += branch.transform.up * length / 2.0f;

        // Store the longest branch for future leaf space.
        if (length > radius)
            radius = length;

    }

    void ConstructLeaf()
    {
        // Only one leaf connector is required.
        if (leafConnector is null)
        {
            leafConnector = _ConstructConnector(
                "LeafConnector", 
                trunk.transform,
                Vector3.up * trunk.transform.localScale.y / 2.0f
            );
        }
        
        // Leaf is flat with randomized XZ-plane.
        Vector3 scale = new Vector3 (
            Noise.GaussianNoise(meanLeafWidth, stdLeafWidth),
            0.01f,
            Noise.GaussianNoise(meanLeafDepth, stdLeafDepth)
        );

        // Rotate leaf orientation to any direction.
        Quaternion rotation = Quaternion.Euler(
            Random.Range(0.0f,360.0f),
            Random.Range(0.0f,360.0f),
            Random.Range(0.0f,360.0f)
        );

        // Leaf position is relative to the leaf connector.
        Vector3 position = leafConnector.transform.position;

        // Generate leaf within chosen shape.
        switch (aggregateLeafShape)
        {
            // Cone with radius limited by longest branch and height of longest
            // branch.
            case AggregateLeafShapes.cone:
                float bottom = -radius / 2.0f;
                float h = Random.Range(bottom, radius);
                float rhoCone = Random.Range(0.0f, radius * (h - radius) / (bottom - radius));
                float phiCone = Random.Range(0.0f, 2.0f) * Mathf.PI;
                position += new Vector3(
                    rhoCone * Mathf.Cos(phiCone),
                    h,
                    rhoCone * Mathf.Sin(phiCone)
                );
                break;

            // Cube with XZ dimensions limited by longest branch and Y
            // dimension limited by 3/4 of longest branch.
            case AggregateLeafShapes.cube:
                position += new Vector3(
                    Random.Range(-radius, radius),
                    Random.Range(-radius / 2.0f, radius),
                    Random.Range(-radius, radius)
                );
                break;
            
            // Cylinder with radius of longest branch Y dimension limited by
            // 3/4 of longest branch.
            case AggregateLeafShapes.cylinder:
                float rhoCylinder = Random.Range(0.0f, radius);
                float phiCylinder = Random.Range(0.0f, 2.0f) * Mathf.PI;
                position += new Vector3(
                    rhoCylinder * Mathf.Cos(phiCylinder),
                    Random.Range(-radius / 2.0f, radius),
                    rhoCylinder * Mathf.Sin(phiCylinder)
                );
                break;

            // Sphere with radius of longest branch, sweeping down to 70% of 
            // total sphere.
            case AggregateLeafShapes.sphere:
                float rhoSphere = Random.Range(0.0f, radius);
                float thetaSphere = Random.Range(0.0f, 0.7f) * Mathf.PI;
                float phiSphere = Random.Range(0.0f, 2.0f) * Mathf.PI;
                position += new Vector3(
                    rhoSphere * Mathf.Sin(thetaSphere) * Mathf.Cos(phiSphere),
                    rhoSphere * Mathf.Cos(thetaSphere),
                    rhoSphere * Mathf.Sin(thetaSphere) * Mathf.Sin(phiSphere)
                );
                break;
        }

        ProceduralObject leaf = _ConstructObject(
            "Leaf",
            leafConnector.transform,
            leafMesh,
            leafMaterial,
            scale,
            position,
            rotation
        );
    }
}

public enum AggregateLeafShapes
{
    cone,
    cube,
    cylinder,
    sphere
}