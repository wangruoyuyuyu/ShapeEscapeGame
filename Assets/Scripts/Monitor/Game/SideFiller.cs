using System.Collections;
using System.Collections.Generic;
// using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Monitor;

namespace Monitor.Game
{
    public class SideFiller : MonoBehaviour
    {
        [SerializeField] ShapeMonitor _shapeMonitor;
        [SerializeField] List<Image> _doNotFillSide;
        private List<Color> usedColor=new List<Color>();
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void FillNow()
        {
            foreach (Image i in _shapeMonitor.sides)
            {
                if (!Extensions.IsWhite(i.color))
                {
                    usedColor.Add(i.color);
                }
            }
            foreach (Image i in _shapeMonitor.sides)
            {
                if (_doNotFillSide.IndexOf(i) != -1)
                {
                    continue;
                }
                if (Extensions.IsWhite(i.color))
                {
                    i.color = GetUnusedColor();
                    Debug.Log("replaced white side"+_shapeMonitor.sides.IndexOf(i));
                    usedColor.Add(i.color);
                }
            }
        }
        private Color GetUnusedColor()
        {
            Color color = new Color();
            bool isColorUsed = true;
            System.Random random = new System.Random();
            while (isColorUsed)
            {
                color = _shapeMonitor.sideColors[random.Next(0, _shapeMonitor.sideColors.Count)];
                isColorUsed = usedColor.IndexOf(color) != -1;
            }
            return color;
        }
    }
}