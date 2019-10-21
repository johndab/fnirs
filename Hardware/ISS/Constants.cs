using System;
using System.Collections.Generic;
using System.Text;
using fNIRS.Hardware.Models;

namespace fNIRS.Hardware.ISS
{
    public static class Constants
    {
        public static string DMC_QUIT = "QUIT";
        public static string DMC_START = "START"; //Start acq. and streaming
        public static string DMC_STOP = "STOP";   //Stop acq. and streaming
        public static string DMC_PAUSE = "PAUSE"; //Pause Streaming
        public static string DMC_RESUME = "RESUME"; //unpause.
        public static string DMC_SET_LEGACY_1_DATA_TRANSFER_METHOD = "SET_LEGACY_1_DATA_TRANSFER";
        public static string DMC_SET_SWITCH_MODE_8 = "SET_SWITCH_MODE_8";
        public static string DMC_SET_SWITCH_MODE_16 = "SET_SWITCH_MODE_16";
        public static string DMC_SET_SWITCH_MODE_32_LEGACY = "SET_SWITCH_MODE_32_LEGACY";
        public static string DMC_SET_SWITCH_MODE_16_LEGACY = "SET_SWITCH_MODE_16_LEGACY";
        public static string DMC_SET_SWITCH_MODE_8_LEGACY = "SET_SWITCH_MODE_8_LEGACY";
        public static string DMC_SET_SWITCH_MODE_32 = "SET_SWITCH_MODE_32";
        public static string DMC_EXTERNAL_TRIGGER = "EXTERNAL_TRIGGER";
        public static string DMC_NO_EXTERNAL_TRIGGER = "NO_EXTERNAL_TRIGGER";
        public static string DMC_RESTART_DATABLOCK_NUM = "RESTART_DATABLOCK#";
        public static string DMC_NUM_CYCLES_PER_DATA_BLOCK = "NUM_CYCLES_PER_DATA_BLOCK";
        public static string DMC_MAX_BYTES_PER_DATA_BLOCK = "MAX_BYTES_PER_DATA_BLOCK";
        public static string DMC_BYTES_PER_DATA_BLOCK = "BYTES_PER_DATA_BLOCK";
        public static string DMC_GET_INSTR_STATUS_PERIODIC = "GET_INSTR_STATUS_PERIODIC";
        public static string DMC_STOP_INSTR_STATUS_PERIODIC = "STOP_INSTR_STATUS_PERIODIC";
        public static string DMC_GET_INSTR_STATUS_ONCE = "GET_INSTR_STATUS_ONCE";
        public static string DMC_SET_DET_FOR_NEXT_BIAS_SET = "SET_DET_FOR_NEXT_BIAS_SET";
        public static string DMC_SET_BIAS = "SET_BIAS";
        public static string DMC_SET_AUTO_BIAS_TARGET = "SET_AUTO_BIAS_TARGET";
        public static string DMC_AUTO_BIAS_ONE_DET = "AUTO_BIAS_ONE";
        public static string DMC_AUTO_BIAS_ALL = "AUTO_BIAS_ALL";
        public static string DMC_AUTO_BIAS_ONE_DET_HIGHER = "AUTO_BIAS_ONE_HIGHER";
        public static string DMC_AUTO_BIAS_ONE_DET_LOWER = "AUTO_BIAS_ONE_LOWER";
        public static string DMC_CANCEL_AUTO_BIAS = "CANCEL_AUTO_BIAS";
        public static string DMC_GET_BIAS_ONE_DET = "GET_BIAS_ONE";
        public static string DMC_GET_BIAS_ALL = "GET_BIAS_ALL";
        public static string DMC_DEBUG_MODE_SEND_TEXT_DATA = "DEBUG_MODE";
        public static string DMC_END_DEBUG_MODE = "END_DEBUG_MODE";
        public static string DMC_NUM_MAIN_ADC_CHNS_SCANNED = "NUM_MAIN_ADC_CHNS_SCANNED";
        public static string DMC_NUM_AUX_ADC_CHNS_SCANNED = "NUM_AUX_ADC_CHNS_SCANNED";
        public static string DMC_GET_HARDWARE_STATUS = "GET_HARDWARE_STATUS";
        public static string DMC_RECONNECT_HARDWARE = "RECONNECT_HARDWARE";
        public static string DMC_SET_BASE_RF_FREQ_BY_INDEX = "SET_BASE_RF_FREQ_BY_INDEX";
        public static string DMC_GET_BASE_RF_FREQ_BY_INDEX = "GET_BASE_RF_FREQ_BY_INDEX";
                /*  Index 1=1.953125MHz, 2=3.90625MHz, 3=7.8125MHz, 4=11.71875MHz, 5=15.625MHz,
                    6=19.53125MHz, 7=31.25MHz, 8=46.875MHz, 9=62.5MHz, 10=78.125MHz, 11=93.75MHz,
                    12=109.375MHz, 13=125MHz, 14=140.625MHz, 15=156.25MHz, 16=171.875MHz,
                    17=187.5MHz, 18=203.125MHz, 19=218.75MHz, 20=234.375MHz, 21=250MHz,
                    22=265.625MHz 23=281.25MHz, 24=296.875MHz, 25=312.5MHz */
                //the defualt startup value is 14 or 140MHz, but this may be changed by command
                //line option on startup.

        public static List<Frequency> DMC_FREQUENCY_LIST = new List<Frequency>() {
                new Frequency(1, "1.953125MHz"),
                new Frequency(2, "3.90625MHz"),
                new Frequency(3, "7.8125MHz"),
                new Frequency(4, "11.71875MHz"),
                new Frequency(5, "15.625MHz"),
                new Frequency(6, "19.53125MHz"),
                new Frequency(7, "31.25MHz"),
                new Frequency(8, "46.875MHz"),
                new Frequency(9, "62.5MHz"),
                new Frequency(10, "78.125MHz"),
                new Frequency(11, "93.75MHz"),
                new Frequency(12, "109.375MHz"),
                new Frequency(13, "125MHz"),
                new Frequency(14, "140.625MHz"),
                new Frequency(15, "156.25MHz"),
                new Frequency(16, "171.875MHz"),
                new Frequency(17, "187.5MHz"),
                new Frequency(18, "203.125MHz"),
                new Frequency(19, "218.75MHz"),
                new Frequency(20, "234.375MHz"),
                new Frequency(21, "250MHz"),
                new Frequency(22, "265.625MHz"),
                new Frequency(23, "281.25MHz"),
                new Frequency(24, "296.875MHz"),
                new Frequency(25, "312.5MHz"),
        };

        public static string DMC_GET_BASE_RF_FREQ = "GET_BASE_RF_FREQ";// not yet implimented
        public static string DMC_SET_BASE_RF_FREQ = "SET_BASE_RF_FREQ";// not yet implimented
        public static string DMC_HELP = "HELP";

        //Define some outbound message tags which might be acted on by client
        //Note: also there are a number of 000 message types that are not intended as
        //machine information, such as the help text.
        public static string DMC_BINARY_DATA_STARTS = "DataPacket Starts\r\n";
        public static string DMC_BINARY_DATA_ENDS = "DataPacket Ends";


        public static string DMC_DATA_STARTS_TAG = "100 Data Starts:";
        public static string DMC_DATA_ENDS_TAG = "100 Data Ends:";
        public static string DMC_STATUS_STARTS_TAG = "100 Status Starts:";
        public static string DMC_STATUS_ENDS_TAG = "100 Status Ends:";
        public static string DMC_ALL_BIAS_VALUES_TAG = "100 ALL BIAS Vals:";
        public static string DMC_SINGLE_BIAS_VALUE_TAG = "100 BIAS Val:";
        public static string DMC_AUTOBIAS_COMPLETE = "100 AUTO_BIAS_COMPLETE:";
        public static string DMC_BASE_RF_FREQ_INDEX_TAG = "100 RF Index:";

        public static string DMC_STOP_DATA_STREAM_ACK = "200 STOP_DATA STREAM RECIEVED BY SERVER";
        public static string DMC_PAUSE_DATA_STREAM_ACK = "200 PAUSE_DATA STREAM RECIEVED BY SERVER";
        public static string DMC_START_DATA_ACK = "200 START_DATA STREAM RECIEVED BY SERVER";
        public static string DMC_RSSUME_DATA_ACK = "200 RESUME_DATA STREAM RECIEVED BY SERVER";
        public static string DMC_SET_LEGACY_1_DATA_TRANSFER_ACK = "200 SET_LEGACY_1_DATA_TRANSFER RECIEVED BY SERVER";

        public static string DMC_STOP_INSTR_STATUS_ACK = "200 STOP_INSTR_STATUS_PERIODIC RECIEVED BY SERVER";
        public static string DMC_START_INSTR_STATUS_ACK = "200 START_DATA STREAM RECIEVED BY SERVER";
        public static string DMC_GET_BIAS_ONE_ACK = "200 GET_BIAS_ONE_DET RECIEVED BY SERVER PMT#:";
        public static string DMC_GET_BIAS_ALL_ACK = "200 GET_BIAS_ALL RECIEVED BY SERVER";
        public static string DMC_SET_AUTO_BIAS_TARGET_ACK = "200 SET_AUTO_BIAS_TARGET RECIEVED BY SERVER, Target:";
        public static string DMC_AUTO_BIAS_ONE_ACK = "200 AUTO_BIAS_ONE_DET RECIEVED BY SERVER PMT#:";
        public static string DMC_AUTO_BIAS_ALL_ACK = "200 AUTO_BIAS_ALL RECIEVED BY SERVER";
        public static string DMC_SET_BIAS_ACK = "200 SET_BIAS RECIEVED BY SERVER BIAS:"; //followed by tab and by bias
        public static string DMC_SET_DETECTOR_FOR_NEXT_BIAS_SET_ACK = "200 SET_DETECTOR_FOR_NEXT_BIAS_SET RECIEVED BY SERVER PMT#:";
                //followed by tab and PMT number
        public static string DMC_SET_BASE_RF_FREQ_BY_INDEX_ACK = "200 DMC_SET_BASE_RF_FREQ_BY_INDEX RECIEVED BY SERVER, Index:";
        public static string DMC_GET_BASE_RF_FREQ_BY_INDEX_ACK = "200 DMC_GET_BASE_RF_FREQ_BY_INDEX RECIEVED BY SERVER, Index:";

        public static string DMC_SET_SWTICH_MODE_8_ACK = "200 SET_SWITCH_MODE_8 RECIEVED BY SERVER";
        public static string DMC_SET_SWTICH_MODE_16_ACK = "200 SET_SWITCH_MODE_16 RECIEVED BY SERVER";
        public static string DMC_SET_SWTICH_MODE_32_ACK = "200 SET_SWITCH_MODE_32 RECIEVED BY SERVER";
        public static string DMC_SET_SWTICH_MODE_8_LEGACY_ACK = "200 SET_SWITCH_MODE_8_LEGACY RECIEVED BY SERVER";
        public static string DMC_SET_SWTICH_MODE_16_LEGACY_ACK = "200 SET_SWITCH_MODE_16_LEGACY RECIEVED BY SERVER";
        public static string DMC_SET_SWTICH_MODE_32_LEGACY_ACK = "200 SET_SWITCH_MODE_32_LEGACY RECIEVED BY SERVER";
        public static string DMC_EXTERNAL_TRIGGER_ACK = "200 EXTERNAL_TRIGGER RECIEVED BY SERVER";
        public static string DMC_NO_EXTERNAL_TRIGGER_ACK = "200 NO_EXTERNAL_TRIGGER RECIEVED BY SERVER";
        public static string DMC_GET_HARDWARE_STATUS_ACK = "200 GET_HARDWARE_STATUS RECIEVED BY SERVER";
        public static string DMC_RECONNECT_HARDWARE_ACK = "200 RECONNECT_HARDWARE RECIEVED BY SERVER";
        public static string DMC_RESTART_DATABLOCK_ACK = "200 RESTART_DATABLOCK # RECIEVED BY SERVER";
        public static string DMC_NUM_MAIN_ADC_CHNS_SCANNED_ACK = "200 NUM_MAIN_ADC_CHNS_SCANNED RECIEVED BY SERVER";
        public static string DMC_NUM_AUX_ADC_CHNS_SCANNED_ACK = "200 NUM_AUX_ADC_CHNS_SCANNED RECIEVED BY SERVER";
        public static string DMC_NUM_CYCLES_PER_DATA_BLOCK_ACK = "200 NUM_CYCLES_PER_DATA_BLOCK RECIEVED BY SERVER:"; //followed by tab and number of data blocks per cycle
        public static string DMC_MAX_BYTES_PER_DATA_BLOCK_ACK = "200 MAX_BYTES_PER_DATA_BLOCK RECIEVED BY SERVER:";  //followed by tab and max number of bytes
        public static string DMC_BYTES_PER_DATA_BLOCK_ACK = "200 BYTES_PER_DATA_BLOCK RECIEVED BY SERVER"; //followed by tab and number of bytes
        public static string DMC_CANCEL_AUTO_BIAS_ACK = "200 CANCEL_AUTO_BIAS RECIEVED BY SERVER";
        public static string DMC_GET_INSTR_STATUS_PERIODIC_ACK = "200 DMC_GET_INSTR_STATUS_PERIODIC RECIEVED BY SERVER";
        public static string DMC_GET_INSTR_STATUS_ONCE_ACK_ALREADY_STREAMING = "200 GET_INSTR_STATUS_ONCE RECIEVED BY SERVER, BUT ALREADY STREAMING STATUS";
        public static string DMC_GET_INSTR_STATUS_ONCE_ACK = "200 GET_INSTR_STATUS_ONCE RECIEVED BY SERVER";
        public static string DMC_DEBUG_MODE_ACK = "200 DEBUG_MODE RECIEVED BY SERVER";
        public static string END_DEBUG_MODE_ACK = "200 END_DEBUG_MODE RECIEVED BY SERVER";

        public static string DMC_HELP_ACK = "200 HELP";

        public static string DMC_GOOD_BYE = "200 Good bye... :"; //after quit command
        public static string DMC_UNSUPPORTED_COMMAND_RECIEVED = "400 UNSUPPORTED COMMAND RECIEVED BY SERVER";

                //Define some more outbound message tags which might be acted on by client
                //These are messages that are send to the client when a connection is established
                //0 will indicate false, 1 will indicate true;
                //These messages will also be sent when the client sends the DMC_GET_HARDWARE_STATUS command
        public static string DMC_Primary_ADC_Connected = "100 Primary ADC connected:";
        public static string DMC_Virtual_Primary_ADC = "100 Virtual primary ADC:";
        public static string DMC_Secondary_ADC_Connected = "100 Secondary ADC connected:";
        public static string DMC_Virtual_Secondary_ADC = "100 Virtual secondary ADC:";
        public static string DMC_Primary_PMT_Bias_Control_Connected = "100 Primary PMT bias control connected:";
        public static string DMC_Virtual_Primary_PMT_Bias_Control = "100 Virtual primary PMT bias control:";
        public static string DMC_Secondary_PMT_Bias_Control_Connected = "100 Secondary PMT bias control connected:";
        public static string DMC_Virtual_Secondary_PMT_Bias_Control = "100 Virtual secondary PMT bias control:";
        public static string DMC_Primary_DDS_System_Connected = "100 Primary DDS system connected:";
        public static string DMC_Virtual_Primary_DDS_System = "100 Virtual primary DDS system:";
        public static string DMC_Secondary_DDS_System_Connected = "100 Secondary DDS system connected:";
        public static string DMC_Virtual_Secondary_DDS_System = "100 Virtual secondary DDS system:";

                //Define other messages sent by server when a client connects
        public static string DMC_SERVER_HELLO = "100 Hello - connected to Imagent Duae server:";//followed
                //date time and client id number
        public static string DMC_MAX_BYTES_PER_BLOCK = "100 Max. bytes per data blook:";
        public static string DMC_VERSION = "100 Version:";

        public static string[] HardwareStatusPrexies = new string[]
        {
            DMC_SERVER_HELLO,
            DMC_MAX_BYTES_PER_BLOCK,
            DMC_Primary_ADC_Connected,
            DMC_Virtual_Primary_ADC,
            DMC_Secondary_ADC_Connected,
            DMC_Virtual_Secondary_ADC,
            DMC_Primary_PMT_Bias_Control_Connected,
            DMC_Virtual_Primary_PMT_Bias_Control,
            DMC_Secondary_PMT_Bias_Control_Connected,
            DMC_Virtual_Secondary_PMT_Bias_Control,
            DMC_Primary_DDS_System_Connected,
            DMC_Virtual_Primary_DDS_System,
            DMC_Secondary_DDS_System_Connected,
            DMC_Virtual_Secondary_DDS_System
        };
    }
}
