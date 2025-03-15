using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public LobbyClient lobClient;
    public PlayClient playClient;
    public static PrefabManager instance;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

}
