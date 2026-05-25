using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// VRコントローラーによるグラブ状態を保持するステートホルダー。
/// Oculus Interaction SDK の Grabbable イベント（Select/Unselect）に
/// UnityEvent として登録し、グラブの開始・終了を追跡する。
/// BattleField / CardColliderEvent がカードをグラブ中かどうかを確認するために参照する。
/// （グラブ中はカードの配置判定を行わず、離したタイミングで固定する）
/// </summary>
public class IsGrabOn : MonoBehaviour
{
    public bool IsGrab; // VRコントローラーでグラブ中のとき true

    void Start()
    {
        IsGrab = false;
    }

    // グラブ開始時に Grabbable の SelectEntered イベントから呼ぶ
    public void UpdateGrabStatus()
    {
        IsGrab = true;
    }

    // グラブ終了時に Grabbable の SelectExited イベントから呼ぶ
    public void UpdateNoGrabStatus()
    {
        IsGrab = false;
    }
}
