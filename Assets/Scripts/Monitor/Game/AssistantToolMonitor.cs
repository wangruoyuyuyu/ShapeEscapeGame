using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Unity.VisualScripting;
using Monitor;
using Manager;

namespace Monitor.Game
{

    public class AssistantToolMonitor : MonoBehaviour
    {
        [Header("关联设定")]
        [SerializeField] AllFinishListObject allFinishListObject;
        [SerializeField] public LevelMonitor levelMonitor;
        private AssistantToolManager manager = new AssistantToolManager();
        [Header("按钮设定")]
        [SerializeField] Image whitenBtn;
        [SerializeField] Image reassignBtn;
        [SerializeField] Image forceBtn;
        [SerializeField] GameObject cancelControl;
        WaitToolProcess waitToolProcess = new WaitToolProcess();
        // Start is called before the first frame update
        void Start()
        {
            levelMonitor.assistantTool = this;
            manager.Init(allFinishListObject);
            SetEvent(GetEventTrigger(whitenBtn.gameObject), delegate () { Whiten(); },clear:true);
            SetEvent(GetEventTrigger(reassignBtn.gameObject), delegate () { Reassign(); },clear:true);
            SetEvent(GetEventTrigger(forceBtn.gameObject), delegate () { Force(); },clear:true);
            whitenBtn.enabled = false;
            reassignBtn.enabled = false;
            forceBtn.enabled = false;
            SetClickEffect();
        }
        void SetClickEffect()
        {
            SetEvent(GetEventTrigger(whitenBtn.gameObject), delegate () { whitenBtn.enabled = true; }, EventTriggerType.PointerDown);
            SetEvent(GetEventTrigger(reassignBtn.gameObject), delegate () { reassignBtn.enabled = true; }, EventTriggerType.PointerDown);
            SetEvent(GetEventTrigger(forceBtn.gameObject), delegate () { forceBtn.enabled = true; }, EventTriggerType.PointerDown);
            SetEvent(GetEventTrigger(whitenBtn.gameObject), delegate () { whitenBtn.enabled = false; }, EventTriggerType.PointerUp);
            SetEvent(GetEventTrigger(reassignBtn.gameObject), delegate () { reassignBtn.enabled = false; }, EventTriggerType.PointerUp);
            SetEvent(GetEventTrigger(forceBtn.gameObject), delegate () { forceBtn.enabled = false; }, EventTriggerType.PointerUp);
         }

        // Update is called once per frame
        void Update()
        {

        }
        void Whiten()
        {
            waitToolProcess.InitPrefab(levelMonitor.transform.parent);
            waitToolProcess.StartWaiting(Whiten_);
        }
        void Whiten_()
        {
            waitToolProcess.Destroy();
            levelMonitor.mode = LevelMonitor.Mode.whitenMode;
            ShowCancel(true);
            Debug.Log("Whitened");
        }
        void Reassign()
        {
            waitToolProcess.InitPrefab(levelMonitor.transform.parent);
            waitToolProcess.StartWaiting(Reassign_);
        }
        void Reassign_()
        {
            waitToolProcess.Destroy();
            levelMonitor.ReassignAll();
            Debug.Log("Reassigned");
        }
        void Force()
        {
            waitToolProcess.InitPrefab(levelMonitor.transform.parent);
            waitToolProcess.StartWaiting(Force_);
        }
        void Force_()
        {
            waitToolProcess.Destroy();
            levelMonitor.mode = LevelMonitor.Mode.forceMode;
            ShowCancel(true);
            Debug.Log("Forces");
        }
        public void ShowCancel(bool is_)
        {
            whitenBtn.gameObject.SetActive(!is_);
            reassignBtn.gameObject.SetActive(!is_);
            forceBtn.gameObject.SetActive(!is_);
            cancelControl.SetActive(is_);
        }
        EventTrigger GetEventTrigger(GameObject go)
        {
            EventTrigger ret = go.GetComponent<EventTrigger>();
            if (ret != null)
            {
                return ret;
            }
            return go.AddComponent<EventTrigger>();
        }
        void SetEvent(EventTrigger trigger, System.Action action,EventTriggerType type=EventTriggerType.PointerClick,bool clear=false)
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