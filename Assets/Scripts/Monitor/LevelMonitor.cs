using System.Collections;
using System.Collections.Generic;
using Monitor.Game;
using TMPro;

// using System.Xml.Linq;
using UnityEngine;
using Utils;

namespace Monitor
{
    public class LevelMonitor : MonoBehaviour
    {
        [Header("关卡设定")]
        [SerializeField] int allowFreeWhitenNum = 0;
        [Header("关联设定")]
        [SerializeField] List<Transform> bgTransforms;
        [SerializeField] AllFinishListObject allFinishList;
        public AssistantToolMonitor assistantTool;
        [Header("提示设定")]
        [SerializeField] TextMeshProUGUI noteText;
        [SerializeField] int promptMillsecs = 3000;
        public enum Mode
        {
            normalMode = 0,
            forceMode = 1,
            whitenMode = 2
        }
        public Mode mode
        {
            get
            {
                return _mode;
            }
            set
            {
                SetMode(value);
            }
        }
        private Mode _mode = 0;
        // Start is called before the first frame update
        void Start()
        {
            foreach (Transform i in bgTransforms)
            {
                i.SetAsFirstSibling();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
        IEnumerator DelayAction(float delayTime, System.Action action)
        {
            // 等待指定秒数（不阻塞界面）
            yield return new WaitForSeconds(delayTime);

            // 延迟结束后执行回调
            if (action != null && mode == Mode.normalMode)
                action();
        }
        void SetMode(Mode mode)
        {
            _mode = mode;
            if (mode == Mode.whitenMode)
            {
                if (noteText != null)
                {
                    if (noteText.text != "请选择要涂白的边")
                    {
                        noteText.text = "请选择要涂白的边";
                    }
                    noteText.gameObject.SetActive(true);
                    SetAllBallDisabled(true);
                }
            }
            else if (mode == Mode.normalMode)
            {
                noteText.gameObject.SetActive(false);
                SetAllBallDisabled(false);
            }
            else if (mode == Mode.forceMode)
            {
                if (noteText.text != "请选择要强制逃离的球")
                {
                    noteText.text = "请选择要强制逃离的球";
                }
                noteText.gameObject.SetActive(true);
                SetAllBallEnabled(true);
            }
        }
        void SetAllBallDisabled(bool is_)
        {
            foreach (ShapeMonitor i in allFinishList.GetShapeMonitors())
            {
                i.SetBallAllDisabled(is_);
            }
        }
        void SetAllBallEnabled(bool is_)
        {
            foreach (ShapeMonitor i in allFinishList.GetShapeMonitors())
            {
                i.SetBallAllEnabled(is_);
            }
        }
        public void NotificationBallAllEnded(ShapeMonitor shape)
        {
            if (allowFreeWhitenNum < 1)
            {
                return;
            }
            if (noteText != null)
            {
                if (noteText.text != "无边可出，随机消除一条边")
                {
                    noteText.text = "无边可出，随机消除一条边";
                }
                noteText.gameObject.SetActive(true);
                StartCoroutine(DelayAction(2, () =>
                {
                    noteText.gameObject.SetActive(false);
                }));
            }
            int a = GetNonWhiteSide(shape);
            Debug.Log("whited:" + a);
            shape.OnNotificatedSetWhiteSide(a);
            allowFreeWhitenNum--;
        }
        private int GetNonWhiteSide(ShapeMonitor shape)
        {
            System.Random random = new System.Random();
            int ranint = random.Next(0, shape.sides.Count);
            while (Extensions.IsWhite(shape.sides[ranint].color))
            {
                ranint = random.Next(0, shape.sides.Count);
            }
            return ranint;
        }
        public void ReassignAll()
        {
            foreach (ShapeMonitor i in allFinishList.GetShapeMonitors())
            {
                List<Color> colors = i.GetColors();
                i.ResetSides(colors);
                i.AssignBallColors();
                i.CheckBallEnabled();
            }
        }
    }
}