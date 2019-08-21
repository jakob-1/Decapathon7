using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug : MonoBehaviour
{

    private Controller controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            printGridCheck();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            printGridValue();
        }
    }

    public void printGridCheck(){
        string report = "";
        for (int x = 0; x < controller.columns; x++)
        {
            report += "[";
            for (int y = 0; y < controller.rows; y++)
            {
                if(controller.grid[x,y] == null)
                {
                    report += "N, ";
                }
                else
                {
                    if (controller.gridChecked(x, y))
                    {
                        report += "T, ";
                    }
                    else
                    {
                        report += "F, ";
                    }
                }
            }
            report += "]\n";
        }
        print (report);
    }

    public void printGridValue()
    {
        string report = "";
        for (int x = 0; x < controller.columns; x++)
        {
            report += "[";
            for (int y = 0; y < controller.rows; y++)
            {
                if (controller.grid[x, y] == null)
                {
                    report += "N, ";
                }
                else
                {
                    report += controller.gridVal(x, y) + ", ";
                }
            }
            report += "]\n";
        }
        print(report);
    }
}
