/**
 * @file Spreadsheet.cpp
 * @author Andrew Osterhout
 * @brief
 * @version 0.1
 * @date 2021-04-14
 *
 * @copyright Copyright (c) 2021
 *
 */


#include <set>
#include <fstream>
#include <iostream>
#include <filesystem>
namespace fs = std::filesystem;

#define _SPREADSHEET_CPP_
#include "Spreadsheet.hpp"

#define __GET_SPREADSHEET_SAVE_INFO
#include "global_defs.hpp"

#include "debug_defs.hpp"


using json_t = nlohmann::json;
using namespace std;


namespace spreadsheet
{


    // this is a little helper function that should never be seen elswhere.
    static void __get_spreadsheet_files(set<string>& file_list)
    {
        for (const auto& entry : fs::directory_iterator(SPREADSHEET_SAVE_DIR))
            file_list.insert(entry.path().filename());
    }



    Spreadsheet::Spreadsheet(string& _name)
        : name(_name) /*, data(), history(), dGraph() */
    {
        set<string> file_list;
        __get_spreadsheet_files(file_list);
        if (file_list.find(_name + SPREADSHEET_FILE_EXT) == file_list.end())
            save();
        else
            read();
    }



    void Spreadsheet::save() const
    {
        json_t j;
        get_json(j, *this);
        ofstream file(SPREADSHEET_SAVE_DIR + name + SPREADSHEET_FILE_EXT, ios::out | ios::trunc);
        file << j;
        file.close();
    }



    bool Spreadsheet::update(const string& cell, const string& content)
    {
            // if (data[cell].size() > 0 && data[cell][0] == '=') // remove from dependency graph if it prev value was a formula.
            //     dGraph.update(cell, "");
        // if new value is a formula update dependency graph (handle exceptions)
        try
        {
            if (content.size() > 0 && content[0] == '=')
                dGraph.update(cell, content);
        }
        catch (const CircularDependencyException ce)
        {
            // DEBUG_LOG("  >> WARNING :: circular dependency discovered when updating the spreadsheet !!            [spreadsheet::Spreadsheet::update]\n"
            //     "  >>                     spreadsheet: " << name << "\n"
            //     "  >>                    updated cell: " << ce.updatedCell << "\n"
            //     //? << "  >>                updated contents: " << ce.updatedContent << '\n'
            //     //? << "  >>             last cell in circle: " << ce.lastCell << '\n'
            //     //? << "  >>   last cell in circle's content: " << ce.lastCellContent << '\n'
            //     "  >> NOTE: this value was not added-to/updated-in the spreadsheet for safety reasons !!\n");
            throw ce;
        }
        catch (...)
        {
            DEBUG_LOG("  >> ERROR :: unknown & unexpected Exception occured !!  [spreadsheet::Spreadsheet::update]"
                "  >>                     spreadsheet: " << name << "\n");
            return false;
        }
        // update spreadsheet ----
        if (data.find(cell) == data.end())
            history.add(cell, "");
        else
            history.add(cell, data[cell]);
        data[cell] = content;
        return true;
    }



    HistoryNode* Spreadsheet::undo()
    {
        HistoryNode* hNode = history.undo();
        if (hNode != NULL)
            this->update(hNode->cell, hNode->old);
        return hNode;
    }



    HistoryNode* Spreadsheet::revert(const string& cell)
    {
        HistoryNode* hNode = history.revert(cell);
        if (hNode != NULL)
            this->update(cell, hNode->old);
        return hNode;
    }



    const std::unordered_map<std::string,std::string>* Spreadsheet::get_data() const
    {
        return &data;
    }



    void Spreadsheet::read()
    {
        json_t j;
        ifstream file(SPREADSHEET_SAVE_DIR + name + SPREADSHEET_FILE_EXT, ios::in);
        file >> j;
        file.close();
        from_json(j, *this);

        // clear history and dependency graph pre read in
        history.clear();
        dGraph.clear();

        // update dependency graph
        for (auto pair : data)
            try
        {
            dGraph.update(pair.first, pair.second);
        }
        catch (const CircularDependencyException ce)
        {
            DEBUG_LOG("\n  >> WARNING :: circular dependency discovered when reading spreadsheet in from file !!  [spreadsheet::Spreadsheet::read]\n"
                << "  >>                     spreadsheet: " << name << '\n'
                << "  >>                    updated cell: " << ce.updatedCell << '\n'
                //? << "  >>                updated contents: " << ce.updatedContent << '\n'
                //? << "  >>             last cell in circle: " << ce.lastCell << '\n'
                //? << "  >>   last cell in circle's content: " << ce.lastCellContent << '\n'
                << "  >> NOTE: this value was removed from the spreadsheet for safety reasons !!\n");
            data[pair.first] = "";
        }
        catch (...)
        {
            DEBUG_LOG("\n  >> ERROR :: unknown & unexpected Exception occured during dGraph construction !!  [spreadsheet::Spreadsheet::read]\n"
                "  >>                     spreadsheet: " << name << '\n');
        }


    }

}
