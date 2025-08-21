using UnityEngine;
using Monitor.Game;

namespace Process
{
    public class GameProcess : MonoBehaviour
    {
        private static GameMonitor _existingGameMonitor;
        public GameMonitor existingGameMonitor
        {
            get
            {
                // Debug.Log("got egm,isnull:" + _existingGameMonitor == null);
                return _existingGameMonitor;
            }
            set
            {
                // Debug.Log("registered egm");
                _existingGameMonitor = value;
            }
        }
        public GameMonitor InitPrefab()
        {
            GameObject gamepre = (GameObject)Resources.Load("Prefabs/process/game/gameProcess");
            GameObject instance = Instantiate(gamepre);
            existingGameMonitor = instance.GetComponent<GameMonitor>();
            return instance.GetComponent<GameMonitor>();
        }
    }
}