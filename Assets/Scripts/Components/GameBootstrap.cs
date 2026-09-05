using UnityEngine;
using LoxaRPG.Player.Components;

namespace LoxaRPG.Core
{
    /// <summary>
    /// Инициализирует игру при старте.
    /// Больше не использует G.cs — только прямые ссылки.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private PlayerWallet playerWallet;

        private void Start()
        {
            // Кошелёк сам инициализируется в Awake, тут ничего не делаем
            Debug.Log("GameBootstrap: Игра инициализирована");
        }
    }
}