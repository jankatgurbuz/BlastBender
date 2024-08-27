using System;
using BoardItems;
using Zenject;

namespace Blast.Factory
{
    public class BoardItemFactory : PlaceholderFactory<Type, object[], IBoardItem>
    {
        private readonly DiContainer _container;

        public BoardItemFactory(DiContainer container)
        {
            _container = container;
        }

        public override IBoardItem Create(Type type, object[] allparams)
        {
            return (IBoardItem)_container.Instantiate(type, allparams);
        }
    }
}