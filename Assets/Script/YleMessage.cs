using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yle
{
    /// <summary>
    /// Class used to mapped json response from server
    /// </summary>
    [System.Serializable]
    public struct YleMessage
    {
        public string apiVersion;
        public List<YleData> data;
        public YleMessageType type;
    }

    /// <summary>
    /// Contains program data
    /// </summary>
    [System.Serializable]
    public struct YleData
    {
        public string id;
        public string duration;
        public string url;
        public YleDataTitle title;
        public YleDataItemTitle itemTitle;
        public YleDataDescription description;
        public YleDataPartOfSeries partOfSeries;
        public List<YleDataPublicationEvent> publicationEvent;
        public YleDataImage image;

    }

    /// <summary>
    /// Represent publication event data
    /// </summary>
    [System.Serializable]
    public struct YleDataPublicationEvent
    {
        public YleDataMedia media;
        public string startTime;
    }


    /// <summary>
    /// Represent the media data
    /// </summary>
    [System.Serializable]
    public struct YleDataMedia
    {
        public string id;
        public bool available;
    }

    /// <summary>
    /// Represent title data
    /// </summary>
    [System.Serializable]
    public struct YleDataTitle
    {
        public string fi;
        public string sv;
        public string und;
        public string se;
    }

    /// <summary>
    /// Represent item title data
    /// </summary>
    [System.Serializable]
    public struct YleDataItemTitle
    {
        public string fi;
        public string sv;
        public string und;
        public string se;
    }

    /// <summary>
    /// Represent description data
    /// </summary>
    [System.Serializable]
    public struct YleDataDescription
    {
        public string fi;
        public string sv;
        public string se;
    }

    /// <summary>
    /// Represent image data
    /// </summary>
    [System.Serializable]
    public struct YleDataImage
    {
        public string id;
        public bool available;
        public string type;
        public string version;
    }

    /// <summary>
    /// Represent part of series data
    /// </summary>
    [System.Serializable]
    public struct YleDataPartOfSeries
    {
        public YleDataImage image;
    }


    /// <summary>
    /// Enum to differentiate message
    /// </summary>
    [System.Serializable]
    public enum YleMessageType
    {
        // The message may contain many programs
        MULTIPLE_PROGRAMS,

        // The message contain single programs
        SINGLE_PROGRAM,

        // The message contain media/file data
        MEDIA_DATA
    }

    /// <summary>
    /// Represent an error message
    /// </summary>
    [System.Serializable]
    public struct YleErrorMessage
    {
        public string errorInfo;// error info
        public object details;  // general purpose object
    }
}