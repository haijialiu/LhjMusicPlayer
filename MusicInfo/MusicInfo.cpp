#include "pch.h"
#include <map>
#include <string>
#include <algorithm>
#include <fstream>
#include "MusicInfo.h"
extern "C"
{
#include <libavformat/avformat.h>
#include <libavutil/avutil.h>
#include <libavcodec/avcodec.h>
#include <libavutil/imgutils.h>
#include <libswscale/swscale.h>
}

static std::map<std::string, std::string> _metadata;
static AVFormatContext* ifmt;
static int audio_index = -1,video_index = -1;
static std::map<std::string, std::string> get_metadata();
static album_info info{ 0 };
static music_info _music_info{ 0 };
std::string GetLastErrorAsString()
{
	//Get the error message ID, if any.
	DWORD errorMessageID = ::GetLastError();
	if (errorMessageID == 0) {
		return std::string(); //No error message has been recorded
	}

	LPSTR messageBuffer = nullptr;

	//Ask Win32 to give us the string version of that message ID.
	//The parameters we pass in, tell Win32 to create the buffer that holds the message for us (because we don't yet know how long the message string will be).
	size_t size = FormatMessageA(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
		NULL, errorMessageID, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPSTR)&messageBuffer, 0, NULL);

	//Copy the error message into a std::string.
	std::string message(messageBuffer, size);

	//Free the Win32's string's buffer.
	LocalFree(messageBuffer);

	return message;
}

/// <summary>
///		宽字符转窄字符
///		参考:https://learn.microsoft.com/zh-cn/windows/win32/api/stringapiset/nf-stringapiset-widechartomultibyte
/// </summary>
/// <param name="wstr"></param>
/// <returns></returns>
static std::string wstr_to_str(const wchar_t* wcstr)
{

	int size_required = WideCharToMultiByte(CP_UTF8, 0, wcstr, -1, NULL, 0, NULL, NULL);
	std::string str(size_required, 0);
	WideCharToMultiByte(CP_UTF8, 0, wcstr, -1, &str[0], size_required, NULL, NULL);
	return str;
}
static void str_to_wstr(const char* str,wchar_t* wstr)
{
	int size_required = MultiByteToWideChar(CP_UTF8, 0, str, -1, NULL, 0);
	if (size_required <= 256)
		MultiByteToWideChar(CP_UTF8, 0, str, -1, wstr, size_required);
}

int music_load(const wchar_t* file_url)
{	
	int ret = -1;
	ifmt = avformat_alloc_context();
	if (!ifmt)
	{
		return -1;
	}
	auto str = wstr_to_str(file_url);
	ret = avformat_open_input(&ifmt, str.c_str(), nullptr, nullptr);
	if (ret != 0)
		return -2;
	ret = avformat_find_stream_info(ifmt, nullptr);
	if (ret != 0)
		return -3;
	audio_index = av_find_best_stream(ifmt, AVMEDIA_TYPE_AUDIO, -1, -1, nullptr, 0);
	video_index = av_find_best_stream(ifmt, AVMEDIA_TYPE_VIDEO, -1, -1, nullptr, 0);
	if (audio_index < 0)
		return audio_index;
	_metadata = get_metadata();
	return 0;
}

static const char* find_data(std::string key)
{
	auto data = _metadata.find(key);
	if (data != _metadata.end())
		return (*data).second.c_str();
	return nullptr;
}

int music_time()
{
	if (audio_index >= 0)
	{
		int64_t duration = ifmt->streams[audio_index]->duration;
		auto time_base = ifmt->streams[audio_index]->time_base;
		return duration * time_base.num / time_base.den;
	}
	return 0;
}



//专辑图片
static album_info media_album_png()
{
	album_info _info{ 0 };
	using namespace std;
	if (video_index < 0)
		return _info;
	AVPacket pkt;

	if (!ifmt)
	{
		return _info;
	}
	auto codec_ctx = avcodec_alloc_context3(nullptr);
	int _ret = -1;
	while (true)
	{
		_ret = av_read_frame(ifmt, &pkt);
		if (_ret < 0)
		{
			avcodec_free_context(&codec_ctx);
			return _info;
		}
		if (pkt.stream_index == video_index)
		{
			_info.image = pkt.data;
			_info.image_size = pkt.size;
#ifdef _DEBUG
			ofstream out("imagecpp.png", ios::out | ios::binary);
			out.write((char*)pkt.data, pkt.size);
			out.close();
#endif // _DEBUG
			break;
		}
		else
		{
			av_packet_unref(&pkt);
		}
	}
	avcodec_free_context(&codec_ctx);
	return _info;
}

music_info get_music_info()
{
	str_to_wstr(find_data("title"), _music_info.title);
	str_to_wstr(find_data("artist"), _music_info.artist);
	str_to_wstr(find_data("album"), _music_info.album);
	_music_info.total_time = music_time();
	_music_info.rgba_album_info = media_album_png();
	return  _music_info;
}
void music_free()
{
	avformat_free_context(ifmt);
	memset(&_music_info, 0, sizeof(_music_info));
}
static std::map<std::string, std::string> get_metadata()
{
	std::map<std::string, std::string> metadata;
	AVDictionaryEntry* tag = nullptr;
	if (audio_index >= 0)
	{
		auto _metadata = ifmt->metadata;
		if (!_metadata)
			_metadata = ifmt->streams[audio_index]->metadata;
		while (tag = av_dict_get(_metadata, "", tag, AV_DICT_IGNORE_SUFFIX))
		{
			std::string key(tag->key), value(tag->value);
			std::transform(key.begin(), key.end(), key.begin(), [](unsigned char c) {return std::tolower(c); });
			metadata.insert({ key,value });
		}
	}
	return metadata;
}
