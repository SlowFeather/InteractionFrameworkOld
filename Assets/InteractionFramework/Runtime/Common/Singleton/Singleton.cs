namespace InteractionFramework.Runtime
{
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        private static T ms_instance = default(T);

        public static T Instance
        {
            get
            {
                if (ms_instance == null)
                {
                    ms_instance = new T();
                }
                return ms_instance;
            }
        }

        public static void InitSingleton()
        {
            if (ms_instance == null)
            {
                ms_instance = new T();
            }
        }
    }

    //**********************************************************************

}
