/* 
 *	BITMAP.CPP
 *	Ū�� BMP ���ɦܰO����
 *
 */

#ifdef WIN32
#include <windows.h>
#endif
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#include <GL/gl.h>
#include "bitmap.h"

GLubyte *LoadBitmapFile(char *fileName, BITMAPINFO *bitmapInfo)
{

	FILE				*fp;
	BITMAPFILEHEADER	bitmapFileHeader;	// Bitmap file header
	GLubyte				*bitmapImage;		// Bitmap image data
	unsigned int		lInfoSize;			// Size of information
	unsigned int		lBitSize;			// Size of bitmap

	fp = fopen(fileName, "rb");

	if (fp == NULL)
		return NULL;

	// Ū�� bitmap header
	fread(&bitmapFileHeader, sizeof(BITMAPFILEHEADER),1 , fp);

	 

	// Check for BM
	if (bitmapFileHeader.bfType != 'MB')
	{
		fclose(fp);
		return NULL;
	}
	
	// Ū�� bitmap information header
	lInfoSize = bitmapFileHeader.bfOffBits - sizeof(BITMAPFILEHEADER);		// Info size
	if(fread(bitmapInfo, 1, lInfoSize, fp) < lInfoSize)
	{
		fclose(fp);
		return NULL;
	}

	// �t�m�O����
	lBitSize = bitmapInfo->bmiHeader.biSizeImage;
	bitmapImage = new BYTE[lBitSize];
	// Verify memory allocation
	if (!bitmapImage)
	{
		fclose(fp);
		return NULL;
	}

	// Ū���v����
	if(fread(bitmapImage, 1, lBitSize, fp) < lBitSize)
	{
		delete [] bitmapImage;
		fclose(fp);
		return NULL;
	}

	fclose(fp);
	return bitmapImage;
}
