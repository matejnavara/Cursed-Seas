using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UpdateHealth : MonoBehaviour
{
    public Ship PlayerShip;
    private Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        if(PlayerShip == null)
        {
            Debug.LogError("UpdateHealth script needs player ship attached");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerShip)
        {
            slider.value = PlayerShip.Health / (float)PlayerShip.MaxHealth;
        }
    }
}
