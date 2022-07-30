#include "Json_Message.hpp"

using json_t = nlohmann::json;
using namespace std;


namespace json
{

    Json_Message::Json_Message(string type)
        : messageType(type)
    {}


    //<< Disconnected Messages >>

    Disconnected_Message::Disconnected_Message(const uid_t user)
        : Json_Message("disconnected"), user(user)
    {}

    string Disconnected_Message::get_json() const
    {
        json_t j = {};
        to_json(j, *this);
        return j.dump();
    }

    void to_json(nlohmann::json& j,  const Disconnected_Message& m)
    {
        j = json_t{
                    {"messageType", m.messageType},
                    {"user", m.user},
        };
    }


    //<< Error Messages >>

    Error_Message_Server_Shutdown::Error_Message_Server_Shutdown(const string& msg)
        : Json_Message("serverError"), message(msg)
    {}

    string	Error_Message_Server_Shutdown::get_json() const
    {
        json_t j = {};
        to_json(j, *this);
        return j.dump();
    }

    void to_json(nlohmann::json& j,  const Error_Message_Server_Shutdown& m)
    {

        j = json_t{
                    {"messageType", m.messageType},{"message", m.message},
        };


    }

    Error_Message_Invalid_Request::Error_Message_Invalid_Request(const string& cellName, const string& msg)
        : Json_Message("requestError"), cellName(cellName), message(msg)
    {}

    string	Error_Message_Invalid_Request::get_json() const
    {
        json_t j = {};
        to_json(j, *this);
        return j.dump()+'\n';
    }

    void to_json(nlohmann::json& j,  const Error_Message_Invalid_Request& m)
    {

        j = json_t{
                    {"messageType", m.messageType},{"cellName", m.cellName},{"message", m.message},
        };


    }


    //<< Cell Selection Messages >>

    Cell_Selected_Message::Cell_Selected_Message(const string& cell, const uid_t user, const string& userName)
        : Json_Message("cellSelected"), cellName(cell), selector(user), selectorName(userName)
    {}

    string Cell_Selected_Message::get_json() const
    {
        json_t j = {};
        to_json(j, *this);
        return j.dump();
    }

    void to_json(nlohmann::json& j,  const Cell_Selected_Message& m)
    {
        j = json_t{
                    {"messageType", m.messageType},
                    {"cellName", m.cellName},{"selector", m.selector},{"selectorName",m.selectorName}
        };
    }


    //<< Cell Update Messages >>

    Cell_Update_Message::Cell_Update_Message(const string& cell, const string& content)
        : Json_Message("cellUpdated"), cellName(cell), contents(content)
    {}

    string Cell_Update_Message::get_json() const
    {
        json_t j = {};
        to_json(j, *this);
        return j.dump();
    }

    void to_json(nlohmann::json& j, const Cell_Update_Message& m)
    {
        j = json_t{
                    {"messageType", m.messageType},
                    {"cellName", m.cellName},{"contents",m.contents}
        };
    }



}
