using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monitor;
using System;

public class WaitToolProcess : MonoBehaviour
{
    WaitToolMonitor monitor;
    private float lt = 30;
    private bool isFirst = true;
    private bool isEnded = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void InitPrefab(Transform transform)
    {
        GameObject prefab = (GameObject)Resources.Load("Prefabs/process/waittool/waittoolprocess");
        GameObject instance = Instantiate(prefab);
        instance.transform.parent = transform.parent;
        monitor = instance.GetComponent<WaitToolMonitor>();
        monitor.process = this;
    }
    public void StartWaiting(Action callback)
    {
        Debug.Log("Started waiting...");
        isEnded = false;
        isFirst = true;
        Interval(30, UpdateTxt, callback);
    }
    private void UpdateTxt()
    {
        monitor.SetText(lt.ToString());
    }

    public IEnumerator DelayAction(float delayTime, System.Action action)
    {
        // 等待指定秒数（不阻塞界面）
        yield return new WaitForSeconds(delayTime);

        // 延迟结束后执行回调
        if (action != null)
            action();
    }
    public void Interval(float at, Action delayaction, Action endaction)
    {
        if (isEnded)
        {
            Destroy();
            return;
        }
        if (isFirst)
        {
            lt = at;
            isFirst = false;
        }
        if (lt < 1)
        {
            Debug.Log("Wait End");
            endaction();
            return;
        }
        Debug.Log("Waited a sec");
        delayaction();
        Debug.Log("performed delayaction");
        lt--;
        monitor.StartCoroutine(DelayAction(1, delegate () { Debug.Log("Delayed1s"); Interval(at, delayaction, endaction); }));
    }
    public void Destroy()
    {
        DestroyImmediate(monitor.gameObject);
    }
    public void OnCancel()
    {
        isEnded = true;
    }
}
