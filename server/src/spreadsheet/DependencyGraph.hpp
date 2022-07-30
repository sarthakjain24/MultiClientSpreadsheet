/**
 * @file DependencyGraph.hpp
 * @author Andrew Osterhout
 * @brief ...
 *
 * @version 0.1
 * @date 2021-04-12
 *
 * @copyright Copyright (c) 2021
 *
 */

#ifndef _DEPENDENCYGRAPH_HPP_
#define _DEPENDENCYGRAPH_HPP_

#include <map>
#include <unordered_set>
#include <string>
#include <mutex> 
#include <regex>
#include <stack>
#include <vector>
#include <iostream>
#include "spreadsheet_exceptions.hpp"


namespace spreadsheet
{

    // /**
    //  * @brief Just a helper object to help keep things organized.
    //  */
    // typedef struct DNode
    // {

    //     /**
    //      * @brief set of all variables that the associated variable depends on. \n
    //      * 		Start out initiated.
    //      */
    //     std::unordered_set<std::string> dependents;

    //     /**
    //      * @brief set of all variables dependent on the associated variable \n
    //      * 		Start out initiated.
    //      */
    //     std::unordered_set<std::string> dependees;

    // } DNode;
    

    /**
     * @brief This class will kep track of the dependencies defined in the
     * 				 formulas stored in a spreadsheet.
     */
    class DependencyGraph
    {
    private:

        /**
         * @brief The graph of dependencies relative to a \c cell / variable.
         */
        std::map<std::string, std::unordered_set<std::string>*> dGraph;

        // /**
        //  * @brief a \c std::mutex to ensure that 
        //  * 				only on thread is updating this dependency graph at a time.
        //  */
        // std::mutex dGraphLock;

        /**
         * @brief Check for circular dependency. Return true
         * if add in variables list to a cell will result in 
         * circular dependency. Return false otherwise. 
         * Note: This method is a iterative DFS
         * 
         * @param cell string name
         * @param variables set to be dependents of the cell
         */
        bool is_circular(const std::string& cell);


        /**
         * @brief parse a formula into the \c dGraph.
         * @param cell the cell in question
         * @param formula the formula to deconstruct.
         */
        void insert(const std::string& cell, const std::string& formula);

        /**
         * @brief remove the dgraph values for the specified cell.
         * @param cell the cell in question.
         */
        void remove(const std::string& cell);

    public:

        /**
         * @brief \b Constructor: \n Construct a new \c Dependency Graph object.
         */
        DependencyGraph();


        /**
         * @brief \b Destructor: \n Destroy this \c Dependency Graph object. \n
         * 				Needed to delete/dealocate memory associated with the \c DNode s.
         */
        ~DependencyGraph();


        /**
         * @brief Add a formula to the dependency graph tracking it's dependents and dependees. \n
         * 			If there already exists a dependency sub-graph for \c cell ,
         * 			it will be updated rather than a new one created. \n
         * 			If \c formula is \c "" then the dependency sub-graph will be removed from the graph.
         * @param cell	the name of the \c cell /variable that holds the formula.
         * @param formula	the string representation of the formula.
         * @exception \c CircularDependencyException - thrown when trying to add a dependency to the dependency graph that,
         * 						would result in a circular dependency. \n
         * 						if this occurs the dependency graph will be in the same state as it was before this method was called.
         */
        void update(const std::string& cell, const std::string& formula);


        /**
         * @brief Clear all contents of the dependency graph.
         */
        void clear();
 
    };


}




#endif
