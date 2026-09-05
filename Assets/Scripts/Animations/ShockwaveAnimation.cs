using UnityEngine;
using System.Collections;

namespace LoxaRPG.Animations
{
    /// <summary>
    /// Анимация ударной волны.
    /// Расширяется и затухает. Не зависит от босса, работает автономно.
    /// </summary>
    public class ShockwaveAnimation : MonoBehaviour
    {
        private float _maxScale;
        private float _duration;
        private bool _isAnimating;

        /// <summary>
        /// Задать параметры волны.
        /// </summary>
        public void Initialize(float maxSize, float animDuration)
        {
            _maxScale = maxSize;
            _duration = animDuration;
        }

        /// <summary>
        /// Запустить анимацию. Повторный вызов игнорируется.
        /// </summary>
        public void StartAnimation()
        {
            if (!_isAnimating)
            {
                StartCoroutine(Animate());
            }
        }

        private IEnumerator Animate()
        {
            _isAnimating = true;
            float timer = 0;

            var startScale = transform.localScale;
            var endScale = Vector3.one * _maxScale;

            while (timer < _duration)
            {
                timer += Time.deltaTime;
                float t = timer / _duration;

                // Расширяемся
                transform.localScale = Vector3.Lerp(startScale, endScale, t);

                // Затухаем
                if (TryGetComponent<SpriteRenderer>(out var renderer))
                {
                    var color = renderer.color;
                    color.a = Mathf.Lerp(0.8f, 0f, t);
                    renderer.color = color;
                }

                yield return null;
            }

            Destroy(gameObject); // всё, отжила своё
        }
    }
}