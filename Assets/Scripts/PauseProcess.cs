using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Monitor.Game;
using Monitor;

namespace Process
{
    public class PauseProcess : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void PauseGame(GameMonitor game)
        {
            GameObject pause_prefab = (GameObject)Resources.Load("Prefabs/process/pause/pauseProcess");
            GameObject instance = Instantiate(pause_prefab);
            instance.transform.parent = game.transform.parent;
            instance.GetComponent<PauseMonitor>().SetGame(game);
            foreach (CustomScrollableArea i in game.GetComponentsInChildren<CustomScrollableArea>())
            {
                i.isEnabled = false;
            }
        }
    }
}