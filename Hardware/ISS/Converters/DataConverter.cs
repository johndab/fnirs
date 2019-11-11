using System;
using System.Collections.Generic;
using fNIRS.Hardware.Models;
using fNIRS.Hardware.ISS;
using System.Runtime.InteropServices;

namespace fNIRS.Hardware.ISS.Converters
{
    public static class DataConverter
    {
        public unsafe static CycleData ToModel(this CYCLEDATA6 cycle)
        {
            return new CycleData()
            {
                Number = cycle.CycleNumber1,
                Mode = cycle.sw_mode,
                DCData = GetDCData(cycle.cc_wfdata, cycle.sw_mode),
            };
        }

        public unsafe static DC[] GetDCData(byte* bytes, short mode)
        {
            var result = new List<DC>();

            var ptr = (IntPtr)bytes;
            var n = Const.max_DATAPACKET6_a2d_chns;
            var m = mode;

            Type type = null;
            if(mode == 8)
            {
                type = typeof(DCrealimage_8D);
            }
            else if(mode == 16)
            {
                type = typeof(DCrealimage_4D);
            }
            else if(mode == 32)
            {
                type = typeof(DCrealimage_2D);
            }

            for(int i=0; i<n; i++)
            {
                for(int j=0; j<m; j++)
                {
                    var image = Marshal.PtrToStructure(ptr, type);
                    var real = new List<double>();
                    var imag = new List<double>();
                    double dc = -1;
                    
                    if(mode == 8)
                    {
                        var img = (DCrealimage_8D)image;
                        dc = img.DC;
                        for(int q=0; q<64/mode; q++)
                        {
                            real.Add(img.real[q]);
                            imag.Add(img.imag[q]);
                        }

                        ptr = IntPtr.Add(ptr, sizeof(DCrealimage_8D));
                    }
                    else if(mode == 16)
                    {
                        var img = (DCrealimage_4D)image;
                        dc = img.DC;
                        for(int q=0; q<64/mode; q++)
                        {
                            real.Add(img.real[q]);
                            imag.Add(img.imag[q]);
                        }
                        ptr = IntPtr.Add(ptr, sizeof(DCrealimage_4D));
                    }
                    else if(mode == 32)
                    {
                        var img = (DCrealimage_2D)image;
                        dc = img.DC;
                        for(int q=0; q<64/mode; q++)
                        {
                            real.Add(img.real[q]);
                            imag.Add(img.imag[q]);
                        }
                        ptr = IntPtr.Add(ptr, sizeof(DCrealimage_2D));  
                    }
                    
                    result.Add(new DC()
                    {
                        Row = i,
                        Col = j,
                        Value = dc,
                        Real = real.ToArray(),
                        Imag = real.ToArray(),
                    });
                }
            }    

            // Console.WriteLine(result.Count);

            return result.ToArray();
        }

        public static unsafe T ToStructure<T>(this byte[] bytes, int start) where T : struct
        {
            fixed (byte* ptr = &bytes[start])
            {
                return (T)Marshal.PtrToStructure((IntPtr)ptr, typeof(T));
            }
        }

    }

}