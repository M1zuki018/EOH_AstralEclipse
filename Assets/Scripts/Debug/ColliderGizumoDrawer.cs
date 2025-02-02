using UnityEngine;

/// <summary>
/// アタッチしたオブジェクトのコライダーのギズモを表示するクラス
/// </summary>
[ExecuteAlways] // エディタ上でも実行されるようにする
public class ColliderGizmoDrawer : MonoBehaviour
{
    public Color gizmoColor = Color.green; // ギズモの色
    private Collider col;

    private void OnValidate()
    {
        col = GetComponent<Collider>(); // コライダーを取得
    }

    private void OnDrawGizmos()
    {
        if (col == null)
        {
            col = GetComponent<Collider>();
            if (col == null) return;
        }

        Gizmos.color = gizmoColor;

        if (col is BoxCollider box)
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.DrawWireCube(box.center, box.size);
        }
        else if (col is SphereCollider sphere)
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireSphere(sphere.center, sphere.radius);
        }
        else if (col is CapsuleCollider capsule)
        {
            DrawCapsuleGizmo(capsule);
        }
        else if (col is MeshCollider mesh)
        {
            if (mesh.sharedMesh != null)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireMesh(mesh.sharedMesh);
            }
        }
    }

    private void DrawCapsuleGizmo(CapsuleCollider capsule)
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        float radius = capsule.radius;
        float height = capsule.height * 0.5f - radius;
        Vector3 center = capsule.center;
        Vector3 up = Vector3.up * height;

        if (capsule.direction == 0) // X方向
        {
            up = Vector3.right * height;
        }
        else if (capsule.direction == 2) // Z方向
        {
            up = Vector3.forward * height;
        }

        Gizmos.DrawWireSphere(center + up, radius);
        Gizmos.DrawWireSphere(center - up, radius);
        Gizmos.DrawLine(center + up + Vector3.forward * radius, center - up + Vector3.forward * radius);
        Gizmos.DrawLine(center + up - Vector3.forward * radius, center - up - Vector3.forward * radius);
        Gizmos.DrawLine(center + up + Vector3.right * radius, center - up + Vector3.right * radius);
        Gizmos.DrawLine(center + up - Vector3.right * radius, center - up - Vector3.right * radius);
    }
}
