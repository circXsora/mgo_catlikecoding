﻿using UnityEngine;

public class LifeZone : MonoBehaviour
{
    [SerializeField]
    float dyingDuration = 0;
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        var c = GetComponent<Collider>();
        var b = c as BoxCollider;
        if (b != null)
        {
            Gizmos.matrix = Matrix4x4.TRS(
                transform.position, transform.rotation, transform.lossyScale
            );
            Gizmos.DrawWireCube(b.center, b.size);
            return;
        }
        var s = c as SphereCollider;
        if (s != null)
        {
            Vector3 scale = transform.lossyScale;
            scale = Vector3.one * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
            Gizmos.matrix = Matrix4x4.TRS(
                transform.position, transform.rotation, scale
            );
            Gizmos.DrawWireSphere(s.center, s.radius);
            return;
        }
    }
    void OnTriggerExit(Collider other)
    {
        var shape = other.gameObject.GetComponent<Shape>();
        if (shape)
        {
            Debug.Log("Collision");
            if (dyingDuration <= 0f)
            {
                Debug.Log("Die");
                shape.Die();
            }
            else if (!shape.IsMarkedAsDying)
            {
                Debug.Log("Mark Die");
                shape.AddBehavior<DyingShapeBehavior>().Initialize(
                    shape, dyingDuration
                );
            }
        }
    }
}