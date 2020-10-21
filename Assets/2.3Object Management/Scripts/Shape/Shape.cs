﻿using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct ShapeInstance
{
    public ShapeInstance(Shape shape) {
        Shape = shape;
        instanceIdOrSaveIndex = shape.InstanceId;
    }
    public ShapeInstance(int saveIndex) {
        Shape = null;
        instanceIdOrSaveIndex = saveIndex;
    }
    public void Resolve() {
        if (instanceIdOrSaveIndex >= 0) {
            Shape = Game.Instance.GetShape(instanceIdOrSaveIndex);
            instanceIdOrSaveIndex = Shape.InstanceId;
        }
    }
    public bool IsValid
    {
        get
        {
            return Shape && instanceIdOrSaveIndex == Shape.InstanceId;
        }
    }
    public static implicit operator ShapeInstance(Shape shape) {
        return new ShapeInstance(shape);
    }
    public Shape Shape
    {
        get; private set;
    }

    int instanceIdOrSaveIndex;
}
public class Shape : PersistableObject
{

    static MaterialPropertyBlock sharedPropertyBlock;
    static int colorPropertyId = Shader.PropertyToID("_Color");
    [SerializeField]
    MeshRenderer[] meshRenderers;
    List<ShapeBehavior> behaviorList = new List<ShapeBehavior>();
    public float Age { get; private set; }
    public int InstanceId
    {
        get; private set;
    }
    public int SaveIndex
    {
        get; set;
    }
    private void Awake()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        colors = new Color[meshRenderers.Length];
    }
    public void GameUpdate()
    {
        Age += Time.deltaTime;

        for (int i = 0; i < behaviorList.Count; i++)
        {
            if (!behaviorList[i].GameUpdate(this)) {
                behaviorList[i].Recycle();
                behaviorList.RemoveAt(i--);
            }
        }
    }
    public int ShapeId
    {
        get
        {
            return shapeId;
        }
        set
        {
            if (shapeId == int.MinValue && value != int.MinValue)
            {
                shapeId = value;
            }
            else
            {
                Debug.LogError("Not allowed to change shapeId.");
            }
        }
    }

    int shapeId = int.MinValue;

    public void SetMaterial(Material material, int materialId)
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material = material;
        }
        MaterialId = materialId;
    }
    public int MaterialId
    {
        get;
        private set;
    }

    Color[] colors;

    public void SetColor(Color color)
    {
        if (sharedPropertyBlock == null)
        {
            sharedPropertyBlock = new MaterialPropertyBlock();
        }
        sharedPropertyBlock.SetColor(colorPropertyId, color);
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            colors[i] = color;
            meshRenderers[i].SetPropertyBlock(sharedPropertyBlock);
        }
    }

    public void SetColor(Color color, int index)
    {
        if (sharedPropertyBlock == null)
        {
            sharedPropertyBlock = new MaterialPropertyBlock();
        }
        sharedPropertyBlock.SetColor(colorPropertyId, color);
        colors[index] = color;
        meshRenderers[index].SetPropertyBlock(sharedPropertyBlock);
    }

    public int ColorCount
    {
        get
        {
            return colors.Length;
        }
    }
    public ShapeFactory OriginFactory
    {
        get
        {
            return originFactory;
        }
        set
        {
            if (originFactory == null)
            {
                originFactory = value;
            }
            else
            {
                Debug.LogError("Not allowed to change origin factory.");
            }
        }
    }

    ShapeFactory originFactory;
    public void Recycle()
    {
        Age = 0f;
        InstanceId += 1;
        for (int i = 0; i < behaviorList.Count; i++)
        {
            behaviorList[i].Recycle();
        }
        behaviorList.Clear();
        OriginFactory.Reclaim(this);
    }
    public T AddBehavior<T>() where T : ShapeBehavior, new()
    {
        T behavior = ShapeBehaviorPool<T>.Get();
        behaviorList.Add(behavior);
        return behavior;
    }
    public override void Save(GameDataWriter writer)
    {

        base.Save(writer);
        writer.Write(colors.Length);
        for (int i = 0; i < colors.Length; i++)
        {
            writer.Write(colors[i]);
        }
        writer.Write(Age);
        writer.Write(behaviorList.Count);
        for (int i = 0; i < behaviorList.Count; i++)
        {
            writer.Write((int)behaviorList[i].BehaviorType);
            behaviorList[i].Save(writer);
        }
    }
    public bool IsMarkedAsDying
    {
        get
        {
            return Game.Instance.IsMarkedAsDying(this);
        }
    }
    public void ResolveShapeInstances() {
        for (int i = 0; i < behaviorList.Count; i++) {
            behaviorList[i].ResolveShapeInstances();
        }
    }
    public void Die()
    {
        Game.Instance.Kill(this);
    }
    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        if (reader.Version >= 5)
        {
            LoadColors(reader);
        }
        else
        {
            SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
        }
        if (reader.Version >= 6)
        {
            Age = reader.ReadFloat();
            int behaviorCount = reader.ReadInt();
            for (int i = 0; i < behaviorCount; i++)
            {
                ShapeBehavior behavior =
                           ((ShapeBehaviorType)reader.ReadInt()).GetInstance();
                behaviorList.Add(behavior);
                behavior.Load(reader);
            }
        }
        else if (reader.Version >= 4)
        {
            AddBehavior<RotationShapeBehavior>().AngularVelocity =
                reader.ReadVector3();
            AddBehavior<MovementShapeBehavior>().Velocity = reader.ReadVector3();
        }
    }
    public void MarkAsDying()
    {
        Game.Instance.MarkAsDying(this);
    }
    void LoadColors(GameDataReader reader)
    {
        int count = reader.ReadInt();
        int max = count <= colors.Length ? count : colors.Length;
        int i = 0;
        for (; i < max; i++)
        {
            SetColor(reader.ReadColor(), i);
        }
        if (count > max)
        {
            for (; i < count; i++)
            {
                reader.ReadColor();
            }
        }
        else if (count < max)
        {
            for (; i < max; i++)
            {
                SetColor(Color.white, i);
            }
        }
    }

}