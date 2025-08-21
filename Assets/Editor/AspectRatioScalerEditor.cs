using UnityEngine;
using UnityEditor;

// 自定义编辑器脚本
[CustomEditor(typeof(Transform), true)]
[CanEditMultipleObjects]
public class AspectRatioScalerEditor : Editor
{
    private Vector2 targetAspectRatio = new Vector2(16, 9);
    private bool showAspectRatioControls = false;
    private Transform targetTransform;

    private void OnEnable()
    {
        targetTransform = (Transform)target;
    }

    public override void OnInspectorGUI()
    {
        // 绘制默认的 Transform Inspector
        base.OnInspectorGUI();

        // 仅当GameObject挂有 AspectRatioScalable 组件时显示自定义选项
        if (!HasAspectRatioScalable(targetTransform))
            return;

        // 添加分隔线
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("按比例缩放", EditorStyles.boldLabel);
        
        // 显示/隐藏控制选项
        showAspectRatioControls = EditorGUILayout.Foldout(showAspectRatioControls, "缩放选项");
        
        if (showAspectRatioControls)
        {
            EditorGUI.indentLevel++;
            
            // 输入目标长宽比
            EditorGUILayout.BeginHorizontal();
            targetAspectRatio.x = EditorGUILayout.FloatField("宽比例", targetAspectRatio.x);
            targetAspectRatio.y = EditorGUILayout.FloatField("高比例", targetAspectRatio.y);
            EditorGUILayout.EndHorizontal();
            
            // 保持当前比例按钮
            if (GUILayout.Button("从当前尺寸计算比例"))
            {
                CalculateCurrentAspectRatio();
            }
            
            // 缩放选项
            EditorGUILayout.LabelField("缩放模式:");
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("按宽度缩放"))
            {
                ScaleByWidth();
            }
            
            if (GUILayout.Button("按高度缩放"))
            {
                ScaleByHeight();
            }
            
            if (GUILayout.Button("适应区域"))
            {
                FitToArea();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.indentLevel--;
        }
    }

    // 检查是否有 AspectRatioScalable 组件
    private bool HasAspectRatioScalable(Transform target)
    {
        return target.GetComponent<AspectRatioScalable>() != null;
    }

    // 从当前尺寸计算长宽比
    private void CalculateCurrentAspectRatio()
    {
        Bounds bounds = GetObjectBounds();
        targetAspectRatio = new Vector2(bounds.size.x, bounds.size.y);
        EditorUtility.SetDirty(target);
    }

    // 按宽度缩放
    private void ScaleByWidth()
    {
        Bounds bounds = GetObjectBounds();
        float currentAspect = bounds.size.x / bounds.size.y;
        float targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        
        if (Mathf.Approximately(currentAspect, targetAspect))
        {
            Debug.Log("当前长宽比已经匹配目标比例: " + targetAspectRatio.x + ":" + targetAspectRatio.y);
            return;
        }
        
        Undo.RecordObject(targetTransform, "按宽度缩放");
        
        // 计算需要调整的比例
        float heightScale = currentAspect / targetAspect;
        
        // 应用缩放
        Vector3 newScale = targetTransform.localScale;
        newScale.y = newScale.y * heightScale;
        targetTransform.localScale = newScale;
        
        EditorUtility.SetDirty(target);
    }

    // 按高度缩放
    private void ScaleByHeight()
    {
        Bounds bounds = GetObjectBounds();
        float currentAspect = bounds.size.x / bounds.size.y;
        float targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        
        if (Mathf.Approximately(currentAspect, targetAspect))
        {
            Debug.Log("当前长宽比已经匹配目标比例: " + targetAspectRatio.x + ":" + targetAspectRatio.y);
            return;
        }
        
        Undo.RecordObject(targetTransform, "按高度缩放");
        
        // 计算需要调整的比例
        float widthScale = targetAspect / currentAspect;
        
        // 应用缩放
        Vector3 newScale = targetTransform.localScale;
        newScale.x = newScale.x * widthScale;
        targetTransform.localScale = newScale;
        
        EditorUtility.SetDirty(target);
    }

    // 适应区域（保持最大尺寸同时符合比例）
    private void FitToArea()
    {
        Bounds bounds = GetObjectBounds();
        float currentAspect = bounds.size.x / bounds.size.y;
        float targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        
        if (Mathf.Approximately(currentAspect, targetAspect))
        {
            Debug.Log("当前长宽比已经匹配目标比例: " + targetAspectRatio.x + ":" + targetAspectRatio.y);
            return;
        }
        
        Undo.RecordObject(targetTransform, "适应区域缩放");
        
        Vector3 newScale = targetTransform.localScale;
        
        if (currentAspect > targetAspect)
        {
            // 当前更宽，按宽度缩放
            float heightScale = currentAspect / targetAspect;
            newScale.y = newScale.y * heightScale;
        }
        else
        {
            // 当前更高，按高度缩放
            float widthScale = targetAspect / currentAspect;
            newScale.x = newScale.x * widthScale;
        }
        
        targetTransform.localScale = newScale;
        EditorUtility.SetDirty(target);
    }

    // 获取对象边界
    private Bounds GetObjectBounds()
    {
        Renderer renderer = targetTransform.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds;
        }
        
        RectTransform rectTransform = targetTransform.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            return new Bounds(Vector3.zero, rectTransform.sizeDelta);
        }
        
        // 如果没有Renderer或RectTransform，返回基于子对象的边界
        Bounds combinedBounds = new Bounds(targetTransform.position, Vector3.zero);
        
        foreach (Transform child in targetTransform)
        {
            Renderer childRenderer = child.GetComponent<Renderer>();
            if (childRenderer != null)
            {
                combinedBounds.Encapsulate(childRenderer.bounds);
            }
            
            RectTransform childRect = child.GetComponent<RectTransform>();
            if (childRect != null)
            {
                Vector3 min = childRect.TransformPoint(childRect.rect.min);
                Vector3 max = childRect.TransformPoint(childRect.rect.max);
                combinedBounds.Encapsulate(min);
                combinedBounds.Encapsulate(max);
            }
        }
        
        return combinedBounds;
    }
}