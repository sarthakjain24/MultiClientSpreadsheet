#ifndef JSON_REQUEST_HPP
#define JSON_REQUEST_HPP




#include "nlohmann-json/json.hpp"
#include <string>

/**
 * @brief
 *
 */
namespace json
{


    /**
     * @brief
     *
     */
    class Json_Request
    {
    public:
        const std::string requestType;
        Json_Request(std::string ref);

    };

    /**
     * @brief
     *
     * @param ref
     * @return Json_Request*
     */
    Json_Request* readJson(std::string& ref);

    /**
     * @brief
     *
     */
    class Json_Select_Cell_Request : public Json_Request
    {
    public:
        const std::string cellName;
        Json_Select_Cell_Request(std::string cell);
    };

    /**
     * @brief
     *
     */
    class Json_Edit_Request : public Json_Request
    {
    public:
        const std::string cellName;
        const std::string contents;
        Json_Edit_Request(std::string cell, std::string content);
    };

    /**
     * @brief
     *
     */
    class Json_Revert_Cell_Request : public Json_Request
    {
    public:
        const std::string cellName;
        Json_Revert_Cell_Request(std::string cell);

    };


}

#endif