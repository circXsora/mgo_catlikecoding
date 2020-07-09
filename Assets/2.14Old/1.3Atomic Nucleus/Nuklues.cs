using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Nuklues : MonoBehaviour {

    public float attrctionForce;

    public Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        body.AddForce(transform.localPosition * -attrctionForce);
    }

}
