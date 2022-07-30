/**
 * @file spreadsheet_exceptions.hpp
 * @author Andrew Osterhout
 * @brief Some Exception definitions that spreadsheet will use and through.
 *
 * @version 0.1
 * @date 2021-04-12
 *
 * @copyright Copyright (c) 2021
 *
 */


#ifndef _SPREADSHEET_EXCEPTIONS_HPP_
#define _SPREADSHEET_EXCEPTIONS_HPP_


#include <exception>
#include <string>


namespace spreadsheet
{


    /**
     * @brief Exception that will be returned by the things in the \c Spreadsheet & \c DependencyGraph
     * 					when a circlar dependency is detected.
     */
    class CircularDependencyException : public std::exception
    {
    public:

        /**
         * @brief The name of the cell that in being updated caused teh circlar dependency.
         */
        const std::string updatedCell;

        /**
         * @brief The new content of the cell that defines a circular dependency.
         */
        const std::string updatedContent;

        /**
         * @brief The name of the cell that points back at the cell being updates casing the circlar dependency. \n
         * 				\em i.e. the last link in the circular chain.
         */
        const std::string lastCell;

        /**
         * @brief The content of the cell \c lastCell that points back at the cell being updates casing the circlar dependency. \n
         * 				\em i.e. the last link in the circular chain.
         */
        const std::string lastCellContent;


        CircularDependencyException(const std::string& _updatedCell, const std::string& _updatedContent);
        
        /**
         * @brief \b Constructor: \n Construct a new \c Circlar Dependency Exception object. \n
         * 		Create a \c spreadsheet::CircularDependencyException exception to \c throw
         * 			when a cell updated would cause a circular dependency.
         * @param _updatedCell The name of the cell that in being updated caused teh circlar dependency.
         * @param _updatedContent The new content of the cell that defines a circular dependency.
         * @param _lastCell The name of the cell that points back at the cell being updates casing the circlar dependency. \n
         * 									 \em i.e. the last link in the circular chain.
         * @param _lastCellContent	The content of the cell \c lastCell that points back at the cell being updates casing the circlar dependency. \n
         * 													 \em i.e. the last link in the circular chain.
         */
        CircularDependencyException(const std::string& _updatedCell, const std::string& _updatedContent, const std::string& _lastCell, const std::string& _lastCellContent);
        // 	: updatedCell(_updatedCell), 
        // 		updatedContent(_updatedContenet), 
        // 		lastCell(_lastCell), 
        // 		lastCellContent(_lastCellContent)
        // {}
    };

}


#endif
