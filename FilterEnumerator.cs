using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using DirectShowLib;

namespace DualScreenDemo
{
    public class FilterEnumerator
    {
        private static readonly Guid IID_IPropertyBag = new Guid("55272A00-42CB-11CE-8135-00AA004BB851");

        public void EnumerateFilters()
        {
            ICreateDevEnum createDevEnum = (ICreateDevEnum)new CreateDevEnum();
            IEnumMoniker enumMoniker;
            int hr = createDevEnum.CreateClassEnumerator(FilterCategory.LegacyAmFilterCategory, out enumMoniker, 0);
            if (hr != 0 || enumMoniker == null)
            {
                Console.WriteLine("No filters found.");
                return;
            }

            IMoniker[] monikers = new IMoniker[1];
            IntPtr fetched = Marshal.AllocHGlobal(sizeof(int));
            while (enumMoniker.Next(1, monikers, fetched) == 0)
            {
                int fetchedCount = Marshal.ReadInt32(fetched);
                if (fetchedCount > 0)
                {
                    object objPropBag;
                    Guid tempGuid = IID_IPropertyBag; // Use a local variable
                    monikers[0].BindToStorage(null, null, ref tempGuid, out objPropBag);
                    IPropertyBag propBag = objPropBag as IPropertyBag;

                    object filterName = null;
                    if (propBag != null)
                    {
                        propBag.Read("FriendlyName", out filterName, null);
                    }

                    if (filterName != null)
                    {
                        Console.WriteLine("Filter: " + filterName.ToString());
                    }

                    Marshal.ReleaseComObject(monikers[0]);
                }
            }
            Marshal.ReleaseComObject(enumMoniker);
            Marshal.FreeHGlobal(fetched);
        }
    }
}