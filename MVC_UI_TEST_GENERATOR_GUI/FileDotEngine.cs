using System;
using System.Diagnostics;
using System.IO;
using DataStorageLibrary;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace MVC_UI_TEST_GENERATOR_GUI {
    public sealed class FileDotEngine : IDotEngine {
        private readonly string _graphVizLocation;

        public FileDotEngine(string graphVizLocation) {
            _graphVizLocation = graphVizLocation;
        }

        public string Run(GraphvizImageType imageType, string dot, string outputFileName) {
            dot = dot.Insert(dot.IndexOf("{\r\n") + 3,
                "    graph[rankdir=LR, center=true, nodesep=0.1, ranksep=0.3]\r\n" +
                "    node[fontname=\"Courier-Bold\", fontsize=10, width=1, height=0.4, fixedsize=false]\r\n" +
                "    edge[arrowsize=0.6, arrowhead=vee]\r\n");


            foreach (string vertex in ApplicationData.Instance.GetAllViewNames()) {
                int startIndex = dot.IndexOf("[label=\"" + vertex + "\"") + 9 + vertex.Length;
                dot = dot.Insert(startIndex, ", color=\"#00FF00\"");
            }

            foreach (string vertex in ApplicationData.Instance.GetAllActionNames()) {
                int startIndex = dot.IndexOf("[label=\"" + vertex + "\"") + 9 + vertex.Length;
                dot = dot.Insert(startIndex, ", color=\"#0000FF\"");
            }

            foreach (string vertex in ApplicationData.Instance.GetViewDataGraph().GetGraph().Vertices) {
                int startIndex = dot.IndexOf("[label=\"" + vertex + "\"") + 9 + vertex.Length;
                dot = dot.Insert(startIndex, ", color=\"#FF0000\"");
            }


            File.WriteAllText(outputFileName + ".dot", dot);


            string args = String.Format(" -T{0}  {1}.dot -o {1}.{0}", imageType.ToString().ToLower(),
                outputFileName);
            // assumes dot.exe is in default install location


            ProcessStartInfo start =
                new ProcessStartInfo {
                    FileName = _graphVizLocation,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = args
                };
            Process process = Process.Start(start);
            process.WaitForExit();
            return dot;
        }
    }
}