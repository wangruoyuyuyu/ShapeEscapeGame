using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(RectTransform))]
    public class CustomScrollableArea : MonoBehaviour
    {
        bool isDragging = false;
        bool isScaling = false;
        Vector2 lastMousePos;
        bool isFirstDragFrame = true;
        float lastScaling;
        bool isFirstScaleFrame = true;
        [SerializeField] public bool isEnabled = true;
        [SerializeField] private float minScale = 0.5f;
        [SerializeField] private float maxScale = 2.0f;
        private Vector3 initialScale;
        private Vector3 pivotPoint;
        private RectTransform rectTransform;
        private Canvas canvas;
        private Camera mainCamera;

        // Start is called before the first frame update
        void Start()
        {
            isDragging = false;
            isScaling = false;
            isFirstDragFrame = true;
            isFirstScaleFrame = true;
            initialScale = transform.localScale;
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            mainCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            if (!isEnabled)
            {
                return;
            }

            // 处理触摸输入
            if (Input.touchCount == 2)
            {
                isDragging = false;
                isScaling = true;
            }
            else
            {
                isFirstScaleFrame = true;
            }

            // 处理鼠标拖拽
            if (Input.GetMouseButtonDown(0) && !isScaling)
            {
                isDragging = true;
                isFirstDragFrame = true;
            }
            else if (Input.GetMouseButtonUp(0) && !isScaling)
            {
                isDragging = false;
            }

            //处理滚轮输入
            float axis = Input.GetAxis("Mouse ScrollWheel");
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                ApplyScaling(axis/30+1);
            }

            // 执行拖拽逻辑
            if (isDragging && !isScaling)
            {
                if (isFirstDragFrame)
                {
                    lastMousePos = Input.mousePosition;
                    isFirstDragFrame = false;
                }
                else
                {
                    if ((Vector2)Input.mousePosition != lastMousePos)
                    {
                        Vector3 worldPosNow = ScreenPosToWorldPos(Input.mousePosition);
                        Vector3 worldPosPrev = ScreenPosToWorldPos(lastMousePos);
                        Vector2 delta = worldPosNow - worldPosPrev;

                        if (!CheckDragState(delta))
                        {
                            return;
                        }

                        transform.position += (Vector3)delta;
                        lastMousePos = Input.mousePosition;
                    }
                }
            }

            // 执行缩放逻辑
            else if (isScaling)
            {
                if (Input.touchCount != 2)
                {
                    isScaling = false;
                    return;
                }

                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                Vector3 worldPos1 = ScreenPosToWorldPos(touch1.position);
                Vector3 worldPos2 = ScreenPosToWorldPos(touch2.position);
                float currentDistance = Vector2.Distance(worldPos1, worldPos2);

                // 计算缩放中心点
                pivotPoint = (worldPos1 + worldPos2) / 2;

                if (isFirstScaleFrame)
                {
                    lastScaling = currentDistance;
                    isFirstScaleFrame = false;
                }
                else
                {
                    // 计算缩放比例
                    float scaleFactor = currentDistance / lastScaling;
                    ApplyScaling(scaleFactor);
                    lastScaling = currentDistance;
                }
            }
        }

        // 将屏幕坐标转换为世界坐标
        private Vector3 ScreenPosToWorldPos(Vector2 screenPos)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                // 处理Overlay模式
                return screenPos;
            }
            else
            {
                // 处理WorldSpace和ScreenSpace-Camera模式
                Vector3 worldPos;
                RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    rectTransform, screenPos, mainCamera, out worldPos);
                return worldPos;
            }
        }

        private void ApplyScaling(float scaleFactor)
        {
            // 保存当前位置
            Vector3 currentPosition = transform.position;

            // 计算相对于缩放中心的位置
            Vector3 pivotOffset = currentPosition - pivotPoint;

            // 计算新的缩放值
            Vector3 newScale = transform.localScale * scaleFactor;

            // 限制最小和最大缩放
            float clampedScale = Mathf.Clamp(newScale.x, minScale, maxScale);
            newScale = new Vector3(clampedScale, clampedScale, initialScale.z);

            // 如果已经达到最大缩放且还想继续放大，则不执行任何操作
            if (transform.localScale.x >= maxScale && scaleFactor > 1)
            {
                return;
            }

            // 如果缩放有变化，则应用新的缩放和位置
            if (newScale != transform.localScale)
            {
                // 计算实际应用的缩放比例
                float actualScaleFactor = newScale.x / transform.localScale.x;

                // 更新位置以保持缩放中心不变
                Vector3 newPosition = pivotPoint + pivotOffset * actualScaleFactor;

                // 应用新的缩放和位置
                transform.localScale = newScale;
                transform.position = newPosition;

                // 确保内容在边界内
                ClampToBounds();
            }
        }

        // 边界检查函数
        private bool CheckDragState(Vector2 delta)
        {
            // 计算拖动后的预期位置
            Vector3 expectedPos = transform.position + (Vector3)delta;

            // 获取内容的半宽高（考虑缩放）
            float halfWidth = (rectTransform.sizeDelta.x * transform.localScale.x) / 2;
            float halfHeight = (rectTransform.sizeDelta.y * transform.localScale.y) / 2;

            // 检查左右边界
            if (expectedPos.x > halfWidth || expectedPos.x < -halfWidth)
            {
                return false;
            }

            // 检查上下边界
            if (expectedPos.y > halfHeight || expectedPos.y < -halfHeight)
            {
                return false;
            }

            return true;
        }

        // 确保内容在边界内
        private void ClampToBounds()
        {
            Vector3 pos = transform.position;

            // 获取内容的半宽高（考虑缩放）
            float halfWidth = (rectTransform.sizeDelta.x * transform.localScale.x) / 2;
            float halfHeight = (rectTransform.sizeDelta.y * transform.localScale.y) / 2;

            // 限制在边界内
            pos.x = Mathf.Clamp(pos.x, -halfWidth, halfWidth);
            pos.y = Mathf.Clamp(pos.y, -halfHeight, halfHeight);

            transform.position = pos;
        }
    }
}