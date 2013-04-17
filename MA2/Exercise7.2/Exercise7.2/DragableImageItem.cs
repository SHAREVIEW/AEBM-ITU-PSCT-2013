using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Exercise7._2
{
    public class DragableImageItem
    {
        private string name;
        private bool canDrag;
        private BitmapSource image;

        public BitmapSource Image
        {
            get { return image; }
            set { image = value; }
        }

        public string Name
        {
            get { return name; }
        }

        public bool CanDrag
        {
            get { return canDrag; }
        }

        public object DraggedElement
        {
            get;
            set;
        }

        public DragableImageItem(string name, bool canDrag, BitmapSource image)
        {
            this.name = name;
            this.canDrag = canDrag;
            this.image = image;
        }
        public DragableImageItem()
        {
        }
    }
}
