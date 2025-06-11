using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// �������� ������ ������ �� ������ �°� �������� Ȱ��ȭ�ؼ� ���
/// </summary>
public abstract class Scope : MonoBehaviour
{
    protected Container container;

    [SerializeField] List<MonoBehaviour> _haierMonobehaviours; //���̾�Ű�� �̸� �����ϴ� ����

    protected virtual void Awake()
    {
        container = new Container();
        Register();
        InjectAll();
    }

    /// <summary>
    /// �����̳ʿ� ����ϴ� �Լ���
    /// </summary>
    public virtual void Register()
    {
        foreach (var monobehaviour in _haierMonobehaviours)
        {
            container.RegisterMonobehaviour(monobehaviour);
        }
    }

    //���̾��Ű�� �ִ� �༮�� ��� �����ϱ�
    //addScenece�� ����� ���� ���� ���°�� ��� ���� ������� �ȱ� ������ �� ���� ���� �����ʿ�����
    protected virtual void InjectAll()
    {
        MonoBehaviour[] monobehavies = GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
       foreach(var monobehavi in monobehavies)
        {
            Inject(monobehavi);
        }
    }

    /// <summary>
    /// �������� ������
    /// </summary>
    /// <param name="target">���� ���</param>
    protected virtual void Inject(object target)
    {
        //���÷��� ���
        Type type = target.GetType();
        //����� ��� �ʵ�, �Լ� �� �� �� ������ �ʿ��� Ÿ���� Ȯ��

        FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic//����ƽ�� ������ �������� �����ϴ� ���� �幰��
                            | BindingFlags.DeclaredOnly); 

        //Ÿ���� ��� ������ ���� �ʵ� ���� 
        foreach(FieldInfo fieldInfo in fieldInfos)
        {
            if(fieldInfo.GetCustomAttribute<InjectAttribute>() != null)
            {
                //fieldInfo.GetType(); //�ϸ�ȵ˴ϴ�. �ʵ�������� Ÿ���� ��� �ִ� Ÿ�Կ� ���� Ÿ���� ����´�
                object value = container.Resolve(fieldInfo.FieldType);

                if(value != null)
                fieldInfo.SetValue(target, value);
            }
        }

    }
}
