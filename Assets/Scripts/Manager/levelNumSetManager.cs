using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
// using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Manager;

public class levelNumSetManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI numText;
    [SerializeField] Image backImage;
    [SerializeField] Image completedImg;
    ConfigManager configManager = new ConfigManager();
    public delegate void DelClickEvent();
    DelClickEvent _del;
    public void SetClickEvent(DelClickEvent del)
    {
        _del = del;
    }
    public void SetNum(int num)
    {
        numText.text = num.ToString();
        if (configManager.getIntValue("GameProgress", "last_level", -1) >= num)
        {
            completedImg.gameObject.SetActive(true);
        }
        else
        {
            completedImg.gameObject.SetActive(false);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        EventTrigger trigger = backImage.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = backImage.gameObject.AddComponent<EventTrigger>();
        }
        trigger.triggers.Clear();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => RealClickEvent());
        trigger.triggers.Add(entry);
    }
    void RealClickEvent()
    {
        Debug.Log("clicked");
        if (_del != null)
        {
            _del();
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
