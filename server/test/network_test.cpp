#include<iostream>
#include<string>
#include"../src/server/Server.hpp" 
#include<boost/asio.hpp> 

int main(int argc, char* argv[])
{
    int port;

    if (argc != 2)
    {
        std::cout << "you forgot port number" << std::endl;
        return -1;
    }

    //get port number 
    port = std::stoi(argv[1]);

    //running server
    try
    {
        boost::asio::io_context* io_context = new boost::asio::io_context();

        SS::Server server(io_context, port);

        std::cout << "server have started..." << std::endl;

        io_context->run();

    }
    catch (std::exception& e)
    {
        std::cerr << e.what() << std::endl;
    }

    return 0;
}