
using System.Runtime.InteropServices;

namespace fNIRS.Hardware.ISS
{
    public class Const {
        public const int max_DATAPACKET6_a2d_chns = 32;
        public const int max_DATAPACKET6_a2d_counters = 8;
        public const int max_DATAPACKET6_ext_mux_chns = 32;
        public const int max_DATAPACKET6_aqs_per_wf = 128;
        public const int max_DATAPACKET6_auxadc_chns = 8;
        public const int max_DATAPACKET6_8D_SW8_ext_mux_chns = 8;
        public const int max_DATAPACKET6_4D_SW16_ext_mux_chns = 16;
        public const int max_DATAPACKET6_2D_SW32_ext_mux_chns = 32;
    }

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public unsafe struct HEADERDATA6 
    {
    //
    //Version and packet_type information section.
    //
        public fixed byte start_msg[64];
        public uint packetsize;
        public uint sizeof_header_data;
        uint sizoef_one_cycle_of_data;
        uint pad1;
        public ushort NumCyclesPerDataPacket;//number of data acquisition (or
                    //source external mux cycles in each packet.
        public ushort packet_type;//always = 1 for this packet_type,
                                //also passed in wparam, used by 16-bit driver.
        public ushort version;  //driver version currently always = 1.
        ushort msg_buffer_id_number; //a number associated with this message
                        //used internally to indicate the buffer region used to store
                        //the data before transmission via TCPIP.

        uint pad2;
        uint pad3;

        byte data_valid;

        byte pad4;
        byte pad5;
        byte pad6;

        //
        //Debug information section. (Some of this information not sent if debug
        // information is turned off.)
        //
        fixed uint Counter[Const.max_DATAPACKET6_a2d_counters];//Value of A2D's counters when data transfer
                            //started. [counter number]
        uint timeproc;//This is the time in microseconds between detecting
                        //when the buffer was full and posting the DataReady
                        //message (buffer processing time).

        ulong data_block_num; //number that increases with each data blcok transferrred from the data acquistion system.

        //Information useful for testing and debugging.
        uint MsgToWin;//32 bit integer equal to the window to which this
                        //message was sent, or would have been sent if a
                        //"monitor" program had not Overriden it. This parameter
                        //can be ignored in must cases, this is a 16 bit HWND
                        //if it this packet is sent from a 16-bit driver and
                        //a 32-bit HWND if sent from a 32-bit driver.
        uint passCnt; //Number of times the timer callback function has
                        //been called

        uint time;//Time tag, time since instance of processor was started.
                        //in us. This time is recorded just before posting the
                        //the message.
        uint pad7;

        //Information about the state of the machine.
        ushort missed_cycles; //Number of these messages missed sinde the
                        //current instances created, of ResetMissed called.
        ushort unproc_msg;//Number of "_DataReady" messages posted but not
                            //responded to, when this message was posted.
                        //(Should be 0-1 unless program is too slow.)
        ushort pad8;
        ushort pad9;

        fixed byte volt_overflow_emc[Const.max_DATAPACKET6_a2d_chns * Const.max_DATAPACKET6_ext_mux_chns];
    //   unsigned __int8 volt_overflow_emc[max_DATAPACKET6_a2d_chns][max_DATAPACKET6_ext_mux_chns];
    // 						//Individual emc boltage overflow flags, true (1) if any A2D values
    // 						//were equal to the limits of the A2D range.
    // 				   //3 if 95% of values within max limits.

    //   unsigned __int16 pad10[max_DATAPACKET6_a2d_chns];
        fixed ushort pad10[Const.max_DATAPACKET6_a2d_chns];
    };

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public unsafe struct DCrealimage_2D
    {
        public double DC; // 8 bytes
        public fixed double real[2]; // 8 * 2 bytes
        public fixed double imag[2]; // 8 * 2 bytes
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct DCrealimage_4D
    {
        public double DC; 
        public fixed double real[4];
        public fixed double imag[4];
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct DCrealimage_8D
    {
        public double DC;
        public fixed double real[8];
        public fixed double imag[8];
    };


    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public unsafe struct CYCLEDATA6 
    {
	  public uint CycleNumber1;
	  public uint MagicNumber1;  //7 lsb should be 1234567890, high byte is copy of cycle number
	  public short sw_mode; // used to interpret data union below
	  short pad1;
	  short pad2;
	  //Raw waveform data the ADC and Mux channels for the raw data are determined in USER_SET6
	  fixed short rawWF[Const.max_DATAPACKET6_aqs_per_wf]; //completely raw data
	  fixed double sum_folded_rawWF[Const.max_DATAPACKET6_aqs_per_wf]; //folded raw data from

	  //Aux data
	  fixed double AnalAuxADCdata[Const.max_DATAPACKET6_auxadc_chns];
	  long DigAuxADCdata;

	  //Main ADC data
	//   CC_WFDATA_UNION cc_wfdata;
	//   CC_WFDATA_UNION cc_wfdata;

// DCrealimage_8D view_8D_sw8[max_DATAPACKET6_a2d_chns][max_DATAPACKET6_8D_SW8_ext_mux_chns];
// 	DCrealimage_4D view_4D_sw16[max_DATAPACKET6_a2d_chns][max_DATAPACKET6_4D_SW16_ext_mux_chns];
      public fixed byte cc_wfdata[40 * Const.max_DATAPACKET6_a2d_chns * Const.max_DATAPACKET6_2D_SW32_ext_mux_chns];
	  public uint CycleNumber2;
	  public uint MagicNumber2;  //7 lsb should be 1234567890, high byte is copy of cycle number
    }
}


