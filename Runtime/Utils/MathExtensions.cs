using System;
using Unity.Mathematics;
using UnityEngine;

namespace Gilzoide.LottiePlayer
{
    public static class MathExtensions
    {
        public static float GetAspect(this Vector2 vector)
        {
            return vector.x / vector.y;
        }

        public static float GetAspect(this Vector2Int vector)
        {
            return (float) vector.x / vector.y;
        }

        public static Vector2 WithX(this Vector2 vector, float x)
        {
            return new Vector2(x, vector.y);
        }

        public static Vector2 WithY(this Vector2 vector, float y)
        {
            return new Vector2(vector.x, y);
        }

        public static Vector3 WithZ(this Vector2 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        public static Vector2 AspectFit(this Vector2 vector, float aspect)
        {
            if (aspect <= 0)
            {
                throw new ArgumentException("Aspect must be positive", nameof(aspect));
            }

            float currentAspect = vector.GetAspect();
            if (currentAspect > aspect)
            {
                return vector.WithX(vector.y * aspect);
            }
            else if (currentAspect < aspect)
            {
                return vector.WithY(vector.x / aspect);
            }
            else
            {
                return vector;
            }
        }

        public static Rect AspectFit(this Rect rect, float aspect)
        {
            if (aspect <= 0)
            {
                throw new ArgumentException("Aspect must be positive", nameof(aspect));
            }

            Vector2 currentSize = rect.size;
            Vector2 newSize = currentSize.AspectFit(aspect);
            rect.min += (currentSize - newSize) * 0.5f;
            rect.size = newSize;
            return rect;
        }

        public static float3 ToFloat3(this Vector2 v)
        {
            return new float3(v.x, v.y, 0);
        }
    }
}
