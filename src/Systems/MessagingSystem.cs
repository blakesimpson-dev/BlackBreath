using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
// using SadConsole.Consoles;
using SConsole = SadConsole.Consoles.Console;

namespace BlackBreath.Systems
{
    public class MessagingSystem
    {
        private readonly Queue<string> _lines;

        public MessagingSystem()
        {
            _lines = new Queue<string>();
        }

        public void Add(string message)
        {
            _lines.Enqueue(message);
            if (_lines.Count > 9)
            {
                _lines.Dequeue();
            }
        }

        public void Render(SConsole console)
        {
            string[] lines = _lines.ToArray();
            for (int i = 0; i < lines.Count(); i++)
            {
                Color messageColor = Game.gameColors.messageDefault;
                console.CellData.Print(1, i + 1, lines[i], messageColor);
            }
        }
    }
}