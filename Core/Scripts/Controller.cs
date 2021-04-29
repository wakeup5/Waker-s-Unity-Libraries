using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Waker
{
    public static class Controller
    {
        public static bool IsExist<T>() where T : Controller<T>
        {
            return Controller<T>.HasInstance;
        }

        public static T Get<T>() where T : Controller<T>
        {
            return Controller<T>.Instance;
        }

        public static T GetOrCreate<T>() where T : Controller<T>
        {
            if (!Controller<T>.HasInstance)
            {
                Controller<T>.CreateInstance();
            }

            return Controller<T>.Instance;
        }
    }

    /// <summary>
    /// 싱글턴인 컴포넌트를 정의. Controller는 싱글턴이면서 GameObject에 포함되는 컴포넌트를 지칭한다.
    /// </summary>
    public abstract class Controller<T> : MonoBehaviour where T : Controller<T>
    {
        private static T instance;
        private static bool isDontDestroyOnLoad = false;

        public static T Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                instance = GameObject.FindObjectOfType<T>();

                if (instance != null)
                {
                    return instance;
                }

                return null;
            }
        }
        public static T I => Instance;
        public static bool HasInstance => Instance != null;
        public static bool IsDontDestroyOnLoad
        {
            get => isDontDestroyOnLoad;
        }

        public static void SetDontDestroyOnLoad()
        {
            if (isDontDestroyOnLoad)
            {
                return;
            }

            isDontDestroyOnLoad = true;

            if (instance == null)
            {
                return;
            }

            DontDestroyOnLoad(instance);
        }

        public static T CreateInstance()
        {
            if (HasInstance)
            {
                return Instance;
            }

            return instance = new GameObject(typeof(T).Name).AddComponent<T>();
        }

        public static void DestroyInstance()
        {
            if (!HasInstance)
            {
                return;
            }

            Destroy(instance);
        }

        //public static T Get()
        //{
        //    return Instance;
        //}

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = (T)this;
            }
            else if (instance != this)
            {
                Destroy(this);
                return;
            }

            if (IsDontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        protected static T GetOrCreate()
		{
            return Instance ?? CreateInstance();
		}
    }
}