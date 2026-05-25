using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rigidbody に独自の重力ベクトルを適用するクラス。
/// Unity 標準の重力（Physics.gravity）を無効にし、
/// Inspector で設定した localGravity を AddForce で毎フレーム加算する。
/// カードが特定方向に落下するような演出に使用する。
/// </summary>
public class ChangeGravity : MonoBehaviour
{
    [SerializeField] private Vector3 localGravity; // 適用するカスタム重力ベクトル（Inspectorで設定）
    private Rigidbody rBody;

    private void Start()
    {
        rBody = this.GetComponent<Rigidbody>();
        rBody.useGravity = false; // Unity 標準の重力を無効化し、カスタム重力のみを使う
    }

    private void FixedUpdate()
    {
        SetLocalGravity(); // 物理演算タイミングで重力を加算（FixedUpdateが推奨）
    }

    // localGravity を Acceleration モードで AddForce し、質量に依らない一定加速を実現する
    private void SetLocalGravity()
    {
        rBody.AddForce(localGravity, ForceMode.Acceleration);
    }
}
