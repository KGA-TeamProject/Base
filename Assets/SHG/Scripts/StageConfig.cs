[System.Serializable]
public class StageConfig
{
  public MapConfig[] Maps;
  [System.Serializable]
  public struct MapConfig
  {
    public TilePrefab Tiles;
    public ObjectPrefab Objects;
    public string[] Sections;

    [System.Serializable]
    public struct TilePrefab
    {
      public string Floor;
      public string Wall;
    }
    [System.Serializable]
    public struct ObjectPrefab
    {
      public string[] Small;
      public string[] Medium;
      public string[] Large;
    }
  }
}
