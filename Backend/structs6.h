//----------------------------------------------------------------------------
//  Part of prohject DMC_ImagentDuae.exe
//
//
//
//  FILE:       structs6.h
//  AUTHOR:       Dennis Hueber, ISS inc.
//
//
// ** OVERVIEW **
//
//This is a collection of types (structures and enums) used by DMC_ImagentDuea
// and its bufproc buffer to send data packets.
//
//----------------------------------------------------------------------------
#ifndef __STRUCTS6_H
#define __STRUCTS6_H



#ifdef __BORLANDC__
#pragma option -a1
#else  //assume microsoft
#pragma pack(push,1)
#endif

#include "new_fftmath.h"

#define max_DATAPACKET6_8D_freqs 8
#define max_DATAPACKET6_4D_freqs 4
#define max_DATAPACKET6_2D_freqs 2



#define max_DATAPACKET6_8D_SW8_ext_mux_chns 8
#define max_DATAPACKET6_4D_SW16_ext_mux_chns 16
#define max_DATAPACKET6_2D_SW32_ext_mux_chns 32

#define max_DATAPACKET6_ext_mux_chns 32
#define max_DATAPACKET6_a2d_counters 8
#define max_DATAPACKET6_a2d_chns 32 //do not decrease because 2xthis number should be
									//equal to (or greater than 64);
#define max_DATAPACKET6_aqs_per_wf 128
#define max_DATAPACKET6_auxadc_chns 8

#define max_DATAPACKET6_ext_mux_cycles_per_message 15  //if this is changed one most concider the collection
		//DATAPACKET6....

typedef unsigned char byte;
typedef unsigned short word;
typedef unsigned int uint;
typedef unsigned short ushort;
typedef unsigned long ulong;
typedef unsigned long dword;


//A structure for the data taken with each cycle of the external mux (light
//source switching cycle).  Not that DCrealimage_4 is defined elsewhere
/*
typedef struct
   {
   double DC;
   double real[4];
   double imag[4];
   } DCrealimage_4D;

typedef struct
   {
   double DC;
   double real[2];
   double imag[2];
   } DCrealimage_2D;

*/





typedef union
	{
	DCrealimage_8D view_8D_sw8[max_DATAPACKET6_a2d_chns][max_DATAPACKET6_8D_SW8_ext_mux_chns];
	DCrealimage_4D view_4D_sw16[max_DATAPACKET6_a2d_chns][max_DATAPACKET6_4D_SW16_ext_mux_chns];
	DCrealimage_2D view_2D_sw32[max_DATAPACKET6_a2d_chns][max_DATAPACKET6_2D_SW32_ext_mux_chns];
	}CC_WFDATA_UNION;


typedef struct {
	  unsigned __int32 CycleNumber1;
	  unsigned __int32 MagicNumber1;  //7 lsb should be 1234567890, high byte is copy of cycle number
	  __int16 sw_mode; // used to interpret data union below
	  __int16 pad1;
	  __int16 pad2;
	  //Raw waveform data the ADC and Mux channels for the raw data are determined in USER_SET6
	  __int16 rawWF[max_DATAPACKET6_aqs_per_wf]; //completely raw data
	  double sum_folded_rawWF[max_DATAPACKET6_aqs_per_wf]; //folded raw data from

	  //Aux data
	  double AnalAuxADCdata[max_DATAPACKET6_auxadc_chns];
	  __int64 DigAuxADCdata;

	  //Main ADC data
	  CC_WFDATA_UNION cc_wfdata;

	  unsigned __int32 CycleNumber2;
	  unsigned __int32 MagicNumber2;  //7 lsb should be 1234567890, high byte is copy of cycle number
	 } CYCLEDATA6;



//A structure to store information that is passed with each block of data, but
//is not repeated with each cycle of the external mux
typedef struct {
//
//Version and packet_type information section.
//
	  char start_msg[64];
	  unsigned __int32 packetsize;
	  unsigned __int32 sizeof_header_data;
	  unsigned __int32 sizoef_one_cycle_of_data;
	  unsigned __int32 pad1;

	  unsigned __int16  NumCyclesPerDataPacket;//number of data acquisition (or
					//source external mux cycles in each packet.
	  unsigned __int16 packet_type;//always = 1 for this packet_type,
								//also passed in wparam, used by 16-bit driver.
	  unsigned __int16 version;  //driver version currently always = 1.
	  unsigned __int16 msg_buffer_id_number; //a number associated with this message
					  //used internally to indicate the buffer region used to store
					  //the data before transmission via TCPIP.

	  unsigned __int32 pad2;
	  unsigned __int32 pad3;


	  bool data_valid;//Presently always true (reserved for future use),
						 // In future false will be used for "bad" data.

	  bool pad4;
	  bool pad5;
	  bool pad6;

//
//Debug information section. (Some of this information not sent if debug
// information is turned off.)
//
	  unsigned __int32 Counter[max_DATAPACKET6_a2d_counters];//Value of A2D's counters when data transfer
							//started. [counter number]
	  unsigned __int32 timeproc;//This is the time in microseconds between detecting
					   //when the buffer was full and posting the DataReady
					  //message (buffer processing time).

	  unsigned __int64 data_block_num; //number that increases with each data blcok transferrred from the data acquistion system.

	  //Information useful for testing and debugging.
	  unsigned __int32 MsgToWin;//32 bit integer equal to the window to which this
						//message was sent, or would have been sent if a
					 //"monitor" program had not Overriden it. This parameter
					 //can be ignored in must cases, this is a 16 bit HWND
					 //if it this packet is sent from a 16-bit driver and
					 //a 32-bit HWND if sent from a 32-bit driver.
	  unsigned __int32 passCnt; //Number of times the timer callback function has
					  //been called

	  unsigned __int32 time;//Time tag, time since instance of processor was started.
						//in us. This time is recorded just before posting the
						//the message.
	  unsigned __int32 pad7;

	  //Information about the state of the machine.
	  unsigned __int16 missed_cycles; //Number of these messages missed sinde the
						//current instances created, of ResetMissed called.
	  unsigned __int16 unproc_msg;//Number of "_DataReady" messages posted but not
							//responded to, when this message was posted.
						//(Should be 0-1 unless program is too slow.)
	  unsigned __int16 pad8;
	  unsigned __int16 pad9;

	  unsigned __int8 volt_overflow_emc[max_DATAPACKET6_a2d_chns][max_DATAPACKET6_ext_mux_chns];
							//Individual emc boltage overflow flags, true (1) if any A2D values
							//were equal to the limits of the A2D range.
					   //3 if 95% of values within max limits.

	  unsigned __int16 pad10[max_DATAPACKET6_a2d_chns];

   } HEADERDATA6;
typedef HEADERDATA6 far *pHEADERDATA6;





//A strucutre defining the largest possible blcok of data, it starts with
//HEADERDATA6 data and than an array of CYCLEDATA6 data.
//Note the cycle data is at the end of the structure, so if the entire
//possible 16 array elemetent are not filled, the
//entire structure would have not need to be copied etc. One could copy
//the "header" and as many CYCLEDATA6 structures as needed. (But never
//exceeding max_DATAPACKET6_ext_mux_cycles_per_message), and never
//exceeding the actual number of data cylces requested when intiating contact
//witht the server.

//For this purpose the "NumCyclesPerDataPacket" element of the HEADERDATA6
//strucuture should always specficy how many cycles are actually used (as
//requested or by default).

//The TCIP server in DMC_ImagentDuae does NOT pass unused cycle data array
//elements to the client (waste of time).  The client
//should expect data blocks consisting of one HEADERDATA6 and as mnay
//CYCLEDATA6 as are specified... and this number is specified by HEADERDATA6.
//This number is requested by the calling program when initiating contact to
//the server (it is however alrways echoed in the header ).  Note: the number
//of cycles in one message may have a default not equal to 1.

//uncomment this structure if you want a non-template version
//typedef struct {
//   HEADERDATA6 Header;
//   CYCLEDATA6 ExtMuxCycleNum[max_DATAPACKET6_ext_mux_cycles_per_message];
//   } DATAPACKET6_MAX_CYCLES;
//typedef DATAPACKET6_MAX_CYCLES far *pDATAPACKET6_MAX_CYCLES;


//a template to make strutures definitions of any size CYCLEDATA6 array.
template< int T=max_DATAPACKET6_ext_mux_cycles_per_message>
struct DATAPACKET6_TEMPLATE {
   HEADERDATA6 Header; //header most be first, as sometimes in streams
						//the actual size of the array below.
   CYCLEDATA6 ExtMuxCycleNum[T];
};


//An example of the use of this template is to make a queue of messages using
//the STL  ie. "static queue<DATAPACKET6_TEMPLATE<X> >  MyQueue"; where x is
//the number of cycles stored in your particularly data data.  This may be
//usefull in a program that does not allow dynamic changes to
//NumCyclesPerDataPacket


//You could also predefine some structures types with the ability to store a
//"specific" number of cycles. This might be usefull if the size needs to be
//fixed at compile time or to create a STL container type. We do not want to
//have to be inefficient when a collection of these structures is needed.
//This types might be usefull for some programs. There are only 15 of them
//in this list! So do not expect more to appear if you increase
//max_DATAPACKET6_ext_mux_cycles_per_message. Uncomment what you need
//(or all of them?).

/*
typedef struct {
   HEADERDATA6 Header;
   CYCLEDATA6 ExtMuxCycleNum[1];
   } DATAPACKET6_1_CYCLE;
typedef DATAPACKET6_1_CYCLE far *pDATAPACKET6_1_CYCLE;

typedef struct {
   HEADERDATA6 Header;
   CYCLEDATA6 ExtMuxCycleNum[2];
   } DATAPACKET6_2_CYCLES;
typedef DATAPACKET6_2_CYCLES far *pDATAPACKET6_2_CYCLES;

typedef struct {
   HEADERDATA6 Header;
   CYCLEDATA6 ExtMuxCycleNum[3];
   } DATAPACKET6_3_CYCLES;
typedef DATAPACKET6_3_CYCLES far *pDATAPACKET6_3_CYCLES;

typedef struct {
   HEADERDATA6 Header;
   CYCLEDATA6 ExtMuxCycleNum[4];
   } DATAPACKET6_4_CYCLES;
typedef DATAPACKET6_4_CYCLES far *pDATAPACKET6_4_CYCLES;

typedef struct {
   HEADERDATA6 Header;
   CYCLEDATA6 ExtMuxCycleNum[5];
   } DATAPACKET6_5_CYCLES;
typedef DATAPACKET6_5_CYCLES far *pDATAPACKET6_5_CYCLES;

typedef struct {
   HEADERDATA6 Header;
   CYCLEDATA6 ExtMuxCycleNum[6];
   } DATAPACKET6_6_CYCLES;
typedef DATAPACKET6_6_CYCLES far *pDATAPACKET6_6_CYCLES;

typedef struct {
   HEADERDATA6 Header;
   CYCLEDATA6 ExtMuxCycleNum[7];
   } DATAPACKET6_7_CYCLES;
typedef DATAPACKET6_7_CYCLES far *pDATAPACKET6_7_CYCLES;

typedef struct {
   HEADERDATA6 Header;
   CYCLEDATA6 ExtMuxCycleNum[8];
   } DATAPACKET6_8_CYCLES;
typedef DATAPACKET6_8_CYCLES far *pDATAPACKET6_8_CYCLES;

typedef struct {
   HEADERDATA6 Header;
   CYCLEDATA6 ExtMuxCycleNum[9];
   } DATAPACKET6_9_CYCLES;
typedef DATAPACKET6_9_CYCLES far *pDATAPACKET6_9_CYCLES;

typedef struct {
   HEADERDATA6 Header;
   CYCLEDATA6 ExtMuxCycleNum[10];
   } DATAPACKET6_10_CYCLES;
typedef DATAPACKET6_10_CYCLES far *pDATAPACKET6_10_CYCLES;

typedef struct {
   HEADERDATA6 Header;
   CYCLEDATA6 ExtMuxCycleNum[11];
   } DATAPACKET6_11_CYCLES;
typedef DATAPACKET6_11_CYCLES far *pDATAPACKET6_11_CYCLES;

typedef struct {
   HEADERDATA6 Header;
   CYCLEDATA6 ExtMuxCycleNum[12];
   } DATAPACKET6_12_CYCLES;
typedef DATAPACKET6_12_CYCLES far *pDATAPACKET6_12_CYCLES;

typedef struct {
   HEADERDATA6 Header;
   CYCLEDATA6 ExtMuxCycleNum[13];
   } DATAPACKET6_13_CYCLES;
typedef DATAPACKET6_13_CYCLES far *pDATAPACKET6_13_CYCLES;

typedef struct {
   HEADERDATA6 Header;
   CYCLEDATA6 ExtMuxCycleNum[14];
   } DATAPACKET6_14_CYCLES;
typedef DATAPACKET6_14_CYCLES far *pDATAPACKET6_14_CYCLES;

typedef struct {
   HEADERDATA6 Header;
   CYCLEDATA6 ExtMuxCycleNum[15];
   } DATAPACKET6_15_CYCLES;
typedef DATAPACKET6_15_CYCLES far *pDATAPACKET6_15_CYCLES;

*/


typedef struct
	{
	double WaveFormFreq;//The Cross Correlation frequency in Hertz.
	ulong ClkFreq; //Frequency in Hz of external clock if used, usually 2MHz.
	ulong pad1;//The Cross Correlation frequency in Hertz.
	ushort num_A2D_chns;//Number of A2D board channels (detector channels)
						//to use (1 or 2 only for draboard.dll, 1 or 4 for
						//dabrd100.dll

	ushort num_ext_mux_chns;//Number of ext. mux channels or (sources)
										//(generally 8, must be 1-64)
	ushort num_wf_avg;//Number of waveforms averaged by external mux
									  //channel (per source) per cycle.

	ushort num_aqs_skip;//Number of waveforms (Cross Correlation
									   //periods) skipped just after switching
								 //external mux channels.

	ushort aqs_per_wf;//Number of aquisitions (points) per waveform.
									//must be 8,16,32,64 or 128.


	ushort num_aux_chns; //Number of auxillary input channels if used.
									//must be 1-8;
	ushort raw_mux_chn;//Number of the mux channel the raw data
								//should be taken from, starting at 0.
								//Must be between 0 and
								//num_wavelengths*num_sources -1; inclusive.
	ushort raw_wf_num;//Number of the waveforms, since the mux
								 //channel started, starting at 0. Must
								 //be between 0 and the
								 //num_wf_skip+num_wf_avg-1 inclusive.
	ushort raw_det_chn;//Number detector channel the raw data should  be taken...
	ushort num_freq_measured;//Number frequencies measured
	ushort ADCboardSampleRateTriggerType; //Trigger data acqustion
				//raising edige=0, falling edge=1, level high=2, level low=3.

    short max_aq_value;//Used to set over voltage level.
      									// For example 2047 is typical for
                                        //12-bit converter.
	short min_aq_value; //Used to set min voltage level. Usually
										// For example -2048 is typical for
                                        // 16-bit converter.
    bool WarmMUX; //If true external mux circuit is driven as soon as
                     //InitForWFaq(...) is called and runs even after
					 //StopDataCollection(...) is called.
    bool SendDebugInfo; //If true extra information is sent with each message.
                        //This switch was for driver debugging only.
	bool DualPulseMode; //Reserved for future use. If driver is in pulse
                        //generator mode, it is always in dual pulse mode.
    bool DemoMode;  //Flag the driver should operated in demo mode sending
                        //simulated data to the application rather than using
                        //the A2D board. This works in wave form acq. mode only.
                        //It is ignored in other modes of operation.
    bool UseExtClock;//Use external clock (normal is true), If false
      					  //an internal 2Mhz signal is used. (For the
                          //dabrd100.dll this clock is in the counter timer
                          //board not the data acquistion board.
	bool UseExtTrig;//Use external triger (normal is false). If true external
					//harware trigger is required to start acquisition, calling
					//start function only prepares board for hardware trigger.
	bool UseExtMUX; //Use external mux (normal is true), if false switch and
                    //reset signals are not generated.
    bool DifMode;  //If true system goes into differential waveform
      				//analyzer mode. Number of channels is overriden. The
                  //hardware uses two A2D channels and reports the
				  // AC and DC from each channel and the phase difference
				  // between the waveform on channel A and channel B
                  //(phase:channleA - phase:channelB). The phase difference
                  //is reported as the phase of channel A (for the various
				  //mux channels), and all the channel B phases are set to 1.0.

	bool UseAuxInput; //If true auxillary input ADC channels are used.
	bool InvertDC; //The value of the DC intensity of the waveforms will
					//be multiplied by -1 if this variable is true.
					//This setting does not effect demo mode data.
					//This setting is not supported by all drivers.
	bool RetDCrealimage; //If true the AC and phase of the average waveform
						//for each ext. MUX channel is not calculated. Instead,
						//the real and imaginary parts of the complex number
						//returned by the FFT are packed in the AC and phase
						//members of the ACDCphaseD structure in the DATAPACKET1
						//structures. Note that the value of RetDCrealimage is
						//ingnored (set to false) when DifMode is true.
						// (see DifMode above).

						//This option is not yet implimented, all current
						//applications should set RetDCrealImage to false
						//for compatability with potential future drivers.
	bool InvertPhase; //The value of the Phase of the waveforms will
					//be multiplied by -1 if this variable is true.
					//This setting does not effect demo mode data.
					//This setting is not supported by all drivers.
	float pulse_duration;//Use to set pulse duration for pulse generator
	float pulse_reprate; //Use to set pulse repitition rate for pulse
						 //generator. dabrd100.dll does not have a
						 //a pulse generator mode.

	ushort ext_mux_chn;//No current meaning, for future expansion, will
						//indicate which ext_mux_chn to use when it is possible
                        //to use only one.
    ushort A2D_chn;//No current meaning, for future expansion, will
							//indicate which a2d channel to use when if it
							//is possible to use channels any single channel.
	ushort num_resets_per_cycle;// normally 1 reset per cycle, however if this
   					//value is greater than 1, and if the num of ext. MUX channels
                  //is divisible by this value. Then more than 1 reset per cycle
				  //is possible. //not yet implimented for dra board driver
	ushort switch_mode; //switch-8=0, switch-16=1, switch-32=2,

	ulong num_cycles_per_message; // not implimented in all drivers, if driver can
                        //can not support the number requested, may it will return less.
                        // DATAPACKET1 data packets never have more then 1 cycle.
						//data packets with multicycle possibility should return
                        //the actual number of cycles packed with each message.

    bool UseExtTrigForRestarts; //ignored if UseExtTrigger is false. If both
				//UserExtTrigForRestarts and UseExtTrig are true. Then system will
				//wait for external trigger for initial data acq. start, and
				//for restarts after data loss. (fifo overflows etc.)
				//not yet implimented for dra board
	bool InvertAcqActive; //if hardware creats an acquisition active signel, it
								//will be positve (logic high) when acquisition
								//if this is false. If this is false zero indicate
								//active
	bool SyncPhaseExtTrigger; //If hardware supports it the phase will be preserved during external trigger starts.
							  //in newer hardware this simply calls for the system to wait the two trigger events
							  //intenal (generally the diode reset) and external. This is likey also phase locked.
	bool SwapFreq1_4w5_8inOutput; //this switch allows one to test and record using boxy which only understands
				//32 mux channels based on 8 diodes at 4 frequences each.  the data from CCF 5-8 will appear
				//in the array values normally used for frequency 1-4. This value is ignored if not 8 freq. total
				//(switch-8).
	bool SwapFreq1_2w3_4inOutput; //this switch allows one to test and record using boxy which only understands
				//32 mux channels based on 16 at 2 frequences each.  the data from CCF 3 and 4 will appear
				//in the array values normally used for frequency 1 and 2. this value is ignored if not
						//4 freq. total (switch-16 mode)
	bool SwapFreq1w2inOutput; //this switch allows one to test and record using boxy which only understands
				//32 mux channels based on 32 at 1 frequence.  the data from CCF 2 will appear
				//in the array values normally used for frequency 1. this value is ignored if not 2 freq. total
						//(switch-32 mode).  NOT YET IMPLIMENTED, ALSO SWITCH-32 IS not implimented

	long int AcqScansToSkipAfterStart; //Number of data acquition point (for all channels) to skip after data
						//acqusition starts.  This can be used to test if the data-acq is synced with
						//source multiplexing.

	bool AddNoiseToDemoData;
	bool AddLowFreqModToDemoData;

	bool pad4;
	bool pad5;

	ulong pad6;
	ulong pad8;

   } USER_SET6;


//A structure to store information that is passed with each status data block
//the structure of this structure should make packing not important, but the
//library is build with packing of 1 byte.
typedef struct {
	  unsigned __int32 packetsize; //set to the size of this structure, could be used to validate.
									//if zero, recieving program should assume invalid.
	  unsigned __int32 pad_0;

	  unsigned __int8 status_packet_version; //should be 6 at least as of now.
	  unsigned __int8 SizeofBoolean; //some c/c++ compiliers use different number of bytes for bool
	  unsigned __int8 pad_1;
	  unsigned __int8 pad_2;

	  unsigned __int8 pad_3;
	  unsigned __int8 pad_4;
	  unsigned __int8 pad_5;
	  unsigned __int8 pad_6;

	  unsigned __int64 StatusDataBlockNum; //unique and incrementing (by +1) number
						//if values skipped, likley buffer is backed up. Blocks
						//(or packets) will be disgarded without notice
						//(unlike data blocks).
	  unsigned __int64 NumDataBlockStoredInWaitToSendBuffer;
	  unsigned __int64 NumStatusBlockStoredInWaitToSendBuffer;
	  unsigned __int64 pad_7;
	  unsigned __int32 NumUnprocDataMessages;
	  unsigned __int32 pad_8;
	  unsigned __int32 pad_9;
	  unsigned __int32 pad_10;

	  bool DataAcqRunning;
	  bool DataStreaming;
	  bool PrimPMTBiasConnected;
	  bool PrimMainAcqBoardConnected;

	  bool PrimDDSDeviceConnected;
	  bool SecPMTBiasConnected;
	  bool SecMainAcqBoardConnected;
	  bool SecDDSDeviceConnected;

	  bool PrimPMTBiasVirtual;
	  bool PrimMainAcqBoardVirtual;
	  bool PrimDDSDeviceVirtual;
	  bool SecPMTBiasVirtual;

	  bool SecMainAcqBoardVirtual;
	  bool SecDDSDeviceVirtual;
	  bool pad_11;
	  bool AutoBiasInProgress;

	  bool pad_12;
	  bool pad_13;
	  bool AutoBiasOneInProgress;
	  bool AutoBiasAllInProgress;

	  unsigned char DataClientIPAddress[32];
	  bool DetectorOverLoadShutdown[64];
	  __int16 DetectorBias[64];
	  __int16 pad_2byteArray[64];

   } STATUSDATA6;
typedef STATUSDATA6 far *pSTATUSDATA6;



#endif





























