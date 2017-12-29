using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Yle.UI
{
    /// <summary>
    /// UI component to view program content in details
    /// </summary>
    public class UI_ContentViewer : MonoBehaviour
    {
        public ScrollRect scrollbar;
        public Image imgIcon;
        public Text txtTitle;
        public Text txtItemTitle;
        public Text txtDescription;
        public Text txtTime;
        public Text txtDate;
        public Text txtFileLink;
        public Sprite defaultImgIcon;

        /// <summary>
        /// program id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// program title
        /// </summary>
        public string Title
        {
            get { return txtTitle.text; }
            set { txtTitle.text = value; }
        }

        /// <summary>
        /// program item title ?
        /// </summary>
        public string ItemTitle
        {
            get { return txtItemTitle.text; }
            set { txtItemTitle.text = value; }
        }

        /// <summary>
        /// program description
        /// </summary>
        public string Description
        {
            get { return txtDescription.text; }
            set { txtDescription.text = value; }
        }

        /// <summary>
        /// The last program publication starting date
        /// </summary>
        public string Date
        {
            get { return txtDate.text; }
            set { txtDate.text = value; }
        }

        /// <summary>
        /// Program or media duration
        /// </summary>
        public string Duration
        {
            get { return txtTime.text; }
            set { txtTime.text = value; }
        }

        /// <summary>
        /// Represent stream url
        /// </summary>
        public string FileLink
        {
            get { return txtFileLink.text; }
            set { txtFileLink.text = value; }
        }

        void Start(){}

        void Update(){}

        /// <summary>
        /// Hide this game object.
        /// Triggered when user click close button
        /// </summary>
        public void Close()
        {
            this.gameObject.SetActive(false);
            ResetUI();
        }

        /// <summary>
        /// Reset any value in the UI
        /// </summary>
        private void ResetUI() {
            imgIcon.sprite = defaultImgIcon;
            txtTitle.text = "";
            txtItemTitle.text = "";
            txtDescription.text = "";
            txtDate.text = "";
            txtTitle.text = "";
            txtFileLink.text = "";
            scrollbar.verticalNormalizedPosition = 1;
        }

        /// <summary>
        /// Copy file link to clipboard.
        /// Triggered when user hit copy button.
        /// </summary>
        public void CopyFileLink() {
            GUIUtility.systemCopyBuffer = txtFileLink.text;
        }
    }

}