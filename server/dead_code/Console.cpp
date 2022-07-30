/**
 * @file Console.cpp
 * @author Andrew Osterhout
 * @brief make a thread safe way to interact with the console.
 * @version 0.1
 * @date 2021-04-14
 *
 * @copyright Copyright (c) 2021
 *
 */

#include "Console.hpp"

#include <iostream>



template<typename T>
threading::Console& operator<< (threading::Console& console, const T value)
{
    console.consoleLock.lock();
    std::cout << value << flush; // possibly skip the flush.
    console.consoleLock.unlock();
}



template<typename T>
threading::Console& operator>> (threading::Console& console, const T value)
{
    console.consoleLock.lock();
    std::cin >> value;
    console.consoleLock.unlock();
}


// defining/seting the value of a static member variable.

namespace threading
{
    std::mutex Console::consoleLock;

    template<typename T>
    void Console::write(T& value)
    {
        Console::consoleLock.lock();
        std::cout << value << flush; // possibly skip the flush.
        Console::consoleLock.unlock();
    }


    std::istream Console::read()
    {
        Console::consoleLock.lock();
        // auto value = std::cin.getline();
        // with _CharT=char, _Traits=std::char_traits<char>]
        Console::consoleLock.unlock();
        // return value;
    }

    template<typename T>
    const Console& Console::operator<< (const T value)
    {
        console.consoleLock.lock();
        std::cout << value << flush; // possibly skip the flush.
        console.consoleLock.unlock();
    }



    template<typename T>
    const Console& Console::operator>> (const T value)
    {
        console.consoleLock.lock();
        std::cin >> value;
        console.consoleLock.unlock();
    }
}