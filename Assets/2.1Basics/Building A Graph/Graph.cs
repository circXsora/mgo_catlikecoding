using UnityEngine;

public class Graph : MonoBehaviour
{
    public Transform pointPrefab;
    [Range(10, 100)]
    public int resolution = 10;
    private Transform[] points;
    public enum GraphFunctionName
    {
        Sine, Sine2D,
        MultiSine, MultiSine2D,
        Ripple,
        Cylinder,
        Sphere,
        Sphere2,
        Torus
    }
    public GraphFunctionName function;
    private static GraphFunction[] functions = {
        SineFunction, Sine2DFunction, MultiSineFunction, MultiSine2DFunction, Ripple, Cylinder, Sphere, Sphere2, Torus
    };
    private void Awake() {
        float step = 2f / resolution;
        Vector3 scale = Vector3.one * step;
        points = new Transform[resolution * resolution];
        for (int i = 0; i < points.Length; i++) {
            Transform point = Instantiate(pointPrefab);
            point.localScale = scale;
            point.SetParent(transform, false);
            points[i] = point;
        }
    }

    private const float pi = Mathf.PI;
    private static Vector3 SineFunction(float u, float v, float t) {
        Vector3 p;
        p.x = u;
        p.y = Mathf.Sin(pi * (u + t));
        p.z = v;
        return p;
    }

    private static Vector3 MultiSineFunction(float u, float v, float t) {
        Vector3 p;
        p.x = u;
        p.y = Mathf.Sin(pi * (u + t));
        p.y += Mathf.Sin(pi * (v + t));
        p.y *= 0.5f;
        p.z = v;
        return p;
    }

    private static Vector3 Sine2DFunction(float u, float v, float t) {
        Vector3 p;
        p.x = u;
        p.y = Mathf.Sin(pi * (u + t));
        p.y += Mathf.Sin(2f * pi * (u + 2f * t)) / 2f;
        p.y *= 2f / 3f;
        p.z = v;
        return p;
    }

    private static Vector3 MultiSine2DFunction(float u, float v, float t) {
        Vector3 p;
        p.y = 4f * Mathf.Sin(pi * (u + v + t * 0.5f));
        p.y += Mathf.Sin(pi * (u + t));
        p.y += Mathf.Sin(2f * pi * (v + 2f * t)) * 0.5f;
        p.y *= 1f / 5.5f;
        p.x = u;
        p.z = v;
        return p;
    }

    private static Vector3 Ripple(float u, float v, float t) {
        Vector3 p;
        float d = Mathf.Sqrt(u * u + v * v);
        p.y = Mathf.Sin(pi * (4f * d - t));
        p.y /= 1f + 10f * d;
        p.x = u;
        p.z = v;
        return p;
    }

    private static Vector3 Cylinder(float u, float v, float t) {
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
    static Vector3 Sphere(float u, float v, float t) {
        Vector3 p;
        float r = Mathf.Cos(pi * 0.5f * v);
        p.x = r * Mathf.Sin(pi * u);
        p.y = Mathf.Sin(pi * 0.5f * v);
        p.z = r * Mathf.Cos(pi * u);
        return p;
    }
    static Vector3 Sphere2(float u, float v, float t) {
        Vector3 p;
        float r = 0.8f + Mathf.Sin(pi * (6f * u + t)) * 0.1f;
        r += Mathf.Sin(pi * (4f * v + t)) * 0.1f;
        float s = r * Mathf.Cos(pi * 0.5f * v);
        p.x = s * Mathf.Sin(pi * u);
        p.y = r * Mathf.Sin(pi * 0.5f * v);
        p.z = s * Mathf.Cos(pi * u);
        return p;
    }
    static Vector3 Torus(float u, float v, float t) {
        Vector3 p;
        float r1 = 0.65f + Mathf.Sin(pi * (6f * u + t)) * 0.1f;
        float r2 = 0.2f + Mathf.Sin(pi * (4f * v + t)) * 0.05f;
        float s = r2 * Mathf.Cos(pi * v) + 0.5f + r1;
        p.x = s * Mathf.Sin(pi * u);
        p.y = r2 * Mathf.Sin(pi * v);
        p.z = s * Mathf.Cos(pi * u);
        return p;
    }

    private void Update() {
        float t = Time.time;
        GraphFunction f = functions[(int)function];
        float step = 2f / resolution;
        for (int i = 0, z = 0; z < resolution; z++) {
            float v = (z + 0.5f) * step - 1f;
            for (int x = 0; x < resolution; x++, i++) {
                float u = (x + 0.5f) * step - 1f;
                points[i].localPosition = f(u, v, t);
            }
        }
    }
}