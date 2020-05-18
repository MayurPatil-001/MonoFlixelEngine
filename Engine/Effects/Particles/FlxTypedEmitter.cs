using Engine.Extensions;
using Engine.Group;
using Engine.MathUtils;
using Engine.Utils.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine.Effects.Particles
{
    public enum FlxEmitterMode
    {
        SQUARE,
        CIRCLE
    }
    public class FlxTypedEmitter<T> : FlxTypedGroup<T> where T : FlxSprite, IFlxParticle
    {
        private int _quantity = 0;
        private bool _explode = true;
        private float _timer = 0;
        private int _counter = 0;
        private bool _waitForKill = false;

        public Vector2 Position;
        public Vector2 Size;
        public Type ParticleClass { get; set; } = typeof(FlxParticle);
        public bool Emitting { get; private set; } = false;
        public float Frequency { get; set; } = 0.1f;
        /// <summary>
        /// Unused
        /// </summary>
        public BlendState Blend { get; set; }
        public float X { get => Position.X; set => Position.X = value; }
        public float Y { get => Position.Y; set => Position.Y = value; }
        public float Width { get => Size.X; set => Size.X = value; }
        public float Height { get => Size.Y; set => Size.Y = value; }
        public FlxEmitterMode LaunchMode { get; set; } = FlxEmitterMode.CIRCLE;
        public bool KeepScaleRatio { get; set; } = false;
        public FlxVectorRangeBounds Velocity { get; set; } = new FlxVectorRangeBounds(-100, -100, 100, 100);
        public FlxRangeBounds<float> Speed { get; set; } = new FlxRangeBounds<float>(0, 100);
        public FlxRangeBounds<float> AngularAcceleration { get; set; } = new FlxRangeBounds<float>(0, 0);
        public FlxRangeBounds<float> AngularDrag { get; set; } = new FlxRangeBounds<float>(0, 0);
        public FlxRangeBounds<float> AngularVelocity { get; set; } = new FlxRangeBounds<float>(0, 0);
        public FlxRangeBounds<float> Angle { get; set; } = new FlxRangeBounds<float>(0);
        public bool IgnoreAngularVelocity { get; set; } = false;
        public FlxBounds<float> LaunchAngle { get; set; } = new FlxBounds<float>(-180, 180);
        public FlxBounds<float> Lifespan { get; set; } = new FlxBounds<float>(3);
        public FlxVectorRangeBounds Scale { get; set; } = new FlxVectorRangeBounds(1, 1);
        public FlxRangeBounds<float> Alpha { get; set; } = new FlxRangeBounds<float>(1);
        public FlxRangeBounds<Color> Color { get; set; } = new FlxRangeBounds<Color>(Microsoft.Xna.Framework.Color.White, Microsoft.Xna.Framework.Color.White);
        public FlxVectorRangeBounds Drag { get; set; } = new FlxVectorRangeBounds(0, 0);
        public FlxVectorRangeBounds Acceleration { get; set; } = new FlxVectorRangeBounds(0, 0);
        public FlxRangeBounds<float> Elasticity { get; set; } = new FlxRangeBounds<float>(0);
        public bool Immovable { get; set; } = false;
        public bool AutoUpdateHitbox { get; set; } = false;
        public FlxObjectDirection AllowCollisions { get; set; } = FlxObjectDirection.NONE;
        public bool Solid
        {
            get { return (AllowCollisions & FlxObjectDirection.ANY) > FlxObjectDirection.NONE; }
            set { AllowCollisions = value ? FlxObjectDirection.NONE : FlxObjectDirection.ANY; }
        }


        public FlxTypedEmitter(float X = 0, float Y = 0, int size = 0) : base(size)
        {
            SetPosition(X, Y);
            Exists = false;
        }

        public FlxTypedEmitter<T> Start(bool explode = true, float frequency = 0.1f, int quantity= 0)
        {
            Exists = true;
            Visible = true;
            Emitting = true;

            _explode = explode;
            Frequency = frequency;
            _quantity = quantity;

            _counter = 0;
            _timer = 0;

            _waitForKill = false;

            FlxG.Log.Info("Started " + this);
            return this;
        }
        #region Overrides
        public override void Update(GameTime gameTime)
        {
            //First Add Remove Pending Items from Members List
            base.Update(gameTime);


            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Emitting)
                if (_explode)
                    Explode();
                else
                    EmitContinuously(elapsed);
            else if (_waitForKill)
            {
                _timer += elapsed;

                if((Lifespan.Max > 0) && _timer > Lifespan.Max)
                {
                    Kill();
                }
            }
        }

        public override void Kill()
        {
            Emitting = false;
            _waitForKill = false;
            base.Kill();
        }

        protected override void Dispose(bool disposing)
        {
            Velocity = null;
            Scale = null;
            Drag = null;
            Acceleration = null;

            Blend = null;
            AngularAcceleration = null;
            AngularDrag = null;
            AngularVelocity = null;
            Angle = null;
            Speed = null;
            LaunchAngle = null;
            Lifespan = null;
            Alpha = null;
            Color = null;
            Elasticity = null;
            base.Dispose(disposing);
        }
        #endregion

        #region public Graphics
        public FlxTypedEmitter<T> LoadParticles(string graphicAssetPath, int quantity = 50, bool multiple = false)
        {
            MaxSize = quantity;
            for (int i = 0; i < quantity; i++)
                AddParticle(LoadParticle(graphicAssetPath, multiple));
            return this;
        }
        public T LoadParticle(string graphicAssetPath, bool multiple = false)
        {
            T particle = (T)Activator.CreateInstance(ParticleClass);
            particle.LoadGraphic(graphicAssetPath, multiple);
            if (multiple)
                particle.Animation.FrameIndex = FlxG.Random.Next(0, particle.Animation.TotalFrames);
            return particle;
        }
        public FlxTypedEmitter<T> MakeParticles(int width = 2, int height = 2, int quantity = 50)
        {
            return MakeParticles(width, height, Microsoft.Xna.Framework.Color.White, quantity);
        }

        public FlxTypedEmitter<T> MakeParticles(int width, int height, Color color, int quantity = 50)
        {
            for(int i =0; i < quantity; i++)
            {
                T particle = (T)Activator.CreateInstance(ParticleClass);
                particle.MakeGraphic(width, height, color);
                AddParticle(particle);
            }
            return this;
        }
        #endregion

        #region Public Methods
        public T EmitParticle()
        {
            T particle = Recycle(ParticleClass);
            particle.Reset(0, 0);

            particle.Immovable = Immovable;
            particle.Solid = Solid;
            particle.AllowCollisions = AllowCollisions;
            particle.AutoUpdateHitbox = AutoUpdateHitbox;

            if (Lifespan.Active)
                particle.Lifespan = FlxG.Random.NextFloat(Lifespan.Min, Lifespan.Max);
            if (Velocity.Active)
            {
                particle.VelocityRange.Active = particle.Lifespan > 0 && !particle.VelocityRange.Start.Equals(particle.VelocityRange.End);

                if (LaunchMode == FlxEmitterMode.CIRCLE)
                {
                    float particleAngle = 0;
                    if (LaunchAngle.Active)
                        particleAngle = FlxG.Random.NextFloat(LaunchAngle.Min, LaunchAngle.Max);

                    particle.VelocityRange.Start = FlxVelocity.VelocityFromAngle(particleAngle, FlxG.Random.NextFloat(Speed.Start.Min, Speed.Start.Max));
                    particle.VelocityRange.End = FlxVelocity.VelocityFromAngle(particleAngle, FlxG.Random.NextFloat(Speed.End.Min, Speed.End.Max));
                    particle.Velocity.X = particle.VelocityRange.Start.X;
                    particle.Velocity.Y = particle.VelocityRange.Start.Y;
                }
                else
                {
                    particle.VelocityRange.Start.X = FlxG.Random.NextFloat(Velocity.Start.Min.X, Velocity.Start.Max.X);
                    particle.VelocityRange.Start.Y = FlxG.Random.NextFloat(Velocity.Start.Min.Y, Velocity.Start.Max.Y);
                    particle.VelocityRange.End.X = FlxG.Random.NextFloat(Velocity.End.Min.X, Velocity.End.Max.X);
                    particle.VelocityRange.End.Y = FlxG.Random.NextFloat(Velocity.End.Min.Y, Velocity.End.Max.Y);
                    particle.Velocity.X = particle.VelocityRange.Start.X;
                    particle.Velocity.Y = particle.VelocityRange.Start.Y;
                }
            }
            else
                particle.VelocityRange.Active = false;

            particle.AngularVelocityRange.Active = particle.Lifespan > 0 && AngularVelocity.Start != AngularVelocity.End;
            if (!IgnoreAngularVelocity)
            {

                if (AngularAcceleration.Active)
                    particle.AngularAcceleration = FlxG.Random.NextFloat(AngularAcceleration.Start.Min, AngularAcceleration.Start.Max);

                if (AngularVelocity.Active)
                {
                    particle.AngularVelocityRange.Start = FlxG.Random.NextFloat(AngularVelocity.Start.Min, AngularVelocity.Start.Max);
                    particle.AngularVelocityRange.End = FlxG.Random.NextFloat(AngularVelocity.End.Min, AngularVelocity.End.Max);
                    particle.AngularVelocity = particle.AngularVelocityRange.Start;
                }

                if (AngularDrag.Active)
                    particle.AngularDrag = FlxG.Random.NextFloat(AngularDrag.Start.Min, AngularDrag.Start.Max);
            }
            else if(AngularVelocity.Active)
            {
                particle.AngularVelocity = (FlxG.Random.NextFloat(Angle.End.Min,Angle.End.Max) - FlxG.Random.NextFloat(Angle.Start.Min, Angle.Start.Max)) / FlxG.Random.NextFloat(Lifespan.Min, Lifespan.Max);
                particle.AngularVelocityRange.Active = false;
            }
            // Particle angle settings
            if (Angle.Active)
                particle.Angle = FlxG.Random.NextFloat(Angle.Start.Min, Angle.Start.Max);
            // Particle Scale settings
            if (Scale.Active)
            {
                particle.ScaleRange.Start.X = FlxG.Random.NextFloat(Scale.Start.Min.X, Scale.Start.Max.X);
                particle.ScaleRange.Start.Y = KeepScaleRatio ? particle.ScaleRange.Start.X : FlxG.Random.NextFloat(Scale.Start.Min.Y, Scale.Start.Max.Y);
                particle.ScaleRange.End.X = FlxG.Random.NextFloat(Scale.End.Min.X, Scale.End.Max.X);
                particle.ScaleRange.End.Y = KeepScaleRatio ? particle.ScaleRange.End.X : FlxG.Random.NextFloat(Scale.End.Min.Y, Scale.End.Max.Y);
                particle.ScaleRange.Active = particle.Lifespan > 0 && !particle.ScaleRange.Start.Equals(particle.ScaleRange.End);
                particle.Scale = new Vector2(particle.ScaleRange.Start.X, particle.ScaleRange.Start.Y);
                //if (particle.AutoUpdateHitbox)
                //    particle.UpdateHitbox();
            }
            else
                particle.ScaleRange.Active = false;

            // Particle Alpha settings
            if (Alpha.Active)
            {
                particle.AlphaRange.Start = FlxG.Random.NextFloat(Alpha.Start.Min, Alpha.Start.Max);
                particle.AlphaRange.End = FlxG.Random.NextFloat(Alpha.End.Min, Alpha.End.Max);
                particle.AlphaRange.Active = particle.Lifespan > 0 && particle.AlphaRange.Start != particle.AlphaRange.End;
                particle.Alpha = particle.AlphaRange.Start;
            }
            else
                particle.AlphaRange.Active = false;

            // Particle color settings
            if (Color.Active)
            {
                particle.ColorRange.Start = FlxG.Random.NextColor(Color.Start.Min, Color.Start.Max);
                particle.ColorRange.End = FlxG.Random.NextColor(Color.End.Min, Color.End.Max);
                particle.ColorRange.Active = particle.Lifespan > 0 && particle.ColorRange.Start != particle.ColorRange.End;
                particle.Color = particle.ColorRange.Start;
            }
            else
                particle.ColorRange.Active = false;

            // Particle Drag settings
            if (Drag.Active)
            {
                particle.DragRange.Start.X = FlxG.Random.NextFloat(Drag.Start.Min.X, Drag.Start.Max.X);
                particle.DragRange.Start.Y = FlxG.Random.NextFloat(Drag.Start.Min.Y, Drag.Start.Max.Y);
                particle.DragRange.End.X = FlxG.Random.NextFloat(Drag.End.Min.X, Drag.End.Max.X);
                particle.DragRange.End.Y = FlxG.Random.NextFloat(Drag.End.Min.Y, Drag.End.Max.Y);
                particle.DragRange.Active = particle.Lifespan > 0 && !particle.DragRange.Start.Equals(particle.DragRange.End);
                particle.Drag.X = particle.DragRange.Start.X;
                particle.Drag.Y = particle.DragRange.Start.Y;
            }
            else
                particle.DragRange.Active = false;

            // Particle Acceleration settings
            if (Acceleration.Active)
            {
                particle.AccelerationRange.Start.X = FlxG.Random.NextFloat(Acceleration.Start.Min.X, Acceleration.Start.Max.X);
                particle.AccelerationRange.Start.Y = FlxG.Random.NextFloat(Acceleration.Start.Min.Y, Acceleration.Start.Max.Y);
                particle.AccelerationRange.End.X = FlxG.Random.NextFloat(Acceleration.End.Min.X, Acceleration.End.Max.X);
                particle.AccelerationRange.End.Y = FlxG.Random.NextFloat(Acceleration.End.Min.Y, Acceleration.End.Max.Y);
                particle.AccelerationRange.Active = particle.Lifespan > 0
                    && !particle.AccelerationRange.Start.Equals(particle.AccelerationRange.End);
                particle.Acceleration.X = particle.AccelerationRange.Start.X;
                particle.Acceleration.Y = particle.AccelerationRange.Start.Y;
            }
            else
                particle.AccelerationRange.Active = false;

            // Particle elasticity settings
            if (Elasticity.Active)
            {
                particle.ElasticityRange.Start = FlxG.Random.NextFloat(Elasticity.Start.Min, Elasticity.Start.Max);
                particle.ElasticityRange.End = FlxG.Random.NextFloat(Elasticity.End.Min, Elasticity.End.Max);
                particle.ElasticityRange.Active = particle.Lifespan > 0 && particle.ElasticityRange.Start != particle.ElasticityRange.End;
                particle.Elasticity = particle.ElasticityRange.Start;
            }
            else
                particle.ElasticityRange.Active = false;

            // Set position
            particle.X = FlxG.Random.NextFloat(X, X + Width) - particle.Width / 2;
            particle.Y = FlxG.Random.NextFloat(Y, Y + Height) - particle.Height / 2;

            // Restart animation
            if (particle.Animation.CurrentAnimation != null)
                particle.Animation.CurrentAnimation.Restart();

            particle.OnEmit();


            return default;
        }

        public void FocusOn(FlxObject flxObject)
        {
            Vector2 mid = flxObject.MidPoint;
            X = mid.X - (((int)Width) >> 1);
            Y = mid.Y - (((int)Height) >> 1);
        }

        public void SetPosition(float X = 0, float Y = 0)
        {
            this.X = X;
            this.Y = Y;
        }

        public void SetSize(float width, float height)
        {
            Width = width;
            Height = height;
        }
        #endregion

        #region Internal Methods
        private void Explode()
        {
            int amount = _quantity;
            if (amount <= 0 || amount > Length)
                amount = Length;
            for (int i = 0; i < amount; i++)
                EmitParticle();
            OnFinished();
        }
        private void EmitContinuously(float elapsed)
        {
            if (Frequency <= 0)
                EmitParticleContinuously();
            else
            {
                _timer += elapsed;
                while(_timer > Frequency)
                {
                    _timer -= Frequency;
                    EmitParticleContinuously();
                }
            }
        }

        private void EmitParticleContinuously()
        {
            EmitParticle();
            _counter++;

            if (_quantity > 0 && _counter >= _quantity)
                OnFinished();
        }

        private void OnFinished()
        {
            Emitting = false;
            _waitForKill = true;
            _quantity = 0;
        }

        private void AddParticle(T particle)
        {
            Add(particle);
        }
        #endregion
    }
}
