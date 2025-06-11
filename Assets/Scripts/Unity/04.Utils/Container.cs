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
    /// 생성자가 있는 일반 C# 클래스 등록 (생성해서 추가함)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Register<T>() where T : class, new()
    {
        //등록할 인스턴스가 없으면 만들어서 넣기(생성자가 필요 -> new() 제한자)
        T obj = new T(); // new() 제한자로인해서 생성자 있는 녀석만 들어와서 
        _registration[typeof(T)] = obj;
    }

    /// <summary>
    /// Monobehaviour 객체 생성하여 추가
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void RegisterMonobehaviour<T>() where T : MonoBehaviour
    {
        T obj = new GameObject(typeof(T).Name).AddComponent<T>();
        _registration[typeof(T)] = obj;
    }


    /// <summary>
    /// 하이어키에 존재하는 모노비해비어 객체 추가 
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
        //없으면 널반환 예외처리 필요
        return  _registration[type];
    }
}
