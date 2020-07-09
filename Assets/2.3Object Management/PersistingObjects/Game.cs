﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : PersistableObject {
    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;
    public KeyCode destroyKey = KeyCode.X;
    [SerializeField] ShapeFactory[] shapeFactories = null;
    private List<Shape> shapes = null;
    private List<ShapeInstance> killList = null, markAsDyingList =null;
    public PersistentStorage storage = null;
    private const int saveVersion = 7;
    private float creationProgress, destructionProgress;
    public int levelCount;
    int loadedLevelBuildIndex;
    Random.State mainRandomState;
    [SerializeField] bool reseedOnLoad = true;
    int dyingShapeCount;
    [SerializeField] float destroyDuration = 0;
    public SpawnZone SpawnZoneOfLevel {
        get;
        set;
    }
    bool inGameUpdateLoop;
    [SerializeField] Slider creationSpeedSlider = null;
    [SerializeField] Slider destructionSpeedSlider = null;
    public static Game Instance
    {
        get; private set;
    }
    private void Start () {
        shapes = new List<Shape> ();
        killList = new List<ShapeInstance>();
        markAsDyingList = new List<ShapeInstance>();
        BeginNewGame ();
        if (Application.isEditor) {
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                Scene loadedScene = SceneManager.GetSceneAt (i);
                if (loadedScene.name.Contains ("Level ")) {
                    SceneManager.SetActiveScene (loadedScene);
                    loadedLevelBuildIndex = loadedScene.buildIndex;
                    return;
                }
            }
        }
        //StartCoroutine(LoadLevel(1));
    }
    void OnEnable () {
        Instance = this;
        if (shapeFactories[0].FactoryId != 0) {
            for (int i = 0; i < shapeFactories.Length; i++) {
                shapeFactories[i].FactoryId = i;
            }
        }
    }
    public float CreationSpeed {
        get;
        set;
    }
    public float DestructionSpeed {
        get;
        set;
    }
    public Shape GetShape(int index) {
        return shapes[index];
    }
    public void AddShape(Shape shape) {
        shape.SaveIndex = shapes.Count;
        shapes.Add(shape);
    }
    public ColorRangeHSV color;

    private void DestroyShape () {
        if (shapes.Count - dyingShapeCount > 0) {
            Shape shape = shapes[Random.Range(dyingShapeCount, shapes.Count)];
            if (destroyDuration <= 0f)
            {
                KillImmediately(shape);
            }
            else
            {
                shape.AddBehavior<DyingShapeBehavior>().Initialize(
                    shape, destroyDuration
                );
            }
        }
    }

    private IEnumerator LoadLevel (int levelBuildIndex) {
        enabled = false;
        if (loadedLevelBuildIndex > 0) {
            yield return SceneManager.UnloadSceneAsync (loadedLevelBuildIndex);
        }
        yield return SceneManager.LoadSceneAsync (levelBuildIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene (SceneManager.GetSceneByBuildIndex (levelBuildIndex));
        loadedLevelBuildIndex = levelBuildIndex;
        enabled = true;
    }

    private void BeginNewGame () {
        Random.state = mainRandomState;
        int seed = Random.Range (0, int.MaxValue) ^ (int) Time.unscaledTime;
        mainRandomState = Random.state;
        Random.InitState (seed);
        creationSpeedSlider.value = CreationSpeed = 0;
        destructionSpeedSlider.value = DestructionSpeed = 0;
        for (int i = 0; i < shapes.Count; i++) {
            shapes[i].Recycle ();
        }
        shapes.Clear ();
        dyingShapeCount = 0;
    }
    public void Kill(Shape shape)
    {
        if (inGameUpdateLoop)
        {
            killList.Add(shape);
        }
        else
        {
            KillImmediately(shape);
        }
    }

    void KillImmediately(Shape shape)
    {
        int index = shape.SaveIndex;
        shape.Recycle();
        if (index < dyingShapeCount && index < --dyingShapeCount)
        {
            shapes[dyingShapeCount].SaveIndex = index;
            shapes[index] = shapes[dyingShapeCount];
            index = dyingShapeCount;
        }
        int lastIndex = shapes.Count - 1;
        if (index < lastIndex)
        {
            shapes[lastIndex].SaveIndex = index;
            shapes[index] = shapes[lastIndex];
        }
        shapes.RemoveAt(lastIndex);
    }
    void MarkAsDyingImmediately(Shape shape)
    {
        int index = shape.SaveIndex;
        if (index < dyingShapeCount)
        {
            return;
        }
        shapes[dyingShapeCount].SaveIndex = index;
        shapes[index] = shapes[dyingShapeCount];
        shape.SaveIndex = dyingShapeCount;
        shapes[dyingShapeCount++] = shape;
    }
    public void MarkAsDying(Shape shape)
    {
        if (inGameUpdateLoop)
        {
            markAsDyingList.Add(shape);
        }
        else
        {
            MarkAsDyingImmediately(shape);
        }
    }
    public bool IsMarkedAsDying(Shape shape)
    {
        return shape.SaveIndex < dyingShapeCount;
    }
    public override void Save (GameDataWriter writer) {
        writer.Write (shapes.Count);
        writer.Write (Random.state);
        writer.Write (creationProgress);
        writer.Write (CreationSpeed);
        writer.Write (destructionProgress);
        writer.Write (DestructionSpeed);
        GameLevel.Current.Save (writer);
        writer.Write (loadedLevelBuildIndex);
        for (int i = 0; i < shapes.Count; i++) {
            writer.Write (shapes[i].OriginFactory.FactoryId);
            writer.Write (shapes[i].ShapeId);
            writer.Write (shapes[i].MaterialId);
            shapes[i].Save (writer);
        }
    }

    public override void Load (GameDataReader reader) {
        int version = reader.Version;
        if (version > saveVersion) {
            Debug.LogError ("Unsupported future save version " + version);
            return;
        }
        StartCoroutine (LoadGame (reader));
    }

    IEnumerator LoadGame (GameDataReader reader) {
        int version = reader.Version;
        int count = reader.ReadInt ();
        if (version >= 3) {
            Random.State state = reader.ReadRandomState ();
            if (!reseedOnLoad) {
                Random.state = state;
            }
            creationProgress = reader.ReadFloat ();
            creationSpeedSlider.value = CreationSpeed = reader.ReadFloat ();

            destructionProgress = reader.ReadFloat ();
            destructionSpeedSlider.value = DestructionSpeed = reader.ReadFloat ();
        }
        yield return LoadLevel (version < 2 ? 1 : reader.ReadInt ());
        for (int i = 0; i < count; i++) {
            int factoryId = version >= 5 ? reader.ReadInt () : 0;
            int shapeId = version > 0 ? reader.ReadInt () : 0;
            int materialId = version > 0 ? reader.ReadInt () : 0;
            Shape s = shapeFactories[factoryId].Get (shapeId, materialId);
            s.Load (reader);
        }
        for (int i = 0; i < shapes.Count; i++) {
            shapes[i].ResolveShapeInstances();
        }
    }
    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate () {
        inGameUpdateLoop = true;
        for (int i = 0; i < shapes.Count; i++) {
            shapes[i].GameUpdate ();
        }
        GameLevel.Current.GameUpdate();
        inGameUpdateLoop = false;
        creationProgress += Time.deltaTime * CreationSpeed;
        while (creationProgress >= 1f) {
            creationProgress -= 1f;
            GameLevel.Current.SpawnShapes();
        }
        destructionProgress += Time.deltaTime * DestructionSpeed;
        while (destructionProgress >= 1f) {
            destructionProgress -= 1f;
            DestroyShape ();
        }
        int limit = GameLevel.Current.PopulationLimit;
        if (limit > 0) {
            while (shapes.Count - dyingShapeCount > limit) {
                DestroyShape();
            }
        }
        if (killList.Count > 0)
        {
            for (int i = 0; i < killList.Count; i++)
            {
                if (killList[i].IsValid)
                {
                    KillImmediately(killList[i].Shape);
                }
            }
            killList.Clear();
        }
        if (markAsDyingList.Count > 0)
        {
            for (int i = 0; i < markAsDyingList.Count; i++)
            {
                if (markAsDyingList[i].IsValid)
                {
                    MarkAsDyingImmediately(markAsDyingList[i].Shape);
                }
            }
            markAsDyingList.Clear();
        }
    }
    private void Update () {

        if (Input.GetKeyDown (createKey)) {
            GameLevel.Current.SpawnShapes();
        } else
        if (Input.GetKeyDown (destroyKey)) {
            DestroyShape ();
        } else
        if (Input.GetKeyDown (newGameKey)) {
            BeginNewGame ();
            StartCoroutine (LoadLevel (loadedLevelBuildIndex));
        } else
        if (Input.GetKeyDown (saveKey)) {
            storage.Save (this, saveVersion);
        } else
        if (Input.GetKeyDown (loadKey)) {
            BeginNewGame ();
            storage.Load (this);
        } else {
            for (int i = 1; i <= levelCount; i++) {
                if (Input.GetKeyDown (KeyCode.Alpha0 + i)) {
                    BeginNewGame ();
                    StartCoroutine (LoadLevel (i));
                    return;
                }
            }
        }
    }

}