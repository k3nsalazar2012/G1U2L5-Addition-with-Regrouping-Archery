// c# / unity class
using System.Collections;

/// <summary>
/// Dummy MTManager class
/// </summary>
public static class MTManager 
{
    public static int CurrentLevel { private set; get; }
    public static bool IsCMS { private set; get; }

    public static IEnumerator UpdateScore(int score, string difficulty, int level)
    {
        yield return null;
    }
}
