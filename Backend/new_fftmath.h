//----------------------------------------------------------------------------
//  Project DMC_ImagentDuae.exe  2013
//
// Some old code from the Original Project: issox100, a 16-bit driver for the
// DRA A2D-160 for use by oximeter software. 1996-97
//
//
//  FILE:         fftmath.h
//  AUTHOR:       Dennis Hueber
//
//  OVERVIEW
//  ~~~~~~~
//
// Functions for Discrete Fast Hartley Transform
// Several small math funciton like random numbers, calculating the phase
// (direction) of two 2 vectors.
//
// DFT functions for specific frequency resutls, rather than full arrau of
// results, not based on FFT. (Ues any interger length input array).
//
//----------------------------------------------------------------------------
#if !defined( __newfftmath_H )

#define __newfftmath_H

#ifdef __BORLANDC__
#pragma option -a1
#else //assume microsoft
#pragma pack(push,1)
#endif

#ifdef __BORLANDC__
#define __PACK1__ option -a1
#define __PACK4__ option -a4
#define __PACKPOP__ option -a.
#else
#define __PACK1__  pack(push,1)
#define __PACK4__  pack(push,4)
#define __PACKPOP__ pack(pop)
#endif


typedef struct
   {
   double DC;
   double real[2];
   double imag[2];
   } DCrealimage_2D;


typedef struct
   {
   double DC;
   double real[4];
   double imag[4];
   } DCrealimage_4D;


typedef struct
   {
   double DC;
   double real[8];
   double imag[8];
   } DCrealimage_8D;

typedef struct
   {
   double DC;
   double real[31];
   double imag[31];
   } DCrealimage_31D;



//Some plain DFT functions for real only input data (assume time).

//max input array 100 points
//results at specified 2 frequencies, list frequencies as interge multiple of full
//measurement time (numpts/sample rate) in harmonicfreq array.
void dft_DCrealimage2(unsigned int num_in_pts,double *in,DCrealimage_2D *out,
	unsigned int harmonicfreqs[2]);


//max input array 100 points
//results at specified 4 frequencies, list frequencies as interge multiple of full
//measurement time (numpts/sample rate) in harmonicfreq array.
void dft_DCrealimage4(unsigned int num_in_pts,double *in,DCrealimage_4D *out,
	unsigned int harmonicfreqs[4]);

//max input array 100 points
//results at specified 8 frequencies, list frequencies as interge multiple of full
//measurement time (numpts/sample rate) in harmonicfreq array.
void dft_DCrealimage8(unsigned int num_in_pts,double *in,DCrealimage_8D *out,
	unsigned int harmonicfreqs[8]);

//max input array 100 points
//caller most allocate memory for, calculates for first 25 frequencies (harmonics
//(numpts/sample rate), set num_out_pts to calculate fewer results, allways lowest first.
void dft_DCrealimage31_or_less(unsigned int num_in_pts,double *in,DCrealimage_31D *out,unsigned int num_out_pts=31);

//max input array 1000 points, max output 1000
//caller must allocate input and output, output is 2-dim array, dimension 1 is
//bin dimension 2 is real or imaginary. Dimension 2 is 2, dimension 1 is at least
//num_out_pts.  Bin 0 will hold DC result.
//very slow? calculates cos and sine each call.
void dft_DCrealimageN(unsigned int num_in_pts,double *time_array_data,unsigned int num_out_pts,double out[][2]);


//fast random generator
void fast_srand( int seed );

int fastrand();



#endif


