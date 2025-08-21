using UnityEngine;

public class CameraAutoPosition : MonoBehaviour
{
    public Transform monitor;
    public float desiredWidth = 1080f;
    public float desiredHeight = 1920f;

    void Start()
    {
        Camera.main.orthographic = true;
        
        // 获取Canvas实际尺寸
        float canvasWidth = 1080f;
        float canvasHeight = 1920f;
        
        // 设置相机正交Size（高度的一半）
        Camera.main.orthographicSize = canvasHeight / 2f;
        
        // 计算相机到Canvas的距离（保持宽高比）
        float distance = canvasWidth / (2f * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad));
        
        // 放置相机在Canvas前方
        transform.position = new Vector3(0, 0, -distance);
        transform.LookAt(monitor.transform);
    }
}