using System.Collections;
using TMPro;
using UnityEngine;

public class UI_LoadAsset : UI_Base
{
    private enum Texts
    {
        LoadingText,
    }

    private WaitForSeconds wait = null;

    private string[] loadingTexts = { "로딩중.", "로딩중..", "로딩중..." };

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<TMP_Text>(typeof(Texts));

        wait = new WaitForSeconds(1f);

        StartCoroutine(WaitRoutine());

        return true;
    }

    private IEnumerator WaitRoutine()
    {
        int idx = 0;
        while (Manager.Resource.AssetVaild == false || Manager.Data.IsVaild == false)
        {
            yield return wait;
            Get<TMP_Text>((int)Texts.LoadingText).text = loadingTexts[idx];

            ++idx;
            if (idx == loadingTexts.Length)
                idx = 0;
        }

        Manager.Resource.Destroy(gameObject);
    }
}
