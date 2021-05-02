using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Addressable : MonoBehaviour
{
    public static Addressable instance;

    private CAsset<GameObject> unit;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        unit = new CAsset<GameObject>("Unit");

        // check this
        StartCoroutine(LoadAll());
    }

    public IEnumerator LoadAll()
    {
        yield return StartCoroutine(unit.Load());
    }

    public GameObject GetUnit(string name) { return unit.Get(name); }

    public class CAsset<T> where T : Object
    {
        private Dictionary<string, T> factory;
        private string label;
        private bool isLoaded = false;

        public CAsset(string label)
        {
            factory = new Dictionary<string, T>();
            this.label = label;
        }

        public IEnumerator Load()
        {
            if (isLoaded)
                yield break;
            var asyncOperationHandle = Addressables.LoadAssetsAsync<T>(label, null);
            while (!asyncOperationHandle.IsDone)
                yield return null;
            foreach(var r in asyncOperationHandle.Result)
            {
                factory.Add(r.name, r);
            }
            isLoaded = true;
        }

        public T Get(string name)
        {
            if (factory.ContainsKey(name))
                return factory[name];
            else
                return null;
        }
    }
}