using UnityEngine;

public static class Helpers
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));

    public static Vector3 ToIsometric(this Vector3 _input) => _isoMatrix.MultiplyPoint3x4(_input);

    public static Vector3 NearestWorldAxis(Vector3 v)
    {
        if (Mathf.Abs(v.x) < Mathf.Abs(v.y))
        {
            v.x = 0;
            if (Mathf.Abs(v.y) < Mathf.Abs(v.z))
                v.y = 0;
            else
                v.z = 0;
        }
        else
        {
            v.y = 0;
            if (Mathf.Abs(v.x) < Mathf.Abs(v.z))
                v.x = 0;
            else
                v.z = 0;
        }
        return v;
    }
}
