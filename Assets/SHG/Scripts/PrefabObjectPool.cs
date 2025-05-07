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
    if (this.pooledObjects.TryGetValue(prefabName, 
          out Queue<UnityEngine.GameObject> queue)) {
      return (queue);
    }
    return (null);
  }

  Queue<UnityEngine.GameObject> CreateQueue(string prefabName, UnityEngine.GameObject prefab) {
    Queue<UnityEngine.GameObject> queue = new();
    for (int i = 0; i < this.pools[prefabName].size; ++i) {
      var newObject = Instantiate(prefab);
      newObject.transform.parent = this.transform;
      newObject.SetActive(false);
      queue.Enqueue(newObject); 
    }
    this.pooledObjects.Add(prefabName, queue);
    return (queue);
  }

  public void RegisterByName(string prefabName, string path, Func<GameObject, GameObject> modifier = null,  int poolSize = PrefabObjectPool.POOL_SIZE) 
  {
    if (this.pools.ContainsKey(name))
      return ;
    var prefab = Resources.Load<UnityEngine.GameObject>(path);
    if (modifier != null) {
      prefab = modifier(Instantiate(prefab));
    }
    prefab.transform.parent = this.transform;
    prefab.SetActive(false);
    this.pools.TryAdd(
        prefabName, (prefab: prefab, size: poolSize)
        ); 
    this.CreateQueue(prefabName, prefab);
  }

  public void RegisterPrefab(string name, GameObject prefab, int poolSize = PrefabObjectPool.POOL_SIZE)
  {
    if (this.pools.ContainsKey(name))
      return ;
    this.pools.TryAdd(name, (prefab: prefab, size: poolSize)) ;
    prefab.transform.parent = this.transform;
    prefab.SetActive(false);
    this.CreateQueue(name, prefab);
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
    gameObject.transform.parent = this.transform;
    var queue = this.GetQueue(prefabName);
    if (this.pooledObjects[prefabName].Count < this.pools[prefabName].size)
      this.pooledObjects[prefabName].Enqueue(gameObject);
    else
      Destroy(gameObject);
  }

  public void ReleasePrefab(string prefabName)
  {
    var queue = this.GetQueue(prefabName);
    if (queue == null) {
      return;
    }
    foreach (var pooledObject in queue) {
      Destroy(pooledObject.gameObject);
    }
    this.pooledObjects.Remove(prefabName);
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
