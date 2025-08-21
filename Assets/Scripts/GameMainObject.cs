using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMainObject : MonoBehaviour
{
    [SerializeField]
    Transform monitor;
    // Start is called before the first frame update
    void Start()
    {
        GameObject prefab = (GameObject)Resources.Load("prefabs/process/title/titleProcess");
        GameObject prefabInstance = Instantiate(prefab);
        prefabInstance.transform.parent = monitor.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
