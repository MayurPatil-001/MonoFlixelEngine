using System.Diagnostics;

namespace Engine.Systems.FrontEnds
{
    public class LogFrontEnd
    {
        public static string INFO = "INFO";
        public static string WARN = "WARN";
        public static string ERROR = "ERROR";
        public void Log(object obj, string category)
        {
#if DEBUG
            Debug.WriteLine(obj, category);
#endif
        }

        public void Warn(object obj)
        {
            Log(obj, WARN);
        }

        public void Info(object obj)
        {
            Log(obj, INFO);
        }

        public void Error(object obj)
        {
            Log(obj, ERROR);
        }
    }
}
