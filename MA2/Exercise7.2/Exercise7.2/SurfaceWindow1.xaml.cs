using System;
using System.Collections.Generic;
using System.Linq;
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
using FlickrNet;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System.IO;

namespace Exercise7._2
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        public static ObservableCollection<DragableImageItem> Images { get; set; }
        Dictionary<long, Color> tagColors = new Dictionary<long, Color>();
        static ScatterView scw;
        Dictionary<long, bool> pinned;
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();
            pinned = new Dictionary<long, bool>();
            PhoneVisualization.PinnedEvent += Register_Pin;
            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
            Images = new ObservableCollection<DragableImageItem>();

            Images.CollectionChanged += Images_Changed;
            Images.CollectionChanged += SendImages_Changed;
            ImgScatterView.ItemsSource = Images;
            scw = ImgScatterView;
        }

        protected void Images_Changed(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ImgScatterView.Dispatcher.Invoke(new Action(() => ImgScatterView.UpdateLayout()));
        }

        protected void SendImages_Changed(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Do what?
        }


        protected void ScatterViewItem_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.Device.GetIsFingerRecognized())
            {
                var grid = sender as Grid;
                BitmapSource bms = ((grid.Background as ImageBrush).ImageSource as BitmapSource);
                var addr = new BluetoothAddress(0xF8DB7F65F19D);
                BluetoothEndPoint bte = new BluetoothEndPoint(addr, new Guid("a60f35f0-b93a-11de-8a39-08002009c666"));
                BluetoothHandler.SendBluetooth(bte, bms);
            }
            if (e.Device.GetIsTagRecognized())
            {
                var item = (Grid)sender;
                var ellipse = new Ellipse();
                ellipse.Name = "Ellipse";
                ellipse.Width = 10;
                ellipse.Height = 10;
                ellipse.Fill = new SolidColorBrush(tagColors[e.Device.GetTagData().Value]);
                (item.Children.Cast<FrameworkElement>().First(uie => uie.Name == "MarkingPanel") as WrapPanel).Children.Add(ellipse);
                item.UpdateLayout();
            }
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }

        private void QueryButton_Click(object sender, RoutedEventArgs e)
        {
            ImgScatterView.Visibility = Visibility.Visible;
            SearchBarPanel.Visibility = Visibility.Hidden;
            string query = QueryBox.Text;
            Task.Factory.StartNew(() => GetImages(query));
        }

        public void GetImages(string searchTerm)
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

                AddImage(img);
            }
        }

        public System.Drawing.Image DownloadImage(string _URL)
        {
            System.Drawing.Image _tmpImage = null;

            try
            {
                // Open a connection
                System.Net.HttpWebRequest _HttpWebRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(_URL);

                _HttpWebRequest.AllowWriteStreamBuffering = true;

                // set timeout for 20 seconds (Optional)
                _HttpWebRequest.Timeout = 20000;

                // Request response:
                System.Net.WebResponse _WebResponse = _HttpWebRequest.GetResponse();

                // Open data stream:
                System.IO.Stream _WebStream = _WebResponse.GetResponseStream();

                // convert webstream to image
                _tmpImage = System.Drawing.Image.FromStream(_WebStream);

                // Cleanup
                _WebResponse.Close();
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
                return null;
            }

            return _tmpImage;
        }

        private void ShowSearchButton_Click(object sender, RoutedEventArgs e)
        {
            ImgScatterView.Visibility = Visibility.Hidden;
            SearchBarPanel.Visibility = Visibility.Visible;
        }

        private void ClearImagesButton_Click(object sender, RoutedEventArgs e)
        {
            Images.Clear();
        }

        public static void AddImage(System.Drawing.Image img)
        {
            var oldBitmap = img as System.Drawing.Bitmap ?? new System.Drawing.Bitmap(img);
            var bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
            oldBitmap.GetHbitmap(System.Drawing.Color.Transparent),
             IntPtr.Zero,
             new Int32Rect(0, 0, oldBitmap.Width, oldBitmap.Height),
             null);
            bitmapSource.Freeze();
            scw.Dispatcher.Invoke(new Action(() => Images.Add(new DragableImageItem("", true, bitmapSource))));
        }

        private void Visualizer_VisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            pinned[e.TagVisualization.VisualizedTag.Value] = false;
            PhoneThumbVisualization ptv = null;
            foreach (PhoneThumbVisualization i in PinnedItems.Items)
            {
                if (i.TagValue == e.TagVisualization.VisualizedTag.Value)
                {
                    ptv = i;
                }
            }
            if (ptv != null)
            {
                PinnedItems.Items.Remove(ptv);
            }
            PhoneThumbVisualization newPtv = new PhoneThumbVisualization(e.TagVisualization.VisualizedTag.Value);
            PinnedItems.Items.Add(newPtv);
        }

        private void Register_Pin(object sender, PinnedEventArgs e)
        {
            if (e.Pinned)
            {
                pinned[e.TagValue] = true;
            }
            else
            {
                pinned[e.TagValue] = false;
            }
        }
        private void Visualizer_VisualizationRemoved(object sender, TagVisualizerEventArgs e)
        {
            if (pinned.ContainsKey(e.TagVisualization.VisualizedTag.Value))
            {
                if (!pinned[e.TagVisualization.VisualizedTag.Value])
                {
                    PhoneThumbVisualization ptv = null;
                    foreach (PhoneThumbVisualization i in PinnedItems.Items)
                    {
                        if (i.TagValue == e.TagVisualization.VisualizedTag.Value)
                        {
                            ptv = i;
                        }
                    }
                    if (ptv != null)
                    {
                        PinnedItems.Items.Remove(ptv);
                    }
                }
            }
        }

        private void ImgScatterView_DragCanceled(object sender, SurfaceDragDropEventArgs e)
        {
            DragableImageItem data = e.Cursor.Data as DragableImageItem;
            ScatterViewItem item = data.DraggedElement as ScatterViewItem;
            if (item != null)
            {
                item.Visibility = Visibility.Hidden;
                item.Orientation = e.Cursor.GetOrientation(this);
                item.Center = e.Cursor.GetPosition(this);
            }
        }

        private void ImgScatterView_DragCompleted(object sender, SurfaceDragCompletedEventArgs e)
        {
            if (e.Cursor.Effects == DragDropEffects.Move)
            {
                Images.Remove(e.Cursor.Data as DragableImageItem);
            }
        }

        private void PinnedItems_DragEnter(object sender, SurfaceDragDropEventArgs e)
        {
            e.Cursor.Visual.Tag = "DragEnter";
        }

        private void PinnedItems_DragLeave(object sender, SurfaceDragDropEventArgs e)
        {
            e.Cursor.Visual.Tag = null;
        }

        private void PinnedItems_Drop(object sender, SurfaceDragDropEventArgs e)
        {
            DragableImageItem image = e.Cursor.Data as DragableImageItem;
            SurfaceListBoxItem s = (SurfaceListBoxItem)sender;
            BluetoothHandler.SendBluetooth((s.Content as PhoneThumbVisualization).BTEndpoint, image.Image);
        }


        private void DragSourcePreviewInputDeviceDown(object sender, InputEventArgs e)
        {
            FrameworkElement findSource = e.OriginalSource as FrameworkElement;
            ScatterViewItem draggedElement = null;

            // Find the ScatterViewItem object that is being touched.
            while (draggedElement == null && findSource != null)
            {
                if ((draggedElement = findSource as ScatterViewItem) == null)
                {
                    findSource = VisualTreeHelper.GetParent(findSource) as FrameworkElement;
                }
            }

            if (draggedElement == null)
            {
                return;
            }

            DragableImageItem data = draggedElement.Content as DragableImageItem;

            // If the data has not been specified as draggable, 
            // or the ScatterViewItem cannot move, return.
            if (data == null || !data.CanDrag || !draggedElement.CanMove)
            {
                return;
            }

            // Set the dragged element. This is needed in case the drag operation is canceled.
            data.DraggedElement = draggedElement;

            // Create the cursor visual.
            ContentControl cursorVisual = new ContentControl()
            {
                Content = draggedElement.DataContext,
                Style = FindResource("CursorStyle") as Style
            };

            // Create a list of input devices, 
            // and add the device passed to this event handler.
            List<InputDevice> devices = new List<InputDevice>();
            devices.Add(e.Device);

            // If there are touch devices captured within the element,
            // add them to the list of input devices.
            foreach (InputDevice device in draggedElement.TouchesCapturedWithin)
            {
                if (device != e.Device)
                {
                    devices.Add(device);
                }
            }

            // Get the drag source object.
            ItemsControl dragSource = ItemsControl.ItemsControlFromItemContainer(draggedElement);

            // Start the drag-and-drop operation.
            SurfaceDragCursor cursor =
                SurfaceDragDrop.BeginDragDrop(
                // The ScatterView object that the cursor is dragged out from.
                  dragSource,
                // The ScatterViewItem object that is dragged from the drag source.
                  draggedElement,
                // The visual element of the cursor.
                  cursorVisual,
                // The data attached with the cursor.
                  draggedElement.DataContext,
                // The input devices that start dragging the cursor.
                  devices,
                // The allowed drag-and-drop effects of the operation.
                  DragDropEffects.Move);

            // If the cursor was created, the drag-and-drop operation was successfully started.
            if (cursor != null)
            {
                // Hide the ScatterViewItem.
                draggedElement.Visibility = Visibility.Hidden;

                // This event has been handled.
                e.Handled = true;
            }
        }
    }
}