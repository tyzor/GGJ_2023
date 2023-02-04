using UnityEngine;

namespace GGJ.Utilities.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector3 xz(this Vector2 vector2)
        {
            return new Vector3(vector2.x, 0f, vector2.y);
        }
    }
}