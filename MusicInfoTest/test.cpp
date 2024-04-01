#include "pch.h"
#include "../MusicInfo/MusicInfo.h"
#include <iostream>
namespace tests
{
	using namespace std;

	class MusicInfoTests :public testing::Test
	{

	protected:
		void SetUp() override
		{
		}

		

	};
	TEST_F(MusicInfoTests, Tests)
	{
		music_load(LR"(C:\Users\haijialiu\Desktop\media\Aiobahn _ Yunomi (ゆのみ) - 银河鉄道のペンギン (银河铁道的企鹅).flac)");
		auto info = get_music_info();
		wcout.imbue(locale(""));
		wcout << info.title << endl;
	}
}