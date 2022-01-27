using UnityEngine;

public static class Helpers
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));

    public static Vector3 ToIsometric(this Vector3 _input) => _isoMatrix.MultiplyPoint3x4(_input);
}
