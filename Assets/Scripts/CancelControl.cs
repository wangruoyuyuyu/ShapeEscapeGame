using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Monitor.Game;
using UnityEngine;
using Utils;
using Monitor;
using UnityEngine.EventSystems;

public class CancelControl : MonoBehaviour
{
    [SerializeField] Image cancelBtn;
    [SerializeField] AssistantToolMonitor assistantTool;
    // Start is called before the first frame update
    void Start()
    {
        Extensions.SetEvent(Extensions.GetEventTrigger(cancelBtn.gameObject), delegate () { Cancel(); },clear:true);
        SetClickEffect();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void SetClickEffect()
    {
        cancelBtn.enabled = false;
        Extensions.SetEvent(Extensions.GetEventTrigger(cancelBtn.gameObject), delegate () { cancelBtn.enabled = true; }, EventTriggerType.PointerDown);
        Extensions.SetEvent(Extensions.GetEventTrigger(cancelBtn.gameObject), delegate () { cancelBtn.enabled = false; },EventTriggerType.PointerUp);
    }
    void Cancel()
    {
        assistantTool.levelMonitor.mode = LevelMonitor.Mode.normalMode;
        assistantTool.ShowCancel(false);
    }
}
