using System.Collections;
using UnityEngine;

public class ChildGenerationInfo
{
    public Vector3 direction;
    public Quaternion rotationDegrees;
}

public class Fractal : MonoBehaviour
{
    public Mesh[] meshs;
    public Material material;
    private Material[,] materials;
    [Range(0.0f, 1.0f)]
    public float spawnPossiblities;

    private float rotationSpeed;
    public float maxRotationSpeed;
    public float maxTwist;

    public int depth;
    public int maxDepth;

    [Range(0.0f, 1.0f)]
    public float childScale;

    private void InitializeMaterials()
    {
        materials = new Material[maxDepth + 1, 2];
        for (int i = 0; i <= maxDepth; i++)
        {
            float t = i / (maxDepth - 1.0f);
            t *= t;
            materials[i, 0] = new Material(material)
            {

                color = Color.Lerp(Color.white, Color.yellow, t)
            };
            materials[i, 1] = new Material(material)
            {

                color = Color.Lerp(Color.white, Color.cyan, t)
            };
        }
        materials[maxDepth, 0].color = Color.magenta;
        materials[maxDepth, 1].color = Color.red;
    }

    private void Start()
    {
        if (materials == null)
        {
            InitializeMaterials();
        }
        rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
        transform.Rotate(Random.Range(-maxTwist, maxTwist), 0, 0);
        gameObject.AddComponent<MeshFilter>().mesh = meshs[Random.Range(0, 3)];
        gameObject.AddComponent<MeshRenderer>().material = materials[depth, Random.Range(0, 2)];

        if (depth < maxDepth)
        {
            StartCoroutine(CreateChildren());
        }
    }

    private IEnumerator ChangeRotationSpeed()
    {

        yield return new WaitForSeconds(Random.Range(5, 8));
        rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);

    }

    private void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        StartCoroutine(ChangeRotationSpeed());
    }

    private static ChildGenerationInfo[] childrenGenInfo =
    {
        new ChildGenerationInfo(){direction=Vector3.up, rotationDegrees = Quaternion.identity },
        new ChildGenerationInfo(){direction=Vector3.right, rotationDegrees = Quaternion.Euler(0,0,-90f) },
        new ChildGenerationInfo(){direction=Vector3.left, rotationDegrees =Quaternion.Euler(0, 0, 90f) },
        new ChildGenerationInfo(){direction=Vector3.forward, rotationDegrees =Quaternion.Euler(90f, 0, 0) },
        new ChildGenerationInfo(){direction=Vector3.back, rotationDegrees =Quaternion.Euler(-90f, 0, 0) }
    };

    private IEnumerator CreateChildren()
    {
        for (int i = 0; i < childrenGenInfo.Length; i++)
        {
            if (Random.value < spawnPossiblities)
            {
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
                new GameObject("Fractal child").AddComponent<Fractal>().InitializeFromParent(this, i);
            }
        }
    }

    private void InitializeFromParent(Fractal parent, int childIndex)
    {
        maxTwist = parent.maxTwist;
        maxRotationSpeed = parent.maxRotationSpeed;
        spawnPossiblities = parent.spawnPossiblities;
        meshs = parent.meshs;
        depth = parent.depth + 1;
        maxDepth = parent.maxDepth;
        childScale = parent.childScale;
        materials = parent.materials;
        transform.parent = parent.transform;
        transform.localRotation = childrenGenInfo[childIndex].rotationDegrees;
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = childrenGenInfo[childIndex].direction * (0.5f + 0.5f * childScale);
    }
}
