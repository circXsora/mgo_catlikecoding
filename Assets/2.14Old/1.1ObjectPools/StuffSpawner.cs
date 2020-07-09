using UnityEngine;

public class StuffSpawner : MonoBehaviour
{

    

    public Stuff[] stuffPrefabs;

    float timeSinceLastSpawn;

    public float velocity;

    public FloatRange timeBetweenSpawns, scale, randomVelocity;

    float currentSpawnDelay;

    public Material material;

    void FixedUpdate()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= currentSpawnDelay)
        {
            timeSinceLastSpawn -= currentSpawnDelay;
            currentSpawnDelay = timeBetweenSpawns.RandomValueInRange;
            SpawnStuff();
        }
    }

    void SpawnStuff()
    {
        Stuff prefab = stuffPrefabs[Random.Range(0, stuffPrefabs.Length)];
        Stuff spawn = prefab.GetPooledInstance<Stuff>();
        spawn.transform.localPosition = transform.position;
        spawn.transform.localScale = Vector3.one * scale.RandomValueInRange;
        spawn.transform.localRotation = Random.rotation;
        spawn.Body.velocity = randomVelocity.RandomValueInRange * velocity * transform.up;
        
        spawn.SetMeshRenders(material);
    }
}