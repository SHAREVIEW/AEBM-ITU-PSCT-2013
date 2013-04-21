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
        public Color TagColor { get; set; }
        public SolidColorBrush DisplayColor
        { 
            get {
                return new SolidColorBrush(TagColor);
            }
        }

        public string ThumbName
        {
            get
            {
                return TagValue + ": " + BTEndpoint.Address;
            }
        }

        private Dictionary<long, BluetoothEndPoint> addressTagMapping;
        private Dictionary<long, Color> colorMapping;
        public PhoneThumbVisualization()
        {
            InitializeComponent();
            InitStaticValues();
        }

        void InitStaticValues()
        {
            addressTagMapping = new Dictionary<long, BluetoothEndPoint>();
            addressTagMapping.Add(1, new BluetoothEndPoint(new BluetoothAddress(0xF8DB7F65F19D), new Guid("a60f35f0-b93a-11de-8a39-08002009c666")));
            addressTagMapping.Add(2, new BluetoothEndPoint(new BluetoothAddress(0xF3EB2F63F23B), new Guid("a60f35f0-b93a-11de-8a39-08002009c666")));

            colorMapping = new Dictionary<long, Color>();
            colorMapping.Add(1, Colors.Blue);
            colorMapping.Add(2, Colors.Red);
        }

        private void PhoneThumbVisualization_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public PhoneThumbVisualization(long tagValue)
        {
            InitStaticValues();
            TagColor = colorMapping.ContainsKey(tagValue) ? colorMapping[tagValue] : Colors.Transparent;
            BTEndpoint = addressTagMapping.ContainsKey(tagValue) ? addressTagMapping[tagValue] : new BluetoothEndPoint(new BluetoothAddress(0x000000000000), new Guid("a60f35f0-b93a-11de-8a39-08002009c666"));
            TagValue = tagValue;
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
