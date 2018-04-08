using ExifLib;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

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
            Label gpsLatitude = new Label { Text = "Широта" };
            Label gpsLongitude = new Label { Text = "Долгота" };

            var map = new CustomMap
            {
                IsShowingUser = true,
                HeightRequest = 100,
                WidthRequest = 960,
                VerticalOptions = LayoutOptions.FillAndExpand
            };


            map.RouteCoordinates.Add(new Position(37.785559, -122.396728));
            // map.RouteCoordinates.Add(new Position(37.780624, -122.390541));
            // map.RouteCoordinates.Add(new Position(37.777113, -122.394983));
            map.RouteCoordinates.Add(new Position(37.776831, -122.394627));

            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(37.79752, -122.40183), Distance.FromMiles(1.0)));



            stackLayout.Children.Add(map);
            stackLayout.Children.Add(takePhotoBtn);
            stackLayout.Children.Add(getPhotoBtn);
            stackLayout.Children.Add(gpsLatitude);
            stackLayout.Children.Add(gpsLongitude);
            this.Content = stackLayout;

            Image img = new Image();

            //Image img = new Image();

            // выбор фото
            getPhotoBtn.Clicked += async (o, e) =>
            {
                if (CrossMedia.Current.IsPickPhotoSupported)
                {
                    MediaFile photoPicked = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions { SaveMetaData = true });

                    if (photoPicked != null)
                    {
                        await DisplayAlert("Photo Location", photoPicked.Path, "OK");

                        using (Stream streamPic = photoPicked.GetStream())
                        {
                            var picInfo = ExifReader.ReadJpeg(streamPic);
                            double[] latitudeComponents = picInfo.GpsLatitude;
                            double[] longitudeComponents = picInfo.GpsLongitude;

                            gpsLatitude.Text = latitudeComponents[0].ToString() + latitudeComponents[1] / 60 + latitudeComponents[2] / 3600;
                            gpsLongitude.Text = longitudeComponents[0].ToString() + longitudeComponents[1] / 60 + longitudeComponents[2] / 3600;
                        }
                    }
                }
            };

            takePhotoBtn.Clicked += async (o, e) =>
            {
                await CrossMedia.Current.Initialize();

                if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
                {
                    //var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);

                    MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions());
                    //{
                    //    SaveToAlbum = true,
                    //    Directory = "android/data/com.android.providers.media",
                    //    Name = $"{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}.jpg",
                    //    DefaultCamera = CameraDevice.Rear,
                    //    Location = new Location()
                    //});

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
