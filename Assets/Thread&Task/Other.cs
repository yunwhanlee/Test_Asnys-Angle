using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class Other : MonoBehaviour
{
    private UI_Manager ui_Manager;

    private void Start()
    {
        ui_Manager = FindObjectOfType<UI_Manager>();

        ui_Manager.btn[5].GetComponentInChildren<Text>().text = "Button6\n(" + "<color=#ff0000>" + "他の" + "</color>" + "スクリプト)\nマルチスレッド" + "\n<color=#0000ff>" + "No Freeze" + "</color>";

    }

    private void Sum()
    {
        Debug.LogFormat("他のグラスSum(): {0}番 スレッド", ui_Manager.ws2 = Thread.CurrentThread.ManagedThreadId);
        ui_Manager.sum = 0;
        for (int i = 0; i < 500000000; i++)
            ui_Manager.sum += i;
        Debug.Log("-----------------------------------------終　　了----------------------------------------------");
    }

    public void OnClickBtn6() //他のスクリプトのマルチスレッド(Thread)
    {
        Debug.LogFormat("他のグラスBtn6(): {0}番 スレッド", ui_Manager.ws1 = Thread.CurrentThread.ManagedThreadId);
        Thread thread = new Thread(Sum);//他のスレッド用意
        thread.Start();
        ui_Manager.CheckThread(ui_Manager.ws1, ui_Manager.ws2);
    }
}
