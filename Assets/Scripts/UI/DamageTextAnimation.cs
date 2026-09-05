using TMPro;
using UnityEngine;

namespace LoxaRPG.UI
{
    /// <summary>
    /// Анимация всплывающего урона.
    /// Текст летит вверх и затухает.
    /// </summary>
    public class DamageTextAnimation : MonoBehaviour
    {
        private float _timer;
        private Vector3 _moveDirection;
        private float _lifeTime;
        private float _moveSpeed;
        private TMP_Text _text;

        public void Setup(float damageAmount, float lifetime, float speed)
        {
            _lifeTime = lifetime;
            _moveSpeed = speed;

            _text = GetComponent<TMP_Text>();
            if (_text == null)
            {
                _text = gameObject.AddComponent<TMP_Text>();
            }

            _text.text = Mathf.RoundToInt(damageAmount).ToString();
            _text.fontSize = 36;
            _text.color = GetColorForDamage(damageAmount);

            // Случайное направление разлёта
            float angle = Random.Range(-45f, 45f) + 90f;
            float radians = angle * Mathf.Deg2Rad;
            _moveDirection = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0).normalized;

            if (Camera.main != null)
            {
                transform.rotation = Camera.main.transform.rotation;
            }
        }

        private Color GetColorForDamage(float damage)
        {
            if (damage >= 20) return Color.red;
            if (damage >= 10) return Color.yellow;
            return Color.white;
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            transform.position += _moveDirection * _moveSpeed * Time.deltaTime;

            if (_text != null && _timer >= _lifeTime * 0.5f)
            {
                var color = _text.color;
                color.a -= Time.deltaTime * 2f;
                _text.color = color;
            }

            if (_timer >= _lifeTime)
            {
                Destroy(gameObject);
            }
        }
    }
}