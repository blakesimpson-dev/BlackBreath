using System;
using Microsoft.Xna.Framework;
using Whetstone.ECMS.Components;

namespace Whetstone.ECMS.Entities
{
    public class Actor : Entity
    {     
        public Actor(
            string name,
            int awareness,
            Color color,
            Char character,
            int x = 0,
            int y = 0)
        {
            this.name = name;

            this.AddComponent<CStats>();
            cStats = this.GetComponent<CStats>();
            cStats.awareness = awareness;

            this.AddComponent<CTransform>();
            cTransform = this.GetComponent<CTransform>();
            cTransform.position.x = x;
            cTransform.position.y = y;

            this.AddComponent<CRenderer>();
            cRenderer = this.GetComponent<CRenderer>();
            cRenderer.color = color;
            cRenderer.character = character;
        }

        public CStats cStats;
        public CTransform cTransform;
        public CRenderer cRenderer;
    }
}