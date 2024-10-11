using UnityEngine;

public class ToolController : MonoBehaviour
{
    [Header("Tool Settings")]
    private Collider toolCollider; // Collider của công cụ

    private void Start()
    {
        // Lấy Collider của công cụ
        toolCollider = GetComponent<Collider>();
        if (toolCollider == null)
        {
            Debug.LogError("Collider không được tìm thấy trên công cụ!");
        }
        else
        {
            toolCollider.enabled = false; // Tắt Collider ban đầu
        }
    }

    public void UseTool()
    {
        // Bật Collider của công cụ
        if (toolCollider != null)
        {
            toolCollider.enabled = true;
        }
    }

    public void NoUseTool()
    {
        // Tắt Collider của công cụ
        if (toolCollider != null)
        {
            toolCollider.enabled = false;
        }
    }
}
