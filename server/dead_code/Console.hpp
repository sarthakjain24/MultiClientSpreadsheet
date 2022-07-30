// !! DEAD CODE !! As of 2021/04/15

/**
 * @file Console.hpp
 * @author Andrew Osterhout
 * @brief make a thread safe way to interact with the console.
 *          Based loosly from https://stackoverflow.com/questions/14718124/how-to-easily-make-stdcout-thread-safe
 * @version 0.1
 * @date 2021-04-14
 * 
 * @copyright Copyright (c) 2021
 * 
 */

#ifndef _THREADING_CONSOLE_HPP_
#define _THREADING_CONSOLE_HPP_


#include <mutex> 

// ! Forward declaration stuffs 😢

namespace threading { class Console; }

template<typename T>
threading::Console& operator<< (threading::Console& console, const T value);
template<typename T>
threading::Console& operator>> (threading::Console& console, const T value);




namespace threading
{

	/**
	 * @brief \b Static Class that makes using \c std::cout , & \c std::cin . \n 
	 * 				You should be able to just use \c << to output to the console and \c >> to get input from the console,
	 * 					in a thread safe way.
	 */
	class Console
	{
		private:
			
			/**
			 * @brief the \c std::mutex used to lock access to the console when waiting for other threads to finish.
			 */
			static std::mutex consoleLock;

		public:

			/**
			 * @brief Safely write to \c std::cout
			 * @tparam T any type that can be handeled by an \c std::ostream
			 * @param value the thing that will be written to console via \c std::cout
			 */
			template<typename T>
			static void write(T& value);

			/**
			 * @brief Safely read from \c std::cin
			 * @return \c  std::istream - the input stream redirected form \c std::cin . 
			 */
			static std::istream read();

			
			/**
			 * @brief Output data from \c std::cout in a thread safe way. ( \c operator<< overload)
			 * @tparam T any type that you can use with \c std::cout . 
             * @param value the thing that will be written to console via \c std::cout
			 */
			template<typename T>
			Console& operator<< (const T& value);


			/**
			 * @brief Get data from \c std::cin in a thread safe way. ( \c operator>> overload)
			 * @tparam T any type that you can use with \c std::cin . 
             * @param value the thing that will have it's value modified by the contents of the console via \c std::cin
			 */
			template<typename T>
			Console& operator>> (T& value);


			/**
			 * @brief overload the stream insertion operator as friend function (because static classes can't have operator overloads)
			 * @tparam T any type that you can use with \c std::cout . 
			 */
			template<typename T>
			friend Console& (::operator<<) (Console& console, const T value);

			/**
			 * @brief overload the stream insertion operator as friend function (because static classes can't have operator overloads)
			 * @tparam T any type that you can use with \c std::cin . 
			 */
			template<typename T>
			friend Console& (::operator>>) (Console& console, const T value);
	};


}

#endif
