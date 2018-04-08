using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App1
{
    public class WayPoint
    {
        public string PicturePath;
        public string DateOfCreation;
        public string GpsLatitude;
        public string GpsLongitude;
        public string Comment;

        public WayPoint()
        {
        }

        public WayPoint(string picturePath, string dateOfCreation, string gpsLatitude, string gpsLongitude, string comment)
        {
            PicturePath = picturePath;
            DateOfCreation = dateOfCreation;
            GpsLatitude = gpsLatitude;
            GpsLongitude = gpsLongitude;
            Comment = comment;
        }
    }
}