using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Whetstone.ECMS
{
    public interface IEntity
    {
        Guid uuid { get; set; }
        List<IComponent> components { get; set; }
        string name { get; set; }
        List<string> tags { get; set; }

        T GetComponent<T>() where T: IComponent;
        void AddComponent<T>() where T: IComponent;
        void RemoveComponent<T>() where T: IComponent;
        void Update(GameTime gameTime);
    }
}