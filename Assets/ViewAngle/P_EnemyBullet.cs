using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_EnemyBullet : MonoBehaviour
{
    public float moveSpeed;
    public int rotateSpeed;
    public GameObject sprite_obj;//子
        
    Enemy e;
    Vector2 target_dir;
    public float angle;
    public float rowPow; //１フレーム当たり、Ｎ度旋回する
    void Start()
    {
        e = FindObjectOfType<Enemy>();
        rowPow = this.rowPow * Mathf.PI / 180;//Mathf.Deg2Rad
    }
    void Update()
    {
        //動かせ
        Vector2 moveVec = new Vector2(Mathf.Cos(this.angle), Mathf.Sin(this.angle)) * moveSpeed * Time.deltaTime;
        transform.Translate(moveVec);

        //旋回しながら、追尾
        target_dir = e.target;
        float cross = moveVec.x * target_dir.y - moveVec.y * target_dir.x;
        if(cross >= 0) { this.angle += this.rowPow; }
        else { this.angle -= this.rowPow; }

        //自体観点
        sprite_obj.transform.Rotate(0, 0, Time.deltaTime * rotateSpeed);//子を回転（回転しても親オブジェクトの角度に影響しない）
    }
}
