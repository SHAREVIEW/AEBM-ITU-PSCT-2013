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

namespace Exercise7._2
{
    /// <summary>
    /// Interaction logic for PhoneVisualization.xaml
    /// </summary>
    public partial class PhoneVisualization : TagVisualization
    {
        public delegate void PinnedEventHandler(object sender, PinnedEventArgs e);
        public static event PinnedEventHandler PinnedEvent;
        public PhoneVisualization()
        {
            InitializeComponent();
        }

        private void PhoneVisualization_Loaded(object sender, RoutedEventArgs e)
        {
            string image = "Resources/iPhone.jpg";
            switch (this.VisualizedTag.Value)
            {
                case 1:
                    image = "Resources/iPhone.jpg";
                    break;
                case 2:
                    image = "Resources/Samsung.jpg";
                    break;

            }


            PhoneImage.Source = new BitmapImage(new Uri(image, UriKind.Relative));
        }

        private void TagVisualization_LostTag(object sender, RoutedEventArgs e)
        {

        }

        private void Lock_Checked(object sender, RoutedEventArgs e)
        {

            if (!(Lock.IsChecked ?? false))
            {
                PinnedEvent(this, new PinnedEventArgs(this.VisualizedTag.Value, false));
            }
            else
            {
                PinnedEvent(this, new PinnedEventArgs(this.VisualizedTag.Value, true));
            }
        }
    }
}
