#pragma once
#include "MediaQueue.h"
#include <map>
extern "C"
{
#include <libavformat/avformat.h>
#include <libavcodec/avcodec.h>

}
namespace media {


	class Demuxer
	{
	public:
		Demuxer(std::shared_ptr<AVPacketQueue> audio_pkt_queue, std::string url);
		~Demuxer();
		void start();
		int run();
		int seek(double target_time);
		unsigned int get_audio_seconds();
		std::map<std::string,std::string> get_metadata() const;
		AVCodecParameters* audio_codec_parameters();
		AVRational audio_stream_timebase();

		
		//线程工作状态
		std::atomic_bool working = false;
		//解复用完成
		std::atomic_bool demux_over = false;
		//解复用推出
		std::atomic_bool demux_abort = false;
	private:
		void clean_queue();
		void clean_audio_queue();
		std::string _url;
		char _error_str[256]{ 0 };
		std::shared_ptr<AVPacketQueue> _audio_queue = nullptr;

		AVFormatContext* ifmt_ctx = nullptr;
		int _audio_index = -1;
		std::unique_ptr<std::jthread> work_thread;
	};
}

