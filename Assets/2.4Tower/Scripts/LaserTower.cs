using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : Tower.Tower
{

    [SerializeField, Range(1f, 100f)]
    float damagePerSecond = 10f;

    [SerializeField]
    Transform turret = default, laserBeam = default;

    TargetPoint target;

    Vector3 laserBeamScale;
    public override TowerType TowerType => TowerType.Laser;
    void Awake()
    {
        laserBeamScale = laserBeam.localScale;
    }

    //void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.yellow;
    //    Vector3 position = transform.localPosition;
    //    position.y += 0.5f;
    //    //Gizmos.DrawWireSphere(position, targetingRange);
    //    for (int i = 0; i < attackRange.attackablePositions.Length; i++)
    //    {
    //        Vector2 offset = attackRange.attackablePositions[i];
    //        Vector3 newPos = new Vector3(position.x + offset.x, position.y, position.z + offset.y);
    //        Gizmos.DrawCube(newPos, Vector3.one);
    //    }
    //    if (target != null)
    //    {
    //        Gizmos.DrawLine(position, target.Position);
    //    }
    //}

    public override void GameUpdate()
    {
        attackRange = AttackRange.ranges[(int)attackRangeType];
        if (TrackTarget(ref target) || AcquireTarget(out target))
        {
            Shoot();
        }
        else
        {
            laserBeam.localScale = Vector3.zero;
        }
    }
    void Shoot()
    {
        Vector3 point = target.Position;
        turret.LookAt(point);
        laserBeam.localRotation = turret.localRotation;

        float d = Vector3.Distance(turret.position, point);
        laserBeamScale.z = d;
        laserBeam.localScale = laserBeamScale;
        laserBeam.localPosition =
    turret.localPosition + 0.5f * d * laserBeam.forward;
        target.Enemy.ApplyDamage(damagePerSecond * Time.deltaTime);
    }

}
