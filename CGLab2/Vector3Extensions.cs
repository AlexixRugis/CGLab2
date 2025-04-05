using OpenTK.Mathematics;

public static class Vector3Extensions
{
    public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
    {
        Vector3 delta = target - current;
        float distance = delta.Length;

        if (distance <= maxDistanceDelta || distance == 0f)
            return target;

        return current + (delta / distance) * maxDistanceDelta;
    }
}