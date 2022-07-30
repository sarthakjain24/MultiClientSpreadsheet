/**
 * @file Debug.hpp
 * @author Andrew Osterhout
 * @brief Make handy debug macros that will allow for thread safe debugging,
 * 				 based on the state of the `DEBUG` preprocessor definition.
 * @version 0.1
 * @date 2021-04-14
 *
 * @copyright Copyright (c) 2021
 *
 */


#ifndef _DEBUG_DEFS_HPP_
#define _DEBUG_DEFS_HPP_


 /* Control if debug features & macros should actually do things or not */
#ifdef DEBUG

/* Debug featrues & macro's should do something */

#include <iostream>


#define __DEBUG_LOG 1



/*
 * Macro that will output to console in a thread safe way,
 * 	but only if `DEBUG` has been defined in this context (it has been).
 * (NOTE: will automatically insert an \c std::endl after your log entry.)
 * 
 * (NOTE: requires you to follow this macro's `)` with a `;`, if the statement should end.)
 *
 * !! `DEBUG` HAS BEEN DEFINED & output WILL be output to `std::cout` !!
 */
#define DEBUG_LOG(message) std::cout << message << std::endl;

 /*
  * Macro that will SURPRESS the section of code surrounded by it `()`'s,
  * 	but only if `DEBUG` has been defined in this context (it has been).
  * This is like commenting out code, or using `#ifdef DEBUG \n ...your code... \n #endif`,
  * 	except in a shorter way that doesn't require preprocessor directives ruining code formating.
  * You could also think of it like commenting out the code block,
  *  except in a way that lets you toggel it active or not when building.
  *
  * !! `DEBUG` HAS BEEN DEFINED & the code WILL be SURPRESSED !!
  */
#define DEBUG_SURPRESS(codeBlock) 

  /*
   * Macro that will allow the enclosed section of code (surrounded by it `()`'s) to execute
   *  (and be compiled at all),
   * 	but only if `DEBUG` has been defined in this context (it has been).
   * This is like commenting out code, or using `\n#ifndef DEBUG \n ...your code... \n#endif`,
   * 	except in a shorter way that doesn't require preprocessor directives ruining code formating.
   * You could also think of it like commenting out the code block,
   *  except in a way that lets you toggel it active or not when building.
   *
   * !! `DEBUG` HAS BEEN DEFINED & the code WILL be COMPILED & be able to be EXECUTED !!
   */
#define DEBUG_DO(codeBlock) codeBlock

#else

/* Debug features & macros should NOT do anything !! */

#define __DEBUG_LOG 0


/*
 * Macro that will out put to console in a thread safe way,
 * 	but only if `DEBUG` has been defined in this context (it has NOT been).
 * (NOTE: will automatically insert an \c std::endl after your log entry.)
 * 
 * (NOTE: requires you to follow this macro's `)` with a `;`, if the statement should end.)
 *
 * `DEBUG` has NOT been defined & output will NOT be output to `std::cout`.
 */
#define DEBUG_LOG(message) do {} while(0)

 /*
  * Macro that will SURPRESS the section of code surrounded by it `()`'s,
  * 	but only if `DEBUG` has been defined in this context (it has NOT been).
  * This is like commenting out code, or using `#ifdef DEBUG \n ...your code... \n #endif`,
  * 	except in a shorter way that doesn't require preprocessor directives ruining code formating.
  * You could also think of it like commenting out the code block,
  *  except in a way that lets you toggel it active or not when building.
  *
  * `DEBUG` has NOT been defined & the code WILL be COMPILED & be able to be EXECUTED.
  */
#define DEBUG_SURPRESS(codeBlock) 

  /*
   * Macro that will allow the enclosed section of code (surrounded by it `()`'s) to execute
   *  (and be compiled at all),
   * 	but only if `DEBUG` has been defined in this context (it has NOT been).
   * This is like commenting out code, or using `\n#ifndef DEBUG \n ...your code... \n#endif`,
   * 	except in a shorter way that doesn't require preprocessor directives ruining code formating.
   * You could also think of it like commenting out the code block,
   *  except in a way that lets you toggel it active or not when building.
   *
   * `DEBUG` has NOT been defined & the code section WILL be SURPRESSED.
   */
#define DEBUG_DO(codeBlock) codeBlock

#endif



#endif