#include "spreadsheet_exceptions.hpp"


namespace spreadsheet
{
    CircularDependencyException::CircularDependencyException(const std::string& _updatedCell, const std::string& _updatedContent)
        : updatedCell(_updatedCell), updatedContent(_updatedContent)
    {}

    CircularDependencyException::CircularDependencyException(const std::string& _updatedCell, const std::string& _updatedContent, const std::string& _lastCell, const std::string& _lastCellContent)
        :   updatedCell(_updatedCell),
            updatedContent(_updatedContent),
            lastCell(_lastCell),
            lastCellContent(_lastCellContent)
    {}

}
