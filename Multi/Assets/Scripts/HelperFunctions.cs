using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperFunctions
{
    public static bool IsLeftOfOrOnRay2D(Vector2 point, Vector2 rayStart, Vector2 rayDir)
    {
        Vector2 rayEnd = rayStart + rayDir;

        return Mathf.Sign(HowLeftOfRayIsPoint2D(point, rayStart, rayEnd)) <= 0;
    }

    public static float HowLeftOfRayIsPoint2D(Vector2 point, Vector2 rayStart, Vector2 rayEnd)
    {
        return (rayEnd.x - rayStart.x) * (point.y - rayStart.y) - (rayEnd.y - rayStart.y) * (point.x - rayStart.x);
    }

    public static Vector3 ReflectVectorOnNormal(Vector3 vectToReflect, Vector3 norm)
    {
        return vectToReflect - 2 * (Vector3.Dot(vectToReflect, norm)) * norm;
    }
}
