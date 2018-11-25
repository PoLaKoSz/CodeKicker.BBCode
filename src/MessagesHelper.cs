using System.Resources;

namespace CodeKicker.BBCode
{
    static class MessagesHelper
    {
        private static readonly ResourceManager _resMgr;



        static MessagesHelper()
        {
            _resMgr = new ResourceManager(typeof(Messages));
        }



        public static string GetString(string key)
        {
            return _resMgr.GetString(key);
        }

        public static string GetString(string key, params string[] parameters)
        {
            return string.Format(_resMgr.GetString(key), parameters);
        }
    }
}
