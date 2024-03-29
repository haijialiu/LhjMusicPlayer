#include "pch.h"
#include "CppUnitTest.h"
#include "Player.h"
#include <vector>
#include <string>
#include <iostream>
#include <filesystem>
using namespace Microsoft::VisualStudio::CppUnitTestFramework;
namespace Test
{
    using namespace media;

    using namespace std;
    using namespace std::filesystem;

    TEST_CLASS(PlayerTests)
    {
        Player player;
        path folder_path{ R"(C:\Users\haijialiu\Desktop\media)" };
        directory_entry entry;
        directory_iterator list;
        vector<string> music_urls;
    public:
        TEST_METHOD_INITIALIZE(PlayerInit)
        {
            // method initialization code
            entry = directory_entry(folder_path);
            list = directory_iterator(folder_path);
            for (auto& it : list)
            {
                auto file_name = it.path().filename();
                if (file_name.extension() != ".lrc")
                {
                    music_urls.push_back(file_name.string());
                }
            }
            player.main_loop();
        }
        TEST_METHOD(MyTestMethod)
        {
            player.set_play_list(music_urls);
            player.resume();
        }

    };
}
