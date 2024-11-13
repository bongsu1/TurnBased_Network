using TMPro;

public class UI_Post : UI_Base
{
    private enum Texts
    {
        PostText,
    }

    private string post = null;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<TMP_Text>(typeof(Texts));

        RefreshUI();

        return true;
    }

    public void SetInfo(string post)
    {
        this.post = post;

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (_init == false)
            return;

        Get<TMP_Text>((int)Texts.PostText).text = post;
    }
}
