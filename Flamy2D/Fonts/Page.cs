namespace Flamy2D.Fonts
{
    public struct Page
    {
        public string FileName;
        public int Id;

        public Page(int id, string name)
            : this()
        {
            Id = id;
            FileName = name;
        }
    }
}
