using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class internalLevelObject : MonoBehaviour
{
    [SerializeField] List<GameObject> internalLevels;
    public List<GameObject> GetInternalLevels()
    {
        return internalLevels;
    }
    public GameObject GetInternalLevelById(int index)
    {
        Debug.Log(index);
        return internalLevels[index];
    }
    public int Count()
    {
        return internalLevels.Count;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
