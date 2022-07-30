/**
 * @file main.cpp
 * @authors Andrew Osterhout, Jerry Zhang, VJ...
 *
 * @brief Where the application starts.
 *
 * @date 2021-04-09
 * @version 0.1
 */

#include <iostream>
#include <string>

#include "controlServer.hpp"
#include "debug_defs.hpp"

int main(int argc, char* argv[])
{
    int port;

    if (argc != 2)
    {
        std::cout << "you forgot port number" << std::endl;
        return -1;
    }

    std::cout << "launching the spreadsheet server..." << std::endl;

    //get port number 
    port = std::stoi(argv[1]);

    server::start(port);

    DEBUG_LOG("  >> DEBUG :: we returned after launching control server thread !! [main]\n");

    server::wait();
}
