/**
 * @file network_helper.hpp
 * @author Jerry Zhang, Andrew Osterhout
 * @brief collection of funtions that help with networking.
 * @version 0.1
 * @date 2021-04-14
 *
 * @copyright Copyright (c) 2021
 *
 */

#include "helpers.hpp"

#define __GET_CLIENT_CONSTANTS
#include "global_defs.hpp"
#include "debug_defs.hpp"


void networking::start_accept(networking::Action action,
    boost::asio::io_context* io_context,
    boost::asio::ip::tcp::acceptor* _acceptor)
{
    networking::Client* client = new networking::Client(io_context);
    client->OnNetworkAction = action;
    try
    {
        _acceptor->async_accept(client->get_socket(),
            boost::bind(
                &networking::accept_handler, client,
                action, io_context, _acceptor,
                boost::asio::placeholders::error)
        );
    }
    catch (...)
    {
        DEBUG_LOG( "  >> ERROR :: error in start_accept !!  [networking::start_accept]\n" );
    }

}



void networking::accept_handler(networking::Client* client, networking::Action action,
    boost::asio::io_context* io_context,
    boost::asio::ip::tcp::acceptor* _acceptor,
    const boost::system::error_code& e)
{
    DEBUG_LOG(">> LOG :: new client has connected !!  [networking::accept_handler]\n");
    
    try
    {
        if (e)
        {
            DEBUG_LOG( "  >> ERROR :: error in accepting socket connection !!  [networking::accept_handler]\n");
        }
        DEBUG_LOG( "  >> DEBUG :: success !! socket connection made !!  [networking::accept_handler]\n" );

        client->OnNetworkAction(client);
        //start accept connections again
        start_accept(action, io_context, _acceptor);
    }
    catch (...)
    {
        client->error_occured = true;
        client->error_msg = e.message();
        DEBUG_LOG( "  >> ERROR :: error in accepting socket connection !!  [networking::accept_handler]\n");
    }

}



void networking::async_send(Client* client, std::string msg)
{
    try
    {
        client->get_socket().async_send(
            boost::asio::buffer(msg, msg.size()),
            boost::bind(&networking::async_send_handler, client,
                boost::asio::placeholders::error,
                boost::asio::placeholders::bytes_transferred)
        );
    }
    catch (boost::system::system_error& e)
    {
        // DEBUG_LOG( "  >> ERROR :: Boost system error occured in async send !!                 [networking::async_send]\n"
        //                "  >>           client: \"" << client->get_name() << "\" (id: " << client->id << ")\n"
        //                "  >>        error msg: " << e.what() << '\n' );
    }

}



void networking::async_send_handler(Client* client,
    const boost::system::error_code& e,
    size_t bytes_transferred)
{

    try
    {
        if (e)
        {
            DEBUG_LOG( "  >> ERROR :: BOOST error in send handler !!                 [networking::async_send_handler]\n"
                       "  >>                 client: \"" << client->get_name() << "\" (id: " << client->id << ")\n"
                       "  >>        boost error msg: " << e.message() << '\n' );
            throw e;
        }
    }
    catch (...)
    {
        DEBUG_LOG( "  >> ERROR :: error in send handler !!    [networking::async_send_handler]\n"
                    "  >>                 client: \"" << client->get_name() << "\" (id: " << client->id << ")\n" );
    }

}



/*
 * Async receive data from a Client (represent a client)
 * and store data into inner buffer, which can be retrived
 * from getdata method.
 *
 * It will also perform Client's OnNetworkAction whenever
 * data is received.
 */
void networking::async_receive(Client* client)
{
    try
    {
        std::vector<char>* temp = new std::vector<char>(CLIENT_BUFFER_SIZE);

        client->get_socket().async_receive(boost::asio::buffer(*temp),
            boost::bind(
                &networking::async_receive_handler, client, temp,
                boost::asio::placeholders::error,
                boost::asio::placeholders::bytes_transferred)
        );
    }
    catch (...)
    {

        DEBUG_LOG( "  >> ERROR :: error in accepting socket connection !!          [networking::async_receive]\n"
                "  >>                 client: \"" << client->get_name() << "\" (id: " << client->id << ")\n");
    }

}

/*
 * This method will be called if one of following is satisfied:
 * 1. Buffer is filled (BUFFER_SIZE bytes of data)
 * 2. Error occurs (Most likely end_of_file)
 */
void networking::async_receive_handler(Client* client, std::vector<char>* msg,
    const boost::system::error_code& e,
    size_t bytes_transferred)
{

    try
    {
        if (e)
        {
            // DEBUG_LOG(  "  >> ERROR :: boost error in accepting socket connection !!          [networking::async_receive_handler]\n"
            //             "  >>                 client: \"" << client->get_name() << "\" (id: " << client->id << ")\n"
            //             "  >>            Boost Error Msg: " << e.message() << '\n');
            client->error_occured = true;
            client->error_msg = e.message();
            client->OnNetworkAction(client);
            return;
        }

        client->append(*msg);
        delete msg;
        client->OnNetworkAction(client);
    }
    catch (std::exception& ex)
    {
        client->error_occured = true;
        client->error_msg = ex.what();
        client->OnNetworkAction(client);
    }
    catch (...)
    {
        client->error_occured = true;
        client->error_msg = e.message();
        client->OnNetworkAction(client);
        DEBUG_LOG( "\n  >> ERROR :: Unknown Exception Occured !!            [networking::async_receive_handler]\n"
                    "  >>                 client: \"" << client->get_name() << "\" (id: " << client->id << ")\n" );
    }

}