/*
dotNetRDF is free and open source software licensed under the MIT License

-----------------------------------------------------------------------------

Copyright (c) 2009-2013 dotNetRDF Project (dotnetrdf-developer@lists.sf.net)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is furnished
to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF.Nodes;

namespace VDS.RDF.Query.Engine
{

    /// <summary>
    /// Comparer for checking whether sets are distinct, check may either be using the entire set or by using only a subset of variables
    /// </summary>
    public class SetDistinctnessComparer
        : IEqualityComparer<ISet>
    {
        private readonly List<String> _vars = new List<String>();

        /// <summary>
        /// Creates a new comparer that compares across all variables
        /// </summary>
        public SetDistinctnessComparer() { }

        /// <summary>
        /// Creates a new comparer that compare only on the specific variables
        /// </summary>
        /// <param name="variables">Variables</param>
        public SetDistinctnessComparer(IEnumerable<String> variables)
        {
            this._vars.AddRange(variables);
        }

        /// <summary>
        /// Determines whether the given sets are equal
        /// </summary>
        /// <param name="x">First Set</param>
        /// <param name="y">Second Set</param>
        /// <returns>True if sets are equal, false otherwise</returns>
        public bool Equals(ISet x, ISet y)
        {
            //Both null considered equal
            if (x == null && y == null) return true;
            //Only one null considered non-equal
            if (x == null || y == null) return false;

            if (this._vars.Count == 0)
            {
                //If no specific variables then use standard ISet implementation of equality
                //i.e. compare for equality across all variables in the sets
                return x.Equals(y);
            }
            //Otherwise compare for equality on specified variables
            return this._vars.All(v => (x[v] == null && y[v] == null) || EqualityHelper.AreNodesEqual(x[v], y[v]));
        }

        /// <summary>
        /// Gets the hash code for a set
        /// </summary>
        /// <param name="obj">Set</param>
        /// <returns>Hash Code</returns>
        public int GetHashCode(ISet obj)
        {
            if (obj == null) return -1;

            if (this._vars.Count == 0)
            {
                // TODO Should really compute something here
                return 0;
            }
            StringBuilder output = new StringBuilder();
            foreach (String var in this._vars)
            {
                output.Append("?" + var + " = " + obj[var].ToSafeString());
                output.Append(" , ");
            }
            output.Remove(output.Length - 3, 3);
            return output.ToString().GetHashCode();
        }
    }
}
