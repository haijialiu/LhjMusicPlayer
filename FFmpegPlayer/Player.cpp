#include "pch.h"
#include "Player.h"
#include <thread>
#include <format>
#include <algorithm>
#include <future>
#include <iostream>
#include "Log.h"
#ifdef _WIN32
#include <windows.h>
#include <processthreadsapi.h>
#endif
namespace media
{

	std::atomic_bool play_over = false;
	std::atomic_bool no_music = true;
	std::atomic_bool music_pause = true;
	std::atomic_bool first = false;
	std::atomic_bool switch_flag = false;

	extern std::atomic_bool audio_play_abort;
	extern std::atomic_bool audio_play_abort_confirm;

	std::shared_ptr<AVPacketQueue> Player::_audio_pkt_queue = std::make_shared<AVPacketQueue>();
	std::shared_ptr<AVFrameQueue> Player::_audio_frame_queue = std::make_shared<AVFrameQueue>();
	std::future<int> audio_future;
	std::future<int> player_future;

	
}



media::Player::~Player()
{
	interrupt_current_play();
	play_over.wait(false);
}
void media::Player::set_play_list(const std::vector<std::string>& music_urls)
{
	_music_urls = music_urls;
	if (!music_urls.empty())
	{
		no_music = false;
		no_music.notify_all();
	}

}

//控制器
int media::Player::operate(std::string action, std::string value)
{
	if (action == "switch")
	{
		play_status = true;
		int index = std::stoi(value);
		switch_next(index);
	}
	else if (action == "pause")
	{
		pause();
	}
	else if (action == "resume")
	{
		resume();
	}
	else if (action == "play_mode")
	{
		int mode = std::stoi(value);
		if (mode >= 0 && mode <= 2)
			play_mode = mode;
		else
		{
			return -1;
		}
	}
	else if (action=="remove")
	{
		int index = std::stoi(value);
		if (index >= 0 && index < _music_urls.size()) 
		{
			_music_urls.erase(_music_urls.begin() + index);
			if (index == _cur_play_index)
			{
				switch_next(index);
			}
		}
	}
	else if (action == "volume")
	{
		//TODO: 音量调节还未实现
	}
	else if (action == "seek")
	{
		double target_time = std::stoi(value);
		_demuxer->seek(target_time);
	}
	else if (action == "system")
	{
		if (value == "quit")
		{
			_abort = true;
		}
	}
	if (first.load())
	{
		first = false;
		first.notify_all();
	}
	return 0;
}

int media::Player::main_loop()
{
	//work_thread = new std::jthread(&Player::start, this);
	//Log::init();
	work_thread = std::make_unique<std::jthread>(&Player::start, this);
	if (!work_thread)
	{
		return -1;
	}

//#ifdef _DEBUG
	//while (!_abort)
	//{
	//	//TODO: 打包前记得注释掉
	//	std::string action;
	//	std::string value;
	//	std::cin >> action >> value;
	//	operate(action, value);
	//}
//
//#endif // _DEBUG




	return 0;
}

int media::Player::start()
{


#ifdef _WIN32
	HRESULT r;
	r = SetThreadDescription(GetCurrentThread(), L"控制器");
#endif
	// 播放模式说明： 0 单曲循环 1 顺序播放 2 列表循环
	
	while (!_abort)
	{

		if (_music_urls.empty())
		{
			no_music = true;
			no_music.wait(true);
		}
		first.wait(true);
		play();
		if (switch_flag.load())
		{
			switch_flag = false;
			continue;
		}
		if (play_mode == 0)
		{
			//单曲循环
		}
		else if (play_mode == 1)
		{
			_cur_play_index++;
			if (_cur_play_index >= _music_urls.size())
			{
				_cur_play_index = 0;
				pause();

			}
		}
		else if (play_mode == 2)
		{
			_cur_play_index++;
			_cur_play_index = _cur_play_index % _music_urls.size();
		}
	}
	return 0;
}

int media::Player::play()
{
	clean_queue();
#ifdef _WIN32
	HRESULT r;
	r = SetThreadDescription(GetCurrentThread(), L"控制器play");
#endif

	if (_music_urls.empty())
		return 0;
	play_over = false;
	int ret = -1;
	_demuxer = std::make_shared<Demuxer>(_audio_pkt_queue, _music_urls[_cur_play_index]);


	_audio_decoder = std::make_shared<Decoder>(_audio_pkt_queue, _audio_frame_queue);

	//ret = _demuxer->init();
	//if (ret < 0)
	//{
	//	return ret;
	//}
	_demuxer->start();

	//2，获取音频解码参数用于解码线程
	//Log::debug("获取音频解码参数");
	auto params = _demuxer->audio_codec_parameters();

	//只放音频的
	prepare_audio_player(params);
	if (first.load())
	{
		_audio_output->pause();
		first = false;
	}
	if (!play_status.load())
	{
		_audio_output->pause();
	}
	//4，加载解码线程
	//Log::info("初始化音频解码线程...");
	_audio_decoder->init(params);
	_audio_decoder->start();


	wait_to_play_over();
	//6，销毁播放器（播放器参数尽量贴近音频文件）
	destroy_audio_player();

	play_over = true;

	wait_demuxer_and_decode_exit();
	return 0;
}

void media::Player::switch_next(int index)
{
	_cur_play_index = index;
	interrupt_current_play();
	if (!music_pause.load())
	{
		play_over.wait(false);
	}



}

void media::Player::pause()
{
	play_status = false;
	if (_audio_output)
	{
		_audio_output->pause();
	}

}

void media::Player::resume()
{
	play_status = true;
	if (_audio_output)
	{
		_audio_output->play();
	}

}

void media::Player::quit()
{
}
void media::Player::prepare_audio_player(AVCodecParameters* params)
{
	//Log::info("初始化播放器");
	_audio_params = std::make_shared<AudioParams>();
	_audio_params->ch_layout = params->ch_layout;
	_audio_params->fmt = (enum AVSampleFormat)params->format;
	_audio_params->freq = params->sample_rate;
	_audio_params->frame_size = params->frame_size;
	_audio_output = std::make_shared<AudioOutput>(_demuxer->audio_stream_timebase(), *_audio_params, _audio_frame_queue);
	//播放器初始化并等待pcm包
	_audio_output->init();
	//Log::info("播放设备已打开");
}

void media::Player::destroy_audio_player()
{
	audio_play_abort = true;
	_audio_output->destory();
	if (_audio_output)
	{
		_audio_output = nullptr;
		//Log::debug("销毁了音频播放器");
	}
}



void media::Player::wait_to_play_over()
{
	//等待解码器把音频帧全部解完
	Log::debug("等待音频解码完成...");
	_audio_decoder->decode_over.wait(false);
	//等待播放器把剩余帧耗尽

	while (!_audio_frame_queue->empty())
	{
		//如果此时播放器都被结束掉了则立即清空
		if (!_audio_output)
		{
			_audio_frame_queue->pop();
		}
	}
}

void media::Player::interrupt_current_play()
{
	switch_flag = true;

	play_over = true;
	wait_demuxer_and_decode_exit();

	destroy_audio_player();

}

void media::Player::wait_demuxer_and_decode_exit()
{
	if (_demuxer->working.load() == false)
	{
		_demuxer->working = true;
		_demuxer->working.notify_all();
	}
	if (_audio_decoder->working.load() == false)
	{
		_audio_decoder->working = true;
		_audio_decoder->working.notify_all();

	}
	_demuxer->demux_abort.wait(false);
	_audio_decoder->decode_abort.wait(false);
}

void media::Player::clean_queue()
{
	while (!_audio_pkt_queue->empty())_audio_pkt_queue->pop();
	while (!_audio_frame_queue->empty())_audio_frame_queue->pop();
}


