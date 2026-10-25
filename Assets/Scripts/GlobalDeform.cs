using UnityEngine;

public enum DeformMode
{
    None,
    Taper,
    Twist,
    Bend
}

public class GlobalDeform : MonoBehaviour
{
    [Header("变形强度")]
    public float strength = 0.3f;
    public DeformMode currentMode;

    private Mesh mesh;
    private Vector3[] originalVertices;

    void Awake()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        mesh = mf.mesh;
        // 保存初始原始顶点
        originalVertices = mesh.vertices.Clone() as Vector3[];
    }

    void Update()
    {
        if (currentMode == DeformMode.None)
        {
            // 恢复原始网格
            mesh.vertices = originalVertices.Clone() as Vector3[];
            mesh.RecalculateNormals();
            return;
        }

        Vector3[] verts = mesh.vertices;

        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 v = verts[i];

            switch (currentMode)
            {
                case DeformMode.Taper:
                    // 锥化 Taper
                    v.y *= 1 - strength * v.z;
                    break;
                case DeformMode.Twist:
                    // 扭转 Twist
                    float angle = strength * v.z;
                    float cosA = Mathf.Cos(angle);
                    float sinA = Mathf.Sin(angle);
                    float newX = v.x * cosA - v.y * sinA;
                    float newY = v.x * sinA + v.y * cosA;
                    v.x = newX;
                    v.y = newY;
                    break;
                case DeformMode.Bend:
                    // 弯曲 Bend
                    float bendAngle = strength * v.z;
                    v.y += Mathf.Sin(bendAngle) * strength;
                    break;
            }
            verts[i] = v;
        }

        mesh.vertices = verts;
        mesh.RecalculateNormals();
    }

    public void SetMode(DeformMode mode)
    {
        currentMode = mode;
    }

    public void SetStrength(float s)
    {
        strength = s;
    }
}
