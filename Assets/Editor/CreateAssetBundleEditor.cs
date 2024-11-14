using System.IO;
using UnityEditor;

public class CreateAssetBundleEditor
{
    [MenuItem("AssetBundle/Build AssetBundle (Stand Alone)")]
    public static void CreateAssetBundle()
    {
        string directory = "./Bundle";

        // 경로 탐색
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // 해당 경로로 에셋번들 빌드, 빌드 옵션, 빌드 타겟 플랫폼(윈도우)
        BuildPipeline.BuildAssetBundles(directory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

        // 에셋번들이 완료되었는지 확인하기 위한 창 띄우기
        EditorUtility.DisplayDialog("에셋번들 빌드", "에셋번들 빌드를 완료했습니다", "완료");
    }

    [MenuItem("AssetBundle/Build AssetBundle (Android)")]
    public static void CreateAssetBundleForAndroid()
    {
        string directory = "./Bundle/Android";

        // 경로 탐색
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // 해당 경로로 에셋번들 빌드, 빌드 옵션, 빌드 타겟 플랫폼(윈도우)
        BuildPipeline.BuildAssetBundles(directory, BuildAssetBundleOptions.None, BuildTarget.Android);

        // 에셋번들이 완료되었는지 확인하기 위한 창 띄우기
        EditorUtility.DisplayDialog("에셋번들 빌드", "에셋번들 빌드를 완료했습니다", "완료");
    }
}
