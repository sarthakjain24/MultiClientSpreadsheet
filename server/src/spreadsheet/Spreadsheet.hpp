/**
 * @file Spreadsheet.hpp
 * @author Andrew Osterhout
 * @brief ...
 *
 * @version 0.1
 * @date 2021-04-12
 *
 * @copyright Copyright (c) 2021
 *
 */



#ifndef _SPREADSHEET_HPP_
#define _SPREADSHEET_HPP_


#include <string>
#include <unordered_map>
#include <mutex>

#include "SheetHistory.hpp"
#include "DependencyGraph.hpp"
#include "nlohmann-json/json.hpp"
#include "global_defs.hpp"



namespace spreadsheet
{


    // << ====================================================================== >> 
    // <<                    SPREADSHEET CLASS DEFINITITION                      >> 
    // << ====================================================================== >> 

    /**
     * @brief The Model of the spreadsheet, that is mostly agnostic of being part of a server,
     * 		but not really of the limited functionality that the server needs.
     */
    class Spreadsheet
    {

        // << ===================== Public Class Data Members ====================== >> 
    public:

        /**
         * @brief The name of the spreadsheet.
         * 		Should correspond with the file name on the server.
         */
        std::string name;


        // << ======================= Private Class Members ======================== >> 
    private:

        // /**
        //  * @brief a \c std::mutex to ensure that 
        //  * 				only on thread is reading or writing to the spreadsheet save file at a time.
        //  */
        // std::mutex fileLock;

        /**
         * @brief A dictionary Representing the contents of the spreadsheet.
         */
        std::unordered_map<std::string, std::string> data;

        // /**
        //  * @brief a \c std::mutex to ensure that 
        //  * 				only on thread is able to update the spreadsheets data contents at a time.
        //  */
        // std::mutex dataLock;

        /**
         * @brief A \c SheetHistory Object that keeps track of changes that
         * 		occured in this session of the spreadsheet being open.
         */
        SheetHistory history;

        /**
         * @brief a \c DependencyGraph Object that keeps track of the dependencies
         * 	of formulas in the spreadsheet.
         */
        DependencyGraph dGraph;


        // << =========================== Constructors ============================= >> 
    public:

        /**
         * @brief \b Constructor: \n Construct a new \c Spreadsheet object.
         *
         * @param _name The name of the spreadsheet. Should correspond with the file loc of the saved spreadsheet.
         * @exception \c std::ios_base::failure - if something \b catastrophic happens with the file stream or file system (stream was not safe).
         * @exception \c std::exception - if something \b non-catastrophic happens during interacting with the filestream,
         * 						but can't reason out how to recover (stream was still safe).
         */
        Spreadsheet(std::string& _name);



        // << ========================== Public Methods ============================ >> 
    public:

        /**
         * @brief Save the Spreadsheet as JSON in the location,
         * 		implied by \c Spreadsheet::name . \n
         * 	Uses \c nlohmann-json/json.hpp to incode as JSON.
         * @exception \c std::ios_base::failure - if something \b catastrophic happens with the file stream or file system (stream was not safe).
         * @exception \c std::exception - if something \b non-catastrophic happens during interacting with the filestream,
         * 		but can't reason out how to recover (stream was still safe).
         */
        void save() const;


        /**
         * @brief Main method for interacting with the spreadsheet.
         * 	Provide a \c cell /variabel that needs to have it's value updated,
         * 		and the value that should be assigned.
         * 	\b NOTE: if the \c cell /variable is not defined in the spreadsheet,
         * 		it will be added to the spreadsheet,
         * 		if \c content  \n
         * 	Additionally, it will check the content is a formula,
         * 		if it is a formula, its relations will be added to a dependency graph for tracking,
         * 		if in that processing the formula creates a Circular dependency a \c spreadsheet::CircularDependencyException will be thrown.
         * @param cell \c string - the name of the \c cell /variable that will be updated.
         * @param content \c string - the contents of the \c cell /variable that will be assigned.
         * @return \c bool - If the value was successfully updated or not.
         * @exception \c spreadsheet::CirclarDependencyException - thrown when an update will cause a circular dependency (will not update spreadsheet if this is the case).
         */
        bool update(const std::string& cell, const std::string& content);


        /**
         * @brief Undo the last change made to the spreadsheet.
         * 				Currently agnostic of who made the change.
         * @return 	\c HistoryNode* - What cell was updated and what content it now holds. \n
         * 					The receiver is now responsible for disposing of the object pointed to by the returned pointer. \n
         * 					Returns \c NULL if no change was undone.
         */
        HistoryNode* undo();


        /**
         * @brief Undo the last change made to the specified cell in the spreadsheet.
         * 				Currently agnostic of who made the change.
         * @return 	\c HistoryNode* - What cell was updated and what content it now holds. \n
         * 					The receiver is now responsible for disposing of the object pointed to by the returned pointer. \n
         * 					Returns \c NULL if no change was undone.
         */
        HistoryNode* revert(const std::string& cell);


        const std::unordered_map<std::string,std::string>* get_data() const;


        // << ========================= Protected Methods ========================== >> 
    protected:

        /**
         * @brief Read the contents of a file contaning a JSON representation of a \c Spreadsheet,
         * 				 and make this instance of spreadsheet match it.
         * @exception \c std::ios_base::failure - if something \b catastrophic happens with the file stream or file system (stream was not safe).
         * @exception \c std::exception - if something \b non-catastrophic happens during interacting with the filestream,
         * 						but can't reason out how to recover (stream was still safe).
         */
        void read();

#ifdef _SPREADSHEET_CPP_
        NLOHMANN_DEFINE_TYPE_INTRUSIVE(Spreadsheet, name, data)
#endif
    };

}




#endif
