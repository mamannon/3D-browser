// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#ifndef __KIRJASTO2_H
#define __KIRJASTO2_H

//#pragma once

#include "targetver.h"

//#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files:

#include <windows.h>

// TODO: reference additional headers your program requires here
//#pragma comment(lib, "libboost_thread-bcb-mt-d-1_39.lib")

#undef wxUSE_PALETTE
#undef __WXMICROWIN__
#define wxUSE_DC_CACHEING 1
#define wxFONT_SIZE_COMPATIBILITY 0
#define WM_LAHETALINKKI (WM_USER+2)

#include <boost/thread.hpp>
#include <windows.h>
#include <stdexcept>
#include <vector>
#include <string>
#include <stdlib.h>





#if defined(__cplusplus)
extern "C"
{
#endif
/*
  namespace boost
  {
	void tss_cleanup_implemented() {
		//tyhjä
	}
  }
*/



  __declspec(dllexport) void annaKorkeusLeveys(int& xIncrement, int& yIncrement, int& xPosition, int& yPosition,
                        int& xRange, int& yRange, int& leveys, int& korkeus);
  __declspec(dllexport) bool lataaTiedosto(wchar_t* tiedosto, HWND& hwndMain, HDC& hdc, HWND& hwnd,
						int& xRange, int& yRange, int& xPosition, int& yPosition, unsigned int& vari, wchar_t *otsikko,
						wchar_t** otsikkoLista, int& listanPituus);
  __declspec(dllexport) bool OnMouseEvent(int& x, int& y, int& tapaus, bool vasen, bool oikea,
						int& xRange, int& yRange, int& xPosition, int& yPosition, unsigned int& vari);
  __declspec(dllexport) bool HistoryBack(int& yPosition, int& yRange, int& xPosition, int& xRange,
										 unsigned int& vari, wchar_t *otsikko, wchar_t** otsikkoLista, int& listanPituus);
  __declspec(dllexport) bool HistoryForward(int& yPosition, int& yRange, int& xPosition, int& xRange,
											unsigned int& vari, wchar_t *otsikko, wchar_t** otsikkoLista, int& listanPituus);
  __declspec(dllexport) bool HistoryCanBack();
  __declspec(dllexport) bool HistoryCanForward();
  __declspec(dllexport) void siirryAnkkuriin(wchar_t *otsikko, int& xRange, int& yRange, int& xPosition, int& yPosition);
  __declspec(dllexport) void lataaUudestaan(int& xRange, int& yRange, int& xPosition, int& yPosition, bool& vasenOikea);
  __declspec(dllexport) void piirra();
  __declspec(dllexport) void tulostetaan();
  __declspec(dllexport) void paivitaOtsikot(wchar_t** otsikkoLista, int& listanPituus);
  __declspec(dllexport) void sammutaTeksti();

#if defined(__cplusplus)
}
#endif

#endif

