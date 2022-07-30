
#include <string>
#include <iostream>
#include "nlohmann-json/json.hpp"

using json_t = nlohmann::json;
using namespace std;

int main()
{
    json_t* j = new json_t();

    *j = json_t::parse(" {\"requestType\": \"editCell\", \"cellName\": \"A1\", \"contents\": \"Hello\"}");

    cout << *j << endl;

    *j = json_t::parse("\n{\"requestType\": \"editCell\", \"cellName\": \"A1\", \"contents\": \"Hello\"}");

    cout << *j << endl;

    *j = json_t::parse("{\"requestType\": \"editCell\", \"cellName\": \"A1\", \"contents\": \"Hello\"}\n");

    cout << *j << endl;

    *j = json_t::parse("{\"requestType\": \"editCell\", \"cellName\": \"A1\", \"contents\": \"Hello\"}");

    cout << *j << endl;
    
    *j = json_t::parse("\u0000{\"requestType\": \"editCell\", \"cellName\": \"A1\", \"contents\": \"Hello\"}");

    cout << *j << endl;

    *j = j->parse("{\"requestType\": \"editCell\", \"cellName\": \"A1\", \"contents\": \"Hello\"}");

    cout << *j << endl;

    delete j;
}
