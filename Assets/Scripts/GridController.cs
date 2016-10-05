using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Random = UnityEngine.Random;

namespace MatchThree
{
    public class GridController : MonoBehaviour
    {
        public class GemMatch
        {
            public List<GameObject> MatchList = new List<GameObject>();
            public MatchDirection Direction;
        }

        public enum MatchDirection
        {
            Horizontal,
            Vertical
        }

        private IAnimationToken animToken;
        private int Width;
        private int Height;
        private Dictionary<GemColors, GameObject> Gems = new Dictionary<GemColors, GameObject>();
        private Vector2 GemSize = new Vector2(100f, 100f);
        private RectTransform GemsRoot;
        private bool IsSelected;
        private Gem SelectedGem;
        private Gem SwapTargetGem;
        public RectTransform GemSelection;
        private bool allowInput;
        [SerializeField]
        private List<Gem>[] GemArray;
        private List<GemMatch> ActiveMatches;

        private bool IsChainAtLeft(int x, int y, GemColors gemColor)
        {
            if (x <= 1) return false;
            var gemsToLeft = (from Gem gem in GemsRoot.gameObject.GetComponentsInChildren<Gem>() where gem.Y == y select gem).ToList();
            int matches = 0;
            foreach (var gem in gemsToLeft)
            {
                if (gem.GemColor == gemColor)
                {
                    matches++;
                }
                else
                {
                    matches = 0;
                }
                if (matches == 2) return true;
            }
            return false;
        }

        private bool IsChainAtTop(int x, int y, GemColors gemColor)
        {
            if (y <= 1) return false;
            var gemsToTop = (from Gem gem in GemsRoot.gameObject.GetComponentsInChildren<Gem>() where gem.X == x select gem).ToList();
            int matches = 0;
            foreach (var gem in gemsToTop)
            {
                if (gem.GemColor == gemColor)
                {
                    matches++;
                }
                else
                {
                    matches = 0;
                }
                if (matches == 2) return true;
            }
            return false;
        }

        private bool IsNeighbourGem(Gem selectedGem, Gem targetGem)
        {
            int selIndex =
                GemArray[selectedGem.GetComponent<Gem>().X].FindIndex(x => x == selectedGem.GetComponent<Gem>());
            int targetIndex =
                GemArray[targetGem.GetComponent<Gem>().X].FindIndex(x => x == targetGem.GetComponent<Gem>());

            if (selIndex == targetIndex && (selectedGem.X + 1 == targetGem.X || selectedGem.X - 1 == targetGem.X))
            {
                return true;
            }

            if (selectedGem.X == targetGem.X && (selIndex + 1 == targetIndex || selIndex - 1 == targetIndex))
            {
                return true;
            }

            return false;
        }

        void EventManager_OnGemClickEvent(Gem gem)
        {
            if(!allowInput) return;
            UpdateGemArray();
            if (!IsSelected)
            {
                IsSelected = true;
                SelectedGem = gem;
                GemSelection.gameObject.SetActive(true);
                GemSelection.localPosition = gem.gameObject.transform.localPosition;
            }
            else
            {
                if (SelectedGem != gem)
                {
                    if (!IsNeighbourGem(SelectedGem, gem)) return;
                    SwapTargetGem = gem;
                    SwapGems(SelectedGem.gameObject, SwapTargetGem.gameObject, true, false);
                }
                else
                {
                    IsSelected = false;
                    SelectedGem = null;
                    SwapTargetGem = null;
                    GemSelection.gameObject.SetActive(false);
                }
            }
        }

        private IEnumerator DestroyMatches()
        {
            IsSelected = false;
            SelectedGem = null;
            SwapTargetGem = null;
            GemSelection.gameObject.SetActive(false);
            foreach (var match in ActiveMatches)
            {
                switch (match.Direction)
                {
                    case MatchDirection.Horizontal:
                        EventManager.CallGemMatchEvent(match.MatchList.Count, MatchDirection.Horizontal);
                        break;
                    case MatchDirection.Vertical:
                        EventManager.CallGemMatchEvent(match.MatchList.Count, MatchDirection.Vertical);
                        break;
                }
                foreach (var gem in match.MatchList)
                {
                    Destroy(gem);
                }
            }
            ActiveMatches = null;
            yield return new WaitForEndOfFrame();
            UpdateGemArray();
            PushGemsDown();
        }

    private void UpdateGemArray()
        {
            for (int i = 0; i < GemArray.Length; i++)
            {
                List<Gem> colums = new List<Gem>();
                for (int j = 0; j < GemArray[i].Count; j++)
                {
                    if (GemArray[i][j] != null)
                    {
                        colums.Add(GemArray[i][j]);
                    }
                }
                GemArray[i] = colums;
            }
        }

        private void PushGemsDown()
        {
            animToken = AnimationHelper.Animate(Time.time, 0.4f, (t) =>
            {
                for (int i = 0; i < GemArray.Length; i++)
                {
                    int counter = Height;
                    for (int j = GemArray[i].Count - 1; j >= 0; j--)
                    {
                        GemArray[i][j].gameObject.transform.localPosition =
                            Vector3.Lerp(GemArray[i][j].gameObject.transform.localPosition,
                                new Vector3(GemArray[i][j].gameObject.transform.localPosition.x,
                                    -counter*GemSize.y + GemSize.y*0.5f, 0f), EasingFunctions.easeInOut(t));
                        counter--;
                    }
                }

            },
                () =>
                {
                    for (int i = 0; i < Width; i++)
                    {
                        for (int j = 0; j < Height; j++)
                        {
                            if (GemArray[i].Count <= j) GemArray[i].Insert(0, GenerateGem(i, Height - j));
                        }
                    }
                    if (!GetMatches())
                    {
                        if (EnsureStepsAvailable())
                        {
                            allowInput = true;
                            Debug.Log("Steps available");
                        }
                        else
                        {
                            StartCoroutine(ShuffleGems());
                            Debug.Log("No steps available!");
                        }
                    }
                    else
                    {
                        StartCoroutine(DestroyMatches());
                    }
                });
        }

        private bool EnsureStepsAvailable()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (i > 0 && j > 0)
                    {
                        bool horizontal = SwapGems(GemArray[i][j].gameObject,
                            GemArray[i - 1][j].gameObject, true,
                            true);
                        if (horizontal) return true;
                        bool vertical = SwapGems(GemArray[i][j].gameObject,
                            GemArray[i][j - 1].gameObject, true,
                            true);
                        if (vertical) return true;
                    }
                }
            }
            return false;
        }

        private bool SwapGems(GameObject a, GameObject b, bool checkMatches, bool checkSteps, Action onDone = null)
        {
            if(!checkSteps && checkMatches) EventManager.CallGameEvent(EventManager.GameEvents.GemSwap);

            Vector3 posA = a.transform.localPosition;
            Vector3 posB = b.transform.localPosition;

            int aIndex = GemArray[a.GetComponent<Gem>().X].FindIndex(x => x == a.GetComponent<Gem>());
            int bIndex = GemArray[b.GetComponent<Gem>().X].FindIndex(x => x == b.GetComponent<Gem>());

            var tempa = GemArray[a.GetComponent<Gem>().X][aIndex];

            GemArray[a.GetComponent<Gem>().X][aIndex] = b.GetComponent<Gem>();
            GemArray[b.GetComponent<Gem>().X][bIndex] = tempa;

            int ax = a.GetComponent<Gem>().X;
            a.GetComponent<Gem>().X = b.GetComponent<Gem>().X;
            b.GetComponent<Gem>().X = ax;

            if (!checkSteps)
            {
                allowInput = false;
                animToken=AnimationHelper.Animate(Time.time, 0.4f, (t) =>
                    {
                        a.gameObject.transform.localPosition = Vector3.Lerp(a.gameObject.transform.localPosition, posB,
                                                                            EasingFunctions.easeInOut(t));
                        b.gameObject.transform.localPosition = Vector3.Lerp(b.gameObject.transform.localPosition, posA,
                                                                            EasingFunctions.easeInOut(t));
                    },
                                        () =>
                                            {
                                                if (checkMatches)
                                                {
                                                    if (GetMatches())
                                                    {
                                                        StartCoroutine(DestroyMatches());
                                                    }
                                                    else
                                                    {
                                                        SwapGems(b, a, false, false, () => { allowInput = true; });
                                                    }
                                                }
                                                if (onDone != null) onDone();
                                            });
            }
            else
            {
                bool matchesFound;
                if (checkMatches)
                {
                    matchesFound = GetMatches();
                }
                else
                {
                    return false;
                }
                SwapGems(b, a, false, true);
                return matchesFound;
            }
            return false;
        }

        private bool CheckHorizontalMatch()
        {
            ActiveMatches = new List<GemMatch>();
            for (int y = 0; y < Height; y++)
            {
                GemMatch newMatch = null;
                for (int x = 1; x < Width; x++)
                {
                    if (GemArray[x - 1][y].GemColor == GemArray[x][y].GemColor)
                    {
                        if (newMatch == null)
                        {
                            newMatch = new GemMatch();
                            newMatch.Direction = MatchDirection.Horizontal;
                            newMatch.MatchList.Add(GemArray[x - 1][y].gameObject);
                            newMatch.MatchList.Add(GemArray[x][y].gameObject);
                        }
                        else
                        {
                            newMatch.MatchList.Add(GemArray[x][y].gameObject);
                        }
                    }
                    else
                    {
                        if (newMatch != null && newMatch.MatchList.Count > 2)
                        {
                            ActiveMatches.Add(newMatch);
                        }
                        newMatch = null;
                    }
                    if (x == Width - 1)
                    {
                        if (newMatch != null && newMatch.MatchList.Count > 2)
                        {
                            ActiveMatches.Add(newMatch);
                        }
                    }
                }
            }
            if (ActiveMatches.Count > 0)
            {
                return true;
            }
            return false;
        }

        private bool CheckVerticalMatch()
        {
            ActiveMatches = new List<GemMatch>();
            for (int x = 0; x < Width; x++)
            {
                GemMatch newMatch = null;
                for (int y = 1; y < Height; y++)
                {
                    if (GemArray[x][y - 1].GemColor == GemArray[x][y].GemColor)
                    {
                        if (newMatch == null)
                        {
                            newMatch = new GemMatch();
                            newMatch.Direction = MatchDirection.Vertical;
                            newMatch.MatchList.Add(GemArray[x][y - 1].gameObject);
                            newMatch.MatchList.Add(GemArray[x][y].gameObject);
                        }
                        else
                        {
                            newMatch.MatchList.Add(GemArray[x][y].gameObject);
                        }
                    }
                    else
                    {
                        if (newMatch != null && newMatch.MatchList.Count > 2)
                        {
                            ActiveMatches.Add(newMatch);
                        }
                        newMatch = null;
                    }
                    if (y == Height - 1)
                    {
                        if (newMatch != null && newMatch.MatchList.Count > 2)
                        {
                            ActiveMatches.Add(newMatch);
                        }
                    }
                }
            }
            if (ActiveMatches.Count > 0)
            {
                return true;
            }
            return false;
        }

        private bool GetMatches()
        {
            return CheckHorizontalMatch() || CheckVerticalMatch();
        }

        private Gem GenerateGem(int x, int y)
        {
            GemColors newColor = (GemColors)Random.Range(0, Gems.Count);
            GameObject gemGO = Instantiate(Gems[newColor]) as GameObject;
            gemGO.transform.SetParent(GemsRoot);
            gemGO.transform.localPosition = new Vector3(GemSize.x * (x + 1) - GemSize.x * 0.5f, -y*GemSize.y + GemSize.y*0.5f, 0f);
            var gem = gemGO.AddComponent<Gem>();
            gem.GemColor = newColor;
            gem.X = x;
            gem.Y = y;
            return gem;
        }

        public void Init(GameSettings settings, RectTransform gemRoot)
        {
            EventManager.OnGemClickEvent += EventManager_OnGemClickEvent;
            EventManager.OnGameEvent += EventManager_OnGameEvent;
            Width = settings.GridWidth;
            Height = settings.GridHeight;
            GemsRoot = gemRoot;
            GemSelection.transform.SetParent(gemRoot);
            GemSelection.gameObject.SetActive(false);
            foreach (var gemData in settings.Gems)
            {
                Gems.Add(gemData.GemColor, gemData.GemPrefab);
            }
        }

        private void EventManager_OnGameEvent(EventManager.GameEvents eventID)
        {
            switch (eventID)
            {
                    case EventManager.GameEvents.GameStart:
                    StartCoroutine(ShuffleGems());
                    break;
            }
        }

        void OnDestroy()
        {
            EventManager.OnGemClickEvent -= EventManager_OnGemClickEvent;
            EventManager.OnGameEvent -= EventManager_OnGameEvent;
        }

        private IEnumerator ShuffleGems()
        {
            ClearGrid();
            yield return null;
            BuildGemGrid();
        }

        private void ClearGrid()
        {
            animToken.SafeCancel();
            allowInput = false;
            IsSelected = false;
            SelectedGem = null;
            SwapTargetGem = null;
            ActiveMatches = null;
            GemArray = null;
            foreach (var gem in GemsRoot.GetComponentsInChildren<Gem>())
            {
                Destroy(gem.gameObject);
            }
        }

        void BuildGemGrid()
        {
            GemArray = new List<Gem>[Width];
            int num = 0;
            for (int x = 0; x < Width; x++)
            {
                GemArray[x] = new List<Gem>();
                for (int y = 0; y < Height; y++)
                {
                    bool colorChosen = false;
                    GemColors newColor = (GemColors) Random.Range(0, Gems.Count);
                    while (!colorChosen)
                    {
                        if (!IsChainAtLeft(x, y, newColor) && !IsChainAtTop(x, y, newColor))
                        {
                            colorChosen = true;
                        }
                        else
                        {
                            newColor = (GemColors) Random.Range(0, Gems.Count);
                        }
                    }
                    GameObject gemGO = Instantiate(Gems[newColor]);
                    gemGO.name = String.Concat(newColor.ToString(),num);
                    gemGO.transform.SetParent(GemsRoot);
                    gemGO.transform.localPosition = new Vector3(GemSize.x*(x + 1) - GemSize.x*0.5f,
                        -GemSize.y*(y + 1) + GemSize.y*0.5f, 0f);
                    var gem = gemGO.AddComponent<Gem>();
                    gem.X = x;
                    gem.Y = y;
                    gem.GemColor = newColor;
                    GemArray[x].Add(gem);
                    num++;
                }
            }
            if (!EnsureStepsAvailable()) StartCoroutine(ShuffleGems());
            allowInput = true;
        }
    }
}