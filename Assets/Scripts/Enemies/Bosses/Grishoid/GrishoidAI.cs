using UnityEngine;
using System.Collections;

namespace LoxaRPG.Enemies.Bosses.Grishoid
{
    /// <summary>
    /// Мозг Гришоида.
    /// Выбирает ОДНУ атаку за раз, не спамит скиллы.
    /// </summary>
    [RequireComponent(typeof(GrishoidMovement))]
    [RequireComponent(typeof(GrishoidDashAttack))]
    [RequireComponent(typeof(GrishoidSlamAttack))]
    [RequireComponent(typeof(GrishoidNormalAttack))]
    public class GrishoidAI : MonoBehaviour
    {
        [Header("Дистанции")]
        [SerializeField] private float attackRange = 5f;

        [Header("Задержка между атаками")]
        [SerializeField] private float attackDelay = 1f; // пауза между атаками

        private Transform _player;
        private GrishoidMovement _movement;
        private GrishoidDashAttack _dashAttack;
        private GrishoidSlamAttack _slamAttack;
        private GrishoidNormalAttack _normalAttack;

        private bool _isAttacking;
        private float _lastAttackTime;

        private void Awake()
        {
            _movement = GetComponent<GrishoidMovement>();
            _dashAttack = GetComponent<GrishoidDashAttack>();
            _slamAttack = GetComponent<GrishoidSlamAttack>();
            _normalAttack = GetComponent<GrishoidNormalAttack>();
        }

        private void Start()
        {
            FindPlayer();
            InvokeRepeating(nameof(FindPlayer), 0f, 2f);

            StartCoroutine(BehaviourLoop());
        }

        private void FindPlayer()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _player = playerObj.transform;
                _movement.SetTarget(_player);
            }
        }

        private IEnumerator BehaviourLoop()
        {
            while (true)
            {
                if (_player == null || _isAttacking)
                {
                    yield return null;
                    continue;
                }

                float distance = Vector2.Distance(transform.position, _player.position);

                if (distance > attackRange)
                {
                    _movement.MoveTowardsPlayer();
                }
                else
                {
                    _movement.Stop();

                    // ПРОВЕРЯЕМ КУЛДАУН МЕЖДУ АТАКАМИ!
                    if (Time.time < _lastAttackTime + attackDelay)
                    {
                        yield return null;
                        continue;
                    }

                    ChooseAttack();
                }

                yield return null;
            }
        }

        private void ChooseAttack()
        {
            float random = Random.value;

            // 33% даш, 33% слэм, 34% обычная атака
            if (random < 0.33f && _dashAttack.CanDash())
            {
                _isAttacking = true;
                _lastAttackTime = Time.time;
                _dashAttack.StartDash();
                StartCoroutine(WaitForAttack(0.6f));
            }
            else if (random < 0.66f && _slamAttack.CanSlam())
            {
                _isAttacking = true;
                _lastAttackTime = Time.time;
                _slamAttack.StartSlam();
                StartCoroutine(WaitForAttack(1.5f));
            }
            else if (_normalAttack.CanAttack())
            {
                _isAttacking = true;
                _lastAttackTime = Time.time;
                _normalAttack.StartAttack();
                StartCoroutine(WaitForAttack(0.5f));
            }
        }

        private IEnumerator WaitForAttack(float duration)
        {
            yield return new WaitForSeconds(duration);
            _isAttacking = false;
        }
    }
}