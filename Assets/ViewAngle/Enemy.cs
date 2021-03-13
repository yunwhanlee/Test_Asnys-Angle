using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float   moveSpeed;
    public float chaseDist;
    public GameObject p_Bullets;
    public GameObject mySprite;
    public Vector2 moveVec;
    public Vector2 target;
    public float span;
    public float cnt = 0;

    public float angle;
    public float rowPow; //１フレーム当たり、Ｎ度旋回する
    public float searchFov; //有効視野角
    
    public Text dotTxt;
    public Text cosTxt;

    //視野角・扇形範囲のチェック　⇒　色を切り替える処理
    private bool findTarget;
    void Start()
    {
        //Mathf.Deg2Rad
        rowPow = rowPow * Mathf.PI / 180;
        searchFov = (searchFov / 2) * Mathf.PI / 180; //ラジアンとか角度とかとちらも結果は変わらない。

        moveVec = Vector2.right;
        findTarget = false;
    }

    void Update()
    {
        //弾のカウント
        cnt += Time.deltaTime;

        //プレイヤー探索
        Search();
    }

    void Search()
    {
        //範囲内のプレイヤー検索
        var col = Physics2D.OverlapCircle(this.transform.position, chaseDist, LayerMask.GetMask("Player"));

        if (col == null)//タゲットが発見されていない
        {
            findTarget = false;
            cosTxt.text = "<color=#0000ff>" + "cos " + searchFov * Mathf.Rad2Deg + "°: " + Mathf.Cos(this.searchFov) + "</color>";
            return;
        }

        else
        {
            target = (col.transform.position - this.gameObject.transform.position);
            float dist = Vector3.Magnitude(target); // ベクトルの長さ
            //Debug.Log("dist = " + dist);
            target = target.normalized;

            if (dist > chaseDist) { return; }

            moveVec = new Vector2(Mathf.Cos(this.angle), Mathf.Sin(this.angle));

            //内積
            float dot = moveVec.x * target.x + moveVec.y * target.y;

            dotTxt.text = "dot : " + dot.ToString("N3");

            if (dot < Mathf.Cos(this.searchFov))//視野に居ない「×」
            {
                findTarget = false;
                cosTxt.text = "<color=#0000ff>" + "cos " + searchFov * Mathf.Rad2Deg + "°: " + Mathf.Cos(this.searchFov) + "</color>";
                return;
            }
            else//視野に居る「○」
            {
                findTarget = true;
                cosTxt.text = "<color=#ff0000>" + "cos" + searchFov * Mathf.Rad2Deg + " : " + Mathf.Cos(this.searchFov) + "</color>";
            }

            if(dot < Mathf.Cos(this.rowPow))
            {
                //外積：旋回しながら追尾
                float cross = moveVec.x * target.y - moveVec.y * target.x;
                //左右に合わせて旋回する
                if (cross >= 0) { this.angle += this.rowPow; }
                else { this.angle -= this.rowPow; }
            }
            else
            {
                //ブルブル～ブレしないように、ターゲットの方向に向く
                this.angle = Mathf.Atan2(target.y, target.x); //ラジアン⇒角度に変換は下の「☆姿の回転」でする
            }

            //「★動かせ」
            transform.Translate(moveVec * moveSpeed * Time.deltaTime, Space.World);//★角度に影響を受けないように　Space.World（グロバル座標）にする！
            //「☆姿の回転」
            transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);//

            //Shoot();
        }
    }

    void Shoot()
    {
        if (cnt > span)
        {
            Destroy(Instantiate(p_Bullets, this.transform.position, Quaternion.identity),2f);
            cnt = 0;
        }
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, chaseDist);

        float movingAngle = Mathf.Atan2(moveVec.y, moveVec.x);
        //視野角
        Vector2 viewAngleTop =
            new Vector2(//　現在位置 + コサイン(視野角度  + キャラの向き) * 範囲の長さ
                transform.position.x + Mathf.Cos(searchFov + movingAngle) * chaseDist,
                transform.position.y + Mathf.Sin(searchFov + movingAngle) * chaseDist
                );
        Vector2 viewAngleBottom =
            new Vector2(
                transform.position.x + Mathf.Cos(searchFov - movingAngle) * chaseDist,
                transform.position.y - Mathf.Sin(searchFov - movingAngle) * chaseDist
                );

        Debug.DrawLine(this.transform.position, viewAngleTop, Color.red);
        Debug.DrawLine(this.transform.position, viewAngleBottom, Color.cyan);

        //視野角の扇形範囲
        //色
        Color yellow = new Color(1, 1, 0, 0.1f);
        Color red = new Color(1, 0, 0, 0.1f);
        if (!findTarget)
            Handles.color = yellow;
        else
            Handles.color = red;

        //表示（＋角度半分、－角度半分）
        Handles.DrawSolidArc(transform.position, Vector3.forward, moveVec, (searchFov * Mathf.Rad2Deg), chaseDist);
        Handles.DrawSolidArc(transform.position, Vector3.forward, moveVec, (-searchFov * Mathf.Rad2Deg), chaseDist);
    }
}