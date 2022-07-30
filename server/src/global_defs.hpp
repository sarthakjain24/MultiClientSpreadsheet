/**
 * @file global_defs.hpp
 * @author Andrew Osterhout, Jerry Zhang, VJ...
 *
 * @brief Headerfile that contains global contents, and definitions
 * 		meant to be possibly defined externally during build.
 *  	( \em AKA Should all be preprocessor things.) \n
 * 	This should be used to keep implimentation objects agnostic of the implimentation.
 *
 * @version 0.1
 * @date 2021-04-12
 *
 * @copyright Copyright (c) 2021
 *
 */



// << ====================================================================== >> 
// <<                         SPREADSHEET SAVE INFO                          >> 
// << ====================================================================== >> 

#ifdef __GET_SPREADSHEET_SAVE_INFO

 /**
  * @brief The location where the spreadsheet save directory is.
  */
#define SPREADSHEET_SAVE_DIR "/spreadsheet_data/"

  /**
   * @brief The file extension that our spreadsheets save with.
   */
#define SPREADSHEET_FILE_EXT ".ss_json"

    
#ifdef __GET_DEFAULT_SPREADSHEET_FILE_INFO


        /**
         * @brief the name in the \c SPREADSHEET_SAVE_DIR that will automatically be genned when the server starts, if it doesn't exist.
         *      \b NOTE: it is excluding \c SPREADSHEET_FILE_EXT & cannot contain any \c '.' 's in it.
         */
#define DEFAULT_SPREADSHEET_FILE_NAME "default"

        /**
         * @brief The contents of the defult spreadsheet that will automatically be genned when the server starts, if it doesn't exist.
         *          As described in \c DEFAULT_SPREADSHEET_FILE_NAME 's description
         */
#define DEFAULT_SPREADSHEET_FILE_CONTENTS R"({"name":")" DEFAULT_SPREADSHEET_FILE_NAME R"(","data":{"A1":""}})"

#endif

#endif



// << ====================================================================== >> 
// <<                    CLIENT / SOCKET STATE CONSTANTS                     >> 
// << ====================================================================== >> 

#ifdef __GET_CLIENT_CONSTANTS

/**
 * @brief The (default) size of the buffer used to receive data from the client.
 */
#define CLIENT_BUFFER_SIZE 4096

/**
 * @brief The default display name a Client will have. \n 
 *        Is currently set up to be a prefix that the \c Client.id is tacked onto.
 */
#define CLIENT_DEFAULT_DISPLAY_NAME_TEMPLATE "client#"

#endif



// << ====================================================================== >> 
// <<                        FUNCTIONALITY CONSTANTS                         >> 
// << ====================================================================== >> 

#ifdef __GET_SAVE_CYCLE_INFO


    /**
     * @brief The time the server will wait from startup till receiveing it's first message.
     */
#define TIME_OUT_TIME 120U

    /**
     * @brief The number of seconds the server will wait before 
     */
#define PROCESS_CYCLE_DELAY 1

    /**
     * @brief The number of cycles 
     *          (1 request from client = 1 cycle) 
     *          that the server will wait before trying to save the spreadsheet.
     *        ( \b NOTE: this is retracted to once every \c AUTO_SAVE_MIN_DELAY seconds)
     */
#define SAVE_CYCLE_COUNT 20U

    /**
     * @brief The number of seconds that need to transpire before the spreadsheet will be saved,
     *          after receiving a certain number of requests from the clients greater than defined in \c SAVE_CYCLE_COUNT.
     */
#define AUTO_SAVE_MIN_DELAY 15U

    /**
     * @brief The number of seconds that need to transpire before the spreadsheet will be saved, 
     *          after receiving a request from a client (ignores \c SAVE_CYCLE_COUNT ).
     */
#define AUTO_SAVE_MAX_DELAY 120U



#endif

