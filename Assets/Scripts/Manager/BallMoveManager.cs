using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BallMoveManager : MonoBehaviour
{
    public float moveSpeed = 500f;
    private RectTransform rectTransform;
    private RectTransform canvasRect;
    private Vector2 moveDirection;
    private Camera mainCamera;
    public bool isMoving = false;
    int id = 0;
    int sideNum;
    bool isSideAtTop = false;
    public bool isEndedUp = false;

    public void StartMoving(int id1, int sideNm, bool isSideAtTop1)
    {
        isMoving = true;
        isEndedUp = true;
        rectTransform = GetComponent<RectTransform>();
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        mainCamera = Camera.main;

        // 计算从Canvas中心指向UI元素的方向（反方向）
        id = id1;
        sideNum = sideNm;
        isSideAtTop = isSideAtTop1;
        CalculateInverseDirection();
    }

    void Update()
    {
        if (!isMoving)
        {
            return;
        }
        // 在Canvas本地空间中移动
        rectTransform.anchoredPosition += moveDirection * moveSpeed * Time.deltaTime;

        // 检查是否移出屏幕
        if (!IsUIElementVisible())
        {
            enabled = false; // 停止移动
            Debug.Log("UI元素已移出屏幕，停止移动");
            gameObject.SetActive(false);//隐藏球，以免用户拖动界面时看到跑出去的球
        }
    }

    void CalculateInverseDirection()
    {
        // 获取UI元素在Canvas中的位置
        Vector2 uiPosition = rectTransform.anchoredPosition;
        float innerAngle = 360 / (float)sideNum;
        Debug.Log("id:" + id + ", innerAngle:" + innerAngle);

        // 计算从Canvas中心(0,0)指向UI元素的方向
        //moveDirection = (uiPosition - Vector2.zero).normalized;
        if (!isSideAtTop) { moveDirection = Vector2Extensions.FromAngle(-innerAngle * id + 90 + innerAngle / 2); }
        else { moveDirection = Vector2Extensions.FromAngle(90 + innerAngle - innerAngle * id); }
    }

    bool IsUIElementVisible()
    {
        // 获取UI元素四个角的世界坐标
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        // 检查是否所有角都在屏幕外
        foreach (Vector3 corner in corners)
        {
            Vector3 screenPoint = mainCamera.WorldToScreenPoint(corner);
            if (screenPoint.x >= 0 && screenPoint.x <= Screen.width &&
                screenPoint.y >= 0 && screenPoint.y <= Screen.height &&
                screenPoint.z > 0)
            {
                return true; // 至少有一个角可见
            }
        }

        return false; // 所有角都不可见
    }
}