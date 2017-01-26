using NRPlanes.Core.Common;
using Microsoft.Xna.Framework;

namespace NRPlanes.Client.Common
{
    public abstract class DrawableStaticObject : MyDrawableGameComponent
    {
        public StaticObject StaticObject { get; private set;  }

        protected DrawableStaticObject(PlanesGame game, StaticObject staticObject, CoordinatesTransformer coordinatesTransformer)
            : base(game, coordinatesTransformer)
        {
            StaticObject = staticObject;
        }
    }
}