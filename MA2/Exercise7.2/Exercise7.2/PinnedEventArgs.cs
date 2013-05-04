using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exercise7._2
{
    public class PinnedEventArgs : EventArgs
    {
        private long tagValue;
        private bool pinned;

        public long TagValue
        {
            get { return tagValue; }
            set { tagValue = value; }
        }

        public bool Pinned
        {
            get { return pinned; }
            set { pinned = value; }
        }

        public PinnedEventArgs(long tagValue, bool pinned)
        {
            this.tagValue = tagValue;
            this.pinned = pinned;
        }
    }
}
