/**
 * @file Client.cpp
 * @author Jerry Zhang, Andrew Osterhout
 * @brief
 * @version 0.1
 * @date 2021-04-14
 *
 * @copyright Copyright (c) 2021
 *
 */

#include "Client.hpp"

#define __GET_CLIENT_CONSTANTS
#include "global_defs.hpp"

namespace networking
{

    uid_t Client::next_id = 0U;



    Client::Client(boost::asio::io_context* io_context)
        : _socket(*io_context), _buffer(CLIENT_BUFFER_SIZE), error_occured(false),
        id(Client::next_id), name(CLIENT_DEFAULT_DISPLAY_NAME_TEMPLATE + std::to_string(Client::next_id))
    {
        Client::next_id++;
    }


    Client::~Client()
    {}



    boost::asio::ip::tcp::socket& Client::get_socket()
    {
        return _socket;
    }



    void  Client::append(std::vector<char>& message)
    {
        m.lock();
        this->_buffer.insert(this->_buffer.end(), message.begin(), message.end());
        m.unlock();
    }



    std::string Client::getdata()
    {
        m.lock();
        std::string data(this->_buffer.begin(), this->_buffer.end());
        m.unlock();
        return data;
    }



    void Client::remove(int start, int length)
    {
        if (_buffer.size() < start + length)
            throw "Bad Remove";
        m.lock();
        this->_buffer.erase(this->_buffer.begin() + start, this->_buffer.begin() + length);
        m.unlock();
    }



    std::string Client::get_name() const
    {
        return this->name;
    }



    void Client::set_name(const std::string& display_name)
    {
        if (this->name == CLIENT_DEFAULT_DISPLAY_NAME_TEMPLATE + std::to_string(id))
            this->name = display_name;
    }

}
