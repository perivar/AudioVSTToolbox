/*
 *  libzplay - windows ( WIN32 ) multimedia library for playing mp3, mp2, ogg, wav, flac and raw PCM streams
 *  WSpectrum class
 *
 *  Copyright (C) 2003-2009 Zoran Cindori ( zcindori@inet.hr )
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 *
 *
 * ver: 1.14
 * date: 15. April, 2010.
 *
 * SUPPORTED BY:
 * ==================================================================================
 * FFT part is a routine made by Mr.Ooura. This routine is a freeware.
 * Contact Mr.Ooura for details of distributing licenses.
 * http://momonga.t.u-tokyo.ac.jp/~ooura/fft.html
 * ==================================================================================
*/

#ifndef _W_SPECTRUM_H_
#define _W_SPECTRUM_H_

#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include "wfft.h"
#include "wbmpfont.h"

#define GRAPH_TYPE_LINE_LEFT 0
#define GRAPH_TYPE_LINE_RIGHT 1
#define GRAPH_TYPE_AREA_LEFT 2
#define GRAPH_TYPE_AREA_RIGHT 3
#define GRAPH_TYPE_BARS_LEFT 4
#define GRAPH_TYPE_BARS_RIGHT 5
#define GRAPH_TYPE_SPECTRUM 6

typedef struct {
	unsigned short nX;
	unsigned short nWidth;
} FREQUENCY_TABLE;

class WSpectrum {
public:
	WSpectrum();
	~WSpectrum();

// =====================================================================================================================
	// initialize spectrum
	int Initialize(unsigned int nFFTPoints, unsigned int nSampleRate, unsigned int nChannel, unsigned int nBitsPerSample);
	//
	//	PARAMETERS:
	//		nFFTPoints
	//			Number of FFT points. Must be power of 2 and can't be 0.
	//
	//		nSampleRate
	//			Sample rate.
	//
	//		nChannel
	//			Number of channels. Must be 1 or 2.
	//
	//		nBitsPerSample
	//			Bits per sample. Must be 8, 16 or 32.
	//
	//	RETURN VALUES:
	//		1	- all OK
	//		0	- error
	//
	//	REMARKS:
	//		Call this function before all other functions to initialize class. You can call
	//		This function later if you need to change FFT points, sample rate, channel or bits per sample.

// =============================================================================================================
	// free allocated resources
	void Free();
	//
	//	PARAMETERS:
	//		None.
	//
	//	RETURN VALUES:
	//		None.
	//
	//	REMARKS:
	//		Call this function to free memory and GDI resources.
// =============================================================================================================
	// set PCM samples
	int SetSamples(void *pSamples, unsigned int nSampleNum);
	//
	//	PARAMETERS:
	//		pSamples
	//			Pointer to buffer with PCM samples. Set type of samples with Initialize function.
	//
	//		nSampleNum
	//			Number of samples in buffer. You must have nFFTPoints samples.
	//
	//	RETURN VALUES:
	//		1	- all OK
	//		0	- error
	//
	//	REMARKS:
	//		This function will calculate FFT and prepare data for drawing.	
// =============================================================================================================
	// draw graph on screen
	int DrawOnHDC(HDC hdc, int nX, int nY, int nWidth, int nHeight);
	//
	//	PARAMETERS:
	//		hdc
	//			Handle of device context.
	//
	//		nX
	//			X coordinate of graph.
	//
	//		nY
	//			Y coordinate of graph.
	//
	//		nWidth
	//			Width of graph. This value can't be smaller than FFT_GRAPH_MIN_WIDTH.
	//
	//		nHeight
	//			Height of graph. This value can't be smaller than FFT_GRAPH_MIN_HEIGHT.
	//
	//	RETURN VALUES:
	//		1	- all OK
	//		0	- error
// =============================================================================================================
//	enable subgrid lines
	int EnableSubGrid(int fEnable);
	int IsSubGridEnabled();



// =============================================================================================================
	// set FFT window
	int SetFFTWindow(unsigned int nWindow);
	//
	//	PARAMETERS:
	//		nWindow
	//			FFT window. Use one of these values:
	//				FFT_WINDOW_RECTANGULAR
	//				FFT_WINDOW_HAMMING
	//				FFT_WINDOW_HANN	
	//				FFT_WINDOW_COSINE
	//				FFT_WINDOW_LANCZOS	
	//				FFT_WINDOW_BARTLETT
	//				FFT_WINDOW_TRIANGULAR
	//				FFT_WINDOW_GAUSS
	//				FFT_WINDOW_BARTLETT_HANN
	//				FFT_WINDOW_BLACKMAN
	//				FFT_WINDOW_NUTTALL
	//				FFT_WINDOW_BLACKMAN_HARRIS
	//				FFT_WINDOW_BLACKMAN_NUTTALL
	//				FFT_WINDOW_FLAT_TOP
	//
	//	RETURN VALUES:
	//		1	- all OK
	//		0	- error
// =============================================

	int GetFFTWindow();
// ===================================================================================================================
	// set graph type
	int SetGraphType(unsigned int nType);
	//
	//	PARAMETERS:
	//		nType
	//			Graph type. Use one of values:
	//				GRAPH_TYPE_LINE_LEFT
	//				GRAPH_TYPE_LINE_RIGHT
	//				GRAPH_TYPE_AREA_LEFT
	//				GRAPH_TYPE_AREA_RIGHT
	//				GRAPH_TYPE_BARS_LEFT
	//				GRAPH_TYPE_BARS_RIGHT
	//
	//	RETURN VALUES:
	//		1	- all OK
	//		0	- error
// ====================================
//	get graph type
	int GetGraphType();
// ===================================================================================================================
	// set graph type
	int SetLinearGraph(unsigned int fLinear);
	//
	//	PARAMETERS:
	//		fLinear
	//			Set linear graph.
	//
	//	RETURN VALUES:
	//		1	- all OK
	//		0	- error
// =======================================
	int IsLinearGraph();	

// ========================================
	// set graph area transparency
	int SetTransparency(int nTransparency);
	// PARAMETERS:
	//		nTransparency
	//			Transparency of area in percent. 0 - not transparent, 100 - full transparent
	//
	//	RETURN VALUES:
	//		1	- all OK
	//		0	- error	
// =========================================
	// get graph area transparency

	int GetTransparency();
	// PARAMETES:
	//		None.
	//
	//	RETURN VALUES:
	//		Current graph area transparency. 0 - not transparent, 100 - full transparent
// ===================================================================================================================
	// return samples buffer
	void *GetSamplesBuffer();
	//
	//	PARAMETERS:
	//		None.
	//
	//	RETURN VALUES:
	//		Pointer to buffer for samples. User can use this buffer for samples. This buffer is
	//		allocated by Initialize function and has size for c_nFFTPoints stereo 32 bit samples.
	//		Class is responsible for this buffer so user don't need to fre this buffer.

// ===================================================================================================================

	// show frequency scale
	//
	// PARAMATERS:
	//		fShow - value 1 show scale, value 0 hide scale, value -1 returns current show state
	//
	//	RETURN VALUES:
	//		1	- all OK
	//		0	- error
	//		if fShow is -1, returned value represents current show state
	int ShowFrequencyScale(int fShow);

// ===================================================================================================================
	// show decibel scale
	//
	// PARAMATERS:
	//		fShow - value 1 show scale, value 0 hide scale, value -1 returns current show state
	//
	//	RETURN VALUES:
	//		1	- all OK
	//		0	- error
	//		if fShow is -1, returned value represents current show state
	int ShowDecibelScale(int fShow);

// ===================================================================================================================


	// show frequency grid
	//
	// PARAMATERS:
	//		fShow - value 1 show grid, value 0 hide grid, value -1 returns current show state
	//
	//	RETURN VALUES:
	//		1	- all OK
	//		0	- error
	//		if fShow is -1, returned value represents current show state
	int ShowFrequencyGrid(int fShow);


// ===================================================================================================================

	// show decibel grid
	//
	// PARAMATERS:
	//		fShow - value 1 show grid, value 0 hide grid, value -1 returns current show state
	//
	//	RETURN VALUES:
	//		1	- all OK
	//		0	- error
	//		if fShow is -1, returned value represents current show state
	int ShowDecibelGrid(int fShow);

// ===================================================================================================================
	// show background bitmap
	//
	// PARAMATERS:
	//		fShow - value 1 show background bitmap, value 0 hide background bitmap, value -1 returns current show state
	//
	//	RETURN VALUES:
	//		1	- all OK
	//		0	- error
	//		if fShow is -1, returned value represents current show state

	int ShowBgBitmap(int fShow);

// ===================================================================================================================
	// set background bitmap, function makes copy of bitmap, user can destroy original
	//
	// PARAMATERS:
	//		hbm - handle to bitmap
	//
	//	RETURN VALUES:
	//		1	- all OK
	//		0	- error

	int SetBgBitmap(HBITMAP hbm);

// ===================================================================================================================

	// stretch background bitmap
	//
	// PARAMATERS:
	//		fStretch - if 1 bitmap is stretched, if 0 bitmap is centered, value -1 returns stretch state
	//
	//	RETURN VALUES:
	//		1	- all OK
	//		0	- error

	int StretchBgBitmap(int fStretch);

// ===================================================================================================================
	// set color
	//
	// PARAMETERS:
	//  nIndex - color index
	//  color  - color

	int SetColor(int nIndex, COLORREF color);

	COLORREF GetColor(int nIndex);

// ===================================================================================================================


private:
	WFFT c_fft;

	int c_fShowHScale;
	int c_fShowVScale;

	int c_fDrawSubGrid;	// draw subgrid lines

	// show decibel grid
	int c_fShowVGrid;
	// show frequency grid
	int c_fShowHGrid;

	int c_fShowBgBitmap;
	HBITMAP c_bgBitmap;
	HBITMAP c_bgExternBitmap;
	int c_nBitmapWidth;
	int c_nBitmapHeight;
	int c_fStretchBgBitmap;

	COLORREF cr1;	// scale background color
	COLORREF cr2;	// graph background color
	COLORREF cr3;	// freq grid
	COLORREF cr4;	// decibel grid
	COLORREF cr5;	// freq subgrid
	COLORREF cr6;
	COLORREF cr7;
	COLORREF cr8;
	COLORREF cr9;
	COLORREF cr10;
	COLORREF cr11;
	COLORREF cr12;
	COLORREF cr13;
	COLORREF cr14;
	COLORREF cr15;
	COLORREF cr16;

	unsigned int c_nLogarithmicOffset;

	unsigned int c_nFFTWindow;

	unsigned int c_nBitsPerSample;
	unsigned int c_nSampleRate;
	unsigned int c_nChannel;
	unsigned int c_nFFTPoints;

	unsigned int c_nSpectrumType;

	int *c_pnSamplesBuffer;

	int c_fLinear;
	

	int c_nAreaTransparency;	// transparency of area ( 0 - 100, 100 transparent )
	int c_nAlpha;
	int c_nInvAlpha;

	unsigned int c_nHarmonicsNumber;	// number of harmonics

	int *c_pnLeftAmplitude;				// left amplitude array
	int *c_pnRightAmplitude;				// right amplitude array
	unsigned int *c_pnReductionTable;
	int c_fNeedReduction;

	POINT *FFT_Left;
	POINT *FFT_Right;
	POINT *FFT_Tmp;	
	POINT *FFT_Tmp1;	
	POINT *FFT_Tmp2;	
	POINT *FFT_Tmp3;
		
	WBmpFont *c_BmpFont;
	WBmpFont *c_BmpFontInverse;
	HDC c_hdcSrc;
	HDC c_hdcDest;
	HBITMAP c_hbmSrc;
	HBITMAP c_hbmDest;
	HBITMAP c_hbmBgBitmap;

	HBITMAP c_hbmSrcOld;
	HBITMAP c_hbmDestOld;

	BYTE *c_pSrcBits;
	BYTE *c_pDestBits;
	BYTE *c_pBgBits;


//	HBRUSH hBrushFFTBackground;
	HBRUSH hBrushFFTLeft;
	HBRUSH hBrushFFTLeftOverlap;
	HBRUSH hBrushFFTRight;
	HBRUSH hBrushFFTRightOverlap;

//	HPEN hPenBgLine;
//	HPEN hPenBgLineStrong;
	HPEN hPenLeft; 
	HPEN hPenRight;

	HPEN hPenLeftOverlap; 
	HPEN hPenRightOverlap;

	HDC c_hdc;
	int c_nWidth;
	int c_nHeight;

	RECT c_InnerRect;
	float flWidthScale;
	float flHeightScale;
	unsigned int c_nPointsNumber;

	int _AllocateFFTMemory();
	void _FreeFFTMemory();

	int _AllocateFFTGDI(HDC hdc, unsigned int nWidth, unsigned int nHeight);
	void _FreeFFTGDI();
	int _CreateBgBitmap(HDC hdc, unsigned int nWidth, unsigned int nHeight);

	// end freq included
	void DrawXLinear(HDC hdc, FREQUENCY_TABLE *freq_table, unsigned int nStartFreq, unsigned int EndFreq, int fDrawZerroGrid, int fDrawZerroText, unsigned int nWidth, unsigned int nHeight);
	void DrawXLogarithmic(HDC hdc, FREQUENCY_TABLE *freq_table, unsigned int nStartFreq, unsigned int EndFreq, int fDrawZerroGrid, int fDrawZerroText, unsigned int nWidth, unsigned int nHeight);
	void alpha_blend();

	int c_spectrum_pos;
	int c_clearFFTDisplay;

	int c_fInitalizeY;	

};








#endif
