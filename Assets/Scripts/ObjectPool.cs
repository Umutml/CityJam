using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private int initialSize = 30;
    [SerializeField] private GameObject elementPrefab;
    private readonly Stack<GameObject> _pool = new();

    private void Start()
    {
        Initialize(elementPrefab);
    }


    private void Initialize(GameObject prefab)
    {
        elementPrefab = prefab;

        // Pre instantiate initialSize number of objects
        for (var i = 0; i < initialSize; i++)
        {
            var obj = Instantiate(prefab,this.transform);
            obj.SetActive(false);
            _pool.Push(obj);
        }
    }

    public GameObject Get()
    {
        if (_pool.Count > 0)
        {
            var obj = _pool.Pop();
            // obj.SetActive(true);
            return obj;
        }

        // If the pool is empty, instantiate a new object
        return Instantiate(elementPrefab, this.transform);
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        _pool.Push(obj);
    }
}
