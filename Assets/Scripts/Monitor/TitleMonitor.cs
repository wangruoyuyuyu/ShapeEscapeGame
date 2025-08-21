using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using Manager;

namespace Monitor
{
    public class TitleMonitor : MonoBehaviour
    {
        [SerializeField] Image startImage;
        [SerializeField] Image exitImage;
        private ConfigManager configFileManager = new ConfigManager();
        // Start is called before the first frame update
        void Start()
        {
            EventTrigger trigger = startImage.GetComponent<EventTrigger>();
            trigger.triggers.Clear();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => StartGame());
            trigger.triggers.Add(entry);
            EventTrigger trigger1 = exitImage.GetComponent<EventTrigger>();
            if (trigger1 == null)
            {
                trigger1 = exitImage.gameObject.AddComponent<EventTrigger>();
            }
            EventTrigger.Entry entry1 = new EventTrigger.Entry();
            entry1.eventID = EventTriggerType.PointerClick;
            entry1.callback.AddListener((data) => Exit());
            trigger1.triggers.Add(entry1);
        }
        void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
        void StartGame()
        {
            Transform monitor = transform.parent;
            //GameObject newPrefab = (GameObject)Resources.Load("prefabs/process/game/gameProcess");
            GameObject newPrefab = (GameObject)Resources.Load("Prefabs/process/levelSelect/levelSelectProcess");
            GameObject newScene = Instantiate(newPrefab);
            newScene.transform.parent = monitor;
            DestroyImmediate(this.gameObject);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}