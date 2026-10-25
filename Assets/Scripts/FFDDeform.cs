using UnityEngine;
using System.Collections.Generic;

public class FFDDeform : MonoBehaviour
{
    [Header("控制点预制体（小球）")]
    public GameObject controlPointPrefab;

    [Header("控制点大小")]
    public float controlPointScale = 0.4f;

    [Header("晶格向外扩展距离（越小越贴模型）")]
    public float latticeExpand = 0.08f;

    private List<Transform> ctrlPoints = new List<Transform>();
    private bool ffdActive = false;

    private Mesh targetMesh;
    private Vector3[] originalVerts;
    private Vector3[] deformedVerts;

    private Transform dragPoint;
    private Plane dragPlane;
    private bool isDragging;

    private Bounds latticeBounds;

    void Awake()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null)
        {
            Debug.LogError("FFDDeform：物体上没有 MeshFilter！");
            return;
        }

        targetMesh = mf.mesh;
        targetMesh.MarkDynamic();
        originalVerts = targetMesh.vertices.Clone() as Vector3[];
        deformedVerts = targetMesh.vertices.Clone() as Vector3[];
    }

    /// <summary>
    /// 开启FFD，生成控制点小球
    /// </summary>
    public void ActivateFFD()
    {
        InitFFD();
    }

    /// <summary>
    /// 关闭FFD，销毁控制点，模型复原
    /// </summary>
    public void DeactivateFFD()
    {
        DisableFFD();
    }

    void InitFFD()
    {
        DisableFFD();

        if (controlPointPrefab == null)
        {
            Debug.LogError("请给 Control Point Prefab 赋值小球预制体！");
            return;
        }

        ffdActive = true;

        // 获取模型包围盒，轻微向外扩张
        Bounds meshBounds = targetMesh.bounds;
        latticeBounds = new Bounds(meshBounds.center, meshBounds.size);
        latticeBounds.Expand(latticeExpand);

        Vector3 min = latticeBounds.min;
        Vector3 max = latticeBounds.max;

        // 3*3*3控制点晶格
        for (int x = 0; x <= 2; x++)
        {
            for (int y = 0; y <= 2; y++)
            {
                for (int z = 0; z <= 2; z++)
                {
                    Vector3 pos = new Vector3(
                        Mathf.Lerp(min.x, max.x, x / 2f),
                        Mathf.Lerp(min.y, max.y, y / 2f),
                        Mathf.Lerp(min.z, max.z, z / 2f)
                    );

                    GameObject pt = Instantiate(controlPointPrefab, pos, Quaternion.identity);
                    pt.transform.SetParent(transform);
                    pt.transform.localScale = Vector3.one * controlPointScale;

                    // 给小球添加碰撞体用于拾取拖拽
                    SphereCollider sc = pt.GetComponent<SphereCollider>();
                    if (sc != null)
                    {
                        sc.radius = 1f;
                    }
                    ctrlPoints.Add(pt.transform);
                }
            }
        }
        Debug.Log("FFD控制点生成成功");
    }

    void DisableFFD()
    {
        foreach (var t in ctrlPoints)
        {
            if (t != null) Destroy(t.gameObject);
        }
        ctrlPoints.Clear();
        ffdActive = false;
        dragPoint = null;
        isDragging = false;

        // 模型恢复原始形状
        if (targetMesh != null && originalVerts != null)
        {
            targetMesh.vertices = originalVerts.Clone() as Vector3[];
            targetMesh.RecalculateNormals();
        }
    }

    void Update()
    {
        if (!ffdActive || ctrlPoints.Count == 0 || targetMesh == null) return;

        HandleDrag();
        UpdateDeformation();
    }

    // 鼠标拖拽控制点逻辑
    void HandleDrag()
    {
        if (Camera.main == null)
        {
            Debug.LogWarning("场景中没有MainCamera！");
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 按下鼠标拾取小球
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (ctrlPoints.Contains(hit.transform))
                {
                    dragPoint = hit.transform;
                    dragPlane = new Plane(-Camera.main.transform.forward, dragPoint.position);
                    isDragging = true;
                }
            }
        }

        // 按住拖拽
        if (Input.GetMouseButton(0) && isDragging && dragPoint != null)
        {
            if (dragPlane.Raycast(ray, out float dist))
            {
                dragPoint.position = ray.GetPoint(dist);
            }
        }

        // 松开鼠标结束拖拽
        if (Input.GetMouseButtonUp(0))
        {
            dragPoint = null;
            isDragging = false;
        }
    }

    // FFD贝塞尔插值变形
    void UpdateDeformation()
    {
        for (int i = 0; i < originalVerts.Length; i++)
        {
            Vector3 localV = transform.InverseTransformPoint(originalVerts[i]);

            float u = Mathf.InverseLerp(
                transform.InverseTransformPoint(latticeBounds.min).x,
                transform.InverseTransformPoint(latticeBounds.max).x,
                localV.x);
            float v = Mathf.InverseLerp(
                transform.InverseTransformPoint(latticeBounds.min).y,
                transform.InverseTransformPoint(latticeBounds.max).y,
                localV.y);
            float w = Mathf.InverseLerp(
                transform.InverseTransformPoint(latticeBounds.min).z,
                transform.InverseTransformPoint(latticeBounds.max).z,
                localV.z);

            u = Mathf.Clamp01(u);
            v = Mathf.Clamp01(v);
            w = Mathf.Clamp01(w);

            Vector3 deformedLocal = Vector3.zero;
            for (int iX = 0; iX <= 2; iX++)
            {
                float bx = Bernstein2(iX, u);
                for (int iY = 0; iY <= 2; iY++)
                {
                    float by = Bernstein2(iY, v);
                    for (int iZ = 0; iZ <= 2; iZ++)
                    {
                        float bz = Bernstein2(iZ, w);
                        int idx = iX * 9 + iY * 3 + iZ;
                        if (idx < ctrlPoints.Count && ctrlPoints[idx] != null)
                        {
                            Vector3 ctrlLocal = transform.InverseTransformPoint(ctrlPoints[idx].position);
                            deformedLocal += ctrlLocal * bx * by * bz;
                        }
                    }
                }
            }
            deformedVerts[i] = transform.TransformPoint(deformedLocal);
        }
        targetMesh.vertices = deformedVerts;
        targetMesh.RecalculateNormals();
    }

    // 二次伯恩斯坦基函数
    float Bernstein2(int n, float t)
    {
        if (n == 0) return (1f - t) * (1f - t);
        if (n == 1) return 2f * (1f - t) * t;
        return t * t;
    }
}
