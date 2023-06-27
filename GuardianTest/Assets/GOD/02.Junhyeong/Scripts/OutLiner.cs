using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutLiner : MonoBehaviour
{
    [SerializeField] GameObject outLine;
    private void OnMouseEnter()
    {
        outLine.SetActive(true);
    }

    private void OnMouseExit()
    {
        outLine.SetActive(false);
    }
}
