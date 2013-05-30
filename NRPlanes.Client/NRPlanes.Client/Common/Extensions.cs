using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Core.Primitives;
using Microsoft.Xna.Framework;

namespace NRPlanes.Client.Common
{
    public static class Extensions
    {
         public static Vector2 ToVector2(this Vector vector)
         {
             return new Vector2((float)vector.X, (float)vector.Y);
         }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action )
        {
            var lst = enumerable.ToList();

            foreach (var element in lst)
            {
                action.Invoke(element);
            }

            return lst;
        }

        public static double ToRange(this double d, double min, double max)
        {
            return Math.Min(Math.Max(d, min), max);
        }

        public static Rectangle ToRectangle(this Rect rect)
        {
            return new Rectangle((int) rect.X, (int) rect.Y, (int) rect.Width, (int) rect.Height);
        }

        public static Vector2 Round(this Vector2 v)
        {
            return new Vector2((int)Math.Round(v.X), (int)Math.Round(v.Y));
        }

        public static Size GetSize(this Rectangle rectangle)
        {
            return new Size(rectangle.Width, rectangle.Height);
        }

        public static void DrawVerticalRepeatedly(this SpriteBatch spriteBatch, Texture2D texture, Rectangle destination, Color color)
        {
            int n = (int) Math.Ceiling((double) destination.Height / texture.Height);

            for (int i = 0; i < n; i++)
            {
                var particularDestination = new Rectangle(destination.X, destination.Y + i * texture.Height,
                                                          destination.Width, texture.Height);

                spriteBatch.Draw(texture, particularDestination, color);
            }
        }

        public static Rectangle NewWithOffset(this Rectangle rectangle, int offsetX, int offsetY)
        {
            return new Rectangle(rectangle.X + offsetX,
                                 rectangle.Y + offsetY,
                                 rectangle.Width,
                                 rectangle.Height);
        }
    }
}