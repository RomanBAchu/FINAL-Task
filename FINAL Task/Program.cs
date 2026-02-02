using System.Numerics;
using System;
using Core;
using Entities;
using Games;
using Services;
using Casino;

namespace FINAL_Task
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Final Task Casino!");

            // Путь для сохранения профилей
            string savePath = "PlayerProfiles";

            // Сервис сохранения/загрузки
            var saveLoadService = new FileSystemSaveLoadService<Player>(savePath);

            // Загрузка или создание профиля игрока
            Player player = LoadOrCreatePlayer(saveLoadService);

            // Создание казино с загруженным игроком
            var casino = new Casino(savePath, player.Name);

            // Запуск игры
            casino.StartGame();

            // Сохранение профиля после игры
            saveLoadService.SaveData(player, player.Name);

            Console.WriteLine("Thanks for playing! Goodbye.");
            Console.ReadKey();
        }

        /// <summary>
        /// Загружает существующий профиль или создаёт новый
        /// </summary>
        /// <param name="service">Сервис сохранения/загрузки</param>
        /// <returns>Объект Player</returns>
        private static Player LoadOrCreatePlayer(ISaveLoadService<Player> service)
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();

            // Пытаемся загрузить профиль
            Player player = service.LoadData(name);

            if (player == null)
            {
                Console.WriteLine("Profile not found. Creating new player...");
                player = new Player(name);
            }
            else
            {
                Console.WriteLine($"Welcome back, {player.Name}! Your bank: {player.Bank}");
            }

            return player;
        }
    }
}
