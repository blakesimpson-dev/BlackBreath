using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Whetstone.ECMS
{
    public class Entity : IEntity
    {
        public Entity()
        {
            _uuid = System.Guid.NewGuid();
            _components = new List<IComponent>();
            _name = "Entity";
            _tags = new List<string>();
        }

        private Guid _uuid;
        public Guid uuid
        {
            get => _uuid;
            set => _uuid = value;
        }

        private List<IComponent> _components;
        public List<IComponent> components
        {
            get => _components;
            set => _components = value;
        }

        private string _name;
        public string name
        {
            get => _name;
            set => _name = value;
        }

        private List<string> _tags;
        public List<string> tags
        {
            get => _tags;
            set => _tags = value;
        }

        public T GetComponent<T>() where T : IComponent
        {
            foreach (IComponent c in _components)
            {
                if (c.GetType().Equals(typeof(T)))
                {
                    return (T)c;
                }
            }
            return default(T);
        }

        public void AddComponent<T>() where T: IComponent
        {
            object c = Activator.CreateInstance(typeof(T), this);
            _components.Add(c as IComponent);
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            foreach (IComponent c in _components)
            {
                if (c.GetType().Equals(typeof(T)))
                {
                    _components.Remove(c);
                }
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (IComponent c in _components)
                c.Update(gameTime);
        }

        /*
        public static void Instantiate(Entity entity)
        {
            SomeObject.allEntitites.Add(entity);
        }

        public static void Destroy(Entity entity)
        {
            SomeObject.allEntitites.Remove(entity);
        }
        */
    }
}