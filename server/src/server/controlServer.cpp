/**
 * @file controlServer.cpp
 * @author Andrew Osterhout, Jerry Zhang, VJ...
 * @brief
 * @version 0.1
 * @date 2021-04-19
 *
 * @copyright Copyright (c) 2021
 *
 */

#include <pthread.h>
#include <sstream>
#include <mutex>
#include <signal.h>
#include <boost/asio.hpp>

#include <filesystem>
#include <fstream>
namespace fs = std::filesystem;

#define __GET_SPREADSHEET_SAVE_INFO
#define __GET_DEFAULT_SPREADSHEET_FILE_INFO
#include "global_defs.hpp" 

#include "controlServer.hpp"
#include "debug_defs.hpp"

using namespace std;


namespace server
{

    /**
     * @brief How we keep track of what spreadsheet servers are active.
     */
    static std::unordered_map<std::string, SpreadsheetServer*> sheetServers;

    /**
     * @brief The thread id fro the control server
     */
    static pthread_t tid;

    /**
     * @brief The port number to use for the application.
     */
    static int port;

    /**
     * @brief Mutex for makign sure we don't double add a spreadsheet
     */
    static std::mutex sheets_lock;



    static void clearSheetServers();


    static inline bool f_exists(const string& name)
    {
        ifstream f(name/* .c_str() */);
        return f.good();
    }

    /**
     * @brief Does some basic stuff in regards to makign sure server runs smoothly.
     */
    static void init()
    {
        if (not f_exists(SPREADSHEET_SAVE_DIR DEFAULT_SPREADSHEET_FILE_NAME SPREADSHEET_FILE_EXT))
        {
            ofstream default_sheet(SPREADSHEET_SAVE_DIR DEFAULT_SPREADSHEET_FILE_NAME SPREADSHEET_FILE_EXT, ios::out);
            default_sheet << DEFAULT_SPREADSHEET_FILE_CONTENTS << endl;
            default_sheet.close();
        }
    }



    static void* controlServer(void* input)
    {
        DEBUG_LOG("  >> DEBUG :: we are in the main control server thread !! [server::controlServer]\n");

        boost::asio::io_context* io_context = new boost::asio::io_context();

        boost::asio::ip::tcp::acceptor* _acceptor = new boost::asio::ip::tcp::acceptor(*io_context,
            boost::asio::ip::tcp::endpoint(boost::asio::ip::tcp::v6(), port));
        networking::start_accept(receive_client_name, io_context, _acceptor);

        DEBUG_LOG("  >> DEBUG :: we returned after launching the TCP listener thread (listening on port: " << port << ") !! [server::controlServer]\n");
        io_context->run();
        DEBUG_LOG("  >> DEBUG :: `io_context->run();` has completed, thread is ending !! [server::controlServer]\n");
        return NULL;
    }


    static pthread_t garbage_tid;


    static void* garbageCollector(void* useless)
    {
        while (1)
        {
            sleep(5);
            sheets_lock.lock();
            for (auto p : sheetServers)
            {
                try
                {
                   //std::cout << "" << p.first << std::endl;
                    if (not p.second->is_running())
                    {
                        std::cout << "here " << p.first << std::endl;
                        std::cout << "address of spreadsheet server:  " << p.second << std::endl;
                        delete (p.second);
                        //sheetServers[p.first] = NULL;
                        sheetServers.erase(p.first);
                       
                            
                    }
                }
                catch (...)
                {
                }
            }
            sheets_lock.unlock();
        }
    }



    void start(const int _port)
    {
        port = _port;
        init();
        int error;
        if ((error = pthread_create(&tid, NULL, controlServer, NULL)) != 0 /*||
            (error = pthread_create(&garbage_tid, NULL, garbageCollector, NULL) != 0)*/) 
            DEBUG_LOG("  >> ERROR :: failed to launch main Control Server thread !!  [server::start]\n"
                "                Error Code: " << error << "\n"
                "                 Error Msg: " << strerror(error) << '\n' );
        // controlServer(NULL);
    }


    void stop()
    {
        pthread_kill(tid, SIGKILL);
        //pthread_kill(garbage_tid, SIGKILL);
        clearSheetServers();
    }


    void** wait()
    {
        void** ret_val;
        pthread_join(tid, ret_val);
        //pthread_kill(garbage_tid, SIGKILL);
        clearSheetServers();
        return ret_val;
    }



    /**
     * @brief A method that will safely stop and clear out all spreadsheet servers
     *          that are currently running.
     */
    static void clearSheetServers()
    {
        sheets_lock.lock();
        for (auto sheet : sheetServers)
        {
            sheetServers.erase(sheet.first);
            sheet.second->stop();
            delete sheet.second;
        }
        sheets_lock.unlock();
    }



    static void split(vector<string>& result, string& data, string& delimiter)
    {
        size_t pos = 0;
        string token;
        while ((pos = data.find(delimiter)) != string::npos)
        {
            token = data.substr(0, pos);
            result.push_back(token);
            data.erase(0, pos + delimiter.length());
        }
    }



    static void receive_sheet(networking::Client* client)
    {
        if (client->error_occured)
        {
            DEBUG_LOG("  >> ERROR :: exception occured !! [server::receive_sheet]\n"
                "  >>              client: \"" << client->get_name() << "\" (id: " << client->id << ")\n"
                "  >>             err msg:" << client->error_msg << '\n');
            delete client;
            return;
        }

        string sheetname = client->getdata();
        client->remove(0, sheetname.size());
        vector<string> res;
        string deli = "\n";
        split(res, sheetname, deli);

        if (res.size() >= 1)
        {
            //do some checking to see if there exists requested sheets
            DEBUG_LOG("  >> DEBUG :: Getting client spreadsheet request.   [server::receive_sheet]\n"
                "  >>                         client: \"" << client->get_name() << "\" (id: " << client->id << ")\n"
                "  >>           requested sheet name: " << res[0] << '\n');
            //send cellupdates here.... right now just empty
            SpreadsheetServer* server;
            sheets_lock.lock();
            auto search = sheetServers.find(res[0]);
            if (search == sheetServers.end())
            {
                DEBUG_LOG(">> LOG :: client requested non-open spreadsheet server, creating a new one with that name.   [server::receive_sheet]\n"
                    ">>                       client: \"" << client->get_name() << "\" (id: " << client->id << ")\n"
                    ">>         new spreadsheet name: " << res[0] << '\n');
                server = new SpreadsheetServer(res[0]);
                sheetServers[res[0]] = server;
                server->start();
                sheets_lock.unlock();
                server->addClient(client);
            }
            else
            {
                // make sure server is running before adding any clients 
                if (not (sheetServers[res[0]])->is_running())
                    (sheetServers[res[0]])->start();
                sheets_lock.unlock();
                (sheetServers[res[0]])->addClient(client);
            }
        }

        networking::async_receive(client);
    }



    static void receive_client_name(networking::Client* client)
    {
        if (client->error_occured)
        {
            delete client;
            DEBUG_LOG("  >> ERROR :: exception occured !! [server::receive_client_name]\n  >>        error msg: " << client->error_msg);
            return;
        }

        string username = client->getdata();
        client->remove(0, username.size());
        vector<string> res;
        string deli = "\n";
        split(res, username, deli);


        if (res.size() >= 1 && res[0] != "")
        {
            client->set_name(res[0]);
            DEBUG_LOG(">> LOG :: Client connected !!                   [server::receive_client_name]\n"
                ">>          client name: " << client->get_name() << "\n"
                ">>            client id: " << client->id << '\n');
            //send all spreadsheets existing in current directory
            stringstream spreadsheets;
            // sheets_lock.lock();
            for (auto& entry : sheetServers)
                spreadsheets << entry.first << '\n';
            for (auto& entry : fs::directory_iterator(SPREADSHEET_SAVE_DIR))
            {
                string file_name = entry.path().stem().string();
                if (sheetServers.find(file_name) == sheetServers.end())
                    spreadsheets << file_name << '\n';
            }
            // sheets_lock.unlock();
            spreadsheets << '\n'; //string("create new spreadsheet\n\n");
            networking::async_send(client, spreadsheets.str());
            //move onto next stage
            client->OnNetworkAction = receive_sheet;
        }

        networking::async_receive(client);
    }



    static void start_receive(networking::Client* client)
    {
        DEBUG_LOG("receiving client name...");
        client->OnNetworkAction = receive_client_name;
        networking::async_receive(client);
    }

}
