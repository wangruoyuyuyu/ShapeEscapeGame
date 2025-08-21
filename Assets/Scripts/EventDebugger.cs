using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RaycastDebugger : MonoBehaviour
{
    [SerializeField]
    bool isDebugging;
    private void Update()
    {
        if (!isDebugging)
        {
            return;
        }if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("鼠标点击已检测"); // 第一步：确认点击事件被捕获
            
            // 第二步：检查EventSystem是否存在
            if (EventSystem.current == null)
            {
                Debug.LogError("EventSystem不存在！请确保场景中有EventSystem对象");
                return;
            }
            
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            Debug.Log("鼠标位置: " + eventData.position); // 第三步：确认鼠标坐标获取正确
            
            // 第四步：强制获取所有Raycaster（包括Canvas上的GraphicRaycaster）
            
            System.Collections.Generic.List<BaseRaycaster> raycasters = new System.Collections.Generic.List<BaseRaycaster>();
             // 根据Unity版本选择调用方式
            #if UNITY_2019_1_OR_NEWER
                // 新版本（2019+）使用带eventData的参数
               FindObjectsOfType<GraphicRaycaster>();
            #else
                // 旧版本（2018-）使用无eventData的参数
                RaycasterManager.GetRaycasters(raycasters);
            #endif
            
            
            if (raycasters.Count == 0)
            {
                Debug.LogError("未找到任何Raycaster组件！请检查Canvas是否添加了GraphicRaycaster");
                return;
            }
            
            // 第五步：手动执行所有Raycaster的射线检测
            System.Collections.Generic.List<RaycastResult> results = new System.Collections.Generic.List<RaycastResult>();
            foreach (var raycaster in raycasters)
            {
                if (raycaster is GraphicRaycaster)
                {
                    raycaster.Raycast(eventData, results);
                    Debug.Log("已通过GraphicRaycaster执行射线检测，命中数: " + results.Count);
                }
            }
            
            if (results.Count > 0)
            {
                Debug.Log("命中UI元素: " + results[0].gameObject.name);
            }
            else
            {
                Debug.Log("未命中任何UI元素");
            }
        }
    }
}