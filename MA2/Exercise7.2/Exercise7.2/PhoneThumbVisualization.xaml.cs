using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using InTheHand.Net;

namespace Exercise7._2
{
    /// <summary>
    /// Interaction logic for PhoneThumbVisualization.xaml
    /// </summary>
    public partial class PhoneThumbVisualization : TagVisualization
    {
        public long TagValue { get; set; }
        public BluetoothEndPoint BTEndpoint { get; set; }
        public string ThumbName
        {
            get
            {
                return TagValue + ": " + BTEndpoint.Address;
            }
        }

        private Dictionary<long, BluetoothEndPoint> addressTagMapping;
        public PhoneThumbVisualization()
        {
            InitializeComponent();
            InitAddresses();
        }

        void InitAddresses()
        {
            addressTagMapping = new Dictionary<long, BluetoothEndPoint>();
            addressTagMapping.Add(1, new BluetoothEndPoint(new BluetoothAddress(0xF8DB7F65F19D), new Guid("a60f35f0-b93a-11de-8a39-08002009c666")));
            addressTagMapping.Add(2, new BluetoothEndPoint(new BluetoothAddress(0x000000000000), new Guid("a60f35f0-b93a-11de-8a39-08002009c666")));
        }

        private void PhoneThumbVisualization_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public PhoneThumbVisualization(long tagValue)
        {
            InitAddresses();
            PhoneType = new Label();
            if (addressTagMapping.ContainsKey(tagValue)) BTEndpoint = addressTagMapping[tagValue];
            TagValue = tagValue;
            switch (tagValue)
            {
                case 1: PhoneType.Content = "iPhone 4";
                    break;
                case 2: PhoneType.Content = "Samsung S3";
                    break;
                default: PhoneType.Content = "Unknown phone";
                    break;
            }
        }

        private void StackPanel_Drop(object sender, SurfaceDragDropEventArgs e)
        {
            DragableImageItem item = e.Cursor.Data as DragableImageItem;
            if (BTEndpoint != null)
            {
                BluetoothHandler.SendBluetooth(BTEndpoint, item.Image);
            }
        }
    }
}
