using System.Collections.Generic;
using UnityEngine;

public class BarbarianObjectPool<T> where T : Component
{
    private T prefab;
    private Queue<T> objects = new Queue<T>();

    public BarbarianObjectPool(T prefab, int initialSize)
    {
        this.prefab = prefab;
        for (int i = 0; i < initialSize; i++)
        {
            AddObject();
        }
    }

    public T Get()
    {
        if (objects.Count == 0)
            AddObject();

        T obj = objects.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        objects.Enqueue(obj);
    }

    private void AddObject()
    {
        T newObj = GameObject.Instantiate(prefab);
        newObj.gameObject.SetActive(false);
        objects.Enqueue(newObj);
    }
}
