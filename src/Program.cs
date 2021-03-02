using System;

namespace BlackBreath
{
    class Program
    {
        [STAThread]
        public static void Main()
        {
            using (var game = new Game())
                game.Run();
        }
    }
}
