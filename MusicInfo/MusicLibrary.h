#pragma once

#ifdef MEDIALIBRARY_EXPORTS
#define MEDIALIBRARY_API __declspec(dllexport)
#else
#define MEDIALIBRARY_API __declspec(dllimport)
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
extern "C" MEDIALIBRARY_API int music_load(const wchar_t* file_url);
extern "C" MEDIALIBRARY_API int music_time();
extern "C" MEDIALIBRARY_API music_info get_music_info();
extern "C" MEDIALIBRARY_API void music_free();


