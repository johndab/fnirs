using System;
using System.Collections.Generic;
using fNIRS.Hardware.Models;
using fNIRS.Hardware.ISS;

namespace fNIRS.Hardware.ISS.Converters
{

    public static class StatusConverter
    {

        public static HardwareStatus FromDictionary(this HardwareStatus status, IDictionary<string, string> dict)
        {
            if(dict.TryGetValue(Constants.DMC_SERVER_HELLO, out var timeStr))
            {
                try
                {
                    status.Time = DateTime.Parse(timeStr);
                } 
                catch (FormatException)
                { }
            }

            if (dict.TryGetValue(Constants.DMC_MAX_BYTES_PER_BLOCK, out var bytes))
            {
                status.MaxBytes = int.Parse(bytes);
            }

            // ADC
            if (dict.TryGetValue(Constants.DMC_Primary_ADC_Connected, out var primaryAdc))
            {
                status.ADC.PrimaryConnected = primaryAdc.Trim().Equals("1");
            }
            if (dict.TryGetValue(Constants.DMC_Virtual_Primary_ADC, out var vPrimaryAdc))
            {
                status.ADC.PrimaryVirtualConnected = vPrimaryAdc.Trim().Equals("1");
            }
            if (dict.TryGetValue(Constants.DMC_Secondary_ADC_Connected, out var secondaryADC))
            {
                status.ADC.SecondaryConnected = secondaryADC.Trim().Equals("1");
            }
            if (dict.TryGetValue(Constants.DMC_Virtual_Secondary_ADC, out var vSecondaryAdc))
            {
                status.ADC.SecondaryVirtualConnected = vSecondaryAdc.Trim().Equals("1");
            }

            // PMT
            if (dict.TryGetValue(Constants.DMC_Primary_PMT_Bias_Control_Connected, out var primaryPMT))
            {
                status.PMT.PrimaryConnected = primaryPMT.Trim().Equals("1");
            }
            if (dict.TryGetValue(Constants.DMC_Virtual_Primary_PMT_Bias_Control, out var vPrimaryPMT))
            {
                status.PMT.PrimaryVirtualConnected = vPrimaryPMT.Trim().Equals("1");
            }
            if (dict.TryGetValue(Constants.DMC_Secondary_PMT_Bias_Control_Connected, out var secondaryPMT))
            {
                status.PMT.SecondaryConnected = secondaryPMT.Trim().Equals("1");
            }
            if (dict.TryGetValue(Constants.DMC_Virtual_Secondary_PMT_Bias_Control, out var vSecondaryPMT))
            {
                status.PMT.SecondaryVirtualConnected = vSecondaryPMT.Trim().Equals("1");
            }


            // DDS
            if (dict.TryGetValue(Constants.DMC_Virtual_Primary_DDS_System, out var primaryDDS))
            {
                status.DDS.PrimaryConnected = primaryDDS.Trim().Equals("1");
            }
            if (dict.TryGetValue(Constants.DMC_Virtual_Primary_DDS_System, out var vPrimaryDDS))
            {
                status.DDS.PrimaryVirtualConnected = vPrimaryDDS.Trim().Equals("1");
            }
            if (dict.TryGetValue(Constants.DMC_Secondary_DDS_System_Connected, out var secondaryDDS))
            {
                status.DDS.SecondaryConnected = secondaryDDS.Trim().Equals("1");
            }
            if (dict.TryGetValue(Constants.DMC_Virtual_Secondary_DDS_System, out var vSecondaryDDS))
            {
                status.DDS.SecondaryVirtualConnected = vSecondaryDDS.Trim().Equals("1");
            }

            return status;
        }
    }
}