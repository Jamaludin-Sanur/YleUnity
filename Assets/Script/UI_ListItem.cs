using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yle.UI
{
    /// <summary>
    /// List item UI containing program information
    /// </summary>
    public class UI_ListItem : MonoBehaviour
    {
        // The UI
        public Text txtTitle;
        public Text txtItemTitle;
        public Text txtDescription;

        /// <summary>
        /// Program id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Media id
        /// </summary>
        public string MediaId { get; set; }

        /// <summary>
        /// Program Title
        /// </summary>
        public string Title
        {
            get
            {
                return txtTitle.text;
            }
            set
            {
                txtTitle.text = value;
            }
        }

        /// <summary>
        /// Program itemTitle
        /// </summary>
        public string ItemTitle
        {
            get
            {
                return txtItemTitle.text;
            }
            set
            {
                txtItemTitle.text = value;
            }
        }

        /// <summary>
        /// Program Description
        /// </summary>
        public string Description
        {
            get
            {
                return txtDescription.text;
            }
            set
            {
                txtDescription.text = value;
            }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}