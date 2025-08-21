using System;
using System.Collections;
using System.Collections.Generic;
// using Palmmedia.ReportGenerator.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System.IO.Compression;
using Process;
using Monitor.Game;
using Manager;
using Utils;

namespace Monitor
{
    public class ResultMonitor : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI timeNumText;
        [SerializeField] Image NextBtn;
        [SerializeField] Image ReplayBtn;
        [SerializeField] Image BackBtn;
        [SerializeField] GameMonitor gm;
        ConfigManager configManager = new ConfigManager();
        private OptionDataManager _optDataManager = new OptionDataManager();
        private List<GameObject> _extenalLevels;
        private GameObject _gamePrefab;
        public void SetGP(GameObject g)
        {
            _gamePrefab = g;
        }
        public void SetGM(GameMonitor g)
        {
            gm = g;
        }
        // Start is called before the first frame update
        void Start()
        {
            _extenalLevels = _optDataManager.LoadLevels();
            EventTrigger trigger1 = NextBtn.GetComponent<EventTrigger>();
            EventTrigger trigger2 = ReplayBtn.GetComponent<EventTrigger>();
            EventTrigger trigger3 = BackBtn.GetComponent<EventTrigger>();
            if (trigger1 == null)
            {
                trigger1 = NextBtn.AddComponent<EventTrigger>();
            }
            if (trigger2 == null)
            {
                trigger2 = ReplayBtn.AddComponent<EventTrigger>();
            }
            if (trigger3 == null)
            {
                trigger3 = BackBtn.AddComponent<EventTrigger>();
            }
            EventTrigger.Entry entry1 = new EventTrigger.Entry();
            entry1.eventID = EventTriggerType.PointerClick;
            entry1.callback.AddListener((data) => Next());
            trigger1.triggers.Add(entry1); EventTrigger.Entry entry2 = new EventTrigger.Entry();
            entry2.eventID = EventTriggerType.PointerClick;
            entry2.callback.AddListener((data) => Replay());
            trigger2.triggers.Add(entry2); EventTrigger.Entry entry3 = new EventTrigger.Entry();
            entry3.eventID = EventTriggerType.PointerClick;
            entry3.callback.AddListener((data) => Back());
            trigger3.triggers.Add(entry3);

            if (gm.GetComponentInChildren<CustomScrollableArea>() != null)
            {
                foreach (CustomScrollableArea i in gm.GetComponentsInChildren<CustomScrollableArea>())
                {
                    i.isEnabled = false;
                }
            }

            if (gm.GetId() > configManager.getIntValue("GameProgress", "last_level", -1))
            {
                configManager.setIntValue("GameProgress", "last_level", gm.GetId());
                configManager.Save();
            }
        }
        void Replay()
        {
            GameMonitor instance = new GameProcess().InitPrefab();
            instance.transform.parent = gm.transform.parent;
            instance.GetComponent<GameMonitor>().StartGame(true, gm.levelIndex, _gamePrefab, gm.prefabList);
            Close();
        }
        void Back()
        {
            GameObject lsm_pre = (GameObject)Resources.Load("Prefabs/process/levelSelect/levelSelectProcess");
            Instantiate(lsm_pre).transform.parent = transform.parent;
            Close();
        }
        void Next()
        {
            if (gm.levelIndex >= gm.prefabList.Count - 1)
            {
                if (gm.levelIndex + 1 - gm.prefabList.Count >= _extenalLevels.Count)
                {
                    return;
                }
                GameObject newGameProcess = (GameObject)Resources.Load("Prefabs/process/game/gameProcess");
                GameObject instance = Instantiate(newGameProcess);
                instance.transform.parent = gm.transform.parent;
                GameObject newGamePrefab = _extenalLevels[gm.levelIndex + 1 - gm.prefabList.Count];
                instance.GetComponent<GameMonitor>().StartGame(false, gm.levelIndex + 1, newGamePrefab, gm.prefabList);
                Close();
            }
            else
            {
                GameObject newGameProcess = (GameObject)Resources.Load("Prefabs/process/game/gameProcess");
                GameObject instance = Instantiate(newGameProcess);
                instance.transform.parent = gm.transform.parent;
                if (gm.levelIndex < gm.prefabList.Count - 1)
                {
                    instance.GetComponent<GameMonitor>().StartGame(true, gm.levelIndex + 1, gm.prefabList[gm.levelIndex + 1], gm.prefabList);
                    Close();
                }
            }
        }
        void Close()
        {
            DestroyImmediate(this.gameObject);
            DestroyImmediate(gm.gameObject);
        }
        public void SetStartTime(long timeStamp, TimeSpan pauseUsed)
        {
            DateTimeOffset now = DateTime.UtcNow;
            DateTimeOffset start = DateTimeOffset.FromUnixTimeSeconds(timeStamp);
            TimeSpan used;
            if (pauseUsed != null) { used = now - start - pauseUsed; }
            else { used = now - start; }
            SetTime(used.ToString(@"hh\:mm\:ss"));
        }
        public void SetTime(string text)
        {
            timeNumText.text = text;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}