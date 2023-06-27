using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour �� �ʿ������ ���� �̱��� ���ø�
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