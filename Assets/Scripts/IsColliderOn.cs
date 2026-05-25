using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// コライダーの接触状態をフラグとして保持するシンプルなステートホルダー。
/// OnTriggerEnter/Exit を持つコンポーネントから参照され、
/// 接触の有無を他のコンポーネントが簡単に参照できるようにする。
/// （現在は使用箇所がコメントアウトされており、参照されていない）
/// </summary>
public class IsColliderOn : MonoBehaviour
{
    public bool IsCollision; // コライダー内にオブジェクトがいるとき true

    void Start()
    {
        IsCollision = false;
    }
}
