using Microsoft.Xna.Framework;
using NRPlanes.Core.Common;

namespace NRPlanes.Client.Common
{
    public abstract class DrawableGameObject : MyDrawableGameComponent
    {
        public GameObject GameObject { get; private set; }

        protected DrawableGameObject(PlanesGame game, GameObject gameObject, CoordinatesTransformer coordinatesTransformer)
            : base(game, coordinatesTransformer)
        {
            GameObject = gameObject;
        }
    }
}