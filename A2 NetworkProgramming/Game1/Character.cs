using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    class Character
    {
        private byte ID;
        private Point location;

        public Character(byte ID, byte xLoc, byte yLoc)
        {
            this.ID = ID;
            location = new Point(xLoc, yLoc);
        }
        public void Draw(SpriteBatch sb, Texture2D tex)
        {
            sb.Draw(tex, new Rectangle(location.X, location.Y, 10, 10), null, Color.White, 0, Vector2.One, SpriteEffects.None, 0);
        }
    }
}
