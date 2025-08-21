using System.Collections;
using System.Collections.Generic;
// using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Utils;
using Process;
using Monitor.Game;

namespace Monitor
{
    public class PauseMonitor : MonoBehaviour
    {
        [SerializeField] Image continueBtn;
        [SerializeField] Image replayBtn;
        [SerializeField] Image backBtn;
        private GameMonitor _game;
        long _startTime;
        public void SetGame(GameMonitor game)
        {
            _game = game;
        }
        // Start is called before the first frame update
        void Start()
        {
            _startTime = Extensions.GetTimeStamp();
            EventTrigger trigger1 = continueBtn.GetComponent<EventTrigger>();
            EventTrigger trigger2 = replayBtn.GetComponent<EventTrigger>();
            EventTrigger trigger3 = backBtn.GetComponent<EventTrigger>();
            if (trigger1 == null)
            {
                trigger1 = continueBtn.gameObject.AddComponent<EventTrigger>();
            }
            if (trigger2 == null)
            {
                trigger2 = replayBtn.gameObject.AddComponent<EventTrigger>();
            }
            if (trigger3 == null)
            {
                trigger3 = backBtn.gameObject.AddComponent<EventTrigger>();
            }
            EventTrigger.Entry entry1 = new EventTrigger.Entry();
            EventTrigger.Entry entry2 = new EventTrigger.Entry();
            EventTrigger.Entry entry3 = new EventTrigger.Entry();
            entry1.callback.AddListener((data) => Continue());
            entry2.callback.AddListener((data) => Replay());
            entry3.callback.AddListener((data) => Back());
            trigger1.triggers.Add(entry1);
            trigger2.triggers.Add(entry2);
            trigger3.triggers.Add(entry3);
        }
        void Continue()
        {
            TimeSpan span = DateTime.UtcNow - DateTimeOffset.FromUnixTimeSeconds(_startTime);
            _game.AddPauseUsed(span);
            foreach (CustomScrollableArea i in _game.GetComponentsInChildren<CustomScrollableArea>())
            {
                i.isEnabled = true;
            }
            Close();
        }
        void Replay()
        {
            // GameObject levelPrefab = _game.prefabList[_game.levelIndex];
            GameMonitor instance = new GameProcess().InitPrefab();
            instance.transform.parent = _game.transform.parent;
            instance.GetComponent<GameMonitor>().StartGame(_game.isInternal, _game.levelIndex, _game.gamePrefab, _game.prefabList);
            DestroyImmediate(_game.gameObject);
            Close();
        }
        void Back()
        {
            GameObject levelSelectPrefab = (GameObject)Resources.Load("Prefabs/process/levelSelect/levelSelectProcess");
            GameObject levelSelectInstance = Instantiate(levelSelectPrefab);
            levelSelectInstance.transform.parent = _game.transform.parent;
            DestroyImmediate(_game.gameObject);
            Close();
        }

        // Update is called once per frame
        void Update()
        {

        }
        void Close()
        {
            DestroyImmediate(gameObject);
        }
    }
}