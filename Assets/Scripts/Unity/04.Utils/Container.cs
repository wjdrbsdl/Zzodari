using System;
using System.Collections.Generic;
using UnityEngine;

public class Container
{
    public Container()
    {
        _registration = new Dictionary<Type, object>();
    }

    Dictionary<Type, object> _registration;

    /// <summary>
    /// �����ڰ� �ִ� �Ϲ� C# Ŭ���� ��� (�����ؼ� �߰���)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Register<T>() where T : class, new()
    {
        //����� �ν��Ͻ��� ������ ���� �ֱ�(�����ڰ� �ʿ� -> new() ������)
        T obj = new T(); // new() �����ڷ����ؼ� ������ �ִ� �༮�� ���ͼ� 
        _registration[typeof(T)] = obj;
    }

    /// <summary>
    /// Monobehaviour ��ü �����Ͽ� �߰�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void RegisterMonobehaviour<T>() where T : MonoBehaviour
    {
        T obj = new GameObject(typeof(T).Name).AddComponent<T>();
        _registration[typeof(T)] = obj;
    }


    /// <summary>
    /// ���̾�Ű�� �����ϴ� �����غ�� ��ü �߰� 
    /// </summary>
    /// <param name="monobehaviour"></param>
    public void RegisterMonobehaviour(MonoBehaviour monobehaviour)
    {
        _registration[monobehaviour.GetType()] = monobehaviour;
    }

    public T Resolve<T>()
    {
        return (T)_registration[typeof(T)];
    }

    public object Resolve(Type type)
    {
        //������ �ι�ȯ ����ó�� �ʿ�
        return  _registration[type];
    }
}
