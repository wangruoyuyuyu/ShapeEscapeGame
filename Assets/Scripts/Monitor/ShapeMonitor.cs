using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Process;
using Unity.VisualScripting;
using Manager;
using Utils;
using Monitor.Game;
// using UnityEngine.UIElements;

namespace Monitor
{
    public class ShapeMonitor : MonoBehaviour
    {
        [Header("关卡嵌入设定")]
        [SerializeField] bool debugMode = false;
        [SerializeField] bool isSub = false;
        [Header("本体设定")]
        [SerializeField] bool isSideAtTop;
        [SerializeField] public List<Image> sides;            // 所有边
        [SerializeField] List<Mask> balls;             // 所有球
        [SerializeField] List<Sprite> ballSprites;     // 球精灵
        [SerializeField] public List<Color> sideColors;       // 边颜色（最后一个是白色）
        [Header("关联设定")]
        [SerializeField] SerializableDictionary<ShapeMonitor, List<int>> connectedSides;
        [SerializeField] AllFinishListObject finishedShapeList;
        [SerializeField] SideFiller sideFiller;
        [SerializeField] LevelMonitor levelMonitor;
        [SerializeField] public WhitePromiser whitePromiser;
        private AllFinishManager _allFinishManager = new AllFinishManager();
        private GameProcess gameProcess = new GameProcess();
        private int currentWhiteSideIndex = 0;         // 当前白边索引
        private int currentActiveBallIndex = 0;        // 当前可点击球的索引
        private List<int> ballOrder = new List<int>(); // 球的点击顺序
        private List<int> ballTargetSideIndices = new List<int>(); // 每个球对应的目标边索引

        private Dictionary<string, int> colorIdToSideColorIndexMap = new Dictionary<string, int>()
    {
        { "red", 0 },
        { "green", 1 },
        { "blue", 2 }
    };

        private bool isGameCompleted = false;          // 游戏是否已通关
        private void SyncConnectedSide(int sideNum)
        {
            if (connectedSides == null || connectedSides.Count == 0)
            {
                return;//没有东西
            }
            foreach (KeyValuePair<ShapeMonitor, List<int>> item in connectedSides)
            {
                if (item.Value.Count != 2)
                {
                    continue;//数量不对
                }
                if (item.Value[0] == sideNum)
                {
                    //是当前边
                    if (Extensions.IsWhite(sides[sideNum].color))
                    {
                        item.Key.OnNotificatedSetWhiteSide(item.Value[1]);
                    }
                }
            }
        }
        private bool CheckBallIsAllDisabled()
        {
            if (CheckBallIsAllEnded())
            {
                return false;//避免球用完后触发保底逻辑
            }
            for (int i = 0; i < balls.Count; i++)
            {
                if (IsBallStill(i))
                {
                    return false;
                }
            }
            return true;
        }
        private bool CheckBallIsAllEnded()
        {
            foreach (Mask i in balls)
            {
                if (!(i.GetComponent<BallMoveManager>().isEndedUp || i.GetComponent<BallMoveManager>().isMoving))
                {
                    return false;
                }
            }
            return true;
        }
        private bool IsBallStill(int ballIndex)
        {
            if (balls[ballIndex].GetComponent<BallMoveManager>().isMoving)
            {
                Debug.Log("ball " + ballIndex + " is moving,false");
                return false;
            }
            if (balls[ballIndex].GetComponent<BallMoveManager>().isEndedUp)
            {
                Debug.Log("ball " + ballIndex + " is ended up,false");
                return false;
            }
            if (!balls[ballIndex].GetComponent<Image>().raycastTarget)
            {
                Debug.Log("ball " + ballIndex + " is disabled,false");
                return false;
            }
            return true;
        }
        private bool CheckConnectedSide(int sideNum)
        {
            foreach (KeyValuePair<ShapeMonitor, List<int>> item in connectedSides)
            {
                if (item.Value.Count != 2)
                {
                    continue;//数量不对
                }
                if (item.Value[0] == sideNum)
                {
                    return true;
                }
            }
            return false;
        }
        public void OnNotificatedSetWhiteSide(int sideid)
        {
            sides[sideid].color = Color.white;
            CheckBallEnabled();
        }
        void Start()
        {
            _allFinishManager.InitObject(finishedShapeList);
            // 验证配置
            if (sides.Count != balls.Count)
            {
                Debug.LogError("边数量必须等于球数量！");
                return;
            }

            // 预计算关卡流程
            if (debugMode)
            {
                Debug.Break();
            }
            PrecomputeLevelFlow();

            // 设置初始白边
            if (debugMode)
            {
                Debug.Break();
            }
            int cnt = 0;
            while (CheckConnectedSide(currentWhiteSideIndex))
            {
                cnt++;
                if (cnt > 100)
                {
                    Debug.Log("CheckConnectedSide卡了:" + name);
                    Debug.Log("都绑完了就算了");
                    break;
                }
                currentWhiteSideIndex = (currentWhiteSideIndex + 1) % sides.Count;
            }
            sides[currentWhiteSideIndex].color = Color.white;

            // 为所有球分配颜色和目标边
            AssignBallColors();

            // 启用第一个球，禁用其他球
            // EnableBallByIndex(currentActiveBallIndex);
            GenerateSides();
            if (sideFiller != null)
            {
                Debug.Log("now filling...");
                sideFiller.FillNow();
            }
            if (whitePromiser != null)
            {
                Debug.Log("Now promising white...");
                whitePromiser.EnsureWhiteSides();
            }
            CheckBallEnabled();
        }

        void PrecomputeLevelFlow()
        {
            // 初始化球的点击顺序和目标边
            for (int i = 0; i < balls.Count - 1; i++) // 最后一个球是通关球
            {
                ballOrder.Add(i);

                // 计算该球对应的目标边索引
                int targetSideIndex = (currentWhiteSideIndex + i + 1) % sides.Count;
                ballTargetSideIndices.Add(targetSideIndex);
            }
        }

        public void AssignBallColors(List<Sprite> useSprites = null)
        {
            List<int> usedColorIndex = new List<int>();
            // 为每个球分配与目标边对应的颜色
            for (int i = 0; i < balls.Count; i++)
            {
                //int targetSideIndex = ballTargetSideIndices[i];
                //Color targetColor = GetColorForSideIndex(targetSideIndex, i);

                // string colorId = colorIdToSideColorIndexMap.FirstOrDefault(kv => kv.Value == sideColors.IndexOf(targetColor)).Key;

                // // 找到该颜色对应的球精灵（左或右）
                // List<int> availableSprites = new List<int>();
                // for (int j = 0; j < ballSprites.Count; j++)
                // {
                //     if (GetColorIdentifierFromSprite(ballSprites[j]) == colorId)
                //     {
                //         availableSprites.Add(j);
                //     }
                // }

                // if (availableSprites.Count > 0)
                // {
                int spriteIndex = Random.Range(0, ballSprites.Count);
                Debug.Log("before:" + (int)((float)(spriteIndex) / 2));

                if (debugMode)
                {
                    Debug.Break();
                }
                int cnt = 0;
                if (useSprites != null)
                {
                    cnt = 0;
                    while ((useSprites.IndexOf(ballSprites[spriteIndex]) == -1) && (usedColorIndex.IndexOf((int)((float)(spriteIndex) / 2)) != -1))
                    {
                        cnt++;
                        if (cnt > 100)
                        {
                            Debug.Log("useColors卡了");
                            break;
                        }
                        spriteIndex = Random.Range(0, ballSprites.Count);
                    }
                }
                else
                {
                    while (usedColorIndex.IndexOf((int)((float)(spriteIndex) / 2)) != -1)
                    {
                        cnt++;
                        if (cnt > 100)
                        {
                            Debug.Log("Assign ball colors卡了");
                            break;
                        }
                        spriteIndex = Random.Range(0, ballSprites.Count);
                    }
                }
                Debug.Log("after:" + (int)((float)(spriteIndex) / 2));
                usedColorIndex.Add((int)((float)(spriteIndex) / 2));
                SetupBall(balls[i], spriteIndex, i);
                // }
            }

            // 设置最后一个球（通关球）
            //SetupFinalBall(usedColorIndex);
        }

        Color GetColorForSideIndex(int sideIndex, int ballIndex)
        {
            // 确保每个球对应的颜色不重复
            List<int> usedColors = new List<int>();
            for (int i = 0; i < ballIndex; i++)
            {
                int usedSideIndex = ballTargetSideIndices[i];
                usedColors.Add(sideColors.IndexOf(sides[usedSideIndex].color));
            }

            // 随机选择一个未使用的颜色
            List<int> availableColors = new List<int>();
            for (int i = 0; i < sideColors.Count - 1; i++) // 排除白色
            {
                if (!usedColors.Contains(i))
                {
                    availableColors.Add(i);
                }
            }

            int randomColorIndex = availableColors[Random.Range(0, availableColors.Count)];
            return sideColors[randomColorIndex];
        }
        public void GenerateSides(List<Color> useColors = null)
        {
            if (useColors == null)
            {
                useColors = sideColors;
            }
            else
            {
                AssignBallColors(GetUsedBalls());
                currentActiveBallIndex = new System.Random().Next(0, balls.Count);
            }
            int prevSide, nextEmptySide, nowBallIndex;
            nowBallIndex = nextEmptySide = currentActiveBallIndex;
            foreach (Image i in sides)
            {
                i.color = Color.white;
            }
            for (int i = 0; i < sides.Count; i++)
            {
                int ballSpriteIndex = ballSprites.IndexOf(Extensions.GetChild(balls[nowBallIndex].gameObject)[0].GetComponent<Image>().sprite);
                int turnSide = (ballSpriteIndex + 1) % 2;
                Debug.Log(turnSide);
                if (turnSide == 0)
                {
                    prevSide = (nowBallIndex - 1 + sides.Count) % sides.Count;
                }
                else
                {
                    prevSide = (nowBallIndex + 1) % sides.Count;
                }
                if (sides[prevSide].color != Color.white || prevSide == currentActiveBallIndex)
                {
                    nextEmptySide = ++nowBallIndex;
                    if (nextEmptySide > balls.Count - 1)
                    {
                        nextEmptySide = nowBallIndex = prevSide;
                    }
                    continue;
                }
                Debug.Log("prevSide:" + prevSide);
                int ballColorIndex = (int)(((float)ballSpriteIndex) / 2);
                Debug.Log("ballColorIndex:" + ballColorIndex);
                sides[prevSide].color = useColors[ballColorIndex];
                nextEmptySide = nowBallIndex = prevSide;
                SetSideClickEvent(sides[i]);
            }
        }

        void SetupBall(Mask ball, int spriteIndex, int ballIndex)
        {
            Image ballImage = ball.transform.GetChild(0).GetComponent<Image>();
            ballImage.sprite = ballSprites[spriteIndex];

            // 设置点击事件
            EventTrigger trigger = ball.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = ball.gameObject.AddComponent<EventTrigger>();
            }

            // 清除旧事件
            trigger.triggers.Clear();

            // 添加新事件
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => OnBallClick(ball, ballIndex));
            trigger.triggers.Add(entry);

            // 根据球的索引决定是否启用
            if (ballIndex == currentActiveBallIndex)
            {
                EnableBall(ball);
            }
            else
            {
                DisableBall(ball);
            }
        }
        void SetSideClickEvent(Image side)
        {
            EventTrigger trigger = side.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = side.gameObject.AddComponent<EventTrigger>();
            }
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => OnSideClick(side));
            trigger.triggers.Clear();
            trigger.triggers.Add(entry);
        }
        private void OnSideClick(Image side)
        {
            if (levelMonitor.mode != LevelMonitor.Mode.whitenMode)
            {
                return;
            }
            side.color = Color.white;
            levelMonitor.mode = LevelMonitor.Mode.normalMode;
            levelMonitor.assistantTool.ShowCancel(false);
        }

        void SetupFinalBall(List<int> usedColorIndex)
        {
            Mask finalBall = balls[balls.Count - 1];

            // 设置随机颜色
            int randomColorIndex = Random.Range(0, ballSprites.Count);
            Debug.Log("before:" + (int)((float)(randomColorIndex) / 2));
            int cnt = 0;
            while (usedColorIndex.IndexOf((int)((float)(randomColorIndex) / 2)) != -1)
            {
                cnt++;
                if (cnt > 100)
                {
                    Debug.Log("Setup Final Ball卡了");
                    break;
                }
                randomColorIndex = Random.Range(0, ballSprites.Count);
            }
            Debug.Log("after:" + (int)((float)(randomColorIndex) / 2));
            Sprite ballSprite = ballSprites[randomColorIndex];
            // string colorId = colorIdToSideColorIndexMap.FirstOrDefault(kv => kv.Value == randomColorIndex).Key;

            // // 找到该颜色对应的球精灵
            // List<int> availableSprites = new List<int>();
            // for (int j = 0; j < ballSprites.Count; j++)
            // {
            //     if (GetColorIdentifierFromSprite(ballSprites[j]) == colorId)
            //     {
            //         availableSprites.Add(j);
            //     }
            // }

            if (true)
            {
                Image ballImage = finalBall.transform.GetChild(0).GetComponent<Image>();
                ballImage.sprite = ballSprite;

                // 初始禁用通关球
                DisableBall(finalBall);

                // 设置点击事件（直接通关）
                EventTrigger trigger = finalBall.GetComponent<EventTrigger>();
                if (trigger == null)
                {
                    trigger = finalBall.gameObject.AddComponent<EventTrigger>();
                }

                trigger.triggers.Clear();

                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((data) => CompleteGame());
                trigger.triggers.Add(entry);
            }
        }
        int sideToTurn;
        void OnBallClick(Mask ball, int ballIndex)
        {
            int targetSideIndex;
            if (isGameCompleted) return;
            if (levelMonitor != null)
            {
                if (levelMonitor.mode == LevelMonitor.Mode.forceMode)
                {
                    levelMonitor.mode = LevelMonitor.Mode.normalMode;
                    levelMonitor.assistantTool.ShowCancel(false);
                }
            }
            int ballSpriteIndex = ballSprites.IndexOf(Extensions.GetChild(balls[ballIndex].gameObject)[0].GetComponent<Image>().sprite);
            int ballColor = (int)(((float)ballSpriteIndex) / 2);
            Debug.Log("球颜色为" + ballColor);
            int turnSide = (ballSpriteIndex + 1) % 2;
            int ind1 = 0;
            sideToTurn = -1;
            foreach (Image side in sides)
            {
                if (sideColors.IndexOf(side.color) == ballColor)
                {
                    sideToTurn = ind1;
                    break;
                }
                ind1++;
            }
            Debug.Log(turnSide);
            // 隐藏当前球
            //ball.gameObject.SetActive(false);
            ball.GetComponent<BallMoveManager>().StartMoving(ballIndex, sides.Count, isSideAtTop);
            CheckIfEndedUp();
            bool checkResult;
            if (sideToTurn < 0)
            {
                Debug.Log("没有此颜色的边。");
                checkResult = CheckBallIsAllDisabled();
                Debug.Log("[" + name + "]isBallAllDisabled:" + checkResult);
                if (checkResult && levelMonitor != null)
                {
                    levelMonitor.NotificationBallAllEnded(this);
                }
                return;
            }
            if (turnSide == 1)
            {
                targetSideIndex = (sideToTurn - 1 + sides.Count) % sides.Count;
            }
            else
            {
                targetSideIndex = (sideToTurn + 1) % sides.Count;
            }

            // 检查是否是当前可点击的球
            if (sides[ballIndex].color != Color.white)
            {
                Debug.Log("请点击当前白边对应的球！");
                return;
            }

            // 执行转边操作
            Debug.Log("边位置：" + sideToTurn + "转到：" + targetSideIndex);
            RotateSide(sideToTurn, targetSideIndex);
            SyncConnectedSide(sideToTurn);

            // 更新白边位置
            currentWhiteSideIndex = targetSideIndex;

            // 更新当前可点击球的索引
            currentActiveBallIndex++;
            CheckBallEnabled();
            // 检查游戏是否完成
            if (currentActiveBallIndex >= balls.Count - 1)
            {
                //EnableFinalBall();
            }
            else
            {
                // 启用下一个球
                //EnableBallByIndex(currentActiveBallIndex);
            }
            checkResult = CheckBallIsAllDisabled();
            Debug.Log("[" + name + "]isBallAllDisabled:" + checkResult);
            if (checkResult && levelMonitor != null)
            {
                levelMonitor.NotificationBallAllEnded(this);
            }
        }
        public void CheckBallEnabled()
        {
            for (int ind = 0; ind < sides.Count; ind++)
            {
                if (Extensions.IsWhite(sides[ind].color))
                {
                    EnableBallByIndex(ind);
                    Debug.Log(ind + "已启用");
                }
                else
                {
                    DisableBallByIndex(ind);
                    Debug.Log(ind + "已禁用");
                }
            }
        }
        void CheckIfEndedUp()
        {
            int endedUpBalls = 0;
            foreach (Mask i in balls)
            {
                if (i.GetComponent<BallMoveManager>().isEndedUp)
                {
                    endedUpBalls++;
                }
            }
            if (endedUpBalls == balls.Count)
            {
                Debug.Log("球已全部结束");
                StartResultProcess();
            }
        }
        void StartResultProcess()
        {
            if (_allFinishManager.HasObject())
            {
                if (!_allFinishManager.RegisterEndedShape(this))
                {
                    return;
                }
            }
            if (this.transform.parent.GetComponent<GameMonitor>())
            {
                this.transform.parent.GetComponent<GameMonitor>().StartResultProcess();
            }
            else
            {
                if (isSub && (gameProcess.existingGameMonitor != null))
                {
                    Debug.Log("if entered");
                    gameProcess.existingGameMonitor.StartResultProcess();
                    return;
                }
                GameObject resultPrefab = (GameObject)Resources.Load("prefabs/process/result/resultProcess");
                GameObject resultGO = Instantiate(resultPrefab);
                resultGO.transform.parent = this.transform.parent;
            }
        }
        void RotateSide(int sideTT, int targetSideIndex)
        {
            if (!Extensions.IsWhite(sides[targetSideIndex].color))
            {
                return;//不是白的转个水坝
            }
            // 获取当前白边颜色和目标边颜色
            Color whiteColor = Color.white;
            Color targetColor = sides[sideTT].color;

            // 交换颜色（目标边变为白色，白边变为目标颜色）
            sides[sideTT].color = whiteColor;
            sides[targetSideIndex].color = targetColor;

            Debug.Log($"转边: {sideTT} → {targetSideIndex}");
        }

        void EnableBallByIndex(int ballIndex)
        {
            // if (ballIndex < balls.Count - 1)
            // {
            EnableBall(balls[ballIndex]);
            // }
        }

        void EnableBall(Mask ball)
        {
            // 启用球的交互
            ball.GetComponent<Image>().raycastTarget = true;
            Image ballImage = ball.transform.GetChild(0).GetComponent<Image>();
            ballImage.raycastTarget = true;

            // 可以添加视觉提示（如高亮）
            ballImage.color = Color.white;
        }
        void DisableBallByIndex(int ballIndex)
        {
            DisableBall(balls[ballIndex]);
        }

        void DisableBall(Mask ball)
        {
            // 禁用球的交互
            ball.GetComponent<Image>().raycastTarget = false;
            Image ballImage = ball.transform.GetChild(0).GetComponent<Image>();
            ballImage.raycastTarget = false;

            // 可以添加视觉提示（如变暗）
            ballImage.color = new Color(1f, 1f, 1f, 0.5f);
        }

        void EnableFinalBall()
        {
            Mask finalBall = balls[balls.Count - 1];
            finalBall.gameObject.SetActive(true);
            EnableBall(finalBall);
        }

        void CompleteGame()
        {
            Debug.Log("游戏通关！");
            isGameCompleted = true;

            // TODO: 显示通关UI
        }

        string GetColorIdentifierFromSprite(Sprite sprite)
        {
            return sprite.name.Split('_')[0];
        }
        public List<Color> GetColors()
        {
            List<Color> ret = new List<Color>();
            foreach (Image side in sides)
            {
                ret.Add(side.color);
            }
            return ret;
        }
        public void SetBallAllDisabled(bool is_)
        {
            if (is_)
            {
                foreach (Mask i in balls)
                {
                    DisableBall(i);
                }
            }
            else
            {
                CheckBallEnabled();
            }
        }
        public void SetBallAllEnabled(bool is_)
        {
            if (is_)
            {
                foreach (Mask i in balls)
                {
                    EnableBall(i);
                }
            }
            else
            {
                CheckBallEnabled();
            }
        }
        public List<Sprite> GetUsedBalls()
        {
            List<Sprite> ballsp = new List<Sprite>();
            foreach (Mask i in balls)
            {
                ballsp.Add(i.gameObject.GetChild()[0].GetComponent<Image>().sprite);
            }
            return ballsp;
        }
        public void ResetSides(List<Color> colors)
        {
            List<Image> setImage = new List<Image>();
            foreach (Color i in colors)
            {
                Image j = GetUnsetImage(setImage);
                j.color = i;
                setImage.Add(j);
            }
        }
        Image GetUnsetImage(List<Image> setImage)
        {
            System.Random random = new System.Random();
            Image img = sides[random.Next(0, sides.Count)];
            while (setImage.IndexOf(img) != -1)
            {
                img = sides[random.Next(0, sides.Count)];
            }
            return img;
        }
    }
}