using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Autodesk.Revit.DB;
using DynamoServices;

using ProtoCore;

using Dynamo.Engine;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Nodes.ZeroTouch;
using Dynamo.Graph.Workspaces;
using RevitServices.Elements;

namespace RevitServices.Persistence
{
    /// <summary>
    /// Holds a  representation of a Revit ID that supports serialisation
    /// </summary>
    [Serializable]
    public class SerializableId : ISerializable
    {
        public String StringID { get; set; }
        public int IntID { get; set; }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("stringID", StringID, typeof(string));
            info.AddValue("intID", IntID, typeof(int));
        }

        public SerializableId()
        {
            StringID = "";
            IntID = int.MinValue;

        }

        /// <summary>
        /// Ctor used by the serialisation engine
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public SerializableId(SerializationInfo info, StreamingContext context)
        {
            StringID = (string) info.GetValue("stringID", typeof (string));
            IntID = (int)info.GetValue("intID", typeof(int));
        }

        public override bool Equals(object other)
        {
            var sID = other as SerializableId;
            if (sID == null)
            {
                return false;
            }

            return (IntID.Equals(sID.IntID) && StringID.Equals(sID.StringID));

        }

        public override int GetHashCode()
        {
            return StringID.GetHashCode() ^ IntID.GetHashCode();
        }
    }


    //@TODO: This could be used to hold all the serializableIds
    [Serializable]
    public class MultipleSerializableId : ISerializable
    {
        public List<String> StringIDs { get; set; }
        public List<int> IntIDs { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Validity.Assert(StringIDs.Count == IntIDs.Count);

            int numberOfElements = StringIDs.Count;

            info.AddValue("numberOfElements", numberOfElements);

            for (int i = 0; i < numberOfElements; i++)
            {
                info.AddValue("stringID-" + i, StringIDs[i], typeof(string));
                info.AddValue("intID-" + i, IntIDs[i], typeof(int));
            }
        }

        public MultipleSerializableId()
        {
            InitializeDataMembers();
        }

        public MultipleSerializableId(IEnumerable<Element> elements)
        {
            InitializeDataMembers();

            foreach (Element element in elements)
            {
                StringIDs.Add(element.UniqueId);
                IntIDs.Add(element.Id.IntegerValue);
            }            
        }

        /// <summary>
        /// Ctor used by the serialisation engine
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public MultipleSerializableId(SerializationInfo info, StreamingContext context)
        {
            InitializeDataMembers();

            int numberOfElements = info.GetInt32("numberOfElements");

            for (int i = 0; i < numberOfElements; i++)
            {
                string stringID = (string) info.GetValue("stringID-" + i, typeof (string));
                int intID = (int) info.GetValue("intID-" + i, typeof (int));

                StringIDs.Add(stringID);
                IntIDs.Add(intID);
            }

        }
        
        private void InitializeDataMembers()
        {
            StringIDs = new List<String>();
            IntIDs = new List<int>();
        }

        public override bool Equals(object other)
        {
            var mult = other as MultipleSerializableId;
            if (mult == null)
            {
                return false;
            }

            return this.IntIDs.SequenceEqual(mult.IntIDs) && this.StringIDs.SequenceEqual(mult.StringIDs);
            

        }

        public override int GetHashCode()
        {
            //concat the strings and int ids into one string and get hashcode and xor
            //a multiserializableID with same IDs in different order will return not equal  
            return StringIDs.Aggregate((i, j) => i + " " + j).GetHashCode() ^
             IntIDs.Select(x => x.ToString()).Aggregate((i, j) => i + " " + j).GetHashCode();

         }
        }


    /// <summary>
    /// Class for handling unique ids in a typesafe ammner
    /// </summary>
    public class ElementUUID
    {
        public String UUID { get; set; }

        public ElementUUID()
        {
            UUID = "";
        }

        public ElementUUID(string uuid)
        {
            this.UUID = uuid;
        }

    }


    /// <summary>
    /// Tools to handle the binding and interaction 
    /// </summary>
    public class ElementBinder
    {
        private const string REVIT_TRACE_ID = "{0459D869-0C72-447F-96D8-08A7FB92214B}-REVIT";

        public static bool IsEnabled = false;

        /// <summary>
        /// Get an ElementId from trace
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete]
        public static ElementId GetElementIdFromTrace(Document document)
        {
            //Get the element ID that was cached in the callsite
            ISerializable traceData = TraceUtils.GetTraceData(REVIT_TRACE_ID);

            SerializableId id = traceData as SerializableId;
            if (id == null)
                return null; //There was no usable data in the trace cache

            var traceDataInt = id.IntID;
            return new Autodesk.Revit.DB.ElementId(traceDataInt);
        }

        /// <summary>
        /// Get an Element unique Identifier from trace
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static ElementUUID GetElementUUIDFromTrace(Document document)
        {
            //Get the element ID that was cached in the callsite
            ISerializable traceData = TraceUtils.GetTraceData(REVIT_TRACE_ID);

            SerializableId id = traceData as SerializableId;
            if (id == null)
                return null; //There was no usable data in the trace cache

            var traceDataUuid = id.StringID;
            return new ElementUUID(traceDataUuid);
        }

        /// <summary>
        /// Get the collection of ElementIDs from Trace
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static List<ElementUUID> GetElementUUIDsFromTrace(Document document)
        {
            //Get the element ID that was cached in the callsite
            ISerializable traceData = TraceUtils.GetTraceData(REVIT_TRACE_ID);

            var multi = traceData as MultipleSerializableId;

            if (multi != null)
            {
                List<ElementUUID> uuids = new List<ElementUUID>();
                foreach (var uuid in multi.StringIDs)
                    uuids.Add(new ElementUUID(uuid));

                return uuids;
            }

            var single = traceData as SerializableId;
            if (single != null)
            {
                var traceDataUuid = single.StringID;
                List<ElementUUID> uuids = new List<ElementUUID>()
                    {
                        new ElementUUID(traceDataUuid)
                    };
                return uuids;
            }

            //No usable data was found
            return null;

        }


        /// <summary>
        /// Get the elementId associated with a UUID, possibly expensive
        /// </summary>
        /// <param name="document"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public static ElementId GetIdForUUID(Document document, ElementUUID uuid)
        {
            Element e = document.GetElement(uuid.UUID);
            if (e != null)
                return e.Id;
            return null;
        }


        /// <summary>
        /// Set the element associated with the current operation from trace
        /// null if there is no object, or it's of the wrong type etc.
        /// </summary>
        /// <param name="element">The element to store in trace</param>
        public static void SetElementForTrace(Element element)
        {
            SetElementForTrace(element.Id, new ElementUUID(element.UniqueId));
        }

        /// <summary>
        /// Set a list of elements for trace
        /// </summary>
        /// <param name="elements"></param>
        public static void SetElementsForTrace(List<Element> elements)
        {
            if (!IsEnabled) return;

            MultipleSerializableId ids = new MultipleSerializableId(elements);
            TraceUtils.SetTraceData(REVIT_TRACE_ID, ids);

        }

        /// <summary>
        /// Set the element associated with the current operation from trace
        /// null if there is no object, or it's of the wrong type etc.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static void SetElementForTrace(ElementId elementId, ElementUUID elementUUID)
        {
            if (!IsEnabled) return;

            SerializableId id = new SerializableId();
            id.IntID = elementId.IntegerValue;
            id.StringID = elementUUID.UUID;

            // if we're mutating the current Element id, that means we need to 
            // clean up the old object

            // Set the element ID cached in the callsite
            TraceUtils.SetTraceData(REVIT_TRACE_ID, id);

        }

        /// <summary>
        /// Get the element associated with the current operation from trace
        /// null if there is no object, or it's of the wrong type etc.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetElementFromTrace<T>(Document document)
            where T : Autodesk.Revit.DB.Element
        {
            var elementUUID = GetElementUUIDFromTrace(document);

            if (elementUUID == null)
                return null;

            T ret;

            if (Elements.ElementUtils.TryGetElement(document, elementUUID.UUID, out ret))
                return ret;
            else
                return null;
        }




        /// <summary>
        /// Delete a possibly outdated Revit Element and set new element for trace.  
        /// This method should be called if the element could not be mutated on a 
        /// second run and the old value must be destroyed.  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static void CleanupAndSetElementForTrace(Document document, Element newElement)
        {
            if (!IsEnabled) return;

            // if the element id has changed on a subsequent run, that means we
            // couldn't mutate the element - hence we need to delete the old
            // element
            var oldId = GetElementUUIDFromTrace(document);
            if (oldId != null && oldId.UUID != newElement.UniqueId)
            {
                DocumentManager.Instance.DeleteElement(oldId);
            }

            SetElementForTrace(newElement);
        }




        /// <summary>
        /// Raw method for setting data into the trace cache, the user of this method is reponsible for handling
        /// the interpretation of the data
        /// </summary>
        /// <param name="data"></param>
        public static void SetRawDataForTrace(ISerializable data)
        {
            if (!IsEnabled) return;
            TraceUtils.SetTraceData(REVIT_TRACE_ID, data);
        }

        /// <summary>
        /// Raw method for getting data from the trace cache, the user is responsible for handling the interpretation
        /// of the data
        /// </summary>
        public static ISerializable GetRawDataFromTrace()
        {
            return TraceUtils.GetTraceData(REVIT_TRACE_ID);
        }

        /// <summary>
        /// Get the Element of type T and the raw traceData of type K from Thread Local Storage.
        /// Requires that K inherits from SerializableId so the element can be retrieved from the Revit Document
        /// </summary>
        /// <returns></returns>
        public static Tuple<TElement, TId> GetElementAndTraceData<TElement, TId>(Document document)
            where TElement : Autodesk.Revit.DB.Element
            where TId: SerializableId
        {
            var id = ElementBinder.GetRawDataFromTrace();
            if (id == null)
                return null;

            var traceData = id as TId;
            if (traceData == null)
                return null;

            var elementId = traceData.IntID;
            var uuid = traceData.StringID;

            var element = default(TElement);
            
            // if we can't get the element, return null
            if (!document.TryGetElement(uuid,out element))
                return null;

            return new Tuple<TElement, TId>(element, traceData);
        }
        /// <summary>
        /// This function gets the nodes which are binding with the elements which have the
        /// given element IDs
        /// </summary>
        /// <param name="ids">The given element IDs</param>
        /// <param name="workspace">The workspace model for the nodes</param>
        /// <param name="engine">The engine controller</param>
        /// <returns>the related nodes</returns>
        public static IEnumerable<NodeModel> GetNodesFromElementIds(IEnumerable<ElementId> ids,
            WorkspaceModel workspace, EngineController engine)
        {
            List<NodeModel> nodes = new List<NodeModel>();
            if (!ids.Any())
                return nodes.AsEnumerable();

            RuntimeCore runtimeCore = null;
            if (engine != null && (engine.LiveRunnerCore != null))
                runtimeCore = engine.LiveRunnerRuntimeCore;

            if (runtimeCore == null)
                return null;

            // Selecting all nodes that are either a DSFunction,
            // a DSVarArgFunction or a CodeBlockNodeModel into a list.
            var nodeGuids = workspace.Nodes.Where((n) =>
            {
                return (n is DSFunction
                        || (n is DSVarArgFunction)
                        || (n is CodeBlockNodeModel));
            }).Select((n) => n.GUID);

            var nodeTraceDataList = runtimeCore.RuntimeData.GetCallsitesForNodes(nodeGuids, runtimeCore.DSExecutable);

            bool areElementsFoundForThisNode;
            foreach (Guid guid in nodeTraceDataList.Keys)
            {
                areElementsFoundForThisNode = false;
                foreach (CallSite cs in nodeTraceDataList[guid])
                {
                    foreach (CallSite.SingleRunTraceData srtd in cs.TraceData)
                    {
                        List<ISerializable> traceData = srtd.RecursiveGetNestedData();

                        foreach (ISerializable thingy in traceData)
                        {
                            SerializableId sid = thingy as SerializableId;

                            if (sid != null)
                            {
                                foreach (var id in ids)
                                {
                                    if (sid.IntID == id.IntegerValue)
                                    {
                                        areElementsFoundForThisNode = true;
                                        break;
                                    }
                                }

                                if (areElementsFoundForThisNode)
                                {
                                    NodeModel inm =
                                        workspace.Nodes.Where((n) => n.GUID == guid).FirstOrDefault();
                                    nodes.Add(inm);
                                    break;
                                }
                            }
                        }

                        if (areElementsFoundForThisNode)
                            break;
                    }

                    if (areElementsFoundForThisNode)
                        break;
                }
            }

            return nodes.AsEnumerable();
        }

        /// <summary>
        /// Sets the freeze state of the elements associated with that node.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <param name="node">The node.</param>
        /// <param name="engine">The engine.</param>
        public static void SetElementFreezeState(WorkspaceModel workspace, NodeModel node, EngineController engine)
        {
            RuntimeCore runtimeCore = null;
            if (engine != null && (engine.LiveRunnerCore != null))
                runtimeCore = engine.LiveRunnerRuntimeCore;

            if (runtimeCore == null || node == null)
                return;

            // Selecting all nodes that are either a DSFunction,
            // a DSVarArgFunction or a CodeBlockNodeModel into a list.
            var nodeGuids = workspace.Nodes.Where((n) =>
            {
                return (n is DSFunction
                        || (n is DSVarArgFunction)
                        || (n is CodeBlockNodeModel));
            }).Where((n) => n.GUID == node.GUID).Select((x) => x.GUID);

            var nodeTraceDataList = runtimeCore.RuntimeData.GetCallsitesForNodes(nodeGuids, runtimeCore.DSExecutable);
            foreach (Guid guid in nodeTraceDataList.Keys)
            {
                foreach (CallSite cs in nodeTraceDataList[guid])
                {
                    foreach (CallSite.SingleRunTraceData srtd in cs.TraceData)
                    {
                        List<ISerializable> traceData = srtd.RecursiveGetNestedData();

                        foreach (ISerializable thingy in traceData)
                        {
                            SerializableId sid = thingy as SerializableId;

                            if (sid != null)
                            {
                                //Get the Autodesk.Revit.Element.
                                Element el;
                                DocumentManager.Instance.CurrentDBDocument.TryGetElement(new ElementId(sid.IntID),
                                    out el);

                                //Get the Revit Element wrapper.
                                if (el != null)
                                {
                                    dynamic elem =
                                        ElementIDLifecycleManager<int>.GetInstance().GetFirstWrapper(el.Id.IntegerValue);
                                    if (elem != null)
                                    {
                                        elem.IsFrozen = node.IsFrozen;
                                    }
                                }

                            }
                        }
                    }
                }
            }
        }
    }

}
