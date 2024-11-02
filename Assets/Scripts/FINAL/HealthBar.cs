using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image _hPBar;

    private void Start()
    {
        _hPBar = gameObject.GetComponentInChildren<Image>();
        
    }

    public void UpdateHPBar(int vidaActual, int vidaMax)
    {
        _hPBar.fillAmount = (float)vidaActual / vidaMax;
        Debug.Log("Vida actual: " +(float) vidaActual / vidaMax);

    }

}