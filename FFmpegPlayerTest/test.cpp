#include "pch.h"
#include "../FFmpegPlayer/Player.h"
#include <filesystem>
#include <future>
namespace tests
{
	using namespace media;
	using namespace std;
	using namespace std::filesystem;
	future<int> fut;
	class MusicPlayerTests :public testing::Test
	{



	protected:
		void SetUp() override
		{
			entry = directory_entry(folder_path);
			list = directory_iterator(folder_path);
			for (auto& it : list)
			{
				auto file_name = it.path().filename();
				if (file_name.extension() != ".lrc")
				{
					music_urls.push_back(it.path().string());
					music_urls1.push_back(it.path().string().c_str());
				}
			}

		}
		path folder_path{ R"(C:\Users\haijialiu\Desktop\media)" };
		directory_entry entry;
		directory_iterator list;
		vector<string> music_urls;
		vector<const char*> music_urls1;
		Player player;

	};
	TEST_F(MusicPlayerTests, Tests)
	{
		auto fun = std::bind(&Player::main_loop, &player);
		fut = std::async(std::launch::async, fun);
		//player.main_loop();
		Sleep(2000);
		player.set_play_list(music_urls);
		while (true)
		{
			//TODO: 打包前记得注释掉
			std::string action;
			std::string value;
			std::cin >> action >> value;
			player.operate(action, value);
			if (value == "quit")
				break;
		}
	}
}