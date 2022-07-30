/**
 * @file controlServer.hpp
 * @author Andrew Osterhout, Jerry Zhang, VJ...
 * @brief The main logic of the application,
 *          it takes the form of non-member functions.
 * @version 0.1
 * @date 2021-04-19
 *
 * @copyright Copyright (c) 2021
 *
 */


#ifndef _CONTROL_SERVER_HPP_
#define _CONTROL_SERVER_HPP_

#include <string>
#include <unordered_map>
#include "SpreadsheetServer.hpp"
#include "Client.hpp"
#include "helpers.hpp"


namespace server
{

    /**
     * @brief Launch the main control server (in another thread).
     *        Return a pointer to the thread used,
     *          so that the parent thread (preferable the main thread)
     *          can monitor it and eventually end it.
     * @param port The port number it should use for comunication.
     * @return \c pthread_t - The thread that will be launched.
     */
    void start(const int port);

    /**
     * @brief Stop the control server and all child servers.
     */
    void stop();

    /**
     * @brief Wait for the control server to end.
     * @return \c void** - nothing as fo right now, just \c NULL
     */
    void** wait();


    /**
     * @brief The main body of the control server.
     *        Launched from a separate thread.
     * @param input pointer to the input that will be passed from the thread.
     * @return \c void* - whatever it will return.
     */
    static void* controlServer(void* input);


    /**
     * @brief temp... (REMOVE POSSIBLY?)
     * @param result 
     * @param data 
     * @param delimiter 
     */
    static void split(std::vector<std::string>& result, std::string& data, std::string& delimiter);


    /**
     * @brief Network action to use for receiving which sheet to open.
     * @param client The Client
     */
    static void receive_sheet(networking::Client* client);


    /**
     * @brief Get the client name and send the list of spreadsheets that can be opened.
     * @param client The Client
     */
    static void receive_client_name(networking::Client* client);


    /**
     * @brief 
     * @param client The Client
     */
    static void start_receive(networking::Client* client);

}

#endif
