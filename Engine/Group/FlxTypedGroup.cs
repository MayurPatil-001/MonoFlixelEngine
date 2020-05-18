using Engine.Extensions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Engine.Group
{
    public delegate bool OverlapsCallback(FlxBasic basic, float x, float y, bool inScreenSpace, FlxCamera camera);
    public class FlxTypedGroup<T> : FlxBasic where T: FlxBasic
    {
        private int _maxSize = 0;
        private int _marker = 0;
        private readonly List<T> _toBeAdded;
        private readonly List<T> _toBeRemoved;
        private FlxTypedGroup<FlxBasic> _flxBasicGroup;
        public delegate T ObjectFactory();

        public List<T> Members { get; private set; }
        public FlxTypedGroup<FlxBasic> _FlxBasicGroupRef => GetBasicTypedGroup();
        public int Length { get { return Members.Count; } }
        public int MaxSize 
        {
            get { return _maxSize; } 
            set 
            { 
                _maxSize = Math.Abs(value);
                if (_marker >= MaxSize)
                    _marker = 0;
            }
        }
        public T FirstAvailable { get { return GetFirstAvailable(); } }
        public T FirstExisting { get { return GetFirstExisting(); } }
        public T FirstAlive { get { return GetFirstAlive(); } }
        public T FirstDead { get { return GetFirstDead(); } }


        #region Rendering
        /// <summary>
        /// Internal Use only
        /// </summary>
        public FlxCamera CurrentCamera { get; set; }
        #endregion
        public FlxTypedGroup(int maxSize = 0)
        {
            MaxSize = maxSize;
            Members = new List<T>();
            FlixelType = FlxType.GROUP;
            _toBeAdded = new List<T>();
            _toBeRemoved = new List<T>();
        }

        public FlxTypedGroup<FlxBasic> GetBasicTypedGroup()
        {
            if (_flxBasicGroup == null)
                _flxBasicGroup = new FlxTypedGroup<FlxBasic>();
            foreach(T baisc in Members)
                if(!_flxBasicGroup.Members.Contains(baisc))
                    _flxBasicGroup.Add(baisc, true);
            return _flxBasicGroup;
        }

        #region Overrides
        public override void Update(GameTime gameTime)
        {
            if (_toBeAdded.Count > 0)
            {
                foreach (T basic in _toBeAdded)
                    Members.Add(basic);
                _toBeAdded.Clear();
            }

            if (_toBeRemoved.Count > 0)
            {
                foreach (T basic in _toBeRemoved)
                    Members.Remove(basic);
                _toBeRemoved.Clear();
            }
            
            foreach (T basic in Members)
                if (basic != null && basic.Exists && basic.Active)
                    basic.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (T basic in Members)
            {
                if (basic != null && basic.Exists && basic.Visible)
                {
                    PreDraw(basic);
                    basic.Draw(gameTime);
                }
            }
        }

        private void PreDraw(T basic)
        {
            basic.SpriteBatch = SpriteBatch;
            if (basic is FlxTypedGroup<T>)
                foreach (FlxBasic flxBasic in (basic as FlxTypedGroup<T>).Members)
                     PreDraw((T)flxBasic);
        }

#if DEBUG
        public override void DebugDrawBoundigbox(GameTime gameTime)
        {
            foreach (T basic in Members)
            {
                if (basic != null && basic.Exists && basic.Visible)
                {
                    PreDraw(basic);
                    basic.DebugDrawBoundigbox(gameTime);
                }
            }
        }
        public override void DebugDrawHitBox(GameTime gameTime)
        {
            foreach (T basic in Members)
            {
                if (basic != null && basic.Exists && basic.Visible)
                {
                    PreDraw(basic);
                    basic.DebugDrawHitBox(gameTime);
                }
            }
        }
#endif

        protected override void Dispose(bool disposing)
        {
            foreach (T t in _toBeAdded)
                t.Dispose();
            foreach (T t in _toBeRemoved)
                t.Dispose();
            foreach (T t in Members)
                t.Dispose();
            Clear();
            //_FlxBasicGroupRef.Dispose();
            base.Dispose(disposing);
#if DEBUG
            FlxG.Log.Info("TypedGroup Disposed for" + this);
#endif
        }
        public override void Kill()
        {
            foreach (T t in _toBeAdded)
                t.Kill();
            foreach (T t in _toBeRemoved)
                t.Kill();
            foreach (T t in Members)
                t.Kill();
            base.Kill();
        }
        public override void Revive()
        {
            foreach (T t in _toBeAdded)
                t.Revive();
            foreach (T t in _toBeRemoved)
                t.Revive();
            foreach (T t in Members)
                t.Revive();
            base.Revive();
        }
        #endregion

        #region public Utils
        public virtual T Add(T obj, bool force = false)
        {
            if (obj == null)
            {
                FlxG.Log.Error("Cannot add a `null` object to a FlxGroup.");
                return null;
            }
            if (force)
                Members.Add(obj);
            else
                _toBeAdded.Add(obj);
            return obj;
        }

        public virtual T Remove(T obj)
        {
            if (Members == null)
                return null;
            if (obj == null)
            {
                FlxG.Log.Error("Cannot remove a `null` object to a FlxGroup.");
                return null;
            }
            _toBeRemoved.Remove(obj);
            return obj;
        }

        public virtual void Sort(Comparison<T> comparison)
        {
            Members.Sort(comparison);
        }

        public T GetFirstAvailable(Type objectType = null, bool force = false)
        {
            foreach(T obj in Members)
                if(obj != null && !obj.Exists && (objectType == null || obj.GetType() == objectType))
                {
                    if (force && !obj.GetType().Name.Equals(objectType.Name))
                        continue;
                    return obj;
                }
            return default;
        }

        public T GetFirstExisting()
        {
            foreach (T obj in Members)
                if (obj != null && obj.Exists)
                    return obj;
            return default;
        }

        public T GetFirstAlive()
        {
            foreach (T obj in Members)
                if (obj != null && obj.Exists && obj.Alive)
                    return obj;
            return default;
        }
        public T GetFirstDead()
        {
            foreach (T obj in Members)
                if (obj != null && !obj.Alive)
                    return obj;
            return default;
        }
        public void Clear()
        {
            _toBeAdded.Clear();
            _toBeRemoved.Clear();
            Members.Clear();

#if DEBUG
            FlxG.Log.Info("TypedGroup Clear for" + this);
#endif
        }

        public T GetRandom(int startIndex = 0, int length = 0)
        {
            if (startIndex < 0)
                startIndex = 0;
            if (length <= 0 || length > Length)
                length = Length;
            return FlxG.Random.GetObject(Members, null, startIndex, length);
        }

        public T Recycle(Type objectClass= default, ObjectFactory objectFactory = null, bool force = false, bool revive = true)
        {
            T basic = null;

            if(MaxSize > 0)
            {
                if (Length < MaxSize)
                    return RecycleCreateObject(objectClass, objectFactory);
                else
                {
                    basic = Members[_marker++];
                    if (_marker >= MaxSize)
                        _marker = 0;
                    if (revive)
                        basic.Revive();

                    return basic;
                }
            }
            else
            {
                basic = GetFirstAvailable(objectClass, force);
                if(basic != null)
                {
                    if (revive)
                        basic.Revive();
                    return basic;
                }
                return RecycleCreateObject(objectClass, objectFactory);
            }
        }


        private T RecycleCreateObject(Type objectClass = default, ObjectFactory objectFactory = null)
        {
            T obj = null;
            if (objectFactory != null)
            {
                obj = objectFactory();
                Add(obj);
            }else if(objectClass != null)
            {
                obj = (T)Activator.CreateInstance(objectClass);
                Add(obj);
            }
            return obj;
        }

        public static FlxTypedGroup<FlxBasic> ResolveGroup(FlxBasic objectOrGroup)
        {
            FlxTypedGroup<FlxBasic> group = null;
            if(objectOrGroup != null)
            {
                if(objectOrGroup.FlixelType == FlxType.GROUP)
                {
                    group = (FlxTypedGroup<FlxBasic>)objectOrGroup;
                }
                else if(objectOrGroup.FlixelType == FlxType.SPRITEGROUP)
                {
                    FlxTypedSpriteGroup<FlxSprite> spriteGroup = (FlxTypedSpriteGroup<FlxSprite>)objectOrGroup;
                    group = spriteGroup.Group._FlxBasicGroupRef;
                }
            }
            return group;
        }


        public static bool Overlaps(OverlapsCallback callback, FlxTypedGroup<FlxBasic> group, float x, float y, bool inScreenSpace, FlxCamera camera)
        {
            bool result = false;
            if(group != null)
            {
                int i = 0;
                int l = group.Length;
                FlxBasic basic;

                while(i < l)
                {
                    basic = group.Members[i++];
                    if (basic != null && callback(basic, x, y, inScreenSpace, camera))
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
