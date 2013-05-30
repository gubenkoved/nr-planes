using NRPlanes.Client.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Core.Common;
using Plane = NRPlanes.Core.Common.Plane;
using System.Linq;

namespace NRPlanes.Client.InfoPanels
{
    public class PlaneInfo : InfoPanelItem
    {
        private Texture2D _background;

        private IndicatorsDrawer _indicatorsDrawer;

        public Plane Plane { get; set; }

        public PlaneInfo(PlanesGame game, Rectangle positionRectangle) 
            : base(game, positionRectangle)
        {
        }

        public override void Initialize()
        {
            _background = Game.Content.Load<Texture2D>("PlaneInfo/background");

            _indicatorsDrawer = new IndicatorsDrawer(Game.Content.Load<Texture2D>("PlaneInfo/indicator_backround"),
                                                     Game.Content.Load<Texture2D>("PlaneInfo/indicator_value"),
                                                     Game.Content.Load<SpriteFont>("Fonts/infopanel_font"));

            base.Initialize();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_background,
                             PositionRectangle,
                             null,
                             Color.White,
                             0f,
                             new Vector2(),
                             SpriteEffects.None,
                             1.0f);

            if (Plane != null)
            {
                const int indention = 16;

                var indicatorPosition = new Rectangle(PositionRectangle.X + indention,
                                                               PositionRectangle.Y + indention,
                                                               PositionRectangle.Width - 2 * indention,
                                                               20);

                _indicatorsDrawer.Draw(spriteBatch,
                                       indicatorPosition,
                                       Plane.Health / Plane.MaximalHealth,
                                       Color.Black, 
                                       Color.Red, 
                                       string.Format("{0:F0} HP", Plane.Health), 
                                       Color.FromNonPremultiplied(255, 255, 255, 120));

                var equipmentWithIndicator = Plane.AllEquipment.Where(e => e.Info != null).ToList();

                foreach (var equipment in equipmentWithIndicator)
                {
                    Color color;

                    if (equipment is Engine)
                        color = Color.SteelBlue;
                    else if (equipment is Weapon)
                        color = Color.Orange;
                    else if (equipment is Shield)
                        color = Color.Green;
                    else
                        color = Color.Gray;

                    indicatorPosition.Offset(0, 24);

                    _indicatorsDrawer.Draw(spriteBatch,
                                           indicatorPosition,
                                           equipment.Charge / equipment.MaximumCharge,
                                           Color.Black,
                                           color,
                                           equipment.Info,
                                           Color.FromNonPremultiplied(255, 255, 255, 120));
                }
            }
        }
    }
}