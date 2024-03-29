#include "pch.h"
#include "../MusicInfo/MusicLibrary.h"
namespace tests {
	class MusicInfoTest :public testing::Test
	{
	protected:
		void TearDown() override
		{
			music_free();
		}
	};
	TEST_F(MusicInfoTest, get_music_info)
	{
		music_load(LR"(C:\Users\haijialiu\Desktop\media\fripSide (フリップサイド) - only my railgun.flac)");
		auto info = get_music_info();
	}
}