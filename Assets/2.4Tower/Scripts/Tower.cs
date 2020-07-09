using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tower
{

    public abstract class Tower : GameTileContent
    {
        static Collider[] targetsBuffer = new Collider[100];
        const int enemyLayerMask = 1 << 9;
        [SerializeField]
        protected AttackRange.AttackRangeType attackRangeType = AttackRange.AttackRangeType.Circle;
        protected AttackRange attackRange;
        public abstract TowerType TowerType { get; }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 position = transform.localPosition;
            position.y += 0.5f;
            //Gizmos.DrawWireSphere(position, targetingRange);
            //for (int i = 0; i < attackRange.attackablePositions.Length; i++)
            //{
            //    Vector2 offset = attackRange.attackablePositions[i];
            //    Vector3 newPos = new Vector3(position.x + offset.x, position.y, position.z + offset.y);
            //    Gizmos.DrawCube(newPos, Vector3.one);
            //}
            //if (target != null)
            //{
            //    Gizmos.DrawLine(position, target.Position);
            //}
        }
        protected bool AcquireTarget(out TargetPoint target)
        {

            int hits = 0;
            List<Collider> tBuffer = new List<Collider>();
            for (int i = 0; i < attackRange.attackablePositions.Length; i++)
            {
                Collider[] tempC = new Collider[10];
                Vector3 a = transform.localPosition;
                Vector2 offset = attackRange.attackablePositions[i];
                a = new Vector3(a.x + offset.x, a.y + 0.5f, a.z + offset.y);
                hits += Physics.OverlapBoxNonAlloc(a, Vector3.one * 0.55f, tempC, Quaternion.identity, enemyLayerMask);
                for (int j = 0; j < tempC.Length; j++)
                {
                    if (tempC[j] != null)
                    {
                        tBuffer.Add(tempC[j]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (hits > 0)
            {
                target = tBuffer[Random.Range(0, hits)].GetComponent<TargetPoint>();
                Debug.Assert(target != null, "Targeted non-enemy!", tBuffer[0]);
                return true;
            }
            target = null;
            return false;
        }
        protected bool TrackTarget(ref TargetPoint target)
        {
            if (target == null)
            {
                return false;
            }
            for (int i = 0; i < attackRange.attackablePositions.Length; i++)
            {
                Vector3 a = transform.localPosition;
                Vector2 offset = attackRange.attackablePositions[i];
                a = new Vector3(a.x + offset.x, a.y, a.z + offset.y);
                Vector3 b = target.Position;
                float x = Mathf.Abs(a.x - b.x);
                float z = Mathf.Abs(a.z - b.z);
                if (x < 0.5f + 0.125 * target.Enemy.Scale && z < 0.5f + 0.125 * target.Enemy.Scale)
                {
                    return true;
                }
            }
            target = null;
            return false;
        }

    }
}
