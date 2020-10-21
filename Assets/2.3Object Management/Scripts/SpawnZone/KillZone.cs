using UnityEngine;

public class KillZone : MonoBehaviour
{
    [SerializeField]
    private float dyingDuration = 0;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Collider c = GetComponent<Collider>();
        BoxCollider b = c as BoxCollider;
        if (b != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(b.center, b.size);
            return;
        }
        SphereCollider s = c as SphereCollider;
        if (s != null)
        {
            Vector3 scale = transform.lossyScale;
            scale = Vector3.one * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(s.center, s.radius);
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Shape shape = other.gameObject.GetComponent<Shape>();
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