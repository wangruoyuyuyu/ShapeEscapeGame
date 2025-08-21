using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

namespace Monitor
{
    public class WaitToolMonitor : MonoBehaviour
    {
        [SerializeField] Image cancelBtn;
        [SerializeField] TextMeshProUGUI countDownText;
        public WaitToolProcess process;
        // Start is called before the first frame update
        void Start()
        {
            EventTrigger trigger = cancelBtn.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = cancelBtn.gameObject.AddComponent<EventTrigger>();
            }
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => process.OnCancel());
            trigger.triggers.Add(entry);
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void SetText(String text)
        {
            countDownText.text = text;
        }
    }
}