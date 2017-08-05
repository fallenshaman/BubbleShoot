using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T> {
    protected static Singleton<T> sInstance
    {
        get
        {
            if(!s_Instance)
            {
                T[] found = Object.FindObjectsOfType(typeof(T)) as T[];

                if(found.Length != 0)
                {
                    if(found.Length == 1)
                    {
                        if(s_Instance != found[0])
                        {
                            s_Instance = found[0];
                            s_Instance.gameObject.name = typeof(T).Name;

                            return s_Instance;
                        }
                    }
                    else
                    {
                        Debug.LogError(string.Format("[Singleton] {0} instance exist more than one... Destory all...", typeof(T).Name));

                        foreach (T item in found)
                        {
                            DestroyImmediate(item.gameObject);
                        }
                    }
                }

                GameObject go = new GameObject(typeof(T).Name, typeof(T));
                s_Instance = go.GetComponent<T>();
                DontDestroyOnLoad(go);
            }

            return s_Instance;
        }
        set
        {
            s_Instance = value as T;
        }
    }

    private static T s_Instance;



}
