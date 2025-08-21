using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Monitor;

public class AllFinishListObject : MonoBehaviour
{
    [SerializeField] SerializableDictionary<ShapeMonitor, bool> finishedList;
    // Start is called before the first frame update
    void Start()
    {
        List<ShapeMonitor> a = finishedList.Keys.ToList();
        Debug.Log("len:" + a.Count);
        for (int i = 0; i < a.Count; i++)
        {
            Debug.Log("i:" + i);
            finishedList[a[i]] = false;
        }
    }
    public void OnRegisterEndedShape(ShapeMonitor shape)
    {
        finishedList[shape] = true;
    }
    public SerializableDictionary<ShapeMonitor, bool> GetDict()
    {
        return finishedList;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public List<ShapeMonitor> GetShapeMonitors()
    {
        return finishedList.Keys.ToList();
    }
}
