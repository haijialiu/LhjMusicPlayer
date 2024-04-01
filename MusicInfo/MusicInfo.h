#pragma once

#ifdef MUSICINFO_EXPORTS
#define MUSICINFO_API __declspec(dllexport)
#else
#define MUSICINFO_API __declspec(dllimport)
#endif

typedef struct 
{
	int image_size;
	uint8_t* image;
} album_info;
typedef struct 
{
	wchar_t title[256];
	wchar_t artist[256];
	wchar_t album[256];
	int total_time;
	album_info rgba_album_info;
} music_info;
extern "C" MUSICINFO_API int music_load(const wchar_t* file_url);
extern "C" MUSICINFO_API int music_time();
extern "C" MUSICINFO_API music_info get_music_info();
extern "C" MUSICINFO_API void music_free();


