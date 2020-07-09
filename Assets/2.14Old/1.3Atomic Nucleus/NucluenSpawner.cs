using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NucluenSpawner : MonoBehaviour {

    public float timeBetweenSpawn;
    public float spawnDistance;
    public Nuklues[] nukluesPrefabs;

    float timeSinceLastSpawn;
    private void FixedUpdate()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= timeBetweenSpawn)
        {
            timeSinceLastSpawn -= timeBetweenSpawn;
            SpawnNucluen();
        }
    }

    private void SpawnNucluen()
    {
        var prefab = nukluesPrefabs[UnityEngine.Random.Range(0, nukluesPrefabs.Length)];
        var spawn = Instantiate<Nuklues>(prefab);
        spawn.transform.localPosition = UnityEngine.Random.onUnitSphere * spawnDistance;
    }
}
