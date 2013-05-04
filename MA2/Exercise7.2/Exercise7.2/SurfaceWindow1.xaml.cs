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
        public static ObservableCollection<PhoneThumbVisualization> Thumbs { get; set; }
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
            AddWindowAvailabilityHandlers();
            Images = new ObservableCollection<DragableImageItem>();
            Thumbs = new ObservableCollection<PhoneThumbVisualization>();

            Images.CollectionChanged += Images_Changed;
            Thumbs.CollectionChanged += Thumbs_Changed;
            ImgScatterView.ItemsSource = Images;
            PinnedItems.ItemsSource = Thumbs;
            scw = ImgScatterView;
        }

        protected void Images_Changed(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ImgScatterView.Dispatcher.Invoke(new Action(() => ImgScatterView.UpdateLayout()));
        }

        protected void Thumbs_Changed(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PinnedItems.Dispatcher.Invoke(new Action(() => PinnedItems.UpdateLayout()));
        }

        protected void ScatterViewItem_TouchDown(object sender, TouchEventArgs e)
        {
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
            Task.Factory.StartNew(() => FlickrHandler.GetImages(query));
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

        public static void AddImage(System.Drawing.Image img, Color tagColor)
        {
            var oldBitmap = img as System.Drawing.Bitmap ?? new System.Drawing.Bitmap(img);
            var bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
            oldBitmap.GetHbitmap(System.Drawing.Color.Transparent),
             IntPtr.Zero,
             new Int32Rect(0, 0, oldBitmap.Width, oldBitmap.Height),
             null);
            bitmapSource.Freeze();
            scw.Dispatcher.Invoke(new Action(() => Images.Add(new DragableImageItem("", true, bitmapSource, tagColor))));
        }

        private void Visualizer_VisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            pinned[e.TagVisualization.VisualizedTag.Value] = false;
            PhoneThumbVisualization ptv = null;
            foreach (PhoneThumbVisualization i in Thumbs)
            {
                if (i.TagValue == e.TagVisualization.VisualizedTag.Value)
                {
                    ptv = i;
                }
            }
            if (ptv == null)
            {
                PhoneThumbVisualization newPtv = new PhoneThumbVisualization(e.TagVisualization.VisualizedTag.Value);
                scw.Dispatcher.Invoke(new Action(() => Thumbs.Add(newPtv)));
            }
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
                    foreach (PhoneThumbVisualization i in Thumbs)
                    {
                        if (i.TagValue == e.TagVisualization.VisualizedTag.Value)
                        {
                            ptv = i;
                        }
                    }
                    if (ptv != null)
                    {
                        Thumbs.Remove(ptv);
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
                item.Visibility = Visibility.Visible;
                item.Orientation = e.Cursor.GetOrientation(this);
                item.Center = e.Cursor.GetPosition(this);
            }
        }

        private void ImgScatterView_DragCompleted(object sender, SurfaceDragCompletedEventArgs e)
        {
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