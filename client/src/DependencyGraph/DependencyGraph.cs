// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)
// Author: Sarthak Jain
// Class: CS 3500 Fall 2020
// Prof.: Prof. Daniel Kopta

using System;
using System.Collections.Generic;


namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        //Dictionary with the values depending on the string(key)
        private Dictionary<string, HashSet<string>> dependents;

        //Dictionary with the values being depended on the string(key)
        private Dictionary<string, HashSet<string>> dependees;

        //A counter to keep track of size
        private int size;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            //Initializes the two dictionaries and the size variable
            dependents = new Dictionary<string, HashSet<string>>();
            dependees = new Dictionary<string, HashSet<string>>();
            size = 0;
        }

        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get
            {
                //Returns the size
                return size;

            }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                //If the dependees dictionary contains the key s, then it returns the number of items in the hashSet associated with that key 
                if (dependees.ContainsKey(s))
                {
                    return dependees[s].Count;
                }

                //Returns 0 if the dependee dictionary doesn't contain the key s
                return 0;
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            //If the dependent's dictionary is empty, then it returns false
            if (dependents.Count == 0)
            {
                return false;
            }

            //If the dependents Dictionary has a key that is s, then it returns true
            if (dependents.ContainsKey(s))
            {
                return true;
            }
            //Returns false otherwise
            return false;

        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            //If the dependee's dictionary is empty, then it returns false
            if (dependees.Count == 0)
            {
                return false;
            }

            //If the dependee Dictionary has a key that is s, then it returns true
            if (dependees.ContainsKey(s))
            {
                return true;
            }

            //Returns false otherwise
            return false;

        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            //A empty hashSet initialized
            HashSet<string> hashSet = new HashSet<string>();

            //If the dependents dictionary contains the key s, then it returns the value that is the HashSet associated with that key from the dictionary
            if (dependents.ContainsKey(s))
            {
                return dependents[s];
            }

            //If dependents could not find the key s, then returns a new HashSet
            return hashSet;
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            //A empty hashSet initialized
            HashSet<string> hashSet = new HashSet<string>();

            //If the dependees dictionary contains the key s, then it returns the value that is the HashSet associated with that key from the dictionary
            if (dependees.ContainsKey(s))
            {
                return dependees[s];

            }
            //Else returns an empty HashSet
            return hashSet;
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///   
        /// dependents("s") = {"t"}
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>         
        public void AddDependency(string s, string t)
        {

            //If the dependents dictionary contains the key s, then it goes into this condition adding to the existing dependency
            if (dependents.ContainsKey(s))
            {
                //Creates a hashSet and sets the values from the dependents associated with the key s
                HashSet<string> hashSet = dependents[s];
                //If the hashSet doesn't contain the value t, then we add t to the hashSet
                if (!hashSet.Contains(t))
                {
                    hashSet.Add(t);
                    //Increments the size of the graph
                    size++;
                }

            }
            //If the dependents dictionary doesn't contain the key s, then it goes into this condition creating a new dependency
            else
            {
                //Creates a new HashSet to be associated with the key s
                HashSet<string> hashSet = new HashSet<string>();
                //Adds t to the hashSet
                hashSet.Add(t);
                //Adds the key s and the hashSet as the value in the dependents dictionary
                dependents.Add(s, hashSet);
                //Increments the size of the graph
                size++;
            }
            //If the dependees dictionary contains the key t, then it goes into this condition adding to the existing dependency
            if (dependees.ContainsKey(t))
            {
                //Creates a hashSet and sets the values from the dependees associated with the key t
                HashSet<string> hashSet = dependees[t];
                //If the hashSet doesn't contain the value s, then we add s to the hashSet 
                if (!hashSet.Contains(s))
                {
                    hashSet.Add(s);
                }
            }
            //If the dependees dictionary doesn't contain the key t, then it goes into this condition creating a new dependency
            else
            {
                //Creates a new HashSet to be associated with the key t
                HashSet<string> hashSet = new HashSet<string>();
                //Adds s to the hashSet
                hashSet.Add(s);
                //Adds the key t and the hashSet as the value in the dependee dictionary
                dependees.Add(t, hashSet);
            }
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s">The dependee being removed to the value t</param>
        /// <param name="t">The dependent being removed to the value s</param>
        public void RemoveDependency(string s, string t)
        {
            //If the dependents dictionary contains the key s, then it goes into this condition
            if (dependents.ContainsKey(s))
            {
                //Sets the hashSet to the values associated with the key s in the dependents dictionary
                HashSet<String> hashSet = dependents[s];

                //If the hashSet contains t, then it removes t from the hashSet, basically removing the dependency
                if (hashSet.Contains(t))
                {
                    hashSet.Remove(t);
                    //Decrements the size of the graph
                    size--;
                }

                //If the size of the HashSet associated with the key s in the dependents dictionary is empty, then it removes the string s as a key from the dependents dictionary
                if (hashSet.Count == 0)
                {
                    dependents.Remove(s);
                }
            }
            //If the dependees dictionary contains the key t, then it goes into this condition
            if (dependees.ContainsKey(t))
            {
                //Sets the hashSet to the values associated with the key t in the dependees dictionary
                HashSet<String> hashSet = dependees[t];

                //If the hashSet contains s, then it removes s from the hashSet, basically removing the dependency
                if (hashSet.Contains(s))
                {
                    hashSet.Remove(s);
                }

                //If the size of the HashSet associated with the key t in the dependees dictionary is empty, then it removes the string t as a key from the dependees dictionary
                if (hashSet.Count == 0)
                {
                    dependees.Remove(t);
                }
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
                //Creates a HashSet called oldDep and sets it to the dependents of s
                HashSet<string> oldDep = new HashSet<string>(GetDependents(s));

                //Iterates through the newDependents provided as a parameter and removing each value from the dictionaries associated with val depending on s
                foreach (string val in oldDep)
                {
                    RemoveDependency(s, val);
                }

                //Iterates through the newDependents provided as a parameter and adds each value from the dictionaries associated with key depending on s
                foreach (string key in newDependents)
                {
                    AddDependency(s, key);
                }

        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
                //Creates a HashSet and sets it to the dependees of s
                HashSet<string> oldDep = new HashSet<string>(GetDependees(s));

                //Iterates through the oldDep HashSet and removes each value from the dictionaries associated with s depending on val
                foreach (string val in oldDep)
                {
                    RemoveDependency(val, s);
                }

                //Iterates through the newDependees provided as a parameter and adds each value from the dictionaries associated with s depending on key
                foreach (string key in newDependees)
                {
                    AddDependency(key, s);
                }
            }
        
    }
}
