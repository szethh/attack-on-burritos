using UnityEngine;

public static class ExtensionMethods
{
    public static Vector2 Random(this Vector2 vector2, float minInclusive, float maxInclusive)
    {
        return new Vector2(vector2.x * UnityEngine.Random.Range(minInclusive, maxInclusive),
                           vector2.y * UnityEngine.Random.Range(minInclusive, maxInclusive));
    }

    public static Vector2 RandomFlip(this Vector2 vector2)
    {
        return new Vector2(vector2.x * (UnityEngine.Random.value * 2 - 1),
                           vector2.y * (UnityEngine.Random.value * 2 - 1));
    }

    public static Vector2 RandomSide(this Vector2 vector2)
    {
        return UnityEngine.Random.value > 0.5f ? new Vector2(vector2.x, 0) : new Vector2(0, vector2.y);
    }
    
    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }
 
    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }
 
    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }
 
    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
}