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
        "Номер блока: Бл-24\nКатегория запасов: C2\nОбъем блока, м³: 8456354.62\nТоннаж, т: 19618742.73",
        "Номер блока: Бл-22\nКатегория запасов: С2\nОбъем блока, м³: 4235409.57\nТоннаж, т: 9826150.20",
        "Номер блока: Бл-20\nКатегория запасов: С2\nОбъем блока, м³: 3875191.70\nТоннаж, т: 8990444.73",
        "Номер блока: Бл-17\nКатегория запасов: С1\nОбъем блока, м³: 3383492.14\nТоннаж, т: 7849701.76",
        "Номер блока: Бл-16\nКатегория запасов: С1\nОбъем блока, м³: 4289131.63\nТоннаж, т: 9950785.38",
        "Номер блока: Бл-14\nКатегория запасов: С1\nОбъем блока, м³: 5006784.38\nТоннаж, т: 11615739.76",
        "Номер блока: Бл-12\nКатегория запасов: С1\nОбъем блока, м³: --\nТоннаж, т: --",
        "Номер блока: Бл-10\nКатегория запасов: С1\nОбъем блока, м³: --\nТоннаж, т: --",
        "Номер блока: Бл-23\nКатегория запасов: С2\nОбъем блока, м³: 2386204.71\nТоннаж, т: 5535994.94",
        "Номер блока: Бл-21\nКатегория запасов: С2\nОбъем блока, м³: 1679464.62\nТоннаж, т: 3896357.91",
        "Номер блока: Бл-19\nКатегория запасов: С2\nОбъем блока, м³: 2196342.37\nТоннаж, т: 5095514.30",
        "Номер блока: Бл-18\nКатегория запасов: С1\nОбъем блока, м³: 3840644.11\nТоннаж, т: 8910294.34",
        "Номер блока: Бл-15\nКатегория запасов: С1\nОбъем блока, м³: 4926823.68\nТоннаж, т: 11430230.94",
        "Номер блока: Бл-13\nКатегория запасов: С1\nОбъем блока, м³: 6153593.46\nТоннаж, т: 14276336.83",
        "Номер блока: Бл-11\nКатегория запасов: С1\nОбъем блока, м³: --\nТоннаж, т: --"
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
