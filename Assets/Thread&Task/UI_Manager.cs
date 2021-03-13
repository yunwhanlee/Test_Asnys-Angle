using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public Button[] btn;
    public Text showTxt, subTxt, threadN1Txt, threadN2Txt;
    public long sum = 0;
    public int cnt = 500000000; //多い作業量

    public int ws1, ws2;//work_space　=　スレッド番号
    private void Update()
    {
        showTxt.text = sum.ToString();
        subTxt.text = "「重たい処理」\n0から" + cnt + "まで\nカウントしながら合計を求める";

        btn[0].GetComponentInChildren<Text>().text = "Button1\n単一スレッド "   + "\n<color=#ff0000>" + "Freeze" + "</color>";
        btn[1].GetComponentInChildren<Text>().text = "Button2\nマルチスレッド " + "\n<color=#0000ff>" + "No Freeze" + "</color>";
        btn[2].GetComponentInChildren<Text>().text = "Button3\n非同期(Task)\nマルチスレッド" + "\n<color=#0000ff>" + "No Freeze" + "</color>";
        btn[3].GetComponentInChildren<Text>().text = "Button4\n非同期(Task)\n単一スレッド" + "\n<color=#0000ff>" + "No Freeze" + "</color>";
        btn[4].GetComponentInChildren<Text>().text = "Button5\n非同期(Task)\n単一スレッド" + "\n<color=#0000ff>" + "No Freeze" + "</color>";
    }

    public void CheckThread(int ws1, int ws2)
    {
        if(ws1 == ws2){
            threadN1Txt.text = ws1.ToString(); threadN1Txt.color = Color.red;
            threadN2Txt.text = ws2.ToString(); threadN2Txt.color = Color.red;
        }
        else{
            threadN1Txt.text = ws1.ToString(); threadN1Txt.color = Color.red;
            threadN2Txt.text = ws2.ToString(); threadN2Txt.color = Color.green;
        }
    }

    //★5億番足す作業するとき、どうなるか実験★
    private void Sum()//同期
    {
        Debug.LogFormat("Sum(): {0}番 スレッド", ws2 = Thread.CurrentThread.ManagedThreadId);
        sum = 0;
        for (int i = 0; i < cnt; i++)
            sum += i;
        Debug.Log("-----------------------------------------終　　了----------------------------------------------");

    }
    private async Task<long> Sum_Task()//②非同期
    {
        Debug.LogFormat("Sum_Task(): {0}番 スレッド", ws2 = Thread.CurrentThread.ManagedThreadId);
        await Task.Factory.StartNew(() =>//非同期生成及び実行
        {
            sum = 0;
            for (int i = 0; i < cnt; i++)
                sum += i;
        });
        Debug.Log("-----------------------------------------終　　了----------------------------------------------");
        return sum;
    }



    public void OnClickBtn1() //単一ススレッド（メインスレッド）　⇒　フリーズになる
    {
        Debug.LogFormat("Btn1(): {0}番 スレッド", ws1 = Thread.CurrentThread.ManagedThreadId);
        Sum();
        CheckThread(ws1, ws2);
    }

    public void OnClickBtn2() //マルチスレッド(Thread)
    {
        Debug.LogFormat("Btn2(): {0}番 スレッド", ws1 = Thread.CurrentThread.ManagedThreadId);
        Thread thread = new Thread(Sum);//他のスレッド用意
        thread.Start();
        CheckThread(ws1, ws2);
    }

    public async void OnClickBtn3() //★★非同期(Task)・マルチスレッドの条件①Task.Run(()=>Func())　＋　②Func()は Task形式のこと！
    {
        Debug.LogFormat("Btn3(): {0}番 スレッド", ws1 = Thread.CurrentThread.ManagedThreadId);

        //★ここでvarは Task<long>型
        var task = Task.Run(()=>Sum_Task()); //① Task.Run()の戻る値はTask<T>クラスです。
        long sum = await task;
        CheckThread(ws1, ws2);
    }

    public async void OnClickBtn4() //非同期(Task)・単一スレッド -> asyncがあっても、①がないのでできない
    {
        Debug.LogFormat("Btn4(): {0}番 スレッド", ws1 = Thread.CurrentThread.ManagedThreadId);
        Func();
        CheckThread(ws1, ws2);
    }

    private async void Func()
    {
        Debug.LogFormat("Sum_Taskを呼び出すために使うFunc(): {0}番 スレッド", Thread.CurrentThread.ManagedThreadId);
        long sum = await Sum_Task();
    }

    public async void OnClickBtn5() //非同期(Task)・単一スレッド（ラムダ式） 
    {
        Debug.LogFormat("Btn5(): {0}番 スレッド", ws1 = Thread.CurrentThread.ManagedThreadId);
        await Task.Factory.StartNew(() => //Task.Run()と同じ
        {
            sum = 0;
            for (int i = 0; i < cnt; i++)
            {

                if(i == 0) Debug.LogFormat("Sum()内容をラムダ式で: {0}番 スレッド", ws2 = Thread.CurrentThread.ManagedThreadId);//一回だけ実行
                sum += i;
            }
        });
        CheckThread(ws1, ws2);
        Debug.Log("-----------------------------------------終　　了----------------------------------------------");
    }

}
