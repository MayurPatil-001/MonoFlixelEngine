using Engine.Group;
using Engine.MathUtils;
using System;
using System.Collections.Generic;

namespace Engine.Systems
{
    public delegate bool ProcessingCallback(FlxObject object1, FlxObject object2);
    public delegate void NotifyCallback(FlxObject object1, FlxObject object2);
    public class FlxQuadTree : RectangleF
    {
        public static int A_LIST = 0;
        public static int B_LIST = 1;
        public static int Divisions;

        public bool Exists;
        private bool _canSubdivide;
        private FlxLinkedList _headA;
        private FlxLinkedList _tailA;
        private FlxLinkedList _headB;
        private FlxLinkedList _tailB;
        private static int _min;

        private FlxQuadTree _northWestTree;
        private FlxQuadTree _northEastTree;
        private FlxQuadTree _southEastTree;
        private FlxQuadTree _southWestTree;

        private float _leftEdge;
        private float _rightEdge;
        private float _topEdge;
        private float _bottomEdge;

        private float _halfWidth;
        private float _halfHeight;
        private float _midpointX;
        private float _midpointY;

        private static FlxObject _object;
        private static float _objectLeftEdge;
        private static float _objectTopEdge;
        private static float _objectRightEdge;
        private static float _objectBottomEdge;

        private static int _list;
        private static bool _useBothLists;

        public static ProcessingCallback _processingCallback;
        public static NotifyCallback _notifyCallback;

        private static FlxLinkedList _iterator;

        private static float _objectHullX;
        private static float _objectHullY;
        private static float _objectHullWidth;
        private static float _objectHullHeight;
        private static float _checkObjectHullX;
        private static float _checkObjectHullY;
        private static float _checkObjectHullWidth;
        private static float _checkObjectHullHeight;

        public static int _NUM_CACHED_QUAD_TREES = 0;
        private static FlxQuadTree _cachedTreesHead;

        FlxQuadTree next;

        public FlxQuadTree(float x, float y, float width, float height, FlxQuadTree parent = null)
        {
            Set(x, y, width, height);
            Reset(x, y, width, height, parent);
        }

        public static FlxQuadTree Recycle(float x, float y, float width, float height, FlxQuadTree parent = null)
        {
            if (_cachedTreesHead != null)
            {
                FlxQuadTree cachedTree = _cachedTreesHead;
                _cachedTreesHead = _cachedTreesHead.next;
                _NUM_CACHED_QUAD_TREES--;

                cachedTree.Reset(x, y, width, height, parent);
                return cachedTree;
            }
            else
                return new FlxQuadTree(x, y, width, height, parent);
        }

        public static void ClearCache()
        {
            while (_cachedTreesHead != null)
            {
                var node = _cachedTreesHead;
                _cachedTreesHead = _cachedTreesHead.next;
                node.next = null;
            }
            _NUM_CACHED_QUAD_TREES = 0;
        }
        public void Reset(float x, float y, float width, float height, FlxQuadTree parent = null)
        {
            Exists = true;
            Set(x, y, width, height);

            _headA = _tailA = FlxLinkedList.Recycle();
            _headB = _tailB = FlxLinkedList.Recycle();

            if (parent != null)
            {
                FlxLinkedList iterator;
                FlxLinkedList ot;
                if (parent._headA.FlxObject != null)
                {
                    iterator = parent._headA;
                    while (iterator != null)
                    {
                        if (_tailA.FlxObject != null)
                        {
                            ot = _tailA;
                            _tailA = FlxLinkedList.Recycle();
                            ot.Next = _tailA;
                        }
                        _tailA.FlxObject = iterator.FlxObject;
                        iterator = iterator.Next;
                    }
                }
                if (parent._headB.FlxObject != null)
                {
                    iterator = parent._headB;
                    while (iterator != null)
                    {
                        if (_tailB.FlxObject != null)
                        {
                            ot = _tailB;
                            _tailB = FlxLinkedList.Recycle();
                            ot.Next = _tailB;
                        }
                        _tailB.FlxObject = iterator.FlxObject;
                        iterator = iterator.Next;
                    }
                }
            }
            else
            {
                _min = (int)Math.Floor((Width + Height) / (2 * Divisions));
            }
            _canSubdivide = Width > _min || Height > _min;

            _northWestTree = null;
            _northEastTree = null;
            _southWestTree = null;
            _southEastTree = null;
            _leftEdge = X;
            _rightEdge = Right;
            _halfWidth = Width / 2;
            _midpointX = _leftEdge + _halfWidth;
            _topEdge = Y;
            _bottomEdge = Bottom;
            _halfHeight = Height / 2;
            _midpointY = _topEdge + _halfHeight;
        }

        public void Dispose()
        {
            _headA = null;
            _headB = null;

            _tailA = null;
            _tailB = null;

            _northWestTree = null;
            _northEastTree = null;

            _southEastTree = null;
            _southWestTree = null;

            _object = null;
            _processingCallback = null;
            _notifyCallback = null;

            Exists = false;

            next = _cachedTreesHead;
            _cachedTreesHead = this;
            _NUM_CACHED_QUAD_TREES++;
        }

        public void Load(FlxBasic objectOrGroup1, FlxBasic objectOrGroup2 = null, NotifyCallback notifyCallback = null, ProcessingCallback processingCallback = null)
        {
            Add(objectOrGroup1, A_LIST);
            if (objectOrGroup2 != null)
            {
                Add(objectOrGroup2, B_LIST);
                _useBothLists = true;
            }
            else
                _useBothLists = false;
            _notifyCallback = notifyCallback;
            _processingCallback = processingCallback;
        }

        public void Add(FlxBasic objectOrGroup, int list)
        {
            _list = list;
            FlxTypedGroup<FlxBasic> group = FlxTypedGroup<FlxBasic>.ResolveGroup(objectOrGroup);
            if (group != null)
            {
                int i = 0;
                FlxBasic basic;
                List<FlxBasic> members = group.Members;
                int l = group.Length;
                while (i < l)
                {
                    basic = members[i++];
                    if (basic != null && basic.Exists)
                    {
                        group = FlxTypedGroup<FlxBasic>.ResolveGroup(basic);
                        if (group != null)
                        {
                            Add(group, list);
                        }
                        else
                        {
                            _object = (FlxObject)basic;
                            if (_object.Exists && _object.AllowCollisions != FlxObjectDirection.NONE)
                            {
                                _objectLeftEdge = _object.X;
                                _objectTopEdge = _object.Y;
                                _objectRightEdge = _object.X + _object.Width;
                                _objectBottomEdge = _object.Y + _object.Height;
                                AddObject();
                            }
                        }
                    }
                }
            }
            else
            {
                _object = (FlxObject)objectOrGroup;
                if (_object.Exists && _object.AllowCollisions != FlxObjectDirection.NONE)
                {
                    _objectLeftEdge = _object.X;
                    _objectTopEdge = _object.Y;
                    _objectRightEdge = _object.X + _object.Width;
                    _objectBottomEdge = _object.Y + _object.Height;
                    AddObject();
                }
            }
        }

        private void AddObject()
        {
            // If this quad (not its children) lies entirely inside this object, add it here
            if (!_canSubdivide
                || (_leftEdge >= _objectLeftEdge && _rightEdge <= _objectRightEdge && _topEdge >= _objectTopEdge && _bottomEdge <= _objectBottomEdge))
            {
                AddToList();
                return;
            }

            // See if the selected object fits completely inside any of the quadrants
            if ((_objectLeftEdge > _leftEdge) && (_objectRightEdge < _midpointX))
            {
                if ((_objectTopEdge > _topEdge) && (_objectBottomEdge < _midpointY))
                {
                    if (_northWestTree == null)
                    {
                        _northWestTree = FlxQuadTree.Recycle(_leftEdge, _topEdge, _halfWidth, _halfHeight, this);
                    }
                    _northWestTree.AddObject();
                    return;
                }
                if ((_objectTopEdge > _midpointY) && (_objectBottomEdge < _bottomEdge))
                {
                    if (_southWestTree == null)
                    {
                        _southWestTree = FlxQuadTree.Recycle(_leftEdge, _midpointY, _halfWidth, _halfHeight, this);
                    }
                    _southWestTree.AddObject();
                    return;
                }
            }
            if ((_objectLeftEdge > _midpointX) && (_objectRightEdge < _rightEdge))
            {
                if ((_objectTopEdge > _topEdge) && (_objectBottomEdge < _midpointY))
                {
                    if (_northEastTree == null)
                    {
                        _northEastTree = FlxQuadTree.Recycle(_midpointX, _topEdge, _halfWidth, _halfHeight, this);
                    }
                    _northEastTree.AddObject();
                    return;
                }
                if ((_objectTopEdge > _midpointY) && (_objectBottomEdge < _bottomEdge))
                {
                    if (_southEastTree == null)
                    {
                        _southEastTree = FlxQuadTree.Recycle(_midpointX, _midpointY, _halfWidth, _halfHeight, this);
                    }
                    _southEastTree.AddObject();
                    return;
                }
            }

            // If it wasn't completely contained we have to check out the partial overlaps
            if ((_objectRightEdge > _leftEdge) && (_objectLeftEdge < _midpointX) && (_objectBottomEdge > _topEdge) && (_objectTopEdge < _midpointY))
            {
                if (_northWestTree == null)
                {
                    _northWestTree = FlxQuadTree.Recycle(_leftEdge, _topEdge, _halfWidth, _halfHeight, this);
                }
                _northWestTree.AddObject();
            }
            if ((_objectRightEdge > _midpointX) && (_objectLeftEdge < _rightEdge) && (_objectBottomEdge > _topEdge) && (_objectTopEdge < _midpointY))
            {
                if (_northEastTree == null)
                {
                    _northEastTree = FlxQuadTree.Recycle(_midpointX, _topEdge, _halfWidth, _halfHeight, this);
                }
                _northEastTree.AddObject();
            }
            if ((_objectRightEdge > _midpointX) && (_objectLeftEdge < _rightEdge) && (_objectBottomEdge > _midpointY) && (_objectTopEdge < _bottomEdge))
            {
                if (_southEastTree == null)
                {
                    _southEastTree = FlxQuadTree.Recycle(_midpointX, _midpointY, _halfWidth, _halfHeight, this);
                }
                _southEastTree.AddObject();
            }
            if ((_objectRightEdge > _leftEdge) && (_objectLeftEdge < _midpointX) && (_objectBottomEdge > _midpointY) && (_objectTopEdge < _bottomEdge))
            {
                if (_southWestTree == null)
                {
                    _southWestTree = FlxQuadTree.Recycle(_leftEdge, _midpointY, _halfWidth, _halfHeight, this);
                }
                _southWestTree.AddObject();
            }
        }

        private void AddToList()
        {
            FlxLinkedList ot;
            if (_list == A_LIST)
            {
                if (_tailA.FlxObject != null)
                {
                    ot = _tailA;
                    _tailA = FlxLinkedList.Recycle();
                    ot.Next = _tailA;
                }
                _tailA.FlxObject = _object;
            }
            else
            {
                if (_tailB.FlxObject != null)
                {
                    ot = _tailB;
                    _tailB = FlxLinkedList.Recycle();
                    ot.Next = _tailB;
                }
                _tailB.FlxObject = _object;
            }
            if (!_canSubdivide)
            {
                return;
            }
            if (_northWestTree != null)
            {
                _northWestTree.AddToList();
            }
            if (_northEastTree != null)
            {
                _northEastTree.AddToList();
            }
            if (_southEastTree != null)
            {
                _southEastTree.AddToList();
            }
            if (_southWestTree != null)
            {
                _southWestTree.AddToList();
            }
        }

        public bool Execute()
        {
            bool overlapProcessed = false;

            if (_headA.FlxObject != null)
            {
                var iterator = _headA;
                while (iterator != null)
                {
                    _object = iterator.FlxObject;
                    if (_useBothLists)
                    {
                        _iterator = _headB;
                    }
                    else
                    {
                        _iterator = iterator.Next;
                    }
                    if (_object != null && _object.Exists && _object.AllowCollisions > 0 && _iterator != null && _iterator.FlxObject != null && OverlapNode())
                    {
                        overlapProcessed = true;
                    }
                    iterator = iterator.Next;
                }
            }

            // Advance through the tree by calling overlap on each child
            if ((_northWestTree != null) && _northWestTree.Execute())
            {
                overlapProcessed = true;
            }
            if ((_northEastTree != null) && _northEastTree.Execute())
            {
                overlapProcessed = true;
            }
            if ((_southEastTree != null) && _southEastTree.Execute())
            {
                overlapProcessed = true;
            }
            if ((_southWestTree != null) && _southWestTree.Execute())
            {
                overlapProcessed = true;
            }

            return overlapProcessed;
        }

        private bool OverlapNode()
        {
            // Calculate bulk hull for _object
            _objectHullX = (_object.X < _object.Last.X) ? _object.X : _object.Last.X;
            _objectHullY = (_object.Y < _object.Last.Y) ? _object.Y : _object.Last.Y;
            _objectHullWidth = _object.X - _object.Last.X;
            _objectHullWidth = _object.Width + ((_objectHullWidth > 0) ? _objectHullWidth : -_objectHullWidth);
            _objectHullHeight = _object.Y - _object.Last.Y;
            _objectHullHeight = _object.Height + ((_objectHullHeight > 0) ? _objectHullHeight : -_objectHullHeight);

            // Walk the list and check for overlaps
            bool overlapProcessed = false;
            FlxObject checkObject;

            while (_iterator != null)
            {
                checkObject = _iterator.FlxObject;
                if (_object == checkObject || !checkObject.Exists || checkObject.AllowCollisions <= 0)
                {
                    _iterator = _iterator.Next;
                    continue;
                }

                // Calculate bulk hull for checkObject
                _checkObjectHullX = (checkObject.X < checkObject.Last.X) ? checkObject.X : checkObject.Last.X;
                _checkObjectHullY = (checkObject.Y < checkObject.Last.Y) ? checkObject.Y : checkObject.Last.Y;
                _checkObjectHullWidth = checkObject.X - checkObject.Last.X;
                _checkObjectHullWidth = checkObject.Width + ((_checkObjectHullWidth > 0) ? _checkObjectHullWidth : -_checkObjectHullWidth);
                _checkObjectHullHeight = checkObject.Y - checkObject.Last.Y;
                _checkObjectHullHeight = checkObject.Height + ((_checkObjectHullHeight > 0) ? _checkObjectHullHeight : -_checkObjectHullHeight);

                // Check for intersection of the two hulls
                if ((_objectHullX + _objectHullWidth > _checkObjectHullX)
                    && (_objectHullX < _checkObjectHullX + _checkObjectHullWidth)
                    && (_objectHullY + _objectHullHeight > _checkObjectHullY)
                    && (_objectHullY < _checkObjectHullY + _checkObjectHullHeight))
                {
                    // Execute callback functions if they exist
                    if (_processingCallback == null || _processingCallback(_object, checkObject))
                    {
                        overlapProcessed = true;
                        _notifyCallback?.Invoke(_object, checkObject);
                    }
                }
                if (_iterator != null)
                {
                    _iterator = _iterator.Next;
                }
            }

            return overlapProcessed;
        }

    }
}
