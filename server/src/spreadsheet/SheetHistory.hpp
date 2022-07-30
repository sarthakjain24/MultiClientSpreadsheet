/**
 * @file SheetHistory.hpp
 * @author Andrew Osterhout
 * @brief ...
 *
 * @version 0.1
 * @date 2021-04-12
 *
 * @copyright Copyright (c) 2021
 *
 */


#ifndef _SHEETHISTORY_HPP_
#define _SHEETHISTORY_HPP_


#include <string>
#include <list>

namespace spreadsheet
{

    /**
     * @brief A structure that ties together a cell name and an old value assigned to it.
     */
    typedef struct HistoryNode
    {

        /**
         * @brief Name of the \c cell /variable that this \c HistoryNode is about.
         */
        std::string cell;

        /**
         * @brief The previouse value of the \c cell /variable.
         */
        std::string old;

        /**
         * @brief \b Constructor: \n Construct a new \c HistoryNode object.
         * 				Constructor used ot
         * @param cell	Name of the \c cell /variable that this \c HistoryNode is about.
         * @param old	The previouse value of the \c cell /variable.
         */
        HistoryNode(std::string cell, std::string old);
        // 	: cell(_cell), old(_old)
        // {}

    } HistoryNode;

    /**
     * @brief ...
     */
    class SheetHistory
    {
    private:

        /**
         * @brief A linked list implimented stack for easier traversal.
         */
        std::list<HistoryNode*> history;


    public:

        /**
         * @brief \b Constructor: \n Construct a new \c Sheet History object.
         */
        SheetHistory();


        /**
         * @brief \b Destructor: \n Destroy this \c Sheet History object.
         * 				Will need to delete all \c HistoryNode s stored in \c history .
         */
        ~SheetHistory();


        /**
         * @brief Add a history log entry to this \c SheetHistory .
         * @param cell	The name of the cell who's content was changed.
         * @param oldContent	The old content the cell used to hold.
         */
        void add(const std::string& cell, const std::string& oldContent);


        /**
         * @brief Add a history log entry to this \c SheetHistory .
         * @param entry	The entry into the history log.
         */
        void add(HistoryNode* entry);


        /**
         * @brief Undo the most recent change to the \c Spreadsheet this \c SheetHistory instance is about.
         * @return	\c HistoryNode - A \c HistoryNode that represents the change the should be made to the \c Spreadsheet
         * 					to complete the undo action. \n
         * 					\b Returns \c NULL if no change to undo.
         */
        HistoryNode* undo();


        /**
         * @brief Undo the most recent change to the specified
         * @param cell	The name of the cell to try and revert/undo the most recent change to it.
         * @return	\c HistoryNode - A \c HistoryNode that represents the change the should be made to the \c Spreadsheet
         * 					to complete the undo action. \n
         * 					\b Returns \c NULL if no change to undo/revert.
         */
        HistoryNode* revert(const std::string& cell);


        /**
         * @brief clear out the Sheet history.
         */
        void clear();


    };


}




#endif
