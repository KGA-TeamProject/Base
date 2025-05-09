using UnityEngine;

public class Singleton<T>: MonoBehaviour where T: MonoBehaviour 
{

  private static T instance;
  protected static void CreateInstance() 
  {
    if (Singleton<T>.instance != null) {
      return ;
    }
    var instance = new UnityEngine.GameObject().AddComponent<T>();  
    instance.name = instance.GetType().ToString();
    UnityEngine.Object.DontDestroyOnLoad(instance);
    Singleton<T>.instance = instance;
  }

  protected static void Release() 
  {
    UnityEngine.Object.Destroy(Singleton<T>.instance.gameObject);
    Singleton<T>.instance = null;
  }

  public static T Shared => Singleton<T>.instance;
}
