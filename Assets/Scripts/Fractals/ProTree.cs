using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProTree: MonoBehaviour 
{
    [SerializeField]
    private float meanTrunkHeight;

    [SerializeField]
    private float stdTrunkHeight;

    [SerializeField]
    private float meanBranchLength;

    [SerializeField]
    private float stdBranchLength;

    [SerializeField]
    private float meanLeafRadius;


    [SerializeField]
    private GameObject trunkPrefab;

    [SerializeField]
    private GameObject branchPrefab;

    [SerializeField]
    private GameObject leafPrefab;

    protected void Start()
    {

        GameObject trunk = ConstructTrunk();
        ConstructBranch(trunk);
    }

    protected GameObject ConstructTrunk()
    {
        Vector3 trunkScale = new Vector3(
            trunkPrefab.transform.localScale.x,
            Noise.GaussianNoise(meanTrunkHeight, stdTrunkHeight),
            trunkPrefab.transform.localScale.z
        );
        trunkPrefab.transform.localScale = trunkScale;
        GameObject trunk = Instantiate(trunkPrefab, this.gameObject.transform.position, Quaternion.identity) as GameObject;
        trunk.transform.parent = this.transform;
        return trunk;
    }

    protected void ConstructBranch(GameObject parent)
    {
        Vector3 branchScale = new Vector3(
            branchPrefab.transform.localScale.x,
            Noise.GaussianNoise(meanBranchLength, stdBranchLength),
            branchPrefab.transform.localScale.z
        );

        Quaternion branchRotation = Quaternion.Euler(
            90f * Random.Range(0f,1f), 0f, 0f
        );

        float thetaX = Mathf.Deg2Rad * (90f - branchRotation.x);

        Vector3 branchPosition = new Vector3(
            0f,
            Random.Range(0f, parent.transform.localScale.y/2f),
            0.5f * branchScale.y * Mathf.Cos(thetaX)
        );

        
        // branchPrefab.transform.localRotation = branchRotation;
        // branchPrefab.transform.localScale = branchScale;
        // branchPrefab.transform.localPosition = branchPosition;
        

        GameObject branch = Instantiate(branchPrefab, branchPosition, Quaternion.identity);
        branch.transform.parent = parent.transform;
    }

}
