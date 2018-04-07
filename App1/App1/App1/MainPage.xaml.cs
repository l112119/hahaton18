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

            Button getPhotoBtn = new Button { Text = "Выбрать фото" };

            stackLayout.Children.Add(getPhotoBtn);
            this.Content = stackLayout;

            Image img = new Image();

            // выбор фото
            getPhotoBtn.Clicked += async (o, e) =>
            {
                //if (CrossMedia.Current.IsPickPhotoSupported)
                //{
                //    MediaFile photo = await CrossMedia.Current.PickPhotoAsync();
                //    img.Source = ImageSource.FromFile(photo.Path);
                //}
               // bool b = await CrossMedia.Current.Initialize();

                if (CrossMedia.Current.IsPickPhotoSupported)
                {
                    MediaFile photoPicked = await CrossMedia.Current.PickPhotoAsync();

                    if (photoPicked != null)
                    {
                        await DisplayAlert("Photo Location", photoPicked.Path, "OK");

                        using (Stream streamPic = photoPicked.GetStream())
                        {
                            var picInfo = ExifReader.ReadJpeg(streamPic);
                            ExifOrientation orientation = picInfo.Orientation;
                        }
                    }
                }
            };
        }
    }
}
