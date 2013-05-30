using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Client.Common;

namespace NRPlanes.Client.InfoPanels
{
    public interface IOnMinimapDrawable
    {
        void DrawOnMinimap(GameTime gameTime, SpriteBatch minimapSpriteBatch, CoordinatesTransformer coordinatesTransformer);
    }
}