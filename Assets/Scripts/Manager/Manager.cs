using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static Manager s_instance = null;

    private static GameManager s_gameManager = new GameManager();
    private static DataManager s_dataManager = new DataManager();
    private static SceneManager s_sceneManager = new SceneManager();
    private static UIManager s_uiManager = new UIManager();
    private static ResourceManager s_resourceManager = new ResourceManager();
    private static SoundManager s_soundManager = new SoundManager();

    public static GameManager Game { get { Init(); return s_gameManager; } }
    public static DataManager Data { get { Init(); return s_dataManager; } }
    public static SceneManager Scene { get { Init(); return s_sceneManager; } }
    public static UIManager UI { get { Init(); return s_uiManager; } }
    public static ResourceManager Resource { get { Init(); return s_resourceManager; } }
    public static SoundManager Sound { get { Init(); return s_soundManager; } }

    private static NetworkManager s_networkManager;
    public static NetworkManager Network { get { return s_networkManager; } }

    private void Start()
    {
        Init();
    }

    private static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("Manager");
            if (go == null)
                go = new GameObject { name = "Manager" };

            s_instance = go.GetOrAddComponent<Manager>();
            s_networkManager = go.GetOrAddComponent<NetworkManager>();
            DontDestroyOnLoad(go);

            s_networkManager.SetConnet(ServerCore.Define.Connect.Domain);

            // 매니저들 초기화
            s_resourceManager.Init();
            s_dataManager.Init();
            s_sceneManager.Init();
            s_soundManager.Init();

            // Init에서 resourceManager를 부르기 때문에 가장 뒤에 실행
            s_uiManager.Init();
            s_gameManager.Init();

#if UNITY_ANDROID
            Application.targetFrameRate = 60;
#endif
        }
    }

    private void OnDiable()
    {
        if (s_dataManager.IsVaild == false)
            return;

        if (s_dataManager.Auth.CurrentUser == null)
            return;

        s_dataManager.Auth.SignOut();
    }
}
