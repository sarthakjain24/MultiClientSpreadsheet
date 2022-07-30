#include "DependencyGraph.hpp"

spreadsheet::DependencyGraph::DependencyGraph() {}

spreadsheet::DependencyGraph::~DependencyGraph()
{
    for (auto& p : dGraph)
    {
        delete p.second;
    }

    this->dGraph.clear();
}


void spreadsheet::DependencyGraph::insert(const std::string& cell, const std::string& formula)
{
    //regex searching variables 
    std::smatch result;
    std::regex pattern("[a-zA-Z_][a-zA-Z0-9_]*");

    std::string copy_formula = formula;

    if (dGraph.find(cell) == dGraph.end())
        dGraph[cell] = new std::unordered_set<std::string>();

    while (std::regex_search(copy_formula, result, pattern))
    {
        //process variable
        for (auto x : result) dGraph[cell]->insert(x);

        //move onto next matching variable
        copy_formula = result.suffix().str();
    }
}


void spreadsheet::DependencyGraph::remove(const std::string& cell)
{
    if(dGraph.find(cell) != dGraph.end())
        dGraph[cell]->clear();
}

void spreadsheet::DependencyGraph::update(const std::string& cell, const std::string& formula)
{

    // check to see if formula is being removed
    remove(cell);
    if (formula == "" || formula[0] != '=')
        return;

    this->insert(cell,formula);

    //checking for circular dependency from the request update
    if (this->is_circular(cell))
    {
        remove(cell);
        throw spreadsheet::CircularDependencyException(cell, formula);
    }
}


bool spreadsheet::DependencyGraph::is_circular(const std::string& cell)
{
    //keep track of visited
    std::unordered_set<std::string> visited;
    //stack for DFS
    std::stack<std::string> _stack;

    _stack.push(cell);

    
    while (!_stack.empty())
    {
        std::string v = _stack.top();
        _stack.pop();
        
        if (visited.find(v) == visited.end())
        {
            //mark v visited
            visited.insert(v);
            
            //haven't visited this vertex, keep visiting
            if (this->dGraph.find(v) != this->dGraph.end())
            {
               
                for (auto& u : *(this->dGraph[v]))
                {
                       _stack.push(u); 
                       
                }
            }   
        }
        else
        {
            
            //visited this vertex already, there is circular dependency
            return true;
        }
    }

    return false;
}

void spreadsheet::DependencyGraph::clear()
{
    this->dGraph.clear();
}





