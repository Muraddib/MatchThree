using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace MatchThree
{
    public class FlowController : MonoBehaviour
    {
        public GameUIFormsKeeper UIFormsKeeper;
        public GameSettingsKeeper SettingsKeeper;
        public AudioBank AudioKeeper;
        public RectTransform UIRoot;
        public RectTransform GridRoot;
        public GameObject GridControllerPrefab;
        public RectTransform GemCanvas;
        public GameObject BackgroundTilePrefab;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            GameplayController.Instance.Init(SettingsKeeper.GameSettings);
            UIController.Instance.Init(UIFormsKeeper.UIForms, UIRoot);
            AudioController.Instance.Init(AudioKeeper);
            EventManager.CallGameEvent(EventManager.GameEvents.GameLoaded);
            InitGemGrid();
            SetGemCanvas();
        }

        private void SetGemCanvas()
        {
            int width = SettingsKeeper.GameSettings.GridWidth;
            int height = SettingsKeeper.GameSettings.GridHeight;
            GemCanvas.sizeDelta = new Vector2(width * 100f, height * 100f);
            for (int i = 0; i < width*height; i++)
            {
                var bTile = Instantiate(BackgroundTilePrefab);
                bTile.transform.SetParent(GemCanvas.GetChild(0));
            }
        }

        private void InitGemGrid()
        {
            var grid = Instantiate(GridControllerPrefab);
            grid.GetComponent<GridController>().Init(SettingsKeeper.GameSettings, GridRoot);
        }

    }
}
