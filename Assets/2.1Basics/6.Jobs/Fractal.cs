﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace New
{
    public class Fractal : MonoBehaviour
    {
        [SerializeField, Range(1, 8)]
        private int depth = 4;

        [SerializeField]
        Mesh mesh = default;

        [SerializeField]
        Material material = default;

        struct FractalPart
        {
            public Vector3 direction;
            public Quaternion rotation;
            public Transform transform;
        }

        FractalPart[][] parts;

        static Vector3[] directions = {
            Vector3.up, Vector3.right, Vector3.left, Vector3.forward, Vector3.back
        };

        static Quaternion[] rotations = {
            Quaternion.identity,
            Quaternion.Euler(0f, 0f, -90f), Quaternion.Euler(0f, 0f, 90f),
            Quaternion.Euler(90f, 0f, 0f), Quaternion.Euler(-90f, 0f, 0f)
        };
        private void Awake()
        {
            parts = new FractalPart[depth][];
            for (int i = 0, length = 1; i < parts.Length; i++, length *= 5)
            {
                parts[i] = new FractalPart[length];
            }
            float scale = 1f;
            CreatePart(0, 0, scale);
            for (int li = 1; li < parts.Length; li++) {
                scale *= 0.5f;
                FractalPart[] levelParts = parts[li];
                for (int fpi = 0; fpi < levelParts.Length; fpi += 5)
                {
                    for (int ci = 0; ci < 5; ci++)
                    {
                        CreatePart(li, ci, scale);
                    }
                }
            }
        }
        FractalPart CreatePart(int levelIndex, int childIndex, float scale)
        {
            var go = new GameObject("Fractal Part " + levelIndex + " C" + childIndex);
            go.transform.SetParent(transform, false);
            go.transform.localScale = scale * Vector3.one;
            go.AddComponent<MeshFilter>().mesh = mesh;
            go.AddComponent<MeshRenderer>().material = material;

            return new FractalPart();
        }
    }
}