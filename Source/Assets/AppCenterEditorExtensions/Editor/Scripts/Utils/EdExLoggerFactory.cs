
//Factory.
namespace AppCenterEditor
{
    class EdExLoggerFactory
    {
        private static IEdExLogger _instance;

        public static IEdExLogger LoggerInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocalLogger();
                }
                return _instance;                    
            }
        }

        private EdExLoggerFactory()
        {
        }        
    }
}
