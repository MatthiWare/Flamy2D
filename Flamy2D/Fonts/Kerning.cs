namespace Flamy2D.Fonts
{
    public struct Kerning
    {
        public char FirstCharacter;

        public char SecondCharacter;

        public int Amount;

        public Kerning(char first, char second, int amount)
            : this()
        {
            FirstCharacter = first;
            SecondCharacter = second;
            Amount = amount;
        }
    }
}
