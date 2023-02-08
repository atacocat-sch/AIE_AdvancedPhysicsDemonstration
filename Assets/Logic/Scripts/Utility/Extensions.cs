
using UnityEngine;

public static class Extensions
{
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.transform.GetOrAddComponent<T>();
    }

    public static T GetOrAddComponent<T>(this Component context) where T : Component
    {
        if (context.TryGetComponent(out T component))
        {
            return component;
        }
        else return context.gameObject.AddComponent<T>();
    }
}
