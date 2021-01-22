using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformationGrid : MonoBehaviour
{
    public Transform prefab;

    public int gridResolution = 10;

    public Transform[,,] grid;

    private List<Transformation> transformations;
    Matrix4x4 transformation;
    private void Awake()
    {
        transformations = new List<Transformation>();
    }

    // Use this for initialization
    void Start()
    {
        grid = new Transform[gridResolution, gridResolution, gridResolution];

        for (int z = 0; z < gridResolution; z++)
        {
            for (int y = 0; y < gridResolution; y++)
            {
                for (int x = 0; x < gridResolution; x++)
                {
                    grid[x, y, z] = CreateGridPoint(x, y, z);
                }
            }
        }
    }

    private void Update()
    {
        UpdateTransformation();
        for (int z = 0; z < gridResolution; z++)
        {
            for (int y = 0; y < gridResolution; y++)
            {
                for (int x = 0; x < gridResolution; x++)
                {
                    grid[x, y, z].localPosition = TransformPoint(x, y, z);
                }
            }
        }
    }

    void UpdateTransformation()
    {
        GetComponents(transformations);
        if (transformations.Count > 0)
        {
            transformation = transformations[0].Matrix;
            for (int i = 1; i < transformations.Count; i++)
            {
                transformation = transformations[i].Matrix * transformation;
            }
        }
    }
    private Vector3 TransformPoint(int x, int y, int z)
    {
        Vector3 coordinates = GetCoordinates(x, y, z);
        return transformation.MultiplyPoint(coordinates);
    }

    private Transform CreateGridPoint(int x, int y, int z)
    {
        Transform point = Instantiate(prefab);
        point.localPosition = GetCoordinates(x, y, z);
        point.GetComponent<MeshRenderer>().material.color = new Color(
            (float) x / gridResolution,
            (float) y / gridResolution,
            (float) z / gridResolution
        );
        return point;
    }

    private Vector3 GetCoordinates(int x, int y, int z)
    {
        return new Vector3(
            (float) x - (gridResolution - 1) * 0.5f,
            (float) y - (gridResolution - 1) * 0.5f,
            (float) z - (gridResolution - 1) * 0.5f
        );
    }

}