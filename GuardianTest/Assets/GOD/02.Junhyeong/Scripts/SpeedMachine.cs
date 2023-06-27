using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedMachine : MonoBehaviour
{
    Material green_Material;
    Color green_OriColor;
    Material green_Material2;

    bool isOn = false;
    // Start is called before the first frame update

    private void Start()
    {
        ChangeEmissionForOnOff.InitImissionColor(this.transform, ref green_Material, ref green_Material2, ref green_OriColor);
        GameManager.Instance.attacked += SpeedUp;
        //GameManager.Instance.acCalling += SpeedDown;
    }
    void SpeedUp()
    {
        ChangeEmissionForOnOff.OnEmissionColor(green_Material, green_Material2, green_OriColor);
        Time.timeScale = GameManager.Instance.speedMachineSpeed;
        isOn = true;

        Achievement.Instance.AchieveCheck(eAchieveState.ActivateSpeed);
    }
    void SpeedDown()
    {
        ChangeEmissionForOnOff.OffEmissionColor(green_Material, green_Material2);
        Time.timeScale = 1f;
        isOn = false;
    }
    private void OnMouseDown()
    {
        if (isOn) SpeedDown();
        else SpeedUp();

    }
}
