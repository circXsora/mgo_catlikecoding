using UnityEngine;
using static UnityEngine.Mathf;
public class GPUGraph : MonoBehaviour
{
    [Range(10, 2000)]
    public int resolution = 10;

    [SerializeField]
    ComputeShader computeShader = default;
    [SerializeField]
    Material material = default;
    [SerializeField]
    Mesh mesh = default;

    static readonly int positionsId = Shader.PropertyToID("_Positions"),
        resolutionId = Shader.PropertyToID("_Resolution"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time");

    void UpdateFunctionOnGPU()
    {
        float step = 2f / resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        computeShader.SetBuffer(0, positionsId, positionsBuffer);
        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(0, groups, groups, 1); 

        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
        material.SetBuffer(positionsId, positionsBuffer);
        material.SetFloat(stepId, step);
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, positionsBuffer.count);
    }

    public enum FunctionName
    {
        Sine, Sine2D,
        MultiSine, MultiSine2D,
        Ripple,
        Cylinder,
        Sphere,
        Sphere2,
        Torus
    }
    public FunctionName function;
    public bool isRandom = true;
    private static GraphFunction[] functions = {
        SineFunction, Sine2DFunction, MultiSineFunction, MultiSine2DFunction, Ripple, Cylinder, Sphere, Sphere2, Torus
    };

    ComputeBuffer positionsBuffer;
    void OnEnable()
    {
        positionsBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);
    }
    void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
    }

    #region Functions

    private const float pi = Mathf.PI;
    private static Vector3 SineFunction(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Mathf.Sin(pi * (u + t));
        p.z = v;
        return p;
    }

    private static Vector3 MultiSineFunction(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Mathf.Sin(pi * (u + t));
        p.y += Mathf.Sin(pi * (v + t));
        p.y *= 0.5f;
        p.z = v;
        return p;
    }

    private static Vector3 Sine2DFunction(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Mathf.Sin(pi * (u + t));
        p.y += Mathf.Sin(2f * pi * (u + 2f * t)) / 2f;
        p.y *= 2f / 3f;
        p.z = v;
        return p;
    }

    private static Vector3 MultiSine2DFunction(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + 0.5f * t));
        p.y += 0.5f * Sin(2f * PI * (v + t));
        p.y += Sin(PI * (u + v + 0.25f * t));
        p.y *= 1f / 2.5f;
        p.z = v;
        return p;
    }

    private static Vector3 Ripple(float u, float v, float t)
    {
        Vector3 p;
        float d = Mathf.Sqrt(u * u + v * v);
        p.y = Mathf.Sin(pi * (4f * d - t));
        p.y /= 1f + 10f * d;
        p.x = u;
        p.z = v;
        return p;
    }

    private static Vector3 Cylinder(float u, float v, float t)
    {
        //float r = 1f;
        //float r = 1f + Mathf.Sin(6f * pi * u) * 0.2f;
        //float r = 1f + Mathf.Sin(2f * pi * v) * 0.2f;
        float r = 0.8f + Mathf.Sin(pi * (6f * u + 2f * v + t)) * 0.2f;
        Vector3 p;
        p.x = r * Mathf.Sin(pi * u);
        p.y = v;
        p.z = r * Mathf.Cos(pi * u);
        return p;
    }
    static Vector3 Sphere(float u, float v, float t)
    {
        Vector3 p;
        float r = Mathf.Cos(pi * 0.5f * v + t);
        p.x = r * Mathf.Sin(pi * u);
        p.y = Mathf.Sin(pi * 0.5f * v + t);
        p.z = r * Mathf.Cos(pi * u);
        return p;
    }
    static Vector3 Sphere2(float u, float v, float t)
    {
        Vector3 p;
        float r = 0.8f + Mathf.Sin(pi * (6f * u + t)) * 0.1f;
        r += Mathf.Sin(pi * (4f * v + t)) * 0.1f;
        float s = r * Mathf.Cos(pi * 0.5f * v);
        p.x = s * Mathf.Sin(pi * u);
        p.y = r * Mathf.Sin(pi * 0.5f * v);
        p.z = s * Mathf.Cos(pi * u);
        return p;
    }
    static Vector3 Torus(float u, float v, float t)
    {
        Vector3 p;
        float r1 = 0.65f + Mathf.Sin(pi * (6f * u + t)) * 0.1f;
        float r2 = 0.2f + Mathf.Sin(pi * (4f * v + t)) * 0.05f;
        float s = r2 * Mathf.Cos(pi * v) + 0.5f + r1;
        p.x = s * Mathf.Sin(pi * u);
        p.y = r2 * Mathf.Sin(pi * v);
        p.z = s * Mathf.Cos(pi * u);
        return p;
    }
    public FunctionName GetNextFunctionName(FunctionName name)
    {
        if (isRandom)
        {
            return (FunctionName)UnityEngine.Random.Range(0, functions.Length);
        }
        else
        {
            return (int)name < functions.Length - 1 ? name + 1 : 0;
        }
    }
    #endregion

    public static Vector3 Morph(float u, float v, float t,
        GraphFunction from, GraphFunction to, float progress)
    {
        return Vector3.LerpUnclamped(from(u, v, t), to(u, v, t), Mathf.SmoothStep(0f, 1f, progress));
    }

    float duration, transitionDuration = 1f;

    bool transitioning;

    FunctionName transitionFunction;

    [SerializeField, Min(0f)]
    float functionDuration = 1f;

    private void Update()
    {
        duration += Time.deltaTime;
        if (transitioning)
        {
            if (duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (duration >= functionDuration)
        {
            duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            function = GetNextFunctionName(function);
        }

        UpdateFunctionOnGPU();
    }
}