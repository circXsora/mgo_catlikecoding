using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformationGrid : MonoBehaviour
{

    public Transform prefab;

    public int gridResolution = 10;

    public Transform[] grid;

    // Use this for initialization
    void Start()
    {
        grid = new Transform[gridResolution * gridResolution * gridResolution];

        for (int i = 0, z = 0; z < gridResolution; z++)
        {
            for (int y = 0; y < gridResolution; y++)
            {
                for (int x = 0; x < gridResolution; x++, i++)
                {
                    grid[i] = CreateGridPoint(x, y, z);
                }
            }
        }

    }

    private Transform CreateGridPoint(int x, int y, int z)
    {
        Transform point = Instantiate<Transform>(prefab);
        point.localPosition = GetCoordinates(x, y, z);
        point.GetComponent<MeshRenderer>().material.color = new Color(
            (float)x / gridResolution,
            (float)y / gridResolution,
            (float)z / gridResolution
        );
        return point;
    }

    private Vector3 GetCoordinates(int x, int y, int z)
    {
        return new Vector3(
            (float)x - (gridResolution - 1) * 0.5f,
            (float)y - (gridResolution - 1) * 0.5f,
            (float)z - (gridResolution - 1) * 0.5f
            );
    }
}
