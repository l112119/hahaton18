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
                            var picInfo = ExifReader.ReadJpeg(streamPic);
                            gpsLatitude.Text = picInfo.GpsLatitude.ToString();
                            gpsLongitude.Text = picInfo.GpsLongitude.ToString();
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
                        SaveToAlbum = true,
                        Directory = "android/data/com.android.providers.media",
                        Name = $"{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}.jpg",
                        SaveMetaData = true
                    });

                    if (file == null)
                    {
                        using (Stream streamPic = file.GetStream())
                        {
                            var picInfo = ExifReader.ReadJpeg(streamPic);
                            gpsLatitude.Text = picInfo.GpsLatitude.ToString();
                            gpsLongitude.Text = picInfo.GpsLongitude.ToString();
                        }
                        return;
                    }
                    
                    img.Source = ImageSource.FromFile(file.Path);
                }
            };
        }
    }
}
