using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Demo
{
    class Player: FlxSprite
    {
        public Player():base(FlxG.Width * 0.5f - 4, FlxG.Height * 0.5f - 4)
        {
            LoadGraphic("dove", true);
            Animation.FrameIndex = 2;
            Animation.Add("flap", new int[] { 1, 0, 1, 2 }, 12, false);
        }

        public override void Update(GameTime gameTime)
        {
            if(FlxG.Keyboard.JustPressed(Keys.Space))
            {
                if(Acceleration.Y == 0)
                {
                    // First time
                    Acceleration.Y = 500;
                    Velocity.X = 80;
                }

                Velocity.Y = -240;

                Animation.Play("flap", true);
            }
            base.Update(gameTime);
        }
        public override void Kill()
        {
            if (!Exists)
                return;

            Reg.PS.LaunchFeathers(X, Y, 20);

            base.Kill();

            FlxG.Camera.Flash(Color.White, 1f, OnFlashDone);
            FlxG.Camera.Shake(0.02f, 0.35f);
        }

        public void OnFlashDone()
        {
            PlayState.SaveScore();
            Revive();
            Reg.PS.Reset();
        }

        public override void Revive()
        {
            X = FlxG.Width * 0.5f - 4;
            Y = FlxG.Height * 0.5f - 4;
            Acceleration.X = 0;
            Acceleration.Y = 0;
            Velocity.X = 0;
            Velocity.Y = 0;

            base.Revive();
        }
    }
}
