using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class LSystem : MonoBehaviour
{
    public int iterations = 3;
    public float angle = 5f;
    public float width = 0.01f;
    public float minLeafLength = 0.2f;
    public float maxLeafLength = 0.5f;
    public float minBranchLength = 1f;
    public float maxBranchLength = 2f;
    public float variance = 10f;

    public GameObject tree;
    public GameObject branch;
    public GameObject leaf;

    private const string axiom = "X";

    private Dictionary<char, string> rules = new Dictionary<char, string>();
    private Stack<SavedTransform> savedTransforms = new Stack<SavedTransform>();
    private Stack<SavedTransform> transformStack = new Stack<SavedTransform>();

    private Vector3 initialPosition;

    private string currentPath = "";
    private float[] randomRotations;

    private void Awake() {
        randomRotations = new float[1000];
        for (int i = 0; i < randomRotations.Length; i++)
        {
            randomRotations[i] = Random.Range(-1f, 1f);
        }

        rules.Add('X', "[-FX][+FX][FX]");
        rules.Add('F', "FF");

        Generate();
    }

    private void Generate()
    {
        currentPath = axiom;

        StringBuilder stringBuilder = new StringBuilder();

        for (int i = 0; i < iterations; i++)
        {
            char[] currentPathChars = currentPath.ToCharArray();
            for (int j = 0; j < currentPathChars.Length; j++)
            {
                stringBuilder.Append(rules.ContainsKey(currentPathChars[j]) ? rules[currentPathChars[j]] : currentPathChars[j].ToString());
            }

            currentPath = stringBuilder.ToString();
            stringBuilder = new StringBuilder();
        }

        for (int k = 0; k < currentPath.Length; k++)
        {
            switch(currentPath[k])
            {
                case 'F':
                    initialPosition = transform.position;
                    bool isLeaf = false;

                    GameObject currentElement;
                    if (currentPath[k+1] % currentPath.Length == 'X' || currentPath[k+3] % currentPath.Length == 'F' && currentPath[k+4] % currentPath.Length == 'X')
                    {
                        currentElement = Instantiate(leaf);
                        isLeaf = true;
                    }
                    else
                    {
                        currentElement = Instantiate(branch);
                    }

                    currentElement.transform.SetParent(tree.transform);

                    TreeElement currentTreeElement = currentElement.GetComponent<TreeElement>();

                    if (isLeaf)
                    {
                        currentElement.transform.Translate(Vector3.up * 2f * Random.Range(minLeafLength, maxLeafLength));
                    }
                    else
                    {
                        currentElement.transform.Translate(Vector3.up * 2f * Random.Range(minBranchLength, maxBranchLength));
                    }

                    currentTreeElement.lineRenderer.startWidth = width;
                    currentTreeElement.lineRenderer.endWidth = width;
                    currentTreeElement.lineRenderer.sharedMaterial = currentTreeElement.material;
                    break;
                
                case 'X':
                    break;
                
                case '+':
                    transform.Rotate(Vector3.forward * angle * (1f + variance / 100f * randomRotations[k % randomRotations.Length]));
                    break;

                case '-':
                    transform.Rotate(Vector3.back * angle * (1f + variance / 100f * randomRotations[k % randomRotations.Length]));
                    break;

                case '*':
                    transform.Rotate(Vector3.up * 120f * (1f + variance / 100f * randomRotations[k % randomRotations.Length]));
                    break;

                case '/':
                    transform.Rotate(Vector3.down * 120f * (1f + variance / 100f * randomRotations[k % randomRotations.Length]));
                    break;
                
                case '[':
                    transformStack.Push(new SavedTransform() {
                        position = transform.position,
                        rotation = transform.rotation
                    });
                    break;

                case ']':
                    SavedTransform savedTransform = transformStack.Pop();

                    transform.position = savedTransform.position;
                    transform.rotation = savedTransform.rotation;
                    break;
            }
        }
    }
}
