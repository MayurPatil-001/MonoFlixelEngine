using Engine;
using Engine.Group;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BreakoutShared
{
    public class PlayState:FlxState
    {
        static int BAT_SPEED = 350;

        FlxSprite _bat;
        FlxSprite _ball;

        FlxGroup _walls;
        FlxSprite _leftWall;
        FlxSprite _rightWall;
        FlxSprite _topWall;
        FlxSprite _bottomWall;

        FlxGroup _bricks;

        protected override void Create()
        {
            FlxG.Mouse.Visible = true;

            _bat = new FlxSprite(180, 220);
            _bat.MakeGraphic(40, 6, Color.Magenta);
            _bat.Immovable = true;

            _ball = new FlxSprite(180, 160);
            _ball.MakeGraphic(6, 6, Color.Magenta);

            _ball.Elasticity = 1;
            _ball.MaxVelocity = new Vector2(200, 200);
            _ball.Velocity.Y = 200;

            _walls = new FlxGroup();

            _leftWall = new FlxSprite(0, 0);
            _leftWall.MakeGraphic(10, 240, Color.Gray);
            _leftWall.Immovable = true;
            _walls.Add(_leftWall);

            _rightWall = new FlxSprite(310, 0);
            _rightWall.MakeGraphic(10, 240, Color.Gray);
            _rightWall.Immovable = true;
            _walls.Add(_rightWall);

            _topWall = new FlxSprite(0, 0);
            _topWall.MakeGraphic(320, 10, Color.Gray);
            _topWall.Immovable = true;
            _walls.Add(_topWall);

            _bottomWall = new FlxSprite(0, 239);
            _bottomWall.MakeGraphic(320, 10, Color.Transparent);
            _bottomWall.Immovable = true;
            _walls.Add(_bottomWall);

            _bricks = new FlxGroup();
            int bx = 10;
            int by = 30;
            Color[] brickColors = { new Color(208, 58, 209), new Color(247, 83, 82), 
                new Color(253, 128, 20), new Color(255, 144, 36), 
                new Color(5, 179, 32), new Color(109, 101, 246) };
            for(int y = 0; y < 6; y++)
            {
                for(int x = 0; x < 20; x++)
                {
                    FlxSprite tempBrick = new FlxSprite(bx, by);
                    tempBrick.MakeGraphic(15, 15, brickColors[y]);
                    tempBrick.Immovable = true;
                    _bricks.Add(tempBrick);
                    bx += 15;
                }
                bx = 10;
                by += 15;
            }

            Add(_walls);
            Add(_bat);
            Add(_ball);
            Add(_bricks);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _bat.Velocity.X = 0;

            //TODO : Touch controls

            if (FlxG.Keyboard.AnyPressed(new Keys[] { Keys.Left, Keys.A }) && _bat.X > 10)
                _bat.Velocity.X = -BAT_SPEED;
            else if (FlxG.Keyboard.AnyPressed(new Keys[] { Keys.Right, Keys.D }) && _bat.X < 270)
                _bat.Velocity.X = BAT_SPEED;

            if (FlxG.Keyboard.JustReleased(Keys.R))
                FlxG.ResetState();

            if (_bat.X < 10)
                _bat.X = 10;
            if (_bat.X > 270)
                _bat.X = 270;

            FlxG.Collide(_ball, _walls);
            FlxG.Collide(_bat, _ball, Ping);
            FlxG.Collide(_ball, _bricks, Hit);
        }

        void Hit(FlxObject ball, FlxObject brick)
        {
            brick.Exists = false;
        }

        void Ping(FlxObject bat, FlxObject ball)
        {
            int batMid = (int)bat.X + 20;
            int ballMid = (int)ball.X + 3;
            int diff;

            if(ballMid < batMid)
            {
                diff = batMid - ballMid;
                ball.Velocity.X = (-10 * diff);
            }
            else if(ballMid > batMid)
            {
                diff = ballMid - batMid;
                ball.Velocity.X = (10 * diff);
            }
            else
            {
                ball.Velocity.X = 2 + FlxG.Random.Next(0, 8);
            }
        }
    }
}
