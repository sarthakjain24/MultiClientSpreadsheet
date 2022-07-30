/**
 * @file file_list_test.cpp
 * @author Andrew Osterhout
 * @brief just a little test of how to use the file system tools
 * @version 0.1
 * @date 2021-04-14
 *
 * @copyright Copyright (c) 2021
 *
 */


#include <string>
#include <iostream>
#include <filesystem>
namespace fs = std::filesystem;

int main()
{
    std::string path = "/mnt/d/Google-Drive/School/UofU/2021-1_Spring/cs3505_SoftwarePractice-II/proj/server-repo/src";
    for (const auto& entry : fs::directory_iterator(path))
        std::cout << entry.path() << std::endl;

    std::cout << '\n' << fs::path("/foo/bar.txt").stem() << '\n'
        << fs::path("/foo/.bar").stem() << '\n';
    std::cout << '\n' << fs::path("/foo/bar.ss.json").stem() << '\n'
        << fs::path("/foo/.bar.ss.json").stem() << '\n';
}
