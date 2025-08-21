using System.Collections;
using System.Collections.Generic;
// using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Process;
using Utils;
using Monitor;

namespace Monitor.Game
{
    public class GameMonitor : MonoBehaviour
    {
        [SerializeField] Image pauseBtn;
        long startTime;
        TimeSpan _pauseUsed;
        private GameObject _gamePrefab;
        public GameObject gamePrefab => _gamePrefab;
        private List<GameObject> _prefabList;
        private int _levelIndex;
        private bool _isInternal;
        public bool isInternal => _isInternal;
        public int levelIndex => _levelIndex;
        public List<GameObject> prefabList => _prefabList;
        public void AddPauseUsed(TimeSpan used)
        {
            _pauseUsed += used;
        }
        // Start is called before the first frame update
        private void Start()
        {
            EventTrigger trigger = pauseBtn.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = pauseBtn.gameObject.AddComponent<EventTrigger>();
            }
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.callback.AddListener((data) => Pause());
            trigger.triggers.Add(entry);
        }
        public void StartGame(bool isInternal, int index, GameObject gamePrefab, List<GameObject> prefabList)
        {
            Debug.Log("Initialized Level:"+index);
            _isInternal = isInternal;
            _gamePrefab = gamePrefab;
            _prefabList = prefabList;
            _levelIndex = index;
            startTime = Extensions.GetTimeStamp();
            //GameObject gamePrefab = (GameObject)Resources.Load("prefabs/process/game/prefabs/triangle");
            GameObject scene = Instantiate(gamePrefab);
            scene.transform.parent = this.transform;
        }

        // Update is called once per frame
        void Update()
        {

        }
        private void Pause()
        {
            new PauseProcess().PauseGame(this);
        }
        public void StartResultProcess()
        {
            GameObject resultPrefab = (GameObject)Resources.Load("prefabs/process/result/resultProcess");
            GameObject resultGO = Instantiate(resultPrefab);
            resultGO.transform.parent = this.transform.parent;
            ResultMonitor rm = resultGO.GetComponent<ResultMonitor>();
            rm.SetStartTime(startTime, _pauseUsed);
            rm.SetGM(this);
            rm.SetGP(_gamePrefab);
        }
        public int GetId()
        {
            return _levelIndex + 1;
        }
    }
}