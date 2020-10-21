using UnityEngine;

public partial class GameLevel : PersistableObject {
    [UnityEngine.Serialization.FormerlySerializedAs("persistentObjects")]
    [SerializeField]
    GameLevelObject[] levelObjects;
    void OnEnable () {
        Current = this;
        if (levelObjects == null) {
            levelObjects = new GameLevelObject[0];
        }
    }
    public static GameLevel Current { get; private set; }
    [SerializeField]
    int populationLimit = 10000;
    public int PopulationLimit
    {
        get
        {
            return populationLimit;
        }
    }
    [SerializeField]
    SpawnZone spawnZone = null;
    public void SpawnShapes () {
        spawnZone.SpawnShapes ();
    }
    public bool HasMissingLevelObjects
    {
        get
        {
            if (levelObjects != null)
            {
                for (int i = 0; i < levelObjects.Length; i++)
                {
                    if (levelObjects[i] == null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
    public void RemoveMissingLevelObjects()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Do not invoke in play mode!");
            return;
        }
        int holes = 0;
        for (int i = 0; i < levelObjects.Length - holes; i++)
        {
            if (levelObjects[i] == null)
            {
                holes += 1;
                System.Array.Copy(levelObjects, i + 1, levelObjects, i,levelObjects.Length - i - 1 - holes);
                i -= 1;
            }
        }
        System.Array.Resize(ref levelObjects, levelObjects.Length - holes);
    }
    public bool HasLevelObject(GameLevelObject o)
    {
        if (levelObjects != null)
        {
            for (int i = 0; i < levelObjects.Length; i++)
            {
                if (levelObjects[i] == o)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public void RegisterLevelObject(GameLevelObject o)
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Do not invoke in play mode!");
            return;
        }
        if (HasLevelObject(o))
        {
            return;
        }
        if (levelObjects == null)
        {
            levelObjects = new GameLevelObject[] { o };
        }
        else
        {
            System.Array.Resize(ref levelObjects, levelObjects.Length + 1);
            levelObjects[levelObjects.Length - 1] = o;
        }
    }
    public void GameUpdate()
    {
        for (int i = 0; i < levelObjects.Length; i++)
        {
            levelObjects[i].GameUpdate();
        }
    }
    public override void Save (GameDataWriter writer) {
        writer.Write (levelObjects.Length);
        for (int i = 0; i < levelObjects.Length; i++) {
            levelObjects[i].Save (writer);
        }
    }

    public override void Load (GameDataReader reader) {
        int savedCount = reader.ReadInt ();
        for (int i = 0; i < savedCount; i++) {
            levelObjects[i].Load (reader);
        }
    }
}