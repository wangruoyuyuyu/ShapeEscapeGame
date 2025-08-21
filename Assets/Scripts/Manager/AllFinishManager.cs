using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monitor;

namespace Manager
{
    public class AllFinishManager : MonoBehaviour
    {
        AllFinishListObject _obj;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void InitObject(AllFinishListObject obj)
        {
            _obj = obj;
        }
        public bool HasObject()
        {
            return _obj != null;
        }
        public bool RegisterEndedShape(ShapeMonitor shape)
        {
            _obj.OnRegisterEndedShape(shape);
            return CheckShapeAllFinished();
        }
        public bool CheckShapeAllFinished()
        {
            SerializableDictionary<ShapeMonitor, bool> dict = _obj.GetDict();
            foreach (ShapeMonitor shapeMonitor in dict.Keys)
            {
                if (!dict[shapeMonitor])
                {
                    return false;
                }
            }
            return true;
        }
    }
}