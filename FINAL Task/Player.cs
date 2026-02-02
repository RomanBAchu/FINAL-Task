namespace Entities
{
    [Serializable]
    public class Player
    {
        public string Name { get; set; }
        public int Bank { get; set; }

        public Player(string name, int bank = 1000)
        {
            Name = name;
            Bank = bank;
        }
    }
}
