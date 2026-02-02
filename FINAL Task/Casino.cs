using Core;
using Entities;
using Games;
using Services;

namespace Casino
{
    public class CasinoManager : IGame
    {
        private readonly Player _player;
        private readonly ISaveLoadService<Player> _saveLoadService;
        private readonly BlackjackGame _blackjack;
        private readonly DiceGame _diceGame;
        private int _currentBet;
        
        public CasinoManager(string savePath, string playerName)
        {
            _saveLoadService = new FileSystemSaveLoadService<Player>(savePath);
            _player = _saveLoadService.LoadData(playerName) ?? new Player(playerName);

            _blackjack = new BlackjackGame(32);
            _diceGame = new DiceGame(2, 1, 6);
        }

        public void StartGame()
        {
            Console.WriteLine($"\n=== Welcome to Casino, {_player.Name}! ===");
            Console.WriteLine($"Your bank: {_player.Bank}");

            if (_player.Bank <= 0)
            {
                Console.WriteLine("No money? Kicked!");
                return;
            }

            Console.WriteLine("\nChoose a game:");
            Console.WriteLine("1. Blackjack (21)");
            Console.WriteLine("2. Dice Game");
            Console.Write("Enter choice (1 or 2): ");

            string choice = Console.ReadLine();
            CasinoGameBase game = null;

            switch (choice)
            {
                case "1":
                    game = _blackjack;
                    break;
                case "2":
                    game = _diceGame;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Exiting...");
                    return;
            }

            Console.Write("Enter your bet: ");
            if (!int.TryParse(Console.ReadLine(), out int bet) || bet <= 0)
            {
                Console.WriteLine("Invalid bet amount!");
                return;
            }

            if (bet > _player.Bank)
            {
                Console.WriteLine("You don't have enough money for this bet!");
                return;
            }

            _currentBet = bet;

            game.OnWin += OnPlayerWin;
            game.OnLoose += OnPlayerLose;
            game.OnDraw += OnDraw;

            Console.WriteLine("\n--- Starting game... ---");
            game.PlayGame();

            game.OnWin -= OnPlayerWin;
            game.OnLoose -= OnPlayerLose;
            game.OnDraw -= OnDraw;

            CheckBankState();
        }

        private void OnPlayerWin()
        {
            _player.Bank += _currentBet;
            Console.WriteLine($"You won {_currentBet}# Total bank: {_player.Bank}");

            if (_player.Bank > 10000)
            {
                int excess = _player.Bank - 10000;
                _player.Bank = 10000;
                Console.WriteLine($"You've raided the casino! Excess {excess} taken.");
                Console.WriteLine("A new casino will be built in your honour!");
            }
        }

        private void OnPlayerLose()
        {
            _player.Bank -= _currentBet;
            Console.WriteLine($"You lost {_currentBet}. Total bank: {_player.Bank}");
        }

        private void OnDraw()
        {
            Console.WriteLine("It's a tie! Your bet is returned.");
        }

        private void CheckBankState()
        {
            if (_player.Bank > 5000)
            {
                int toSpend = _player.Bank / 2;
                _player.Bank /= 2;
                Console.WriteLine($"You wasted half of your bank ({toSpend}) in the casino's bar.");
            }
        }
    }
}
