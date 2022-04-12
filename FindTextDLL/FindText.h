// FindText.h - Contains Declartions etc.
#pragma once

#ifdef FINDTEXTDLL_EXPORTS
#define FINDTEXT_API __declspec(dllexport)
#else
#define FINDTEXT_API __declspec(dllimport)
#endif

extern "C" FINDTEXT_API int PicFindC(
    int mode, unsigned int c, unsigned int n, int dir
    , unsigned char* Bmp, int Stride, int zw, int zh
    , int sx, int sy, int sw, int sh
    , char* ss, unsigned int* s1, unsigned int* s0
    , char* text, int w, int h, int err1, int err0
    , unsigned int* allpos, int allpos_max);
