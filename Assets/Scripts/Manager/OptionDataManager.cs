using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utils;
using Monitor;

public class OptionDataManager : MonoBehaviour
{
    LoadLevelsProcess loadLevelsProcess;
    private LevelSelectMonitor _lsm;
    private static List<GameObject> _loadedLevels;
    private static List<GameObject> loadedLevels
    {
        get
        {
            return _loadedLevels;
        }
        set
        {
            _loadedLevels = value;
        }
    }
    public bool isInitiated = false;
    private string _abpath
    {
        get
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            return Path.Combine(Application.streamingAssetsPath, "levels", "StandaloneWindows");
#elif UNITY_ANDROID
            return Path.Combine(Application.persistentDataPath,"opt","levels","Android");
#else
            return Path.Combine(Application.streamingAssetsPath,"levels");
#endif
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Init(LevelSelectMonitor levelSelectMonitor)
    {
        isInitiated = true;
        _lsm = levelSelectMonitor;
    }
    public bool CheckIsNotEmpty()
    {
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "opt")))
        {
            return false;
        }
        if (Extensions.IsDirectoryEmpty(Path.Combine(Application.persistentDataPath, "opt")))
        {
            return false;
        }
        return true;
    }
    public List<GameObject> LoadLevels(bool isAndroidLoaded = false)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if(!isAndroidLoaded && !CheckIsNotEmpty()){
            loadLevelsProcess=new LoadLevelsProcess();
            loadLevelsProcess.InitPrefab(_lsm,Path.Combine(Application.persistentDataPath,"opt"));
            loadLevelsProcess.StartLoading();
            return new List<GameObject>();
        }
#endif
        if (loadedLevels != null)
        {
            return loadedLevels;
        }
        if (!Directory.Exists(_abpath))
        {
            Debug.Log(_abpath);
            Debug.Log("not found");
            return new List<GameObject>();
        }
        DirectoryInfo directoryInfo = new DirectoryInfo(_abpath);
        List<GameObject> levels = new List<GameObject>();
        foreach (FileInfo f in directoryInfo.GetFiles())
        {
            string fn = f.FullName;
            if (f.FullName.EndsWith(".ab"))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(f.FullName);
                foreach (Object i in ab.LoadAllAssets())
                {
                    levels.Add((GameObject)i);
                }
            }
        }
        loadedLevels = levels;
        return levels;
    }
}
