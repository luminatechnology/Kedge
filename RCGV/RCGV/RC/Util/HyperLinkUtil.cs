using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC.Util
{
    public class HyperLinkUtil<Graph> where Graph : PXGraph, new()
    {
        public HyperLinkUtil(object current, bool isNewWindow){
            Graph graph = PXGraph.CreateInstance<Graph>();
            string viewName = graph.PrimaryView;
            graph.GetPrimaryCache().Current = current;
            if (graph.GetPrimaryCache().Current != null)
            {
                if (isNewWindow)
                    throw new PXRedirectRequiredException(graph, typeof(Graph).Name)
                    {
                        Mode = PXBaseRedirectException.WindowMode.NewWindow
                    };
                else throw new PXRedirectRequiredException(graph, typeof(Graph).Name);
            }
        }
    }
}
