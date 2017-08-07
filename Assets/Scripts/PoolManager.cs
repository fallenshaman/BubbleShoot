using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PoolManager : Singleton<PoolManager>
{
    public static PoolManager Instance
    {
        get
        {
            return (PoolManager)sInstance;
        }
        set
        {
            sInstance = value;
        }
    }

    public List<Pool> listPool = new List<Pool>();

    private Dictionary<string, Pool> dicPools = new Dictionary<string, Pool>();

    private bool initialized = false;

    public void Initiallize()
    {
        if (initialized)
            return;

        initialized = true;

        foreach (Pool pool in listPool)
        {
            pool.SetRoot(this.transform);
            pool.Preload();

            if (!dicPools.ContainsKey(pool.poolName))
            {
                dicPools[pool.poolName] = pool;
            }
        }
    }

    public Pool GetPool(string poolName)
    {
        Pool pool = null;
        if(!dicPools.TryGetValue(poolName, out pool))
        {
            Debug.LogError(string.Format("Pool [{0}] is not exist!!", poolName));
        }

        return pool;
    }
}

[Serializable]
public class Pool
{
    // 관리되는 아이템들이 위치할 Transform
    private Transform root = null;

    private bool preloaded = false;

    public string poolName;     // 풀 이름
    public int preLoadCount = 1;    // 미리 로드해둘 오브젝트 수
    public GameObject prefab;   // 풀에서 사용하는 프리펩

    // 풀에서 관리 되는 활성화 되어 있는 아이템 목록
    private List<GameObject> listSpawnedItems = new List<GameObject>();
    // 풀에서 관리 되는 비활성화 되어 있는 아이템 목록
    private Stack<GameObject> stackDespawnedItems = new Stack<GameObject>();
    
    public int TotalItemCount
    {
        get
        {
            return listSpawnedItems.Count + stackDespawnedItems.Count;
        }
    }

    public List<GameObject> SpawnedItems
    {
        get
        {
            return listSpawnedItems;
        }
    }


    

    public void SetRoot(Transform _root)
    {
        root = _root;
    }

    // 풀링할 아이템을 미리 생성한다.
    public void Preload()
    {
        if (preloaded)
        {
            Debug.LogError(string.Format("[Pool] pool {0} is already preloaded", poolName));
            return;
        }

        preloaded = true;


        // 생성할 프리펩이 설정되어 있지 않음
        if(prefab == null)
        {
            Debug.LogError(string.Format("[Pool] pool {0}'s prefab is missing", poolName));
            return;
        }

        // PreloadCount는 1보다 작을 수 없다.
        if (preLoadCount < 1)
            preLoadCount = 1;
        
        Load(preLoadCount);
    }

    private void Load(int loadCount)
    {
        if (loadCount < 1)
            loadCount = 1;

        // 미리 생성할 갯수에 맞게 생성한다.
        for (int i = 0; i < loadCount; ++i)
        {
            GameObject goItem = GameObject.Instantiate(prefab);
            Transform item = goItem.transform;

            item.parent = root;

            goItem.SetActive(false);
            stackDespawnedItems.Push(goItem);
        }
    }

    public GameObject Spawn()
    {
        GameObject goItem = null;

        // 생성된 모든 아이템 사용 중이면 미리 로딩해둔 갯수 만큼 추가 로딩한다.
        if(stackDespawnedItems.Count == 0)
        {
            Debug.LogError(string.Format("[Pool] pool {0} addtional load occur!!", poolName));
            Load(preLoadCount);
        }
        
        goItem = stackDespawnedItems.Pop();
        listSpawnedItems.Add(goItem);

        goItem.transform.parent = null;
        goItem.SetActive(true);

        return goItem;
    }

    public void Desapwn(GameObject item)
    {
        if(listSpawnedItems.Contains(item))
        {
            item.SetActive(false);
            item.transform.parent = root;

            listSpawnedItems.Remove(item);
            stackDespawnedItems.Push(item);
        }
    }

    public void DesapwnAll()
    {
        Debug.Log("DespawnALL " + listSpawnedItems.Count);

        for(int i = listSpawnedItems.Count -1; i >= 0; --i)
        {
            GameObject item = listSpawnedItems[i];
            
            item.SetActive(false);
            item.transform.parent = root;

            listSpawnedItems.Remove(item);
            stackDespawnedItems.Push(item);
        }
    }
}
