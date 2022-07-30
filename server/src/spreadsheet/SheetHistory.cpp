/**
 * @file SheetHistory.cpp
 * @author vj ..., Andrew Osterhout
 * @brief Code for the spreadsheet sheet history object.
 *          (to state the obvious, see header file for actual doc)
 * @version 0.1
 * @date 2021-04-16
 *
 * @copyright Copyright (c) 2021
 *
 */



#include "SheetHistory.hpp"

using namespace std;



namespace spreadsheet
{

    SheetHistory::SheetHistory()
    {}



    SheetHistory::~SheetHistory()
    {
        for (auto entry : history)
            delete entry;
    }



    void SheetHistory::add(const string& cell, const string& oldContent)
    {
        //? NOTE: create a new History node in the heap like this: `HistoryNode* v_name = new HistoryNode(cell_name, old_content);`
        //?          then store it in the history list. (make sure to read the method descriptions to understand how the memory model works.)
        HistoryNode* entry = new HistoryNode(cell, oldContent);
        history.push_front(entry);
    }



    void SheetHistory::add(HistoryNode* entry)
    {
        history.push_front(entry);
    }



    HistoryNode* SheetHistory::undo()
    {
        if (not history.empty())
        {
            HistoryNode* val = history.front();
            history.pop_front();
            return val;
        }
        else
            return NULL;
    }



    HistoryNode* SheetHistory::revert(const string& cell)
    {
        if (not history.empty())
        {
            for (auto entry = history.begin(); entry != history.end(); ++entry)
                if (cell == (*entry)->cell)
                {
                    HistoryNode* val = *entry;
                    entry = history.erase(entry);
                    return val;
                }
        }
        return NULL;
    }



    void SheetHistory::clear()
    {
        history.clear();
    }



    HistoryNode::HistoryNode(string cell, string old_content)
        : cell(cell), old(old_content)
    {}

}