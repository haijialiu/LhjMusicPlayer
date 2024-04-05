#include "pch.h"
#include "../FFmpegPlayer/FFmpegAudioMain.h"
#include <vector>
#include <string>
#include <iostream>
#include <filesystem>
namespace gtests
{
	using namespace std;
	using namespace std::filesystem;
	class MusicPlayerTests :public testing::Test
	{

		path folder_path{ R"(C:\Users\haijialiu\Desktop\media)" };
		directory_entry entry;
		directory_iterator list;
		vector<string> music_urls;
		vector<const char*> music_urls1;
	protected:
		void SetUp() override
		{
			create_player();
			entry = directory_entry(folder_path);
			list = directory_iterator(folder_path);
			for (auto& it : list)
			{
				auto file_name = it.path().filename();
				if (file_name.extension() != ".lrc")
				{
					music_urls.push_back(file_name.string());
					music_urls1.push_back(file_name.string().c_str());
				}
			}
			input_music_urls(music_urls1.data(),music_urls1.size());
			
		}

		

	};
	TEST_F(MusicPlayerTests, Tests)
	{

	}
}