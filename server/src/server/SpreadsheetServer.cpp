/**
 * @file SpreadsheetServer.cpp
 * @author Andrew Osterhout, Jerry Zhang, VJ
 * @brief ...
 * @version 0.1
 * @date 2021-04-19
 *
 * @copyright Copyright (c) 2021
 *
 */

#define _SPREADSHEET_SERVER_CPP_
#include "SpreadsheetServer.hpp"

#include <sstream>
#include <ctime>
#include <algorithm>
#include <pthread.h>
#include <functional>

#define __GET_SAVE_CYCLE_INFO
#include "global_defs.hpp"

#include "helpers.hpp"

using namespace std;

namespace server
{
    // static pthread_t tid;

    SpreadsheetServer::SpreadsheetServer(string& name)
        : name(name), sheet(new spreadsheet::Spreadsheet(name)),
        jobs(), msgs_out(), status(false),
        jobs_lock()
    {}


    SpreadsheetServer::~SpreadsheetServer()
    {
        sheet->save();  // might be overkill since `stop()` should be called first
        while (not jobs.empty())
        {
            delete jobs.front();
            jobs.pop();
        }
        while (not msgs_out.empty())
        {
            delete msgs_out.front();
            msgs_out.pop();
        }
        
            for (auto client : clients)
                delete client;
        
        
        delete sheet;
    }




    void* event_loop_thread_launch_helper(void* t)
    {
        ((SpreadsheetServer*)t)->event_loop();
        return NULL;
    }


    void* send_loop_thread_launch_helper(void* t)
    {
        ((SpreadsheetServer*)t)->send_loop();
        return NULL;
    }




    void SpreadsheetServer::start()
    {
        // don't double start
        if (is_running())
            return;

        // actually start
        int error;
        if ((error = pthread_create(&event_tid, NULL, event_loop_thread_launch_helper, (void*)this)) != 0 ||
            (error = pthread_create(&send_tid, NULL, send_loop_thread_launch_helper, (void*)this)) != 0)
            DEBUG_LOG("  >> ERROR :: failed to launch main Control Server thread !!  [server::SpreadsheetServer::start]\n"
                "                Error Code: " << error << "\n"
                "                 Error Msg: " << strerror(error) << '\n');
        status_lock.lock();
        this->status = true; //.store(true, memory_order_relaxed);
        status_lock.unlock();
    }




    void SpreadsheetServer::addClient(networking::Client* client)
    {
        // check if server is running.
        if (not is_running())
        {
            DEBUG_LOG("  >> ERROR :: Tried to add client to a server that is not running !!     [server::SpreadsheetServer::adddClient]\n"
                      "  >>                       client: \"" << client->get_name() << "\" (id: " << client->id << ")\n"
                      "  >>                 spreadsheet name: " << name << '\n');
            networking::async_send(client, json::Error_Message_Server_Shutdown("ERROR :: 404 :: This Spreadsheet Server is not running !!").get_json());
            delete client;  // might cause issues but hopefully closing socket in constructor should help.
            exit(0);
        }

        // else add client
        stringstream builder;
        for (auto cell : *(sheet->get_data()))
            builder << json::Cell_Update_Message(cell.first, cell.second).get_json() << '\n';

        builder << client->id << '\n';

        clients_lock.lock();
        for (auto client : clients)
            builder << json::Cell_Selected_Message(client->selected_cell, client->id, client->get_name()).get_json() << '\n';
        clients_lock.unlock();

        networking::async_send(client, builder.str());

        clients_lock.lock();
        clients.insert(client);
        clients_lock.unlock();

        client->OnNetworkAction = this->proccess_client_requests;

        DEBUG_LOG(">> LOG :: Client was added to the spreadsheet server !!       [server::SpreadsheetServer::addClient]\n"
            ">>                       client: \"" << client->get_name() << "\" (id: " << client->id << ")\n"
            ">>             spreadsheet name: " << name << '\n');

        // json_t* j = new json_t({{"requestType","selectCell"}, 
        //                     {"cellName",client->selected_cell}, 
        //                     {"selector",client->id},
        //                     {"selectorName", client->get_name()}});
        // jobs_lock.lock();
        // jobs.push(j);
        // jobs_lock.unlock();
    }



    json::Disconnected_Message* SpreadsheetServer::disconnect(networking::Client* client)
    {

        if (clients.find(client) == clients.end())
            return NULL;


        clients_lock.lock();
        clients.erase(client);
        clients_lock.unlock();

        int id = client->id;
        delete client;
        return new json::Disconnected_Message(id);
    }



    void SpreadsheetServer::event_loop()
    {
        time_t t, tt, last_save, curr;
        int request_cycle = 0;
        bool jobs_empty, clients_empty;
        time(&last_save);

        // stall until things start happening
        time(&t);
        time_t ttt = t + TIME_OUT_TIME;
        do {
            sleep(1);
            time(&tt);
            if (tt >= ttt)
                stop();
            if (event_tid == 0)    // if told externally to stop running, exit loop.
                return;
            jobs_lock.lock();
            jobs_empty = jobs.empty();
            jobs_lock.unlock();
        } while (jobs_empty);

        while (event_tid)
        {
            // Note: save should be upfront, if this piece of code being put at the end
            // then there will be extra implicit if conditions from the while loop
            time(&curr);
            if ((request_cycle++ >= SAVE_CYCLE_COUNT && curr >= last_save + AUTO_SAVE_MIN_DELAY) || (curr >= last_save + AUTO_SAVE_MAX_DELAY))
            {
                sheet->save();
                time(&last_save);
                request_cycle = 0;
            }

            // wait to make sure at least 1 second has gone by before processign again.
            // you can try reducing `PROCESS_CYCLE_DELAY` if things are laggy, but we should have some delay between checks.
            // time(&t);
            do
            { /* <<    SPIN / BUSY LOOP   >> */
                clients_lock.lock();
                clients_empty = clients.empty();
                clients_lock.unlock();
                if (clients_empty)    // Check to see if server should shut down
                    stop();
                if (event_tid == 0)    // if told externally to stop running, exit loop.
                    return;
                jobs_lock.lock();
                jobs_empty = jobs.empty();
                jobs_lock.unlock();
            } while (jobs_empty /* || t + PROCESS_CYCLE_DELAY > time(nullptr) */);

            // ----- process jobs from the queue... ----//
            jobs_lock.lock();
            json_t* job = jobs.front();
            jobs.pop();
            jobs_lock.unlock();

            json::Json_Message* msg = process_job(job);
            delete job;

            //put messages in the msg_out queue, which
            //will be sent out
            if (msg != NULL)
            {
                msgs_out_lock.lock();
                msgs_out.push(msg);
                msgs_out_lock.unlock();
            }

            // DEBUG_LOG(">> LOG :: Finished one Event Loop !!\n");

        }
    }



    json::Json_Message* SpreadsheetServer::process_job(json_t* job)
    {
        string reqType;
        string cellName;
        string contents;
        uid_t user_id;
        string user_name;
        spreadsheet::HistoryNode* h;
        json::Json_Message* msg;

        // DEBUG_LOG("  >> DEBUG :: Processing a job !!                             [server::SpreadsheetServer::process_job]\n"
        //           "  >>           spreadsheet name: " << name << "\n"
        //           "  >>         job content (json): " << *job << '\n');

        try
        {
            job->at("requestType").get_to(reqType);
            //a guard here reqType[0] might be null or empty
            switch (reqType[0])
            {
            case 'e': //for "editCell"
                job->at("cellName").get_to(cellName);
                job->at("contents").get_to(contents);

                if (sheet->update(cellName, contents))
                    return new json::Cell_Update_Message(cellName, contents);
                else
                {
                    job->at("clientId").get_to(user_id);
                    return handle_bad_request(user_id, cellName, "ERROR :: Unknown Exception Occured when processing request !!\n"
                        "  spreadsheet name: " + name + "\n"
                        "         from cell: " + cellName + '\n' +
                        "      with formula: " + contents);
                }
                break;

            case 'r': //for "revertCell"
                job->at("cellName").get_to(cellName);

                h = sheet->revert(cellName);

                if (h == NULL)
                {
                    job->at("clientId").get_to(user_id);                    
                    return handle_bad_request(user_id, cellName, "ERROR :: Nothing to REVERT in specified cell !!\n"
                        "  spreadsheet name: " + name + "\n"
                        "         cell name: " + cellName + '\n');
                }

                msg = new json::Cell_Update_Message(h->cell, h->old);

                delete h;
                return msg;
                break;

            case 's': //for "selectCell"
                job->at("cellName").get_to(cellName);
                job->at("clientId").get_to(user_id);
                job->at("selectorName").get_to(user_name);

                return new json::Cell_Selected_Message(cellName, user_id, user_name);
                break;

            case 'u': //for "undo"

                h = sheet->undo();
                if (h == NULL)
                {
                    job->at("clientId").get_to(user_id);                    
                    return handle_bad_request(user_id, "", "ERROR :: Nothing to UNDO !!\n"
                        "  spreadsheet name: " + name);
                }
                msg = new json::Cell_Update_Message(h->cell, h->old);

                delete h;
                return msg;
                break;

            default:
                //todo make a better error message, that includes what the requestType was.
                DEBUG_LOG("\n  >> ERROR :: This should never be seen unless we get a bad JSON Message w/ a bad \"requestType\" !!      [server::SpreadsheetServer::process_job]\n"
                    "  >>           spreadsheet name: " << name << "\n"
                    "  >>           json value: " << *job << '\n');
                //thought throw an exception
                break;
            }
        }
        catch (spreadsheet::CircularDependencyException& cdex)
        {
            job->at("clientId").get_to(user_id);
            return handle_bad_request(user_id, cellName, "ERROR :: CircularException !!\n"
                        "  spreadsheet name: " + name + "\n"
                        "         from cell: " + cdex.updatedCell + '\n' +
                        "      with formula: " + cdex.updatedContent);
        }
        catch (std::exception& ex)
        {
            DEBUG_LOG("  >> ERROR :: Exception occured prossesing client request/job (likely json lib) !!       [server::SpreadsheetServer::process_job]\n"
                "  >>           spreadsheet name: " << name << "\n"
                "  >>            exception: " << ex.what() << "\n"
                "  >>           json value: " << *job << '\n');
        }
        catch (...)
        {
            DEBUG_LOG("  >> ERROR :: Exception occured prossesing client request/job (likely json lib) !!       [server::SpreadsheetServer::process_job]\n"
                "  >>           spreadsheet name: " << name << "\n"
                "  >>           json value: " << *job << '\n');
        }
        return NULL;
    }


    json::Json_Message* SpreadsheetServer::handle_bad_request(const uid_t user_id, const std::string& cellName, const std::string& msg)
    {
        string err = json::Error_Message_Invalid_Request(cellName, msg).get_json();
        try {
            clients_lock.lock();
            for (auto client : clients)
                if (client->id == user_id)
                {
                    networking::async_send(client, err);
                    break;
                }
            clients_lock.unlock();
        }
        catch (...)
        {}
        return NULL;
    }



    void SpreadsheetServer::send_loop()
    {
        time_t t;
        bool msgs_out_empty;


        while (1)
        {
            // wait to make sure at least 1 second has gone by before processign again.
            //? comment out wait if things are laggy
            time(&t);
            do
            { /* <<    SPIN / BUSY LOOP    >> */
                if (send_tid == 0)  // if told externally to stop running, exit loop.
                    return;
                msgs_out_lock.lock();
                msgs_out_empty = msgs_out.empty();
                msgs_out_lock.unlock();
            } while (msgs_out_empty || t + PROCESS_CYCLE_DELAY > time(nullptr)); /* spin */

            // - do the send loop ----
            msgs_out_lock.lock();
            json::Json_Message* msg = msgs_out.front();
            msgs_out.pop();
            msgs_out_lock.unlock();

            DEBUG_LOG("  >> DEBUG :: Sending message to clients !!           [server::SpreadsheetServer::send_loop]\n"
                "  >>           spreadsheet name: " << name << "\n"
                "  >>         msg content (json): " << msg->get_json() << '\n');


            // - send to all clients ----
            clients_lock.lock();
            for (auto client : clients)
                networking::async_send(client, msg->get_json() + '\n');
            clients_lock.unlock();
            delete msg;
        }
    }



    void SpreadsheetServer::stop()
    {
        // don't double stop
        if (not is_running())
            return;

        // set server status to not running
        status_lock.lock();
        this->status = false;//.store(true, memory_order_relaxed);
        status_lock.unlock();

        // let clients still connected know that the server is shutting down.
        clients_lock.lock();
        bool clients_empty = clients.empty();
        clients_lock.unlock();
        if (!clients_empty)
        {
            msgs_out_lock.lock();
            msgs_out.push(new json::Error_Message_Server_Shutdown("MESSAGE :: This Spreadsheet Server is shutting down !!"));
            msgs_out_lock.unlock();
        }

        void* useless;
        pthread_t me = pthread_self(), temp = event_tid;
        // wait for main event loop to finish processing it's current job
        event_tid = 0;
        if (temp != 0 && temp != me)
            pthread_join(temp, &useless);
        jobs_lock.lock();
        while (not jobs.empty())
        {
            json_t* job = jobs.front();
            jobs.pop();
            delete job;
        }
        jobs_lock.unlock();

        // now that we know it is safe to interact with the spreadsheet: save it.
        sheet->save();

        // wait for send thread to finish sending all of it's messages.
        temp = send_tid;
        send_tid = 0;
        if (temp != 0 && temp != me)
            pthread_join(temp, &useless);
        msgs_out_lock.lock();
        while (not msgs_out.empty())
        {
            json::Json_Message* msg = msgs_out.front();
            msgs_out.pop();
            delete msg;
        }
        msgs_out_lock.unlock();

        // if clients are still connected disconect them
        clients_lock.lock();
        
            for (auto client : clients)
                delete client;  // the deconstructor for `networking::Client` should close the socket, disconnecting them.
        
        clients.clear();
        clients_lock.unlock();

    }



    bool SpreadsheetServer::is_running()
    {
        status_lock.lock();
        bool temp = this->status;
        status_lock.unlock();
        return temp; // .load(memory_order_relaxed);
    }




    static void split(vector<string>& result, string& data, char delimiter)
    {
        size_t pos = 0;
        string token;
        while ((pos = data.find(delimiter)) != string::npos)
        {
            token = data.substr(0, pos);
            result.push_back(token);
            data.erase(0, pos + 1);
        }
    }

}
