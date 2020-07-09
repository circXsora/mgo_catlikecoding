using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Stuff : PooledObject
{
    public Rigidbody Body { get; private set; }
    public MeshRenderer[] meshRenders;


    public void SetMeshRenders(Material m)
    {
        foreach (var mr in meshRenders)
        {
            mr.material = m;
        }
    }

    private void Awake()
    {
        Body = GetComponent<Rigidbody>();
        meshRenders = GetComponentsInChildren<MeshRenderer>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Kill Zone"))
        {
            ReturnToPool();
        }
    }


}
