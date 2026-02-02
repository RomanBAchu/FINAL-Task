namespace Entities
{
    public struct Card
    {
        public readonly Suit Suit;
        public readonly Rank Rank;

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public int GetValue()
        {
            if (Rank >= Rank.Six && Rank <= Rank.Ten)
                return (int)Rank;
            if (Rank == Rank.Ace)
                return 11;
            return 10;
        }
    }
}
