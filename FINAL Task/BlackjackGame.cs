using Entities;
using Core;

namespace Games
{
    public class BlackjackGame : CasinoGameBase
    {
        private Queue<Card> _deck;
        private List<Card> _playerHand;
        private List<Card> _dealerHand;
        private int _numCards;

        public BlackjackGame(int numCards)
        {
            if (numCards <= 0)
                throw new ArgumentException("Number of cards must be positive.");

            _numCards = numCards;
            FactoryMethod();
            _deck = new Queue<Card>(Shuffle());
            _playerHand = new List<Card>();
            _dealerHand = new List<Card>();
        }

        protected override void FactoryMethod()
        {
            var cards = new List<Card>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    cards.Add(new Card(suit, rank));
                }
            }

            // Перемешиваем и берём нужное количество
            var rng = new Random();
            cards = cards.OrderBy(x => rng.Next()).ToList();
            _deck = new Queue<Card>(cards.Take(_numCards));
        }

        private List<Card> Shuffle()
        {
            var list = new List<Card>(_deck);
            var rng = new Random();
            return list.OrderBy(x => rng.Next()).ToList();
        }

        private int CalculateHandValue(List<Card> hand)
        {
            int value = 0;
            int aces = 0;

            foreach (var card in hand)
            {
                value += card.GetValue();
                if (card.Rank == Rank.Ace) aces++;
            }

            while (value > 21 && aces > 0)
            {
                value -= 10;
                aces--;
            }

            return value;
        }

        public override void PlayGame()
        {
            // Раздаём по 2 карты
            _playerHand.Add(_deck.Dequeue());
            _playerHand.Add(_deck.Dequeue());

            _dealerHand.Add(_deck.Dequeue());
            _dealerHand.Add(_deck.Dequeue());

            int playerValue = CalculateHandValue(_playerHand);
            int dealerValue = CalculateHandValue(_dealerHand);

            Console.WriteLine("=== Blackjack ===");
            Console.WriteLine($"Your cards: {string.Join(", ", _playerHand.Select(c => $"{c.Rank} of {c.Suit}"))}");
            Console.WriteLine($"Dealer's cards: {_dealerHand[0].Rank} of {_dealerHand[0].Suit}, ?");

            // Проверка на блэкджек у игрока
            if (playerValue == 21)
            {
                Console.WriteLine("Blackjack# You win!");
                OnWinInvoke();
                return;
            }

            // Ход игрока
            while (true)
            {
                Console.Write("Do you want to hit? (y/n): ");
                string input = Console.ReadLine()?.ToLower();

                if (input == "n")
                    break;

                _playerHand.Add(_deck.Dequeue());
                playerValue = CalculateHandValue(_playerHand);

                Console.WriteLine($"You drew: {_playerHand.Last().Rank} of {_playerHand.Last().Suit}");
                Console.WriteLine($"Your total: {playerValue}");

                if (playerValue > 21)
                {
                    Console.WriteLine("Bust# You lose.");
                    OnLooseInvoke();
                    return;
                }
            }

            // Ход дилера
            Console.WriteLine($"Dealer's hand: {string.Join(", ", _dealerHand.Select(c => $"{c.Rank} of {c.Suit}"))}");
            Console.WriteLine($"Dealer's total: {dealerValue}");

            while (dealerValue < 17)
            {
                _dealerHand.Add(_deck.Dequeue());
                dealerValue = CalculateHandValue(_dealerHand);
                Console.WriteLine($"Dealer draws: {_dealerHand.Last().Rank} of {_dealerHand.Last().Suit}");
                Console.WriteLine($"Dealer's total: {dealerValue}");
            }

            if (dealerValue > 21)
            {
                Console.WriteLine("Dealer busts# You win.");
                OnWinInvoke();
            }
            else if (playerValue > dealerValue)
            {
                Console.WriteLine("You win!");
                OnWinInvoke();
            }
            else if (dealerValue > playerValue)
            {
                Console.WriteLine("You lose.");
                OnLooseInvoke();
            }
            else
            {
                Console.WriteLine("Push (tie).");
                OnDrawInvoke();
            }
        }
    }
}
