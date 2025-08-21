using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Monitor
{
    public class LoadLevelsMonitor : MonoBehaviour
    {
        private float startX;
        [SerializeField] float endX;
        [SerializeField] Image progressBarImage;
        [SerializeField] TextMeshProUGUI progressText;
        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Loading process shown,parent:" + transform.parent.name);
            startX = progressBarImage.transform.position.x;
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void SetValue(float value)
        {
            progressText.SetText(100*value+"%");
            Vector2 pos = progressBarImage.transform.position;
            pos.x = startX + (endX - startX) * value;
            Debug.Log("pbar updated,x:" + pos.x);
            progressBarImage.transform.position = pos;
        }
    }
}