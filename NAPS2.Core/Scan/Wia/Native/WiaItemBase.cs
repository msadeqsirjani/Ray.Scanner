using System;
using System.Collections.Generic;
using System.Linq;

namespace NAPS2.Scan.Wia.Native
{
    public class WiaItemBase : NativeWiaObject
    {
        private WiaPropertyCollection properties;

        protected internal WiaItemBase(WiaVersion version, IntPtr handle) : base(version, handle)
        {
        }

        public WiaPropertyCollection Properties
        {
            get
            {
                if (properties == null)
                {
                    WiaException.Check(NativeWiaMethods.GetItemPropertyStorage(Handle, out var propStorage));
                    properties = new WiaPropertyCollection(Version, propStorage);
                }
                return properties;
            }
        }

        public List<WiaItem> GetSubItems()
        {
            var items = new List<WiaItem>();
            WiaException.Check(Version == WiaVersion.Wia10
                ? NativeWiaMethods.EnumerateItems1(Handle, itemHandle => items.Add(new WiaItem(Version, itemHandle)))
                : NativeWiaMethods.EnumerateItems2(Handle, itemHandle => items.Add(new WiaItem(Version, itemHandle))));
            return items;
        }

        public WiaItem FindSubItem(string name)
        {
            return GetSubItems().FirstOrDefault(x => x.Name() == name);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                properties?.Dispose();
            }
        }
    }
}