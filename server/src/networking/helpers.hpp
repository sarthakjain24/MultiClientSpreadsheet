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


#ifndef _NETWORKING_HELPERS_HPP_
#define _NETWORKING_HELPERS_HPP_

#include <string>
#include <boost/asio.hpp>
#include <boost/bind/bind.hpp>
#include <iostream>

#include "Action.hpp"
#include "Client.hpp"

 /*Wrapper networking class using boost::asio for Server application*/

namespace networking
{

    void accept_handler(networking::Client* client,
        networking::Action action,
        boost::asio::io_context* io_context,
        boost::asio::ip::tcp::acceptor* _acceptor,
        const boost::system::error_code& e);

    void async_send_handler(Client* s,
        const boost::system::error_code& e,
        size_t bytes_transferred);

    void async_receive_handler(Client* s, std::vector<char>* temp,
        const boost::system::error_code& e,
        size_t bytes_transferred);


    //Async accept connections 
    void start_accept(networking::Action action,
        boost::asio::io_context* io_context,
        boost::asio::ip::tcp::acceptor* _acceptor);

    //Async send msg to State s
    void async_send(Client* s, std::string msg);

    //Async receive msg from s, and save to s's buffer
    void async_receive(Client* s);

}

#endif
