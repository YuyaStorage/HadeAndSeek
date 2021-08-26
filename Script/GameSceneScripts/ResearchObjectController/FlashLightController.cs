using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashLightController : ResearchObject
{
    void Start()
    {
        Buttons[0].GetComponent<Button>().onClick.AddListener(GetFlashLight);
    }

    // カメラについている懐中電灯をアクティブにする．
    void GetFlashLight()
    {
        Camera.main.transform.Find("FlashLight").gameObject.SetActive(true);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().BeMovablePlayer();
        Destroy(gameObject);
    }
}
