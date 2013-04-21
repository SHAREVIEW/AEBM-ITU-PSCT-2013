using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlickrNet;

namespace Exercise7._2
{
    class FlickrHandler
    {

        public static void GetImages(string searchTerm)
        {
            Flickr fl = new Flickr("8704e391e50a52774ee2bf393c35b10c");

            PhotoSearchOptions opt = new PhotoSearchOptions();
            opt.Text = searchTerm;
            opt.PerPage = 10;
            opt.Page = 1;
            var photos = fl.PhotosSearch(opt);
            foreach (Photo p in photos)
            {
                System.Drawing.Image img = DownloadImage(p.LargeUrl);

                SurfaceWindow1.AddImage(img, System.Windows.Media.Colors.Transparent);
            }
        }

        public static System.Drawing.Image DownloadImage(string _URL)
        {
            System.Drawing.Image _tmpImage = null;

            try
            {
                System.Net.HttpWebRequest _HttpWebRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(_URL);
                _HttpWebRequest.AllowWriteStreamBuffering = true;
                _HttpWebRequest.Timeout = 20000;
                System.Net.WebResponse _WebResponse = _HttpWebRequest.GetResponse();
                System.IO.Stream _WebStream = _WebResponse.GetResponseStream();
                _tmpImage = System.Drawing.Image.FromStream(_WebStream);
                _WebResponse.Close();
            }
            catch (Exception _Exception)
            {
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
                return null;
            }

            return _tmpImage;
        }
    }
}
