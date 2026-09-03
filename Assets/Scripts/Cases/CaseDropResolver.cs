using System;
using System.Collections.Generic;
using Cases.Drop;
using UnityEngine;

namespace Cases
{
    public class CaseDropResolver : MonoBehaviour
    {
        [SerializeField] private List<CaseDropData> drops = new();

        public IReadOnlyList<CaseDropData> Drops => drops;

        public CaseDropData GetRandomDrop()
        {
            if (drops == null || drops.Count == 0)
                throw new InvalidOperationException("Case has no drops.");

            float totalWeight = 0f;

            foreach (CaseDropData drop in drops)
            {
                if (drop != null && drop.itemChance > 0f)
                    totalWeight += drop.itemChance;
            }

            if (totalWeight <= 0f)
                throw new InvalidOperationException("Total drop chance must be greater than 0.");

            float random = UnityEngine.Random.Range(0f, totalWeight);
            float current = 0f;

            foreach (CaseDropData drop in drops)
            {
                if (drop == null || drop.itemChance <= 0f)
                    continue;

                current += drop.itemChance;

                if (random <= current)
                    return drop;
            }

            return drops[drops.Count - 1];
        }

        // Используется UI для заполнения визуальной ленты.
        // На результат открытия это НЕ влияет.
        public CaseDropData GetRandomVisualItem()
        {
            return GetRandomDrop();
        }
    }
}