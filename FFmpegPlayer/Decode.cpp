#include "pch.h"
#include "Decode.h"
#ifdef _DEBUG
#include "Log.h"
#endif
#include <format>

using std::format;
namespace media
{

    //播放设备关闭状态
    extern std::atomic_bool audio_play_abort;

    extern std::atomic_bool play_over;
}
media::Decoder::Decoder(std::shared_ptr<AVPacketQueue> packet_queue, std::shared_ptr<AVFrameQueue> frame_queue)
    :_packet_queue(packet_queue),_frame_queue(frame_queue)
{
    _codec_ctx = avcodec_alloc_context3(nullptr);
    _frame = av_frame_alloc();
    if (!_codec_ctx|| !_frame)
        throw std::bad_alloc();


}

media::Decoder::Decoder(std::shared_ptr<AVPacketQueue> packet_queue, std::shared_ptr<AVFrameQueue> frame_queue, AVCodecParameters* params)
{
    _codec_ctx = avcodec_alloc_context3(nullptr);
    _frame = av_frame_alloc();
    if (!_codec_ctx || !_frame)
        throw std::bad_alloc();
    try 
    {
        if (params == nullptr)
            throw std::invalid_argument("AVCodec参数为空");
        int ret = avcodec_parameters_to_context(_codec_ctx, params);
        if (ret < 0)
        {
            av_strerror(ret, _error_str, sizeof(_error_str));
            //TODO: 可自定义异常
            throw std::runtime_error(_error_str);
        }
        const AVCodec* codec = avcodec_find_decoder(_codec_ctx->codec_id);
        if (!codec)
        {
            av_strerror(ret, _error_str, sizeof(_error_str));
            throw std::runtime_error(_error_str);
        }
        ret = avcodec_open2(_codec_ctx, codec, nullptr);
        if (ret < 0)
        {
            av_strerror(ret, _error_str, sizeof(_error_str));
            throw std::runtime_error(_error_str);
        }
    }
    catch (std::runtime_error& e)
    {
        avcodec_close(_codec_ctx);
        av_frame_free(&_frame);
        throw e;
    }
}

media::Decoder::~Decoder()
{

    avcodec_close(_codec_ctx);
    av_frame_free(&_frame);
#ifdef _DEBUG
    Log::info("Decode::~Decode");
#endif
}
__declspec(deprecated)
int media::Decoder::init(AVCodecParameters* params)
{ 
    _codec_ctx = avcodec_alloc_context3(nullptr);
    _frame = av_frame_alloc();


    if (params == nullptr)
    {
#ifdef _DEBUG
        Log::error("参数为空");
#endif
        return -1;
    }

    int ret = avcodec_parameters_to_context(_codec_ctx, params);
    if (ret < 0)
    {
        av_strerror(ret, _error_str, sizeof(_error_str));
#ifdef _DEBUG
        Log::error(format("avcodec_parameters_to_context 失败：{}", _error_str));
#endif
        return -1;
    }
    const AVCodec* codec = avcodec_find_decoder(_codec_ctx->codec_id);
    if (!codec)
    {
        av_strerror(ret, _error_str, sizeof(_error_str));
#ifdef _DEBUG
        Log::error(format("avcodec_find_decoder 失败：{}", _error_str));
#endif
        return -1;
    }
    ret = avcodec_open2(_codec_ctx, codec, nullptr);
    if (ret < 0)
    {
        av_strerror(ret, _error_str, sizeof(_error_str));
#ifdef _DEBUG
        Log::error(format("avcodec_open2 失败：{}", _error_str));
#endif
        return -1;
    }
    decode_over = false;
#ifdef _DEBUG
    Log::debug("解码初始化完成");
#endif

    return 0;
}


int media::Decoder::start()
{
    work_thread = std::make_unique<std::jthread>(&Decoder::run, this);
    return 0;
}


int media::Decoder::run()
{
#ifdef _WIN32
    HRESULT r;
    r = SetThreadDescription(GetCurrentThread(), L"解码");
#endif
    this->working = true;

#ifdef _DEBUG
    Log::debug(format("解码运行中"));
#endif
    AVFrame* frame = _frame;
    while (!play_over.load()) {

        if (_frame_queue->size() > 10 && !play_over.load())
        {
            std::this_thread::sleep_for(std::chrono::milliseconds(10));
            continue;
        }
        auto ret = _packet_queue->pop(10);
        //Log::debug(format("DECODE thread _packet_queue is: {}",(int) & _packet_queue));
        AVPacket* pkt = ret.value_or(nullptr);

        if (pkt) //获取一个包准备解码
        {
            //将包发给解码器
            int ret = avcodec_send_packet(_codec_ctx, pkt);
            av_packet_free(&pkt);
            if (ret < 0)
            {
                av_strerror(ret, _error_str, sizeof(_error_str));
#ifdef _DEBUG
                Log::error(format(" avcodec_send_packet failed：{}", _error_str));
#endif
                //发现了一个bug，ogg中断
                //break;
                return -1;
            }
            //从解码器得到解包数据
            while (!decode_over.load())
            {
                ret = avcodec_receive_frame(_codec_ctx, frame);
                if (ret == 0)
                {
                    _frame_queue->push(frame);
                    continue;
                }
                else if (ret == AVERROR(EAGAIN))
                {
                    break;
                }
                else if (ret == AVERROR_EOF) //帧读取完了
                {
#ifdef _DEBUG
                    Log::info(format("receive frame：end of file"));
#endif
                    //avcodec_flush_buffers(_codec_ctx);
                    decode_over = true;
                    decode_over.notify_all();

                    this->decode_over = true;
                    this->decode_over.notify_all();

                    this->working = false;
                    Log::info("解码休眠");
                    this->working.wait(false);
                    break;
                }
                else                //在flac文件中遇到文件尾部会异常
                {
                    av_strerror(ret, _error_str, sizeof(_error_str));
#ifdef _DEBUG
                    Log::error(format("receive frame：{}", _error_str));
#endif
                    this->decode_over = true;
                    this->decode_over.notify_all();
                    this->working = false;
                    Log::info("解码休眠");
                    this->working.wait(false);
                    break;
                }
            }
        }
    }

    this->decode_over = true;
    this->decode_over.notify_all();

    this->decode_over = true;
    this->working = false;

    this->decode_abort = true;
    this->decode_abort.notify_all();
#ifdef _DEBUG
    Log::debug(format("解码结束"));
#endif
    return 0;
}
void media::Decoder::clean_queue()
{
    clean_pkt_queue();
    clean_frame_queue();
}
void media::Decoder::clean_frame_queue()
{
    while (!_frame_queue->empty())_packet_queue->pop();
}
void media::Decoder::clean_pkt_queue()
{
    while (!_packet_queue->empty())_packet_queue->pop();
}