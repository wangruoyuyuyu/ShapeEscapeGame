using UnityEngine;
using UnityEngine.EventSystems;

public static class EventTriggerExtensions
{
    public static void AddEventTriggerListener(this EventTrigger trigger, 
        EventTriggerType eventType, 
        System.Action<PointerEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener((data) => callback((PointerEventData)data));
        trigger.triggers.Add(entry);
    }
}