using UnityEngine;

public class RotatingObject : GameLevelObject {

    [SerializeField]
    Vector3 angularVelocity = Vector3.zero;

    public override void GameUpdate()
    {
        transform.Rotate (angularVelocity * Time.deltaTime);
    }
}