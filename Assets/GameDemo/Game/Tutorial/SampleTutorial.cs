public class SampleTutorial : TutorialAction
{
    public override void OnExecute()
    {
        //ShowPointerAnimButton("HOME_PLAY_BUTTON", () =>
        //{
        //    OnShowScreen<MapScreen>(() =>
        //    {
        //        ShowPointerAnimButton("MAP_FIRST_MISSION", () =>
        //        {
        //            OnShowScreen<PlayScreen>(() =>
        //            {
        //                var playScreen = UIManager.Instance.GetScreen<PlayScreen>();

        //                ShowPointer(playScreen.TapStart.gameObject, "ON_PLAY", () =>
        //                {
        //                    OnTrigger("SHOW_END_POPUP", () =>
        //                    {
        //                        ShowPointerAnimButton("END_OK_BUTTON", Finish).Translate(0, -30);
        //                    });
        //                }).Translate(0, -50);
        //            });
        //        }).SetRotation(180).Translate(0, 50);
        //    });
        //}).Translate(0, -30);
    }
}