using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static Manager s_instance = null;

    // 게임매니저, 데이터매니저, UI매니저, 리소스매니저, 사운드매니저
    private static GameManager s_gameManager = new GameManager(); // 미구현
    private static DataManager s_dataManager = new DataManager(); // 미구현
    private static SceneManager s_sceneManager = new SceneManager(); // 미구현
    private static UIManager s_uiManager = new UIManager();
    private static ResourceManager s_resourceManager = new ResourceManager();
    private static SoundManager s_soundManager = new SoundManager();

    public static GameManager Game { get { Init(); return s_gameManager; } }
    public static DataManager Data { get { Init(); return s_dataManager; } }
    public static SceneManager Scene { get { Init(); return s_sceneManager; } }
    public static UIManager UI { get { Init(); return s_uiManager; } }
    public static ResourceManager Resource { get { Init(); return s_resourceManager; } }
    public static SoundManager Sound { get { Init(); return s_soundManager; } }

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

            s_instance = Utils.GetOrAddComponent<Manager>(go);
            DontDestroyOnLoad(go);

            // 매니저들 초기화
            s_gameManager.Init();
            s_dataManager.Init();
            s_sceneManager.Init();
            s_uiManager.Init();
            s_resourceManager.Init();
            s_soundManager.Init();

            // Application.targetFrameRate = 60;
        }
    }

    private void OnApplicationQuit()
    {
        s_dataManager.Auth.SignOut();
    }
}
