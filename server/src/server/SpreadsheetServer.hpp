/**
 * @file SpreadsheetServer.hpp
 * @author Andrew Osterhout, Jerry Zhang, VJ...
 * @brief A class object that represents a spreadsheet server.
 *        With connections to clients and a spreadsheet model,
 *          that is mostly agnostic to being on a server.
 *
 * @version 0.1
 * @date 2021-04-19
 *
 * @copyright Copyright (c) 2021
 */


#ifndef _SPREADSHEET_SERVER_HPP_
#define _SPREADSHEET_SERVER_HPP_


#include <set>
#include <queue>
#include <mutex>
#include <vector>
#include <string>
#include <exception>

#include "nlohmann-json/json.hpp"
using json_t = nlohmann::json;

#include "Spreadsheet.hpp"
#include "Json_Message.hpp"
#include "Client.hpp"
#include "Action.hpp"
#include "helpers.hpp"
#include "debug_defs.hpp"




namespace server
{


    static void split(std::vector<std::string>& result, std::string& data, char delimiter);

    /**
     * @brief A class object that represents a spreadsheet server.
     *        With connections to clients and a spreadsheet model,
     *          that is mostly agnostic to being on a server.
     */
    class SpreadsheetServer
    {

        // << ====================================================================== >> 
        // <<                           MEMBER VARIABLES                             >> 
        // << ====================================================================== >> 

        // << ================= Public Constant Member Variables =================== >> 
    public:

        /**
         * @brief The name of the spreadsheet this server manages.
         */
        const std::string name;

        // << ===================== Private Member Variables ======================= >> 
    private:

        /**
         * @brief Thread (id) for the main logic section of the server.
         */
        pthread_t event_tid;

        /**
         * @brief Thread (id) for the thread that sends out the messages to the clients.
         */
        pthread_t send_tid;

        /**
         * @brief \c bool if the server is running or not.
         */
        bool status;

        /**
         * @brief A \c std::set that is being used as a list of unique elements.
         *        For the purpose of efficiently adding, checking existence, and removing clients
         *          as they connect and discconect.
         */
        std::set<networking::Client*> clients;

        /**
         * @brief A \c spreadsheet::Spreadsheet that is the functional spreadsheet that clients interact with.
         */
        spreadsheet::Spreadsheet* sheet;

        /**
         * @brief A \c std::queue that contains the \em complete messages from the clients,
         *          in the order they were received and finished initial processing on the server.
         */
        std::queue<json_t*> jobs;

        /**
         * @brief A \c std:queue that contains \c json::Json_Message 's that will be sent out to all connected clients.
         */
        std::queue<json::Json_Message*> msgs_out;


        /**
         * @brief Friend function \n 
         *        Just a "middle man" to allow us to launch a thread with the 
         *          \c server::SpreadsheetServer::event_loop() member function, 
         *          using the standard C threading lib.
         * @param t really \c this:server::SpreadsheetServer* , but passed via C's threading lib.
         * @return \c void* - always \c NULL
         */
        friend void* event_loop_thread_launch_helper(void* t);

        /**
         * @brief Friend function \n 
         *        Just a "middle man" to allow us to launch a thread with the 
         *          \c server::SpreadsheetServer::send_loop() member function, 
         *          using the standard C threading lib.
         * @param t really \c this:server::SpreadsheetServer* , but passed via C's threading lib.
         * @return \c void* - always \c NULL
         */
        friend void* send_loop_thread_launch_helper(void* t);


        // << ========================== Mutex Variables =========================== >> 
    private:

        /**
         * @brief The mutex used to lock when editing the jobs queue
         */
        std::mutex jobs_lock;

        /**
         * @brief The mutex used to lock when editing the \c msgs_out queue
         */
        std::mutex msgs_out_lock;

        /**
         * @brief The mutex to use when doing anything too serous with the set of Clients connected to the server.
         */
        std::mutex clients_lock;

        /**
         * @brief The mutex to use when doing anything too serous with the set of Clients connected to the server.
         */
        std::mutex status_lock;


        /**
         * @brief A functional/delegate/function-pointer 
         *          used to allow the spreadsheet to remove itself from
         *          the main control server.
         *        ( \b NOTE: must be passed in with the constructor)
         */
        // std::function<void(const std::string& name)> remove_me; 



        // << ====================================================================== >> 
        // <<                     CONSTRUCTORS / DECONSTRUCTORS                      >> 
        // << ====================================================================== >> 
    public:

        /**
         * @brief \b Constructor: \n Construct a new \c Spreadsheet Server object.
         * @param name the name of the spreadsheet to open.
         * @param remove_me A functional/delegate used to allow this spreadsheet to remove itself from the main control server.
         */
        SpreadsheetServer(std::string& name);

        /**
         * @brief \b Destructor: \n Destroy this \c Spreadsheet Server object.
         */
        ~SpreadsheetServer();



        // << ====================================================================== >> 
        // <<                           MEMBER FUNCTIONS                             >> 
        // << ====================================================================== >> 

        // << ====================== Public Member Functions ======================= >> 
    public:

        /**
         * @brief Start whatever logical loops that need to happen.
         *        \b NOTE: this function will launch a new thread internally before returning.
         */
        void start();


        /**
         * @brief Registers a new \c networking::Client to this \c server::SpreadsheetServer.
         *        It will also register the appropriate \c networking::Action .
         */
        void addClient(networking::Client* client);


        /**
         * @brief Stop the server and all of it's (other) threads, 
         *      then remove server from \c server::controlServer w/ 
         *      \c server::stopSS(std::string name) method. \n  
         *      ( \b NOTE: if called externally spreadsheet might save improperly,
         *          if requests are being processed.)
         */
        void stop();


        /**
         * @brief Returns true if server is currently running.
         * @return \c bool - if server is running or not.
         */
        bool is_running();


        // << ================== Private Helper Member Functions =================== >> 
    private:

        /**
         * @brief Remove a client from the spreadsheet server,
         *          and return a \c json::Disconnected_Message* to send to all other clients. \n
         *        Called after the socket disconnects,
         *          on either send or receive. \n
         *        ( \b NOTE: you are responsible for deleting the \c json::Disconnected_Message after receiving it.)
         * @param client pointer to the client object
         * @return \c json::Disconnected_Message* - A message to send to all clients still connected to this \c spreadsheet::SpreadsheetServer . \n
         *                                          ( \b NOTE: you are responsible for deleting this after receiving it.)
         */
        json::Disconnected_Message* disconnect(networking::Client* client);


        /**
         * @brief The main logic for the Server.
         */
        void event_loop();


        /**
         * @brief The sendign of things in the \c msgs_out queue.
         */
        void send_loop();


        /**
         * @brief do the actual logic from \c event_loop() .
         * @param job the json request.
         */
        json::Json_Message* process_job(json_t* job);


        /**
         * @brief just make code cleaner by segmenting it to method.
         * @param msg the message contents
         * @return \c void* - always NULL;
         */
        json::Json_Message* handle_bad_request(const uid_t user_id, const std::string& cellName, const std::string& msg);



        // << ====================================================================== >> 
        // <<                      NETWORK ACTION FUNCTIONALS                        >> 
        // << ====================================================================== >> 
    protected:

#ifdef _SPREADSHEET_SERVER_CPP_

        /**
         * @brief The network Action that will handle adding json request objects to the jobs queue.
         */
        const networking::Action proccess_client_requests = [&, this](networking::Client* client) {
            using namespace std;

            if (client->error_occured)
            {
                // DEBUG_LOG(  "  >> ERROR :: exception occured !!                 [server::SpreadsheetServer::proccess_client_requests]\n"
                //             "  >>                     client: \"" << client->get_name() << "\" (id: " << client->id << ")\n"
                //             "  >>           spreadsheet name: " << name << "\n"
                //             "  >>            error msg: " << client->error_msg << '\n');
                json::Disconnected_Message* disc_msg = disconnect(client);
                msgs_out_lock.lock();
                msgs_out.push(disc_msg);
                msgs_out_lock.unlock();
                return;
            }

            

            string msg = client->getdata();
            
            // DEBUG_LOG("    >> TEMP DEBUG :: Message contents form client buffer !!     [server::SpreadsheetServer::proccess_client_requests]\n"
            //                 "    >>                     client: \"" << client->get_name() << "\" (id: " << client->id << ")\n"
            //                 "    >>           spreadsheet name: " << name << "\n"
            //                 "    >>                        msg: " << msg << '\n');
            vector<string> res;
            split(res, msg, '\n');
            
            for (string & j_str : res)
            {
                try
                { 
                    // if (j_str[0] == '}')
                    // {
                    //     j_str.erase(j_str.begin()+1, j_str.end());
                    //     client->remove(0,1);
                    // }
                    if (j_str != "" /* && j_str[j_str.size()-1] == '}' */)
                    {
                        string type;
                        json_t* j = new json_t();
                        try
                        {
                            for (auto &c : j_str)
                                if (c == 0)
                                    c = ' ';
                            *j = json_t::parse(j_str);
                        }
                        catch(const std::exception& ex)
                        {

                            delete j;
                            DEBUG_LOG("  >> ERROR :: Failed to parse JSON (BAD JSON FORMATTING) recoverable !!         [server::SpreadsheetServer::proccess_client_requests]\n"
                                "  >>                     client: \"" << client->get_name() << "\" (id: " << client->id << ")\n"
                                "  >>           spreadsheet name: " << name << "\n"
                                "  >>                  error msg: " << ex.what() << "\n"
                                "  >>                 first char: " << (int)j_str[0] << "\n"
                                "  >>              data received: " << j_str << '\n');
                            break;
                        }
                        
                        // (*j)["client"] = (uint64_t)client;    //! THIS IS JANK !!!
                        (*j)["clientId"] = client->id;    //! THIS IS LESS JANK !!!
                        j->at("requestType").get_to(type);
                        if (type == "selectCell")
                        {
                            // (*j)["selector"] = client->id;
                            (*j)["selectorName"] = client->get_name();
                            client->selected_cell = j->at("cellName");
                        }
                        DEBUG_LOG("  >> DEBUG :: processing some data from the client !!             [server::SpreadsheetServer::proccess_client_requests]\n"
                                "  >>                     client: \"" << client->get_name() << "\" (id: " << client->id << ")\n"
                                "  >>           spreadsheet name: " << name << "\n"
                                "  >>          complete json msg: " << *j << '\n');
                        jobs_lock.lock();
                        jobs.push(j);
                        jobs_lock.unlock();
                        client->remove(0, j_str.size()+1);
                    }
                    else
                        DEBUG_LOG("  >> DEBUG :: processing some data from the client (NOT a valid json msg, yet) !!             [server::SpreadsheetServer::proccess_client_requests]\n"
                                "  >>                     client: \"" << client->get_name() << "\" (id: " << client->id << ")\n"
                                "  >>           spreadsheet name: " << name << '\n');
                }
                catch (const std::exception& ex)
                {
                    if (ex.what() == "Bad Remove")
                        throw;
                    // else probs json lib and the stuff is improperly formatted
                    DEBUG_LOG("  >> ERROR :: exception occured (most likely with json lib stuff) !!               [server::SpreadsheetServer::proccess_client_requests]\n"
                                "  >>                     client: \"" << client->get_name() << "\" (id: " << client->id << ")\n"
                                "  >>           spreadsheet name: " << name << "\n"
                                "  >>                  error msg: " << ex.what() << "\n"
                                "  >>              data received: " << j_str << '\n');
                }
            }

            networking::async_receive(client);
        };


#else

        /**
         * @brief The network Action that will handle adding json request objects to the jobs queue.
         */
        const networking::Action proccess_client_requests;

#endif

        };

    }

#endif
