using System;

namespace Demo
{
    [Serializable]
    public class SaveFile
    {
        public int HighScore;
        public override string ToString()
        {
            return $"HighScore : {HighScore}";
        }
    }
}
