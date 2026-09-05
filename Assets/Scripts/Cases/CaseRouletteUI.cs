using System;
using System.Collections;
using System.Collections.Generic;
using Cases.Drop;
using UnityEngine;
using UnityEngine.UI;

namespace Cases
{
    public class CaseRouletteUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform viewport;
        [SerializeField] private RectTransform content;
        [SerializeField] private CaseItemView itemPrefab;

        [Header("Roulette")]
        [SerializeField, Min(20)] private int itemCount = 50;
        [SerializeField, Min(5)] private int winnerIndex = 40;
        [SerializeField, Min(0.1f)] private float spinDuration = 5f;

        [Header("Animation")]
        [SerializeField]
        private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private readonly List<CaseItemView> spawnedItems = new();

        public bool IsSpinning { get; private set; }

        public void Spin(IReadOnlyList<CaseDropData> availableDrops, CaseDropData winner, Action onFinished = null)
        {
            if (IsSpinning)
                return;

            if (availableDrops == null || availableDrops.Count == 0)
            {
                Debug.LogError("No drops provided.");
                return;
            }

            StartCoroutine(SpinRoutine(availableDrops, winner, onFinished));
        }

        private IEnumerator SpinRoutine(IReadOnlyList<CaseDropData> availableDrops, CaseDropData winner, Action onFinished)
        {
            IsSpinning = true;

            Clear();

            // Ждём кадр, чтобы Unity полностью удалила предыдущую ленту.
            yield return null;

            content.anchoredPosition = Vector2.zero;

            int actualWinnerIndex = Mathf.Clamp(
                winnerIndex,
                1,
                itemCount - 2
            );

            for (int i = 0; i < itemCount; i++)
            {
                CaseDropData data;

                if (i == actualWinnerIndex)
                {
                    data = winner;
                }
                else
                {
                    data = availableDrops[
                        UnityEngine.Random.Range(0, availableDrops.Count)
                    ];
                }

                CaseItemView view = Instantiate(itemPrefab, content);
                view.Setup(data);
                spawnedItems.Add(view);
            }

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            Canvas.ForceUpdateCanvases();

            RectTransform winnerRect =
                spawnedItems[actualWinnerIndex].GetComponent<RectTransform>();

            Vector3 winnerWorldCenter =
                winnerRect.TransformPoint(winnerRect.rect.center);

            Vector3 winnerLocalInViewport =
                viewport.InverseTransformPoint(winnerWorldCenter);

            Vector3 viewportLocalCenter = viewport.rect.center;

            float deltaX =
                winnerLocalInViewport.x - viewportLocalCenter.x;

            Vector2 startPosition = content.anchoredPosition;

            Vector2 targetPosition = startPosition;

            // Победитель остановится РОВНО по центру.
            targetPosition.x -= deltaX;

            float elapsed = 0f;

            while (elapsed < spinDuration)
            {
                elapsed += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(elapsed / spinDuration);
                float curvedT = movementCurve.Evaluate(t);

                content.anchoredPosition =
                    Vector2.LerpUnclamped(
                        startPosition,
                        targetPosition,
                        curvedT
                    );

                yield return null;
            }

            // В конце принудительно ставим точную позицию.
            content.anchoredPosition = targetPosition;

            IsSpinning = false;
            onFinished?.Invoke();
            
            Clear();
        }

        public void Clear()
        {
            // Сразу убираем старые элементы из LayoutGroup.
            foreach (CaseItemView item in spawnedItems)
            {
                if (item == null)
                    continue;

                item.gameObject.SetActive(false);
                Destroy(item.gameObject);
            }

            spawnedItems.Clear();

            // Полностью возвращаем Content в начальное положение.
            content.anchoredPosition = Vector2.zero;

            // Принудительно обновляем Layout.
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            Canvas.ForceUpdateCanvases();
        }
    }
}