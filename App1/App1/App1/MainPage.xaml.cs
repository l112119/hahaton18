using ExifLib;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App1
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            StackLayout stackLayout = new StackLayout();

            Button takePhotoBtn = new Button { Text = "Сделать фото" };
            Button getPhotoBtn = new Button { Text = "Выбрать фото" };
            Label gpsLatitude = new Label { Text =  "Широта" };
            Label gpsLongitude = new Label { Text = "Долгота" };

            stackLayout.Children.Add(takePhotoBtn);
            stackLayout.Children.Add(getPhotoBtn);
            stackLayout.Children.Add(gpsLatitude);
            stackLayout.Children.Add(gpsLongitude);
            this.Content = stackLayout;

            Image img = new Image();

            //*Точки маршрута
            List<WayPoint> wayPoints = new List<WayPoint>() {  };
            wayPoints.Add(new WayPoint("", "08.04.2018", "55,750904", "48,748493", "the first point"));

            //Image img = new Image();

            // выбор фото
            getPhotoBtn.Clicked += async (o, e) =>
            {
                if (CrossMedia.Current.IsPickPhotoSupported)
                {
                    MediaFile photoPicked = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions {  SaveMetaData = true });

                    if (photoPicked != null)
                    {
                        await DisplayAlert("Photo Location", photoPicked.Path, "OK");

                        using (Stream streamPic = photoPicked.GetStream())
                        {
                            //    ExifReader picInfo = new ExifReader(ExifReader.ReadJpeg(streamPic));
                            //picInfo.

                            //double[] latitudeComponents;
                            //streamPic.GetTagValue(ExifTags.GPSLatitude, out latitudeComponents);

                            //double[] longitudeComponents;
                            //reader.GetTagValue(ExifTags.GPSLongitude, out longitudeComponents);

                            //// Lat/long are stored as D°M'S" arrays, so you will need to reconstruct their values as below:
                            //var latitude = latitudeComponents[0] + latitudeComponents[1] / 60 + latitudeComponents[2] / 3600;
                            //var longitude = longitudeComponents[0] + longitudeComponents[1] / 60 + longitudeComponents[2] / 3600;

                            JpegInfo picMetadata = ExifReader.ReadJpeg(streamPic);
                            double[] latitudeComponents = picMetadata.GpsLatitude;
                            double[] longitudeComponents = picMetadata.GpsLongitude;

                            gpsLatitude.Text = latitudeComponents[0].ToString() + latitudeComponents[1] / 60 + latitudeComponents[2] / 3600;
                            gpsLongitude.Text = longitudeComponents[0].ToString() + longitudeComponents[1] / 60 + longitudeComponents[2] / 3600;

                            WayPoint openedPhotoWP = new WayPoint();
                            openedPhotoWP.DateOfCreation = picMetadata.DateTimeOriginal;
                            openedPhotoWP.GpsLatitude = gpsLatitude.Text;
                            openedPhotoWP.GpsLongitude = gpsLongitude.Text;
                            openedPhotoWP.PicturePath = photoPicked.Path;
                            openedPhotoWP.Comment = picMetadata.Description; 

                            wayPoints.Add(openedPhotoWP);
                        }
                    }
                }
            };

            takePhotoBtn.Clicked += async (o, e) =>
            {
                if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
                {
                    MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        DefaultCamera = CameraDevice.Rear,
                        SaveToAlbum = true,
                        //Directory = "android/data/com.android.providers.media",
                        Name = $"{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}.jpg",
                        SaveMetaData = true
                    });

                    //Intent intent = new Intent(MediaStore.ActionImageCapture);
                    //App._file = new File(App._dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
                    //intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(App._file));
                    //StartActivityForResult(intent, 0);

                    if (file == null)
                    {
                        using (Stream streamPic = file.GetStream())
                        {
                            var picInfo = ExifReader.ReadJpeg(streamPic);
                            gpsLatitude.Text = picInfo.GpsLatitude[0].ToString();
                            gpsLongitude.Text = picInfo.GpsLongitude[0].ToString();
                        }
                        return;
                    }
                    
                    img.Source = ImageSource.FromFile(file.Path);
                }
            };
        }
    }
}
