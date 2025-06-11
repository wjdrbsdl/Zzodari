using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// 스코프의 영역을 나눠서 각 영역에 맞게 스코프를 활성화해서 사용
/// </summary>
public abstract class Scope : MonoBehaviour
{
    protected Container container;

    [SerializeField] List<MonoBehaviour> _haierMonobehaviours; //하이어키상 미리 존재하는 모노들

    protected virtual void Awake()
    {
        container = new Container();
        Register();
        InjectAll();
    }

    /// <summary>
    /// 컨테이너에 등록하는 함수들
    /// </summary>
    public virtual void Register()
    {
        foreach (var monobehaviour in _haierMonobehaviours)
        {
            container.RegisterMonobehaviour(monobehaviour);
        }
    }

    //하이어라키에 있는 녀석들 모두 주입하기
    //addScenece인 관계로 여러 씬이 나온경우 모든 씬의 모노비헤비어를 훑기 때문에 씬 따라 영역 나눌필요잇음
    protected virtual void InjectAll()
    {
        MonoBehaviour[] monobehavies = GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
       foreach(var monobehavi in monobehavies)
        {
            Inject(monobehavi);
        }
    }

    /// <summary>
    /// 의존성을 주입함
    /// </summary>
    /// <param name="target">주입 대상</param>
    protected virtual void Inject(object target)
    {
        //리플렉션 사용
        Type type = target.GetType();
        //대상의 모든 필드, 함수 뭐 등 다 따져서 필요한 타겟을 확인

        FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic//스태틱한 변수에 의존성을 주입하는 경우는 드물다
                            | BindingFlags.DeclaredOnly); 

        //타겟의 멤버 변수에 대한 필드 인포 
        foreach(FieldInfo fieldInfo in fieldInfos)
        {
            if(fieldInfo.GetCustomAttribute<InjectAttribute>() != null)
            {
                //fieldInfo.GetType(); //하면안됩니다. 필드인포라는 타입을 담고 있는 타입에 대한 타입을 갖고온다
                object value = container.Resolve(fieldInfo.FieldType);

                if(value != null)
                fieldInfo.SetValue(target, value);
            }
        }

    }
}
