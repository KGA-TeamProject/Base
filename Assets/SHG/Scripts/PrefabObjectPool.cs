using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabObjectPool: Singleton<PrefabObjectPool> 
{

  new public static void CreateInstance() {
    Singleton<PrefabObjectPool>.CreateInstance(); 
  }

  Dictionary<string, (UnityEngine.GameObject prefab, int size)> pools = new();
  public const int POOL_SIZE = 5;

  Dictionary<string, Queue<UnityEngine.GameObject>> pooledObjects = new();

  Queue<UnityEngine.GameObject> GetQueue(string prefabName) {
    if (this.pooledObjects.TryGetValue(prefabName, out Queue<UnityEngine.GameObject> queue)) {
      return (queue);
    }
    return (null);
  }

  Queue<UnityEngine.GameObject> CreateQueue(string prefabName, UnityEngine.GameObject prefab) {
    Queue<UnityEngine.GameObject> queue = new();
    for (int i = 0; i < this.pools[prefabName].size; ++i) {
      var newObject = Instantiate(prefab);
      newObject.SetActive(false);
      queue.Enqueue(newObject); 
    }
    this.pooledObjects.Add(prefabName, queue);
    return (queue);
  }

  public void SetConfig(string prefabName, string path, int poolSize = PrefabObjectPool.POOL_SIZE) 
  {
    var loaded = Resources.Load<UnityEngine.GameObject>($"Prefabs/{prefabName}");
    this.pools.TryAdd(
        prefabName,(prefab: loaded, size: poolSize)
        ); 
  }

  public UnityEngine.GameObject GetPooledObject(string prefabName) {
    if (!this.pools.ContainsKey(prefabName)) 
      throw (new ApplicationException($"not found prefab for: {prefabName}"));
    var queue = this.GetQueue(prefabName) ?? this.CreateQueue(prefabName, this.pools[prefabName].prefab);
    UnityEngine.GameObject pooledObject = null;
    if (queue.Count > 0)
      pooledObject = queue.Dequeue();
    if (pooledObject == null)
      pooledObject = Instantiate(this.pools[prefabName].prefab);
    return (pooledObject);
  }

  public void ReturnObject(UnityEngine.GameObject pooledObject, string prefabName) {
    var gameObject = pooledObject as UnityEngine.GameObject;
    if (gameObject == null)
      throw (new ArgumentException($"{pooledObject} is not UnityEngine.GameObject"));
    gameObject.SetActive(false);
    var queue = this.GetQueue(prefabName);
    if (this.pooledObjects[prefabName].Count < this.pools[prefabName].size)
      this.pooledObjects[prefabName].Enqueue(gameObject);
    else
      Destroy(gameObject);
  }

  void OnDestory() {
    foreach (var (key, queue) in this.pooledObjects) {
      while (queue.Count > 0) {
        var pooledObject = queue.Dequeue();
        Destroy(pooledObject); 
      }
    }
    this.pooledObjects = null;
    Singleton<PrefabObjectPool>.Release();
  }
}
