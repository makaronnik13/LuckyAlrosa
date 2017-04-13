using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoManager : MonoBehaviour {

    public GameObject LableController;

    public enum InfoType
    {
        pipe,
        mine
    }

    private string[] PipesInformation = new string[] {
        "1",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8",
        "9",
        "10",
        "11",
        "12",
        "13",
        "14",
        "15"
    };

    private string[] MinesInformation = new string[] {
        "1",
        "2",
        "3",
        "4"
    };


    private Dictionary<string, string> Information = new Dictionary<string, string>()
    {
                    {"airboxTubes", "Трубки какие-то."},
                    {"airboxCaps", "Какая-то залупа, к трубкам крепится."},
                    {"mainBlock","Корпус двигателя."},
                    {"caps1","Гайки нижние."},
                    {"caps2","Гайки верхние."},
                    {"springs1","Пружины верхние."},
                    {"springs2","Пружины нижние"},
                    {"wlywheel","Маховик - массивное вращающееся колесо, использующееся в качестве накопителя (инерционный аккумулятор) кинетической энергии."},
                    {"myffler","Глушитель — устройство для снижения шума от выходящих в атмосферу газов или воздуха из различных устройств."},
                    {"filter","Фильтр."} 
                };

    internal void HideLable()
    {
        LableController.GetComponent<HitechLabelController>().Hide();
    }

    internal void ShowLable(InfoType type, int id, Transform tr)
    {
        LableController.transform.SetParent(tr);
        LableController.transform.localPosition = Vector3.zero;
        switch (type)
        {
            case InfoType.pipe:
                LableController.GetComponent<HitechLabelController>().Show(PipesInformation[id]);
                break;
            case InfoType.mine:
                LableController.GetComponent<HitechLabelController>().Show(MinesInformation[id]);
                break;
        }
        
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    internal string GetInfo(string partName)
    {
        if (!Information.ContainsKey(partName))
        {
            return "Типа описание части. Бла-бла-бла, эта приблуда крутит эту штуку и используется для всякого.";
        }
        return Information[partName];
    }
}
