using Core;
using Entities;
using Services;
using Casino;

namespace FINAL_Task
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Final Task Casino!");

            // Путь для сохранения профилей. В нём же вручную можно хакнуть деньги.
            string savePath = "PlayerProfiles";

            // Сервис сохранения/загрузки
            var saveLoadService = new FileSystemSaveLoadService<Player>(savePath);

            // Загрузка или создание профиля игрока
            Player player = LoadOrCreatePlayer(saveLoadService);

            // Создание казино с загруженным игроком
            var casino = new CasinoManager(savePath, player.Name);

            // Запуск игры
            casino.StartGame();

            // Сохранение профиля после игры
            saveLoadService.SaveData(player, player.Name);

            Console.WriteLine("Thanks for playing! Goodbye.");
            Console.ReadKey();
        }

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
