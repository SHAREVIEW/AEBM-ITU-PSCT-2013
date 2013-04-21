using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Exercise7._2
{
    public class DragableImageItem
    {
        private string name;
        private bool canDrag;
        private BitmapSource image;
        private SolidColorBrush tagColor;

        public SolidColorBrush TagColor
        {
            get { return tagColor; }
            set { tagColor = value; }
        }

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

        public DragableImageItem(string name, bool canDrag, BitmapSource image, Color tagColor)
        {
            this.name = name;
            this.canDrag = canDrag;
            this.image = image;
            this.tagColor = new SolidColorBrush(tagColor);
        }
        public DragableImageItem()
        {
        }
    }
}
