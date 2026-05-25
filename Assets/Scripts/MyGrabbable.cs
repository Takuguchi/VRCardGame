namespace Oculus.Interaction
{
    /// <summary>
    /// Oculus Interaction SDK の Grabbable を継承した拡張クラス。
    /// 当初は CanGrab フラグによるグラブ制御を独自実装する予定だったが、
    /// SDK 側の Grabbable.CanGrab プロパティで同等の機能が実現できたため
    /// 現在は内部実装がコメントアウトされており、基底クラスをそのまま使用している。
    /// </summary>
    public class MyGrabbable : Grabbable
    {
        /*
        public bool CanGrab = true;

        public override void ProcessPointerEvent(PointerEvent evt)
        {
            if (!CanGrab) return;

            base.ProcessPointerEvent(evt);
        }
        */
    }
}
