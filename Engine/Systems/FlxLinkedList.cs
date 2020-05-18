namespace Engine.Systems
{
    public class FlxLinkedList
    {
        public static int _NUM_CACHED_FLX_LIST = 0;
        static FlxLinkedList _cachedListsHead;
        public FlxObject FlxObject;
        public FlxLinkedList Next;
        public bool Exists = true;

        private FlxLinkedList()
        {

        }

        public static FlxLinkedList Recycle()
        {
            if (_cachedListsHead != null)
            {
                FlxLinkedList cachedList = _cachedListsHead;
                _cachedListsHead = _cachedListsHead.Next;
                _NUM_CACHED_FLX_LIST--;

                cachedList.Exists = true;
                cachedList.Next = null;
                return cachedList;
            }
            else
                return new FlxLinkedList();
        }

        public static void ClearCache()
        {
            while(_cachedListsHead != null)
            {
                FlxLinkedList node = _cachedListsHead;
                _cachedListsHead = _cachedListsHead.Next;
                node.FlxObject = null;
                node.Next = null;
            }
            _NUM_CACHED_FLX_LIST = 0;
        }


        public void Dispose()
        {
            if (!Exists)
                return;
            FlxObject = null;
            if (Next != null)
                Next.Dispose();
            Exists = false;

            Next = _cachedListsHead;
            _cachedListsHead = this;
            _NUM_CACHED_FLX_LIST++;
        }
    }
}
