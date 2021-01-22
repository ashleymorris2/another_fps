using UnityEngine;

namespace Core
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour 
    {
        public static T Instance 
        { 
            get
            {
                if(instance == null)
                    instance = FindObjectOfType<T>();

                return instance;
            }
        }

        private static T instance;
    }
}