using System.Collections;
using System.Collections.Generic;
// using UnityEditor.SceneManagement;
using UnityEngine;
using Monitor;
using Utils;
using System.IO;

public class LoadLevelsProcess : MonoBehaviour
{
    LoadLevelsMonitor monitor;
    LevelSelectMonitor _levelSelectMonitor;
    string _unpackPath;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void InitPrefab(LevelSelectMonitor levelSelectMonitor, string unpackPath)
    {
        _levelSelectMonitor = levelSelectMonitor;
        _unpackPath = unpackPath;
        GameObject prefab = (GameObject)Resources.Load("Prefabs/process/loadLevels/loadLevelsProcess");
        GameObject instance = Instantiate(prefab);
        instance.transform.parent = levelSelectMonitor.transform.parent;
        monitor = instance.GetComponent<LoadLevelsMonitor>();
    }
    public void StartLoading()
    {
        AssetUnpacker.OnProgressUpdated += OnProgressUpdated;
        AssetUnpacker.OnUnpackCompleted += OnUnpackCompleted;
        AssetUnpacker.UnpackFromList(_unpackPath);
    }
    void OnProgressUpdated(float value)
    {
        monitor.SetValue(value);
    }
    void OnUnpackCompleted()
    {
        DestroyImmediate(monitor.gameObject);
        _levelSelectMonitor.LateLoad();
    }
}
