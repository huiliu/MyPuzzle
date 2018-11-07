using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MapEditor
{
    public class CheckResultComponent
        : MonoBehaviour
        , IPointerDownHandler
        , IPointerUpHandler
    {
        [SerializeField] private Text ResultText;

        private void OnDisable()
        {
            this.ResultText.text = "";
        }

        public void SetResult(string result)
        {
            this.ResultText.text = result;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            this.gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }
    }
}
