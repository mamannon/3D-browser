// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

//#define NO_STRICT


#include "targetver.h"
#include <WinSock2.h>
#include <windows.h>
#include <gl/gl.h>
#include <gl/glu.h>
#include <gl/wglext.h>
#include <gl/glext.h>
#include <CL/opencl.h>
#include <vector>
#include <map>
#include <math.h>
#include "jakoHeader.h"
#include "historia.h"
#include "renderointi.h"
#include "simulointi.h"
#include "pilvipartikkelit.h"
#include "shaderit.h"
#include "saie.h"
//#include "arial72.h"
#include "Arial16.h"
#include "chartowchar.h"
#include "openclKytkin.h"
#include "TarkastusSummaLista.h"

#if defined (__cplusplus)
extern "C"
{
#endif

  __declspec(dllexport) bool kaynnistaCad(wchar_t* tiedosto, HDC& ikkunaKahva, int& leveys,
										  int& korkeus, bool& eteen, bool& taakse,
										  int& animaatioTila, bool* valintaListaB, wchar_t** valintaListaC,
										  bool* valintaListaB2, wchar_t** valintaListaC2,
											int& valintoja, int& valintoja2);
  __declspec(dllexport) void muutaAla(int leveys, int korkeus);
  __declspec(dllexport) void pyoraAlas();
  __declspec(dllexport) void pyoraYlos();
  __declspec(dllexport) void hiiriPoistuu();
  __declspec(dllexport) void hiiriLiikkuu(int& X, int& Y);
  __declspec(dllexport) void hiiriLiikkuuVasen(int& X, int& Y, int& vanhaHiiriX, int& vanhaHiiriY);
  __declspec(dllexport) void hiiriLiikkuuOikea(int& X, int& Y);
  __declspec(dllexport) void hiiriLiikkuuMolemmat(int& X, int& Y, int& vanhaHiiriX, int& vanhaHiiriY);
  __declspec(dllexport) void eteen(wchar_t* tiedosto, bool& eteen, bool& taakse, int& animaatioTila,
								   bool* valintaListaB, wchar_t** valintaListaC,
								   bool* valintaListaB2, wchar_t** valintaListaC2,
								   int& valintoja, int& valintoja2);
  __declspec(dllexport) void taakse(wchar_t* tiedosto, bool& eteen, bool& taakse, int& animaatioTila,
									bool* valintaListaB, wchar_t** valintaListaC,
									bool* valintaListaB2, wchar_t** valintaListaC2,
									int& valintoja, int& valintoja2);
  __declspec(dllexport) void tulosta();
  __declspec(dllexport) void vieValinnat(bool* valintaListaB1, wchar_t** valintaListaC1,
										 bool* valintaListaB2, wchar_t** valintaListaC2,
										 int& valintoja1, int& valintoja2);
  __declspec(dllexport) bool onkoLaajennuksia(HDC& ikkunaKahva, HWND& hwndMain, HWND& hwnd, bool& eiValaisua);
  __declspec(dllexport) void eloKuva(int kytkimet);
  __declspec(dllexport) void paivita();
  __declspec(dllexport) void sammutaCad();
  __declspec(dllexport) void annaOrientaatio1(float* orientaatio);
  __declspec(dllexport) void asetaOrientaatio1(float*orientaatio);

#if defined (__cplusplus)
}
#endif