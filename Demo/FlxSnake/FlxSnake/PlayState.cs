using Engine;
using Engine.Group;
using Engine.Systems;
using Engine.Texts;
using Engine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlxSnake
{
    class PlayState : FlxState
    {
        static readonly float MIN_INTERVAL = 2f;
        static readonly int BLOCK_SIZE = 8;

        FlxText _scoreText;
        FlxSprite _fruit;
        FlxSprite _snakeHead;
        FlxSpriteGroup _snakeBody;
        SoundEffect _gameOverSound;
        SoundEffect _beepSound;

        List<Vector2> _headPositions;
        float _movementInterval = 8;
        int _score = 0;

        FlxObjectDirection _currentDirection = FlxObjectDirection.LEFT;
        FlxObjectDirection _nextDirection = FlxObjectDirection.LEFT;

        protected override void Create()
        {
            //VisibleHitbox = true;
            int screenMiddleX = (int)Math.Floor(FlxG.Width / 2f);
            int screenMiddleY = (int)Math.Floor(FlxG.Height / 2f);

            _snakeHead = new FlxSprite(screenMiddleX - BLOCK_SIZE * 2, screenMiddleY);
            _snakeHead.MakeGraphic(BLOCK_SIZE - 2, BLOCK_SIZE - 2, Color.Lime);
            _snakeHead.DebugColor = Color.Yellow;
            OffsetSprite(_snakeHead);

            _headPositions = new List<Vector2>
            {
                new Vector2(_snakeHead.X, _snakeHead.Y)
            };

            _snakeBody = new FlxSpriteGroup();
            Add(_snakeBody);

            for (int i = 0; i < 3; i++)
            {
                AddSegment();
                MoveSnake();
            }

            Add(_snakeHead);

            _fruit = new FlxSprite();
            _fruit.MakeGraphic(BLOCK_SIZE - 2, BLOCK_SIZE - 2, Color.Red);
            RandomizeFruitPosition();
            OffsetSprite(_fruit);
            Add(_fruit);

            _scoreText = new FlxText(0, 0, 200, "Score: " + _score);
            Add(_scoreText);

            ResetTimer();

            _gameOverSound = Game.Content.Load<SoundEffect>(FlxAssets.SOUND_FLIXEL);
            _beepSound = Game.Content.Load<SoundEffect>(FlxAssets.SOUND_BEEP);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (_scoreText.Alpha < 1)
                _scoreText.Alpha += 0.1f;

            if (!_snakeHead.Alive)
            {
                if (FlxG.Keyboard.AnyJustReleased(new Keys[] { Keys.Space, Keys.R }))
                {
                    FlxG.ResetState();
                }
                return;
            }

            FlxG.Overlap(_snakeHead, _fruit, CollectFruit);
            FlxG.Overlap(_snakeHead, _snakeBody, GameOver);


            if (FlxG.Keyboard.AnyPressed(new Keys[] { Keys.Up, Keys.W }) && _currentDirection != FlxObjectDirection.DOWN)
            {
                _nextDirection = FlxObjectDirection.UP;
            }

            else if (FlxG.Keyboard.AnyPressed(new Keys[] { Keys.Down, Keys.S }) && _currentDirection != FlxObjectDirection.UP)
            {
                _nextDirection = FlxObjectDirection.DOWN;
            }

            else if (FlxG.Keyboard.AnyPressed(new Keys[] { Keys.Left, Keys.A }) && _currentDirection != FlxObjectDirection.RIGHT)
            {
                _nextDirection = FlxObjectDirection.LEFT;
            }

            else if (FlxG.Keyboard.AnyPressed(new Keys[] { Keys.Right, Keys.D }) && _currentDirection != FlxObjectDirection.LEFT)
            {
                _nextDirection = FlxObjectDirection.RIGHT;
            }
        }

        private void GameOver(FlxObject object1, FlxObject object2)
        {
            _snakeHead.Alive = false;
            UpdateText("Game Over - Space to Restart!");

            _gameOverSound.Play();
        }


        private void CollectFruit(FlxObject object1, FlxObject object2)
        {
            _score += 10;
            UpdateText("Score: " + _score);

            RandomizeFruitPosition();

            AddSegment();

            _beepSound.Play();

            if (_movementInterval >= MIN_INTERVAL)
                _movementInterval -= 0.25f;
        }

        void OffsetSprite(FlxSprite sprite)
        {
            sprite.Offset = Vector2.One;
            sprite.CenterOffset();
        }

        void RandomizeFruitPosition(FlxObject object1 = null, FlxObject object2 = null)
        {
            _fruit.X = FlxG.Random.Next(0, (int)Math.Floor(FlxG.Width / 8f) - 1) * 8f;
            _fruit.Y = FlxG.Random.Next(0, (int)Math.Floor(FlxG.Height / 8f) - 1) * 8f;

            FlxG.Overlap(_fruit, _snakeBody, RandomizeFruitPosition);
        }

        void AddSegment()
        {
            FlxSprite segment = new FlxSprite(-20, -20);
            segment.MakeGraphic(BLOCK_SIZE - 2, BLOCK_SIZE - 2, Color.Green);
            segment.DebugColor = Color.Yellow;
            _snakeBody.Add(segment, true);
        }

        void ResetTimer(FlxTimer timer = null)
        {
            if(!_snakeHead.Alive && timer != null)
            {
                timer.Dispose();
                return;
            }
            new FlxTimer().Start(_movementInterval / FlxG.UpdateFrameRate, ResetTimer);
            MoveSnake();
        }

        void MoveSnake()
        {
            _headPositions.Insert(0, new Vector2(_snakeHead.X, _snakeHead.Y));
            if (_headPositions.Count > _snakeBody.Members.Count)
                _headPositions.RemoveAt(_headPositions.Count - 1);

            switch (_nextDirection)
            {
                case FlxObjectDirection.LEFT:
                    _snakeHead.X -= BLOCK_SIZE;
                    break;
                case FlxObjectDirection.RIGHT:
                    _snakeHead.X += BLOCK_SIZE;
                    break;
                case FlxObjectDirection.UP:
                    _snakeHead.Y -= BLOCK_SIZE;
                    break;
                case FlxObjectDirection.DOWN:
                    _snakeHead.Y += BLOCK_SIZE;
                    break;
            }
            _currentDirection = _nextDirection;

            FlxSpriteUtil.ScreenWrap(_snakeHead);

            List<Vector2> headpositions = _headPositions.ToList();
            for (int i = 0; i < _headPositions.Count; i++)
                _snakeBody.Members[i].SetPosition(headpositions[i].X, headpositions[i].Y);
        }

        private void UpdateText(string newText)
        {
            _scoreText.Text = newText;
            _scoreText.Alpha = 0;
        }
    }
}
