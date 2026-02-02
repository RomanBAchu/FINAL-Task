using Core;

namespace Games
{
    public class DiceGame : CasinoGameBase
    {
        private readonly List<Dice> _playerDice;
        private readonly List<Dice> _dealerDice;

        // Поля для хранения параметров
        private readonly int _count;
        private readonly int _min;
        private readonly int _max;

        public DiceGame(int count, int min, int max)
        {
            if (count <= 0)
                throw new ArgumentException("Dice count must be positive.");
            if (min < 1 || max < 1 || min > max)
                throw new ArgumentException("Invalid min/max values for dice.");

            // Сохраняем параметры в поля
            _count = count;
            _min = min;
            _max = max;

            // Инициализируем списки ДО вызова FactoryMethod
            _playerDice = new List<Dice>();
            _dealerDice = new List<Dice>();

            // Вызываем фабричный метод (без параметров!)
            FactoryMethod();
        }

        // Исправленный FactoryMethod: без параметров!
        protected override void FactoryMethod()
        {
            // Используем сохранённые поля
            for (int i = 0; i < _count; i++)
            {
                _playerDice.Add(new Dice(_min, _max));
                _dealerDice.Add(new Dice(_min, _max));
            }
        }

        private int GetTotalValue(List<Dice> dice)
        {
            return dice.Sum(d => d.Number);
        }

        public override void PlayGame()
        {
            Console.WriteLine("=== Dice Game ===");

            int playerTotal = GetTotalValue(_playerDice);
            int dealerTotal = GetTotalValue(_dealerDice);

            Console.WriteLine($"Your dice: {string.Join(", ", _playerDice.Select(d => d.Number))}");
            Console.WriteLine($"Dealer's dice: {string.Join(", ", _dealerDice.Select(d => d.Number))}");
            Console.WriteLine($"Your total: {playerTotal}");
            Console.WriteLine($"Dealer's total: {dealerTotal}");


            if (playerTotal > dealerTotal)
            {
                Console.WriteLine("You win!");
                OnWinInvoke();
            }
            else if (dealerTotal > playerTotal)
            {
                Console.WriteLine("You lose.");
                OnLooseInvoke();
            }
            else
            {
                Console.WriteLine("It's a tie!");
                OnDrawInvoke();
            }
        }
    }
}
