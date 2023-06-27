using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour 가 필요없을때 쓰는 싱글톤 템플릿
/// </summary>

public class SingletonTemplete<T> where T : new()
{
    protected static T instance;

    protected static T Instance
    {
        get
        {
            if (instance == null) instance = new T();
            return instance;
        }
    }
}