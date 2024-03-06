using Stride.Engine;

namespace Bell_Epoque
{
    class Bell_EpoqueApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
