using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ApplicationScope : Scope
{
    protected override void Awake()
    {
        base.Awake();

        SceneManager.sceneLoaded += (scene, mode) =>
        {
            InjectAll(scene);
        };

        DontDestroyOnLoad(gameObject);
    }

    protected void InjectAll(Scene scene)
    {
        GameObject[] roots = scene.GetRootGameObjects();
        foreach (GameObject root in roots)
        {
            MonoBehaviour[] monobehaviours = root.GetComponentsInChildren<MonoBehaviour>();
            foreach (var mono in monobehaviours)
            {
                Inject(mono);
            }
        }
    }
}
