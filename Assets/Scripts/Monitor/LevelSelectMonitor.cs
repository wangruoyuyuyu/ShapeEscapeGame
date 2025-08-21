using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monitor.Entry.Parts;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Process;
using Monitor.Game;
// using UnityEngine.UIElements;

namespace Monitor
{
    public class LevelSelectMonitor : MonoBehaviour
    {
        [SerializeField] Image backBtn;
        [SerializeField] internalLevelObject inlevels;
        [SerializeField] GridLayoutGroup theGridLayout;
        OptionDataManager optDataManager = new OptionDataManager();
        private bool hasLoaded = false;
        // Start is called before the first frame update
        void Start()
        {
            List<GameObject> extlevels;
            optDataManager.Init(this);
            if (optDataManager.CheckIsNotEmpty())
            {
                gameObject.SetActive(true);
                hasLoaded = true;
            }
            if (Application.platform != RuntimePlatform.Android || hasLoaded)
            {
                gameObject.SetActive(true);
                extlevels = optDataManager.LoadLevels();
            }
            else
            {
                gameObject.SetActive(false);
                optDataManager.LoadLevels();
                Debug.Log("loading process started");
                return;
            }
            PrefabBuilder builder = theGridLayout.GetComponent<PrefabBuilder>();
            int cnt = inlevels.Count();
            builder._prefabs = new GameObject[inlevels.Count() + extlevels.Count];
            for (int i = 0; i++ < cnt;)
            {
                builder._prefabs[i - 1] = (GameObject)Resources.Load("prefabs/process/levelSelect/prefabs/levelNum");
            }
            for (int j = 0; j++ < extlevels.Count;)
            {
                builder._prefabs[cnt + j - 1] = (GameObject)Resources.Load("prefabs/process/levelSelect/prefabs/levelNum");
            }
            Debug.Log("length:" + builder._prefabs.Length);
            builder.Build();
            int index;
            for (int i = 0; i++ < cnt + extlevels.Count;)
            {
                index = i - 1;
                SetupClickEvent(builder.Entities[index], index);
            }
            EventTrigger backtrigger = backBtn.GetComponent<EventTrigger>();
            if (backtrigger == null)
            {
                backtrigger = backBtn.gameObject.AddComponent<EventTrigger>();
            }
            backtrigger.triggers.Clear();
            EventTrigger.Entry backentry = new EventTrigger.Entry();
            backentry.callback.AddListener((data) => Back());
            backtrigger.triggers.Add(backentry);
        }
        public void LateLoad()
        {
            hasLoaded = true;
            Start();
        }
        // Update is called once per frame
        void Update()
        {

        }
        void Back()
        {
            GameObject backObj = (GameObject)Resources.Load("Prefabs/process/title/titleProcess");
            GameObject backIst = Instantiate(backObj);
            backIst.transform.parent = transform.parent;
            Close();
        }
        private void Close()
        {
            DestroyImmediate(gameObject);
        }
        private void SetupClickEvent(GameObject entity, int levelIndex)
        {
            entity.GetComponent<levelNumSetManager>().SetClickEvent(() =>
            {
                Transform parent = transform.parent;
                DestroyImmediate(gameObject);
                GameObject levelpre;
                bool isInternal = true;
                if (levelIndex < inlevels.Count())
                {
                    levelpre = inlevels.GetInternalLevelById(levelIndex);
                }
                else
                {
                    isInternal = false;
                    Debug.Log("preind:" + (levelIndex - inlevels.Count()));
                    if (!optDataManager.isInitiated)
                    {
                        optDataManager.Init(this);
                    }
                    levelpre = optDataManager.LoadLevels()[levelIndex - inlevels.Count()];
                }
                GameProcess gameProcess = new GameProcess();
                GameMonitor instance = gameProcess.InitPrefab();
                instance.transform.parent = parent;
                instance.StartGame(isInternal, levelIndex, levelpre, inlevels.GetInternalLevels());
            });
        }
    }
}