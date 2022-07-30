#ifndef JSON_MESSAGES_HPP
#define JSON_MESSAGES_HPP



#include "nlohmann-json/json.hpp"
// #include "json.hpp"
#include <string>


namespace json
{
  /**
   * @brief
   *
   */
  class Json_Message
  {
  public:
    const std::string messageType;
    virtual std::string get_json() const = 0;
    Json_Message(std::string type);
    // NLOHMANN_DEFINE_TYPE_INTRUSIVE(Json_Message, messageType);

  private:

    // friend void to_json(nlohmann::json& j, const Json_Message& m);

  };

  /**
   * @brief
   *
   */
  class Disconnected_Message : public Json_Message
  {
  public:
    const uid_t user;
    Disconnected_Message(const uid_t user);
    virtual std::string get_json() const;
    // NLOHMANN_DEFINE_TYPE_INTRUSIVE(Disconnected_Message, messageType, user);

  private:

    friend void to_json(nlohmann::json& j, const Disconnected_Message& m);

  };

  /**
   * @brief
   *
   */
  class Error_Message_Server_Shutdown : public Json_Message
  {
  public:
    const std::string message;
    Error_Message_Server_Shutdown(const std::string& message);
    virtual std::string get_json() const;
    // NLOHMANN_DEFINE_TYPE_INTRUSIVE(Error_Message, messageType, message);

  private:

    friend void to_json(nlohmann::json& j, const Error_Message_Server_Shutdown& m);
  };

  /**
   * @brief
   *
   */
  class Error_Message_Invalid_Request : public Json_Message
  {
  public:
    const std::string message;
		const std::string cellName;
    Error_Message_Invalid_Request(const std::string& cellName, const std::string& message);
    virtual std::string get_json() const;
    // NLOHMANN_DEFINE_TYPE_INTRUSIVE(Error_Message, messageType, message);

  private:

    friend void to_json(nlohmann::json& j, const Error_Message_Invalid_Request& m);
  };

  /**
   * @brief
   *
   */
  class Cell_Selected_Message : public Json_Message
  {
  public:
    const std::string cellName;
    const uid_t selector;
    const std::string selectorName;
    Cell_Selected_Message(const std::string& cell, uid_t user, const std::string& userName);
    virtual std::string get_json() const;
    // NLOHMANN_DEFINE_TYPE_INTRUSIVE(Cell_Selected_Message, messageType, cellName, selector, selectorName);

  private:

    friend void to_json(nlohmann::json& j, const Cell_Selected_Message& m);
  };

  /**
   * @brief ...
   *
   */
  class Cell_Update_Message : public Json_Message
  {
  public:
    const std::string cellName;
    const std::string contents;
    Cell_Update_Message(const std::string& cell, const std::string& content);
    // Cell_Update_Message(std::string& cell, std::string& content);
    virtual std::string get_json() const;
    // NLOHMANN_DEFINE_TYPE_INTRUSIVE(Cell_Update_Message, messageType, cellName, contents);

  private:

    friend void to_json(nlohmann::json& j, const Cell_Update_Message& m);

  };

}

#endif