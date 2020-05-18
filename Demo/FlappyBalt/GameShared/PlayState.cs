using Engine;
using Engine.Texts;
using Engine.Utils;
using Engine.Utils.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Demo
{
    class PlayState : FlxState
    {
        Player _player;
        Paddle _paddleLeft;
        Paddle _paddleRight;
        FlxSprite _spikeBottom;
        FlxSprite _spriteTop;
        FlxSprite _bounceLeft;
        FlxSprite _bounceRight;
        FlxText _scoreDisplay;
        FlxText _highScore;
        FlxEmitter _feathers;
        static string SAVE_DATA= "FLAPPYBALT";
        public PlayState()
        {
            BackgroundColor = new Color(100, 106, 125);
        }

        protected override void Create()
        {
            Reg.PS = this;
            //DebugDraw = false;
            
            Add(new FlxSprite(0, 0, "bg"));

            _scoreDisplay = new FlxText("DefaultFont", 0, 180, FlxG.Width);
            _scoreDisplay.Alignment = FlxTextAlign.CENTER;
            _scoreDisplay.Color = new Color(134, 134, 150);
            Add(_scoreDisplay);
            _scoreDisplay.Text = "Press Space to Start";

            Reg.HighScore = LoadScore();

            _highScore = new FlxText("DefaultFont", 0, 40, FlxG.Width);
            _highScore.Alignment = FlxTextAlign.CENTER;
            _highScore.Color = new Color(134, 134, 150);
            Add(_highScore);

            if (Reg.HighScore > 0)
                _highScore.Text = Reg.HighScore.ToString();

            _bounceLeft = new FlxSprite(1, 17);
            _bounceLeft.LoadGraphic(Reg.GetBounceImage(FlxG.Height - 34), true, 4, FlxG.Height - 34);
            _bounceLeft.Animation.Add("flash", new int[] { 1, 0 }, 8, false);
            Add(_bounceLeft);

            _bounceRight = new FlxSprite(FlxG.Width - 5, 17);
            _bounceRight.LoadGraphic(Reg.GetBounceImage(FlxG.Height - 34), true, 4, FlxG.Height - 34);
            _bounceRight.Animation.Add("flash", new int[] { 1, 0 }, 8, false);
            Add(_bounceRight);

            _paddleLeft = new Paddle(6, 1);
            Add(_paddleLeft);

            _paddleRight = new Paddle(FlxG.Width - 15, 0);
            Add(_paddleRight);

            _spikeBottom = new FlxSprite(0, 0, "spike");
            _spikeBottom.Y = FlxG.Height - _spikeBottom.Height;
            Add(_spikeBottom);

            _spriteTop = new FlxSprite(0, 0, "spike");
            _spriteTop.Rotation = MathHelper.ToRadians(180);
            _spriteTop.Y = 0;
            Add(_spriteTop);

            _player = new Player();
            Add(_player);


            _feathers = new FlxEmitter();
            _feathers.Scale.Set(2, 2);
            _feathers.KeepScaleRatio = true;
            _feathers.LoadParticles("feather", 50);
            _feathers.Velocity.Set(-10, -10, 10, 10);
            _feathers.Acceleration.Set(0, 10);
            Add(_feathers);
        }

        public override void Update(GameTime gameTime)
        {
            if (FlxG.PixelPerfectOverlap(_player, _spikeBottom) || FlxG.PixelPerfectOverlap(_player, _spriteTop)
                || FlxG.PixelPerfectOverlap(_player, _paddleLeft) || FlxG.PixelPerfectOverlap(_player, _paddleRight)){
                _player.Kill();
            }

            if (_player.X < 5)
            {
                _player.X = 5;
                _player.Velocity.X = -_player.Velocity.X;
                _player.FlipX = false;
                IncreaseScore();
                _bounceLeft.Animation.Play("flash");
                _paddleRight.Randomize();
            }
            else if (_player.X + _player.Width > FlxG.Width - 5)
            {
                _player.X = FlxG.Width - _player.Width - 5;
                _player.Velocity.X = -_player.Velocity.X;
                _player.FlipX = true;
                IncreaseScore();
                _bounceRight.Animation.Play("flash");
                _paddleLeft.Randomize();
            }

            if (FlxG.Keyboard.JustPressed(Keys.E) && (FlxG.Keyboard.AnyPressed(new Keys[] { Keys.LeftShift, Keys.RightShift, Keys.LeftControl, Keys.RightControl, Keys.RightAlt, Keys.LeftAlt})))
            {
                ClearSave();
                FlxG.ResetState();
            }

            base.Update(gameTime);
        }

        public void LaunchFeathers(float x, float y, int amount)
        {
            _feathers.X = x;
            _feathers.Y = y;
            _feathers.Start(true, 0, amount);
            FlxG.Log.Info("LaunchFeathers");
        }

        public int RandomPaddleY()
        {
            return FlxG.Random.Next((int)_bounceLeft.Y, (int)(_bounceLeft.Y + _bounceLeft.Height - _paddleLeft.Height));
        }

        void IncreaseScore()
        {
            Reg.Score++;
            _scoreDisplay.Text = Reg.Score.ToString();
            _scoreDisplay.Scale = new Vector2(3, 3); 
        }

        public void Reset()
        {
            _paddleLeft.Y = FlxG.Height;
            _paddleRight.Y = FlxG.Height;
            _player.FlipX = false;
            Reg.Score = 0;
            _scoreDisplay.Text = "";
            Reg.HighScore = LoadScore();

            if (Reg.HighScore > 0)
                _highScore.Text = Reg.HighScore.ToString();
        }

        public static void SaveScore()
        {
            Reg.SaveFile.HighScore = Reg.Score;
            Reg.Save.WriteToFile(FileFormats.Binary, Reg.SaveFile);
            FlxG.Log.Info("SaveScore - " + Reg.SaveFile.ToString());
        }

        public static int LoadScore()
        {
            Reg.Save = new FlxSave("FlappyBalt");
            if (Reg.Save.Bind(SAVE_DATA))
            {
                Reg.SaveFile = Reg.Save.ReadFromFile<SaveFile>(FileFormats.Binary);
            }
            FlxG.Log.Info("LoadScore - " + Reg.SaveFile);
            if (Reg.SaveFile == null)
                Reg.SaveFile = new SaveFile { HighScore = 0 };
            else
            {
                Reg.HighScore = Reg.SaveFile.HighScore;
            }
            return Reg.HighScore;
        }

        public static void ClearSave()
        {
            Reg.Save.DeleteFile();
            FlxG.Log.Info("ClearSave");
        }
    }
}
