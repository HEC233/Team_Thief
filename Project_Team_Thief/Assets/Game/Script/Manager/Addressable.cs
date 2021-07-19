using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Addressable : MonoBehaviour
{
    public static Addressable instance;

    private CAsset<GameObject> unit;
    private CAsset<GameObject> prefab;
    private CAsset<TextAsset> text;
    private CAsset<Sprite> sprite;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            DestroyImmediate(this.gameObject);
        }

        unit = new CAsset<GameObject>("Unit");
        prefab = new CAsset<GameObject>("Prefab");
        text = new CAsset<TextAsset>("Text");
        sprite = new CAsset<Sprite>("Sprite");

        // check this
        StartCoroutine(LoadAll());
    }

    public IEnumerator LoadAll()
    {
        yield return StartCoroutine(unit.Load());
        yield return StartCoroutine(prefab.Load());
        yield return StartCoroutine(text.Load());
        yield return StartCoroutine(sprite.Load());
            
        var list = CSVReader.Read("PlayerData");

        for (int i = 0; i < list.Count; i++)
        {
            Debug.Log("stateLevel " + list[i]["stateLevel"] + " " +
                      "maxHp" + list[i]["maxHp"] + " " +
                      "runSpeed" + list[i]["runSpeed"] + " " +
                      "jumpPower " + list[i]["jumpPower"] + " " +
                      "dashRange " + list[i]["dashCoolTime"] + " " +
                      "deathCount " + list[i]["deathCount"] + " " +
                      "deathHP " + list[i]["deathHP"] + " " +
                      "critical " + list[i]["critical"] + " " +
                      "attackSpeed " + list[i]["attackSpeed"] + " " +
                      "def " + list[i]["def"] + " "
            );
        }
        
        Debug.Log(list[0]["stateLevel"].GetType());
    }

    public GameObject GetUnit(string name) { return unit.Get(name); }
    public GameObject GetPrefab(string name) { return prefab.Get(name); }
    public TextAsset GetText(string name) { return text.Get(name); }
    public Sprite GetSprite(string name) { return sprite.Get(name); }

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
                if (factory.ContainsKey(r.name))
                {
                    Debug.LogWarning("there is already exist key " + r.name + " in " + typeof(T).Name);
                }
                else
                {
                    factory.Add(r.name, r);
                }
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