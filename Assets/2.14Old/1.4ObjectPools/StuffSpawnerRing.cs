using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuffSpawnerRing : MonoBehaviour {

    public StuffSpawner spawnerPrefab;

    public float rudis, tiltAngle;

    public int numberOfSpawner;

    public Material[] materials;

    private void Awake()
    {
        for (int i = 0; i < numberOfSpawner; i++)
        {
            CreateSpawner(i);
        }
    }

    private void CreateSpawner(int i)
    {
        Transform rotater = new GameObject("Rotater").transform;
        rotater.SetParent(transform, false);
        rotater.localRotation = Quaternion.Euler(0f, i * 360f / numberOfSpawner, 0f);
        var spawner = Instantiate<StuffSpawner>(spawnerPrefab);
        spawner.transform.SetParent(rotater);
        spawner.transform.localPosition = new Vector3(0f, 0f, rudis);
        spawner.transform.localRotation = Quaternion.Euler(tiltAngle, 0f, 0f);
        spawner.material = materials[i % materials.Length];
    }
}
