using Engine;
using Engine.Texts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Sandbox
{
    class PlayState: FlxState
    {
        FlxSprite spriteA;
        FlxSprite spriteB;
        FlxText text1;
        FlxText text2;
        FlxText text3;
        FlxText text4;
        FlxText text5;
        protected override void Create()
        {
            BackgroundColor = Color.Gray;
            //VisibleBoundingbox = true;
            spriteA = new FlxSprite(10, 10);
            spriteA.MakeGraphic(128, 128, Color.White);
            spriteA.ScrollFactor = Vector2.Zero;

            spriteB = new FlxSprite(200, 400);
            spriteB.MakeGraphic(160, 16, Color.Yellow);
            spriteB.Immovable = true;
            spriteB.ScrollFactor = new Vector2(0.5f, 0.5f);

            text1 = new FlxText("Font");

            text2 = new FlxText("Font", 0, 0, FlxG.Width);
            text2.Alignment = FlxTextAlign.LEFT;

            text3 = new FlxText("Font", 0, 0, FlxG.Width);
            text3.Alignment = FlxTextAlign.CENTER;

            text4 = new FlxText("Font", 0, 0, FlxG.Width);
            text4.Alignment = FlxTextAlign.RIGHT;

            text5 = new FlxText("BigFont", 0, 0, FlxG.Width, "Hello World");
            text5.BorderStyle = FlxTextBorderStyle.SHADOW;
            text5.BorderColor = Color.Black;
            text5.BorderSize = 4;
            text5.ScreenCenter();

            text1.X = 10;
            text1.Y = FlxG.Height - 40;
            text2.X = 10;
            text2.Y = FlxG.Height - 20;
            text3.X = 10;
            text3.Y = FlxG.Height - 60;
            text4.X = 10;
            text4.Y = FlxG.Height - 80;

            Add(spriteA);
            Add(spriteB);
            Add(text1);
            Add(text2);
            Add(text3);
            Add(text4);
            Add(text5);
        }

        public override void Update(GameTime gameTime)
        {
            if (FlxG.Keyboard.Pressed(Keys.W))
                spriteA.Y -= 1;
            if (FlxG.Keyboard.Pressed(Keys.S))
                spriteA.Y += 1;
            if (FlxG.Keyboard.Pressed(Keys.A))
                spriteA.X -= 1;
            if (FlxG.Keyboard.Pressed(Keys.D))
                spriteA.X += 1;
            if (FlxG.Keyboard.Pressed(Keys.Q))
                spriteA.Angle -= 1;
            if (FlxG.Keyboard.Pressed(Keys.E))
                spriteA.Angle += 1;

            if (FlxG.Keyboard.Pressed(Keys.Up))
                //spriteB.Y -= 1;
                FlxG.Camera.Y -= 1;
            if (FlxG.Keyboard.Pressed(Keys.Down))
                //spriteB.Y += 1;
                FlxG.Camera.Y += 1;
            if (FlxG.Keyboard.Pressed(Keys.Left))
                //spriteB.X -= 1;
                FlxG.Camera.X -= 1;
            if (FlxG.Keyboard.Pressed(Keys.Right))
                //spriteB.X += 1;
                FlxG.Camera.X += 1;
            if (FlxG.Keyboard.Pressed(Keys.O))
                //spriteB.Angle -= 1;
                FlxG.Camera.Rotation -= MathHelper.ToRadians(1);
            if (FlxG.Keyboard.Pressed(Keys.P))
                //spriteB.Angle += 1;
                FlxG.Camera.Rotation += MathHelper.ToRadians(1);


            text1.Text = $"1 spriteA :{spriteA.Position} Renderposition {spriteA.RenderPosition}";
            text2.Text = $"2 camera :{FlxG.Camera.Position} rotation :{FlxG.Camera.Rotation} viewoffset {FlxG.Camera.ViewOffset}";

            bool intersects = spriteA.BoundingBox.Intersects(spriteB.BoundingBox);
            //bool collision = flxg.pixelperfectoverlap(spritea, spriteb);
            bool collision = FlxG.Collide(spriteA, spriteB);
            if (collision)
            {
                spriteA.Color = Color.Red;
            }
            else
            {
                spriteA.Color = Color.White;
            }
            text3.Text = $"3 intersects: {intersects} collision : {collision}";
            text4.Text = $"4 Mouse : {FlxG.Mouse.Position} ScreenPos : {FlxG.Mouse.ScreenPosition}";
            base.Update(gameTime);
        }
    }
}
