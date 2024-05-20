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
    public Button CircuitCheckBtn;

    public GameObject LED;
    public GameObject LED_5R;
    public GameObject LED_7R;
    public GameObject Battery;
    public GameObject Battery_3V;
    public GameObject Battery_5V;
    public GameObject MRLine;

    public Button ItemBtn;
    public GameObject circuitPanel;

    public Button ItemBox_Pressed;
    public GameObject itemPanel;

    public Button LEDBtn;
    public Button LEDBtn_5R;
    public Button LEDBtn_7R;
    public Button BatteryBtn;
    public Button BatteryBtn_3V;
    public Button BatteryBtn_5V;
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
    public void createBattery_3V()
    {
        Instantiate(Battery_3V, new Vector3(0, 0, 0), this.transform.rotation);
    }
    public void createLED_5R()
    {
        Instantiate(LED_5R, new Vector3(0, 0, 0), this.transform.rotation);
    }
    public void createBattery_5V()
    {
        Instantiate(Battery_5V, new Vector3(0, 0, 0), this.transform.rotation);
    }
    public void createLED_7R()
    {
        Instantiate(LED_7R, new Vector3(0, 0, 0), this.transform.rotation);
    }




    public void circuitCheck()
    {
        Debug.Log("checking...");
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
        LEDBtn_5R.onClick.AddListener(createLED_5R);
        BatteryBtn_3V.onClick.AddListener(createBattery_3V);
        LEDBtn_7R.onClick.AddListener(createLED_7R);
        BatteryBtn_5V.onClick.AddListener(createBattery_5V);
        MRLineBtn.onClick.AddListener(createLine);
        CircuitCheckBtn.onClick.AddListener(circuitCheck);


}


    void Update()
    {

    }


}
