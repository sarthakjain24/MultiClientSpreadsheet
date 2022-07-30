/**
 * @file Action.hpp
 * @author Andrew Osterhout
 * @brief type declaration for our network action functional
 * @version 0.1
 * @date 2021-04-14
 *
 * @copyright Copyright (c) 2021
 *
 */

#ifndef _NETWORKING_ACTION_HPP_
#define _NETWORKING_ACTION_HPP_

#include <functional>

namespace networking
{
    class Client;

    typedef std::function<void(Client*)> Action;
}

#endif