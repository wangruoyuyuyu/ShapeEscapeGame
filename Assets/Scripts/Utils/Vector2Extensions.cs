using UnityEngine;

public static class Vector2Extensions
{
    // 将角度（以度为单位）转换为方向向量
    public static Vector2 FromAngle(float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    }
}