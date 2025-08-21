using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class AssistantToolManager : MonoBehaviour
    {
        List<Monitor.ShapeMonitor> shapeMonitors;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void Init(AllFinishListObject listObject)
        {
            shapeMonitors = listObject.GetShapeMonitors();
        }
    }
}