using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    void Start()
    {
        
    }

    void Update()
    {
        //move
        var x = Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime;
        var y = Input.GetAxisRaw("Vertical") * moveSpeed * Time.deltaTime;

        float angle = Mathf.Atan2(y, x) * 180 / Mathf.PI;//아크탄젠트는 라디안 값이라서, 라디안->각도로 치환해줘야 한다!

        //Debug.Log("Mathf.Atan2(y,x) = "+ Mathf.Atan2(y, x));
        //Debug.Log("Mathf.Atan2(y,x) * 180 / Mathf.PI = " + angle);

        //★Space.World로 하지않으면, 로컬좌표로 각도에 영향을 받아 이동이 이상해진다.
        this.gameObject.transform.Translate(x, y, this.transform.position.z,Space.World);

        transform.rotation = Quaternion.Euler(0,0,angle);
    }
}
