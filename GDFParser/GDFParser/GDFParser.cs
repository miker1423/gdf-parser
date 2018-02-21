using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using QuickGraph;

using GDFParser.Models;

namespace GDFParser
{
    public class GDFParser
    {
        private string[] _lines;
        private ConcurrentBag<GDFRelation> _relations = new ConcurrentBag<GDFRelation>();
        private ConcurrentDictionary<string, GDFNode> _nodes = new ConcurrentDictionary<string, GDFNode>();

        public IDictionary<string, GDFNode> Nodes
        {
            get => _nodes;
        }

        public IEnumerable<GDFRelation> Relations
        {
            get => _relations.AsEnumerable();
        }

        public void LoadFile(string path)
            => _lines = File.ReadAllLines(path);

        public AdjacencyGraph<GDFNode, GDFEdge> GetGraph()
        {
            var edges = new ConcurrentBag<GDFEdge>();

            Parallel.ForEach(_relations, (relation) =>
            {
                var fromNode = _nodes[relation.From];
                var toNode = _nodes[relation.To];

                edges.Add(new GDFEdge
                {
                    Source = fromNode,
                    Target = toNode
                });
            });

            var graph = new AdjacencyGraph<GDFNode, GDFEdge>();
            var vertexCount = graph.AddVertexRange(Nodes.Values);
            var edgesCount = graph.AddEdgeRange(edges.AsEnumerable());

            if (vertexCount != _nodes.Count && edgesCount != edges.Count)
            {
                throw new Exception("Something went wrong when building the graph");
            }

            return graph;
        }


        public void Parse()
        {
            Parallel.ForEach(_lines, (line, state) =>
            {
                if (!(line[0] == 'n' || line[0] == 'e'))
                {
                    var splittedLine = Split(line.AsSpan());
                    if (splittedLine.Length == 3)
                    {
                        BuildRelation(splittedLine);
                    }
                    else if (splittedLine.Length == 9)
                    {
                        BuildNode(splittedLine);
                    }
                }
            });
        }

        private Span<string> Split(ReadOnlySpan<char> line)
        {
            var stringBuilder = new StringBuilder();
            var stack = new Stack<char>();
            var list = new List<string>();

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '\"')
                {
                    if (stack.Count != 0)
                    {
                        stack.Pop();
                    }
                    else
                    {
                        stack.Push(line[i]);
                    }
                }
                else if (line[i] == ',')
                {
                    if (stack.Count != 0)
                    {
                        stringBuilder.Append(line[i]);
                    }
                    else
                    {
                        list.Add(stringBuilder.ToString());
                        stringBuilder.Clear();
                    }
                }
                else
                {
                    stringBuilder.Append(line[i]);
                }
            }

            list.Add(stringBuilder.ToString());
            return list.ToArray().AsSpan();
        }

        private void BuildNode(ReadOnlySpan<string> span)
        {
            var added = _nodes.TryAdd(span[0], new GDFNode
            {
                Name = span[0],
                Username = span[1],
                Label = span[2],
                Category = span[3],
                PostActivity = float.Parse(span[4]),
                FanCount = long.Parse(span[5]),
                TalkingAboutCount = long.Parse(span[6]),
                UsersCanPost = BoolConvert(span[7]),
                Link = span[8]
            });
        }

        private void BuildRelation(ReadOnlySpan<string> span)
        {
            _relations.Add(new GDFRelation
            {
                From = span[0],
                To = span[1],
                IsDirected = bool.Parse(span[2])
            });
        }

        private bool BoolConvert(string str)
            => str == "yes";
    }
}
