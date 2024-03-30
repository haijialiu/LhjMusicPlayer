#pragma once
#include "MediaQueue.h"

#ifdef _WIN32
#include <windows.h>
#include <processthreadsapi.h>
#endif
extern "C"
{
#include <libavformat/avformat.h>
#include <libavcodec/avcodec.h>

}
namespace media 
{

	
	class Decoder
	{
	public:
		Decoder(std::shared_ptr<AVPacketQueue> packet_queue, std::shared_ptr<AVFrameQueue> frame_queue);
		Decoder(std::shared_ptr<AVPacketQueue> packet_queue,
			std::shared_ptr<AVFrameQueue> frame_queue,
			AVCodecParameters* params);
		~Decoder();
		int init(AVCodecParameters* params);
		int start();
		int run();

		//线程工作状态
		std::atomic_bool working = false;
		//解码完成
		std::atomic_bool decode_over = false;
		//解码run推出
		std::atomic_bool decode_abort = false;
	private:
		void clean_queue();
		void clean_frame_queue();
		void clean_pkt_queue();

		char _error_str[256]{ 0 };
		AVCodecContext* _codec_ctx = nullptr;
		std::shared_ptr<AVPacketQueue> _packet_queue = nullptr;
		std::shared_ptr<AVFrameQueue> _frame_queue = nullptr;
		AVFrame* _frame = nullptr;

		//std::jthread* work_thread;
		std::unique_ptr<std::jthread> work_thread;
	};
}

