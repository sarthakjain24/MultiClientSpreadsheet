#ifndef _NETWORKING_CLIENT_HPP_
#define _NETWORKING_CLIENT_HPP_


#include<string>
#include<boost/asio.hpp>
#include<boost/bind/bind.hpp>

#include "Action.hpp"

namespace networking
{

    class Client
    {
    private:
        boost::asio::ip::tcp::socket _socket;
        std::vector<char> _buffer;
        std::string name;
        static uid_t next_id;
        

    public:
        const uid_t id;

        bool error_occured = false;
        std::string error_msg;
        std::string selected_cell;

        std::mutex m;

        Action OnNetworkAction;

        Client(boost::asio::io_context* io_context);
        ~Client();

        boost::asio::ip::tcp::socket& get_socket();

        //append data to the buffer thread safe 
        void append(std::vector<char>& message);

        //return string data thread safe
        std::string getdata();

        //remove buffer data thread safe
        void remove(int start, int length);

        /**
         * @brief Get the display name of this client.
         * @return \c std::string - the display name of the client.
         */
        std::string get_name() const;

        /**
         * @brief Set the display name of this client. \n 
         *        This can/should only be called once per \c networking::Client , 
         *          after receiving the preferred display name from a client. \n 
         *        ( \b NOTE: the constructor assigns a default name in case this never gets called.)
         * @param display_name the name the Client would like to display to identify them.
         */
        void set_name(const std::string& display_name);

    };


}

#endif