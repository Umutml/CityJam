using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LayerLab.CasualGame
{
    public class PanelView : MonoBehaviour
    {
        [SerializeField] private GameObject[] otherPanels;

        public void OnEnable()
        {
            for (var i = 0; i < otherPanels.Length; i++) otherPanels[i].SetActive(true);
        }

        public void OnDisable()
        {
            for (var i = 0; i < otherPanels.Length; i++) otherPanels[i].SetActive(false);
        }
    }
}
