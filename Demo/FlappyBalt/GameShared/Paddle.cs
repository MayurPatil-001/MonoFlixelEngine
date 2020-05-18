using Engine;
using Microsoft.Xna.Framework;

namespace Demo
{
    class Paddle: FlxSprite
    {
        public int TargetY = 0;
        public static int SPEED = 480;
        public Paddle(float x = 0, int facing = 0):base(x, FlxG.Height)
        {
            LoadGraphic("paddle");

            if (facing == 0)
                FlipX = true;
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (((Velocity.Y < 0) && (Y <= TargetY + SPEED * elapsed)) ||
                ((Velocity.Y > 0) && (Y >= TargetY - SPEED * elapsed)))
            {
                Velocity.Y = 0;
                Y = TargetY;
            }
            base.Update(gameTime);
        }

        public void Randomize()
        {
            TargetY = Reg.PS.RandomPaddleY();

            if (TargetY < Y)
                Velocity.Y = -SPEED;
            else
                Velocity.Y = SPEED;
        }
    }
}
