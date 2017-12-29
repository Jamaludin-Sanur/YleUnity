using System;
using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Yle.UI
{
    /// <summary>
    /// UI to search arenaa's content
    /// </summary>
    public class UI_SearchForm : MonoBehaviour
    {

        public YleAPI yleAPI;
        private int searchLimit = 10;       // search limit
        private int searchOffset = 0;       // search result index
        private string searchKeyword = "";  // search keyword

        public UI_ContentViewer uiContentViewerScript; // UI script to display program content in details

        public InputField inputKeyword;
        public GameObject uiListContainer;
        public GameObject uiListItem;
        public GameObject uiLoading;
        public GameObject uiEndResult;
        public ScrollRect uiScrollRect;

        // Use this for initialization
        void Start()
        {
            // Register event when receiving message from server
            yleAPI.OnMessage += OnReceiveMessage;

            // Register event when receiving error accessing server
            yleAPI.OnError += OnError;

            uiContentViewerScript = Instantiate(uiContentViewerScript);
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Search programs when user hit pre defined key 'Submit' in input field.
        /// Could be 'Enter' in Windows
        /// </summary>
        public void onInputFieldSubmit()
        {
            if (Input.GetButtonDown("Submit"))
            {
                SearchPrograms();
            }
        }

        /// <summary>
        /// Clear current search result then search new programs.
        /// Triggered when user hit search  button.
        /// </summary> 
        public void SearchPrograms()
        {
            // clear current search result
            clearList();

            // display loading UI
            showLoading(true);

            // cache search keyword for searching more deeper later when user sroll down the list
            searchKeyword = inputKeyword.text;

            // reset current search offset
            searchOffset = 0;

            // hide end result UI which may visible due to previous search
            showEndResult(false);

            // request programs list to server
            yleAPI.SearchPrograms(searchKeyword, searchLimit, searchOffset);
        }

        /// <summary>
        /// Search programs based on existing keyword and offset.
        /// Triggered when user scroll the middle mouse or dragging  the vertical scrollbar
        /// </summary>
        public void onListScroll()
        {
            // check if user scroll near the bottom
            if (uiScrollRect.verticalNormalizedPosition <= 0.01 && !isLoading() && !isEndResult())
            {
                // display loading UI
                showLoading(true);

                // request programs list to server
                yleAPI.SearchPrograms(searchKeyword, searchLimit, searchOffset);
            }
        }

        /// <summary>
        /// Get program information.
        /// Triggered when program in the list clicked.
        /// </summary>
        /// <param name="source"></param>
        public void onListItemSelected(UI_ListItem source)
        {
            // request program information to server
            yleAPI.GetProgram(source.Id);
        }

        /// <summary>
        /// Add program to the list in UI
        /// </summary>
        /// <param name="id">program id</param>
        /// <param name="imgId">program image id</param>
        /// <param name="title">program title</param>
        /// <param name="itemTitle">program item title</param>
        /// <param name="description">program description</param>
        /// <returns>Game object containg the program </returns>
        private GameObject addListItem(string id, string imgId, string title, string itemTitle, string description)
        {
            // Create the list item UI
            GameObject objListItem = Instantiate(uiListItem);

            // Set the value in the UI
            UI_ListItem listItem = objListItem.GetComponent<UI_ListItem>();
            listItem.Id = id;                    // set ID
            listItem.Title = title;             // set title
            listItem.ItemTitle = itemTitle;     // set item title
            listItem.Description = description;  // set description

            // Add click listener
            Button listItemButton = listItem.GetComponentInChildren<Button>();
            listItemButton.onClick.AddListener(delegate { onListItemSelected(listItem); });

            // Add the item to list container
            listItem.transform.SetParent(uiListContainer.transform, false);

            return objListItem;
        }

        /// <summary>
        /// Load program image
        /// </summary>
        /// <param name="url"></param>
        /// <param name="listItem"></param>
        /// <returns></returns>
        public IEnumerator LoadListItemImage(string url, GameObject listItem)
        {
            // accessing image url
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url, false);
            yield return www.SendWebRequest();
            while (!www.isDone)
            {
                yield return www;
            }
            if (www.isNetworkError || www.isHttpError || www.responseCode != 200)
            {
                yield return www;

            }

            // get image
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            
            // load image
            GameObject imgObject = listItem.transform.Find("img").gameObject;
            Image img = imgObject.GetComponentInChildren<Image>();
            img.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 100);
        }

        /// <summary>
        /// Clear current search result
        /// </summary>
        private void clearList()
        {
            foreach (Transform child in uiListContainer.transform)
            {
                /// dont destroy loading ui in the list
                if (child.name == uiLoading.name)
                {
                    continue;
                }
                /// dont destroy end ui result in the list
                if (child.name == uiEndResult.name)
                {
                    continue;
                }
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Show/hide UI loading
        /// </summary>
        /// <param name="show"></param>
        private void showLoading(bool show)
        {
            if (show)
            {
                // diplay loading UI in most bottom
                uiLoading.transform.SetSiblingIndex(uiListContainer.transform.childCount - 1);
                uiLoading.SetActive(true);
            }
            else
            {
                uiLoading.SetActive(false); // hide UI
            }
        }

        /// <summary>
        /// Show/hide End Result UI
        /// </summary>
        /// <param name="show"></param>
        private void showEndResult(bool show)
        {
            if (show)
            {
                // diplay End Result UI in most bottom
                uiEndResult.transform.SetSiblingIndex(uiListContainer.transform.childCount);
                uiEndResult.SetActive(true);
            }
            else
            {
                uiEndResult.SetActive(false);
            }
        }

        /// <summary>
        /// Check if the loading UI is visible
        /// </summary>
        private bool isLoading()
        {
            return uiLoading.activeSelf;
        }

        /// <summary>
        /// Check if search result is in the end.
        /// </summary>
        private bool isEndResult()
        {
            return uiEndResult.activeSelf;
        }

        /// <summary>
        /// Triggered when received message from server
        /// </summary>
        /// <param name="msg"></param>
        private void OnReceiveMessage(YleMessage msg)
        {
            // propagate the event
            switch (msg.type)
            {
                case YleMessageType.MULTIPLE_PROGRAMS:
                    OnReceiveProgramList(msg);
                    break;
                case YleMessageType.SINGLE_PROGRAM:
                    OnReceiveProgram(msg);
                    break;
                case YleMessageType.MEDIA_DATA:
                    onReceiveMedia(msg);
                    break;
            }
        }

        /// <summary>
        /// Triggered when 'OnReceiveMessage' receive multiple programs from server.
        /// This will populate the UI with the received programs
        /// </summary>
        /// <param name="msg"></param>
        private void OnReceiveProgramList(YleMessage msg)
        {
            // hide loading UI since the result already received
            showLoading(false);

            // move the vertical scrollbar to top
            uiScrollRect.verticalNormalizedPosition = 0;

            foreach (var data in msg.data)
            {
                // extract program ID
                string id = data.id;
                // extract program title
                string title = data.title.fi ?? data.title.sv ?? data.title.und ?? data.title.se;
                // extract item title
                string itemTitle = data.itemTitle.fi ?? data.itemTitle.sv ?? data.itemTitle.und ?? data.itemTitle.se;
                // extract description
                string description = data.description.fi ?? data.description.sv ?? data.description.se;
                // extract image id
                string imgId = data.image.id ?? data.partOfSeries.image.id;        

                // add list item to UI
                GameObject listItem = addListItem(id, imgId, title, itemTitle, description);

                if (!string.IsNullOrEmpty(imgId))
                {
                    // load image asyncronously
                    var imgUrl = yleAPI.GetProgramImage(imgId);
                    StartCoroutine(LoadListItemImage(imgUrl, listItem));
                }
            }

            // Increase offset for next search
            searchOffset += msg.data.Count;

            // Show end result UI if there's no more data
            if (msg.data.Count < searchLimit)
            {
                showEndResult(true);
            }
        }

        /// <summary>
        /// Triggered when 'OnReceiveMessage' receive program from server.
        /// This will show the program with more details in UI ContentViewer
        /// </summary>
        /// <param name="msg"></param>
        private void OnReceiveProgram(YleMessage msg)
        {
            if (msg.data.Count <= 0)
            {
                return;
            }

            // get the program
            var data = msg.data[0];
            // extract program ID
            uiContentViewerScript.Id = data.id;
            // extract program title
            uiContentViewerScript.Title = data.title.fi ?? data.title.sv ?? data.title.und ?? data.title.se;
            // extract item title
            uiContentViewerScript.ItemTitle = data.itemTitle.fi ?? data.itemTitle.sv ?? data.itemTitle.und ?? data.itemTitle.se;
            // extract description
            uiContentViewerScript.Description = data.description.fi ?? data.description.sv ?? data.description.se;  

            // Set Duration
            TimeSpan ts = XmlConvert.ToTimeSpan(data.duration); 
            uiContentViewerScript.Duration = string.Format("{0:0} h {1:0} min", ts.Hours, ts.Minutes);

            // Put the media file link as "In progres ..." at first to display the UI quickly.
            // We will access the file link asyncronously
            uiContentViewerScript.FileLink = "In progres ...";

            // check publication event
            if (data.publicationEvent.Count >= 0)
            {
                // get latest publication event
                YleDataPublicationEvent lastEvt = data.publicationEvent[0];

                // set date
                string dateString = lastEvt.startTime;
                DateTime date = Convert.ToDateTime(dateString);
                uiContentViewerScript.Date = string.Format("{0:dd-MM-yyyy}", date);

                // get media file link
                yleAPI.GetMedia(data.id, lastEvt.media.id);
            }

            // no publication event data
            else {
                uiContentViewerScript.Date = "";
                uiContentViewerScript.FileLink = "Not Available";
            }

            // extract image id
            string imgId = data.image.id ?? data.partOfSeries.image.id;

            // load image
            if (!string.IsNullOrEmpty(imgId))
            {
                var imgUrl = yleAPI.GetProgramImage(imgId, 1280, 720, "c_fit", "jpg");
                StartCoroutine(LoadListItemImage(imgUrl, uiContentViewerScript.gameObject));
            }

            // show UI Content Viewer
            uiContentViewerScript.gameObject.SetActive(true);
        }

        /// <summary>
        /// Triggered when 'OnReceiveMessage' receive media data from server.
        /// This will show the link to stream file  
        /// </summary>
        /// <param name="msg"></param>
        private void onReceiveMedia(YleMessage msg)
        {
            if (msg.data.Count <= 0)
            {
                return;
            }

            // extract media data
            var data = msg.data[0];
            if (!string.IsNullOrEmpty(data.url))
            {
                // decrypt file url
                string decryptedUrl = yleAPI.DecryptStreamUrl(data.url);

                // show the file link
                uiContentViewerScript.FileLink = decryptedUrl;
            }
        }

        /// <summary>
        /// Triggered when there is an error accessing the server
        /// </summary>
        /// <param name="errorMsg">error message</param>
        private void OnError(YleErrorMessage errorMsg)
        {
            // check if the error details is instance of Yle message
            if (errorMsg.details is YleMessageType)
            {
                // check if the error caused by accessing the media file link
                YleMessageType errorEvt = (YleMessageType)errorMsg.details;
                if (errorEvt == YleMessageType.MEDIA_DATA) {
                    uiContentViewerScript.FileLink = "Not Available";
                }
            }
        }

    }
}