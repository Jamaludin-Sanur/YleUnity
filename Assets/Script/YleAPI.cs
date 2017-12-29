using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Yle
{
    /// <summary>
    /// API to get Areena’s content
    /// </summary>
    public class YleAPI : MonoBehaviour
    {
        /* Account credential, 
         (should remove this values)
         end user can set this values from method SetCredential */
        private string appId = "97f22537";
        private string appKey = "b0665fab6daa58e05afad26a56b0f495";
        private string secretKey = "f8455853ae3ddb8b";

        /// <summary>
        /// Contain the search keyword
        /// </summary>
        private StringBuilder _searchKeyword = new StringBuilder();

        /// <summary>
        /// Event which triggered when receive message from server
        /// </summary>
        public event Action<YleMessage> OnMessage;

        /// <summary>
        /// Triggered when there is error accessing the server
        /// </summary>
        public event Action<YleErrorMessage> OnError;

        /// <summary>
        /// Set account credential
        /// </summary>
        public void SetCredential(string appId, string appKey, string secretKey)
        {
            this.appId = appId;
            this.appKey = appKey;
            this.secretKey = secretKey;
        }

        /// <summary>
        /// Asyncronously find programs based on given keyword. 
        /// The result can be retrieve by listening to Event onMessage().
        /// Any error can be retrieve in Event onError()
        /// </summary>
        /// <param name="keyword">search keyword</param>
        /// <param name="limit"> search result limit</param>
        /// <param name="offset">search result index </param>
        public void SearchPrograms(string keyword, int limit, int offset)
        {
            SearchPrograms(keyword, limit, offset, YleConfiguration.DEFAULT_SEARCH_AVAILABILITY);
        }

        /// <summary>
        /// Asyncronously find programs based on given keyword. 
        /// The result can be retrieve by listening to Event onMessage().
        /// Any error can be retrieve in Event onError()
        /// </summary>
        /// <param name="keyword">search keyword</param>
        /// <param name="limit"> search result limit</param>
        /// <param name="offset">search result index </param>
        /// <param name="availability">program availability</param>
        public void SearchPrograms(string keyword, int limit, int offset, YleAvailability availability)
        {
            // clear/reset the search keyword
            _searchKeyword.Length = 0;
            _searchKeyword.Capacity = 0;

            // add keyword in url
            _searchKeyword.Append(YleConfiguration.URL_YLE_PROGRAMS + "?app_id=" + appId + "&app_key=" + appKey);

            // Check keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                _searchKeyword.Append("&q=" + keyword);
            }

            // Set data limit
            if (limit > 0)
            {
                _searchKeyword.Append("&limit=" + limit);
            }

            // Set data offset
            if (offset > 0)
            {
                _searchKeyword.Append("&offset=" + offset);
            }

            // set search availabillity
            if (availability.Equals(null))
            {
                _searchKeyword.Append("&availability=" + YleConfiguration.DEFAULT_SEARCH_AVAILABILITY.ToString().ToLower());
            }
            else
            {
                _searchKeyword.Append("&availability=" + availability.ToString().ToLower());
            }
            // send request
            StartCoroutine(sendRequest(_searchKeyword.ToString(), YleMessageType.MULTIPLE_PROGRAMS));
        }

        /// <summary>
        /// Get program based on given id asyncronously. 
        /// The result can be retrieve by listening to Event onMessage().
        /// Any error can be retrieve in Event onError()
        /// </summary>
        public void GetProgram(string id)
        {
            string url = string.Format("{0}?app_id={1}&app_key={2}&id={3}", YleConfiguration.URL_YLE_PROGRAMS, appId, appKey, id); ;
            StartCoroutine(sendRequest(url, YleMessageType.SINGLE_PROGRAM));
        }

        /// <summary>
        /// Get program image url based on given Id
        /// </summary>
        /// <param name="imageId">the image id</param>
        /// <returns>image url</returns>
        public string GetProgramImage(string imageId)
        {
            return GetProgramImage(imageId, YleConfiguration.DEFAULT_THUMBNAIL_WIDTH, YleConfiguration.DEFAULT_THUMBNAIL_HEIGHT, YleConfiguration.DEFAULT_THUMBNAIL_CROP,
                YleConfiguration.DEFAULT_THUMBNAIL_FORMAT);
        }

        /// <summary>
        /// Get program image url
        /// </summary>
        /// <param name="imageId">image id</param>
        /// <param name="width">image width</param>
        /// <param name="height">image height</param>
        /// <param name="scale">scale type</param>
        /// <param name="format">image format</param>
        /// <returns> the image url</returns>
        /// <remarks>
        /// Further information please check 'https://cloudinary.com/documentation/image_transformations#resizing_and_cropping_images'
        ///  </remarks>
        public string GetProgramImage(string imageId, int width, int height, string scale, string format)
        {
            var imgUrl = YleConfiguration.URL_CDN_IMAGE + "w_" + width + ",h_" + height + "," + scale + "/" + imageId + "." + format;
            return imgUrl;
        }

        /// <summary>
        /// Get media information asyncronously.
        /// The result can be retrieve by listening to Event onMessage().
        /// Any error can be retrieve in Event onError()
        /// </summary>
        /// <param name="programId"></param>
        /// <param name="mediaId"></param>
        public void GetMedia(string programId, string mediaId)
        {
            string url = YleConfiguration.URL_YLE_PLAYOUTS + "?program_id=" + programId + "&media_id=" + mediaId + "&protocol=" + YleConfiguration.DEFAULT_STREAM_PROTOCOL + "&app_id=" + appId + "&app_key=" + appKey;
            StartCoroutine(sendRequest(url, YleMessageType.MEDIA_DATA));
        }

        /// <summary>
        /// Decrypt media stream url
        /// </summary>
        /// <param name="url">url to decrypt</param>
        /// <returns> decrypted url </returns>
        public string DecryptStreamUrl(string url)
        {
            // decode the encrypted url
            byte[] baseDecoded = Convert.FromBase64String(url);

            // copy the first 16 bytes of the decoded url as 'iv' and the rest as 'msg'
            byte[] iv = new byte[16];
            byte[] msg = new byte[baseDecoded.Length - 16];
            Array.Copy(baseDecoded, 0, iv, 0, 16);
            Array.Copy(baseDecoded, 16, msg, 0, baseDecoded.Length - 16);

            // Create a decryptor for AES / CBC / PKCS7Padding
            AesManaged aes = new AesManaged();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            byte[] key = Encoding.UTF8.GetBytes(secretKey);
            ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);

            // will contain decryption result
            string urlResult;

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(msg))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        // Read the decrypted bytes
                        urlResult = srDecrypt.ReadToEnd();
                    }
                }
            }
            return urlResult;
        }

        /// <summary>
        /// Access the given url.
        /// The result will passed to event onMessage().
        /// Any error will trigger Event onError()
        /// </summary>
        /// <param name="url">url to access</param>
        /// <param name="msgType">flag to differentiate the message event</param>
        /// <returns></returns>
        private IEnumerator sendRequest(string url, YleMessageType msgType)
        {

#if UNITY_EDITOR
            Debug.Log(url);
#endif

            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                // error
                if (www.isNetworkError || www.isHttpError)
                {
                    YleErrorMessage errorMsg = new YleErrorMessage();
                    errorMsg.errorInfo = www.error;
                    errorMsg.details = msgType;
                    OnError(errorMsg);
                }
                //success
                else
                {
                    YleMessage msg = JsonUtility.FromJson<YleMessage>(www.downloadHandler.text);
                    msg.type = msgType;
                    OnMessage(msg);
                }
            }
        }
    }

    /// <summary>
    /// Flag to search program based on the availability
    /// </summary>
    public enum YleAvailability
    {
        OnDemand, FutureOndemand, FutureScheduled, InFuture
    }

}