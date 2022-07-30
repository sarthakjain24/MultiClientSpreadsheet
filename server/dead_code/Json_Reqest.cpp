/**
 * @file Json_Reqest.cpp
 * @author Andrew Osterhout
 * @brief
 * @version 0.1
 * @date 2021-04-12
 *
 * @copyright Copyright (c) 2021
 *
 */

#include "Json_Request.hpp"
#include "debug_defs.hpp"



using json_t = nlohmann::json;
using namespace std;

namespace json
{
    Json_Request* readJson(string& j_str)
    {
        json_t j = json_t::parse(j_str);
        string reqType;
        string cellName;
        string contents;
        uid_t user_id;
        string user_name;

        j.at("requestType").get_to(reqType);

        switch (reqType[0])
        {
        case 'e': //for "editCell"
            j.at("cellName").get_to(cellName);
            j.at("contents").get_to(contents);
            //todo stuff for cell updates
            break;
        case 'r': //for "revertCell"
            j.at("cellName").get_to(cellName);
            //todo stuff for spreadsheet revert cell action.
            break;
        case 's': //for "selectCell"
            j.at("cellName").get_to(cellName);
            j.at("selector").get_to(user_id);
            j.at("selectorName").get_to(user_name);
            //todo stuff for cell selection (make sure info from client is added in listen loop)
            break;
        case 'u': //for "undo"
            j.at("cellName").get_to(cellName);
            //todo stuff for spreadsheet undo action
            break;
        default:
            //todo make a better error message, that includes what the requestType was.
            DEBUG_LOG("this shouldnever be seen !!" << "more disc");
            //thought throw an exception
            break;
        }
    }
}


