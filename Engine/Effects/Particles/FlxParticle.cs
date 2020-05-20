using Engine.Extensions;
using Engine.Utils.Helpers;
using Microsoft.Xna.Framework;

namespace Engine.Effects.Particles
{
    public interface IFlxParticle
    {
        float Lifespan { get; set; }
        float Age { get; set; }
        float Percent { get; set; }
        bool AutoUpdateHitbox { get; set; }
        FlxRange<Vector2> VelocityRange { get; set; }
        FlxRange<float> AngularVelocityRange { get; set; }
        FlxRange<Vector2> ScaleRange { get; set; }
        FlxRange<float> AlphaRange { get; set; }
        FlxRange<Color> ColorRange { get; set; }
        FlxRange<Vector2> DragRange { get; set; }
        FlxRange<Vector2> AccelerationRange { get; set; }
        FlxRange<float> ElasticityRange { get; set; }

        void OnEmit();
    }
    public class FlxParticle : FlxSprite, IFlxParticle
    {
        float _delta = 0;
        public float Lifespan { get; set; } = 0f;
        public float Age { get; set; } = 0f;
        public float Percent { get; set; } = 0f;
        public bool AutoUpdateHitbox { get; set; } = false;
        public FlxRange<Vector2> VelocityRange { get; set; }
        public FlxRange<float> AngularVelocityRange { get; set; }
        public FlxRange<Vector2> ScaleRange { get; set; }
        public FlxRange<float> AlphaRange { get; set; }
        public FlxRange<Color> ColorRange { get; set; }
        public FlxRange<Vector2> DragRange { get; set; }
        public FlxRange<Vector2> AccelerationRange { get; set; }
        public FlxRange<float> ElasticityRange { get; set; }


        public FlxParticle():base(0,0)
        {
            VelocityRange = new FlxRange<Vector2>(Vector2.Zero, Vector2.Zero);
            AngularVelocityRange = new FlxRange<float>(0);
            ScaleRange = new FlxRange<Vector2>(Vector2.One, Vector2.One);
            AlphaRange = new FlxRange<float>(1, 1);
            ColorRange = new FlxRange<Color>(Color.White);
            DragRange = new FlxRange<Vector2>(Vector2.Zero, Vector2.Zero);
            AccelerationRange = new FlxRange<Vector2>(Vector2.Zero, Vector2.Zero);
            ElasticityRange = new FlxRange<float>(0);

            Exists = false;
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Age < Lifespan)
                Age += elapsed;
            if (Age >= Lifespan && Lifespan != 0)
                Kill();
            else
            {
                _delta = elapsed / Lifespan;
                Percent = Age / Lifespan;

                if (VelocityRange.Active)
                {
                    Velocity.X += (VelocityRange.End.X - VelocityRange.Start.X) * _delta;
                    Velocity.Y += (VelocityRange.End.Y - VelocityRange.Start.Y) * _delta;
                }

                if (AngularVelocityRange.Active)
                {
                    AngularVelocity += (AngularVelocityRange.End - AngularVelocityRange.Start) * _delta;
                }
                if (ScaleRange.Active)
                {
                    Scale = new Vector2(
                    Scale.X + (ScaleRange.End.X - ScaleRange.Start.X) * _delta,
                    Scale.Y + (ScaleRange.End.Y - ScaleRange.Start.Y) * _delta
                    );
                }
                if (AlphaRange.Active)
                {
                    Alpha += (AlphaRange.End - AlphaRange.Start) * _delta;
                }
                if (ColorRange.Active)
                {
                    Color = Color.Interpolate(ColorRange.Start, ColorRange.End, Percent);
                }
                if (DragRange.Active)
                {
                    Drag.X = (DragRange.End.X - DragRange.Start.X) * _delta;
                    Drag.Y = (DragRange.End.Y - DragRange.Start.Y) * _delta;
                }
                if (AccelerationRange.Active)
                {
                    Acceleration.X = (AccelerationRange.End.X - AccelerationRange.Start.X) * _delta;
                    Acceleration.Y = (AccelerationRange.End.Y - AccelerationRange.Start.Y) * _delta;
                }
                if (ElasticityRange.Active)
                {
                    Elasticity += (ElasticityRange.End - ElasticityRange.Start) * _delta;
                }
            }
            base.Update(gameTime);
        }

        public override void Reset(float x, float y)
        {
            base.Reset(x, y);
            Age = 0;
            Visible = true;
        }

        public virtual void OnEmit()
        {
            
        }

        protected override void Dispose(bool disposing)
        {
            VelocityRange = null;
            AngularVelocityRange = null;
            ScaleRange = null;
            AlphaRange = null;
            ColorRange = null;
            DragRange = null;
            AccelerationRange = null;
            ElasticityRange = null;
            base.Dispose(disposing);
        }
    }
}
