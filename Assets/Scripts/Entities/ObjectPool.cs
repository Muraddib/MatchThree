using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MatchThree
{
    public class ObjectPool : Singleton<ObjectPool>
    {
        public List<GameObject> GemPool;
        private int maxCountPerColor;

        private void SetGemPool(GameSettings settings)
        {
            for (int i = 0; i < settings.Gems.Length; i++)
            {
                for (int j = 0; j < maxCountPerColor; j++)
                {
                    GameObject go = Instantiate(settings.Gems[i].GemPrefab);
                    var gem = go.GetComponent<Gem>();
                    gem.IsActive = false;
                    switch (gem.GemColor)
                    {

                    }
                    GemPool.Add(go);
                }
            }
        }

        public void Init(GameSettings settings)
        {
            GemPool = new List<GameObject>();
            //maxCountPerColor = GameplayController.Instance.Settings.MaxCountPerColor;
            SetGemPool(settings);
        }

        public GameObject GetGem(GemColors gemRequestType)
        {
            if (
                GemPool.FindAll(a => a.GetComponent<Gem>().GemColor == gemRequestType && a.GetComponent<Gem>().IsActive)
                       .Count >= maxCountPerColor)
            {
                return null;
            }

            GameObject gem =
                GemPool.First(
                    a => !a.GetComponent<Gem>().IsActive && a.GetComponent<Gem>().GemColor == gemRequestType);
            gem.GetComponent<Gem>().IsActive = true;
            return gem;
        }
    }
}