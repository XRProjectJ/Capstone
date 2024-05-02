using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;
    public Button startBtn;
    public GameObject startPanel;
    public GameObject explainPanel;
    public Button CircuitStartBtn;
    public Button backBtn;

    public GameObject LED;
    public GameObject Battery;
    //public GameObject resister;
    public GameObject MRLine;

    public Button ItemBtn;
    public GameObject circuitPanel;

    public Button ItemBox_Pressed;
    public GameObject itemPanel;
    public Button LEDBtn;
    public Button BatteryBtn;
    public Button MRLineBtn;



    private void Awake()
    {
        if(GM == null)
        {
            GM = this;
        }
    }

    public enum GameState
    {
        Ready,
        Explain,
        Run,
        GameOver
    }

    public GameState state;





    public void Explain()
    {
        Debug.Log("start pressed");
        state = GameState.Explain;
        startPanel.SetActive(false);
        explainPanel.SetActive(true);
    }

    public void StartCircuit()
    {
        Debug.Log("circuit Start");
        explainPanel.SetActive(false);
        circuitPanel.SetActive(true);
        itemPanel.SetActive(false);
    }


    public void item()
    {
        Debug.Log("itemBtn Pressed");
        circuitPanel.SetActive(false);
        itemPanel.SetActive(true);
    }


    public void createLine()
    {
        Instantiate(MRLine, new Vector3(0, 0, 0), this.transform.rotation);
    }
    public void createBattery()
    {
        Instantiate(Battery, new Vector3(0,0,0), this.transform.rotation);
    }
    public void createLED()
    {
        Instantiate(LED, new Vector3(0, 0, 0), this.transform.rotation);
    }



    void Start()
    {
        state = GameState.Ready;
        startPanel.SetActive(true);
        explainPanel.SetActive(false);
        circuitPanel.SetActive(false);

        startBtn.onClick.AddListener(Explain);
        CircuitStartBtn.onClick.AddListener(StartCircuit);
        backBtn.onClick.AddListener(Start);
        ItemBtn.onClick.AddListener(item);
        ItemBox_Pressed.onClick.AddListener(StartCircuit);

        LEDBtn.onClick.AddListener(createLED);
        BatteryBtn.onClick.AddListener(createBattery);
        MRLineBtn.onClick.AddListener(createLine);


}


    void Update()
    {

    }


}
