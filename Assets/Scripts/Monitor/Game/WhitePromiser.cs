using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Monitor;
using UnityEngine.UI;
using Utils;

namespace Monitor.Game
{
    public class WhitePromiser : MonoBehaviour
    {
        [SerializeField] ShapeMonitor _shape;
        [SerializeField] List<int> promisedSides;
        List<int> usedSideIndexes=new List<int>();
        // Start is called before the first frame update
        void Start()
        {
            // _shape.whitePromiser = this;
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void EnsureWhiteSides()
        {
            if (GetWhiteSideNum(_shape) >= promisedSides.Count)
            {
                //已满
                return;
            }
            for (int i = 0; i < promisedSides.Count - GetWhiteSideNum(_shape); i++)
            {
                _shape.sides[GetUnusedSideIndex()].color = Color.white;
            }
        }
        private int GetUnusedSideIndex()
        {
            System.Random random = new System.Random();
            int sideInd = promisedSides[random.Next(0, promisedSides.Count)];
            bool isSideUsed = (usedSideIndexes.IndexOf(sideInd) != -1);
            while (isSideUsed)
            {
                sideInd = promisedSides[random.Next(0, promisedSides.Count)];
                isSideUsed = (usedSideIndexes.IndexOf(sideInd) != -1);
            }
            usedSideIndexes.Add(sideInd);
            return sideInd;
        }
        private int GetWhiteSideNum(ShapeMonitor sm)
        {
            int ret = 0;
            foreach (Image i in sm.sides)
            {
                if (Extensions.IsWhite(i.color))
                {
                    ret++;
                }
            }
            return ret;
        }
    }
}