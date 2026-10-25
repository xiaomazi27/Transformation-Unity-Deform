using UnityEngine;

public class MeshDeformBase : MonoBehaviour
{
    protected Mesh originalMesh;
    protected Mesh workingMesh;

    protected virtual void Awake()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf != null && mf.mesh != null)
        {
            originalMesh = Instantiate(mf.mesh);
            workingMesh = Instantiate(originalMesh);
            mf.mesh = workingMesh;
        }
    }

    public virtual void ResetMesh()
    {
        if (originalMesh == null) return;
        MeshFilter mf = GetComponent<MeshFilter>();
        workingMesh = Instantiate(originalMesh);
        mf.mesh = workingMesh;
    }
}
