using System.Collections.Generic;
using Reach.Messages.Data;


namespace Reach.Responses
{
    public class LibraryItemsListResponse : Response
    {
        /// <summary>
        /// List of available models
        /// </summary>
        public IEnumerable<LibraryItem> LibraryItems { get; private set; }

        /// <summary>
        /// The response which contains all necessary data for creating a node in Reach editor
        /// </summary>
        /// <param name="libraryItems">List of available models</param>
        /// <example>
        /// This sample shows Json structure
        /// </example>
        /// <code>
        /// {
        ///     "$type":"LibraryItemsListResponse",
        ///     "libraryItems":[
        ///         {
        ///             "$type":"LibraryItem",
        ///             "category":"Core.Input",
        ///             "name":"Code Block",
        ///             "creationName":"Code Block",
        ///             "displayName":"Code Block",
        ///             "description":"Allows for DesignScript code to be authored directly",
        ///             "weight":1.0,
        ///             "keywords":[
        ///                 "core.input.code block",
        ///                 "code block",
        ///                 "allows for designscript code to be authored directly"
        ///             ],
        ///             "parameters":[],
        ///             "returnKeys":[]
        ///         },
        ///         ...
        ///     ],
        ///     "status":0
        /// }
        /// </code>
        public LibraryItemsListResponse(IEnumerable<LibraryItem> libraryItems)
        {
            LibraryItems = libraryItems;
        }
    }
}
