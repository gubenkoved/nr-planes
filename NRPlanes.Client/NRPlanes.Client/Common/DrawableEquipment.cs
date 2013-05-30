using Microsoft.Xna.Framework;
using NRPlanes.Core.Common;

namespace NRPlanes.Client.Common
{
    public abstract class DrawableEquipment : MyDrawableGameComponent
    {
        public Equipment Equipment { get; private set; }

        protected DrawableEquipment(PlanesGame game, Equipment equipment, CoordinatesTransformer coordinatesTransformer)
            : base(game, coordinatesTransformer)
        {
            Equipment = equipment;
        }
    }

}