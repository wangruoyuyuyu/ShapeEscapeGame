using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;

namespace Utils
{
    public static class Extensions
    {
        public static Vector3 ScreenPosToWorldPos(Camera camera, Vector2 screenPos, Transform transform_)
        {
            Vector3 theScreen3DPos = new Vector3(screenPos.x, screenPos.y, transform_.position.z);
            return camera.ScreenToWorldPoint(theScreen3DPos);
        }
        public static List<GameObject> GetChild(this GameObject obj)
        {
            List<GameObject> tempArrayobj = new List<GameObject>();
            foreach (Transform child in obj.transform)
            {
                tempArrayobj.Add(child.gameObject);
            }
            return tempArrayobj;
        }
        public static long GetTimeStamp(bool isMillisecond = false)
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var timeStamp = isMillisecond ? Convert.ToInt64(ts.TotalMilliseconds) : Convert.ToInt64(ts.TotalSeconds);
            return timeStamp;
        }
        public static DateTime GetTime(long timeStamp, bool accurateToMilliseconds = false)
        {
            if (accurateToMilliseconds)
            {
                // 精确到毫秒
                return DateTimeOffset.FromUnixTimeMilliseconds(timeStamp).LocalDateTime;
            }
            else
            {
                // 精确到秒
                return DateTimeOffset.FromUnixTimeSeconds(timeStamp).LocalDateTime;
            }
        }
        public static bool IsWhite(Color color)
        {
            int r = (int)(color.r * 255);
            int g = (int)(color.g * 255);
            int b = (int)(color.b * 255);
            int a = (int)(color.a * 255);
            return r == 255 && g == 255 && b == 255 && a == 255;
        }
        public static bool IsDirectoryEmpty(string path)
        {
            return Directory.GetFiles(path).Length == 0 && Directory.GetDirectories(path).Length == 0;
        }
        public static EventTrigger GetEventTrigger(GameObject go)
        {
            EventTrigger ret = go.GetComponent<EventTrigger>();
            if (ret != null)
            {
                return ret;
            }
            return go.AddComponent<EventTrigger>();
        }
        public static void SetEvent(EventTrigger trigger, System.Action action, EventTriggerType type = EventTriggerType.PointerClick, bool clear = false)
        {
            if (clear)
            {
                trigger.triggers.Clear();
            }
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener((data) =>
            {
                // Debug.Log("事件触发，执行回调"); // 新增日志
                action.Invoke();
            });
            trigger.triggers.Add(entry);
            // Debug.Log($"已为{trigger.gameObject.name}绑定点击事件"); // 新增日志
        }
    }
}