using UnityEngine;
using System.Collections;

public class AttackRange
{
    static public AttackRange circle = new AttackRange()
    {
        attackablePositions = new Vector2[9]
        {
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,0),
            new Vector2(1,1),
            new Vector2(0,-1),
            new Vector2(-1,0),
            new Vector2(-1,-1),
            new Vector2(1,-1),
            new Vector2(-1,1),
        }
    };
    static public AttackRange shizi = new AttackRange()
    {
        attackablePositions = new Vector2[]
        {
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,0),
            new Vector2(-1,0),
            new Vector2(0,-1),
        }
    };
static public AttackRange[] ranges = new AttackRange[] {
        circle,
        shizi
    };
    public enum AttackRangeType
    {
        Circle,
        Shizi
    }
    public Vector2[] attackablePositions;
}
