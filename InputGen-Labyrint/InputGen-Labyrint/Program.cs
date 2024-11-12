using System.Text;

namespace InputGen_Labyrint
{
    internal class Program
    {
        class Position
        {
            public int x;
            public int y;
            public Position(int _x, int _y)
            {
                x = _x;
                y = _y;
            }
            public override string ToString()
            {
                return $"{x} {y}";
            }
        }
        class Node
        {
            public Position pos;
            public int iter;
            public int cost;
            public int minCost;
            public Node? lastNode;
            public Node(int _cost, int x, int y)
            {
                iter = -1;
                cost = _cost;
                pos = new(x, y);
                lastNode = null;
            }

        }

        int WIDTH = 10;
        int HEIGHT = 10;
        int PATHS = 20;

        int wallChance = 20;
        int routeChance = 60;

        int iterations = 0;
        Node[][]? lab;
        StringBuilder? inputBuilder;
        StringBuilder? outputBuilder;

        static void Main(string[] args)
        {
            Program p = new();
            p.GetInput();
            p.GenerateLab();
        }
        void GetInput()
        {
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt")))
            {
                string[] input = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt"));
                WIDTH = int.Parse(input[0]);
                HEIGHT = int.Parse(input[1]);
                PATHS = int.Parse(input[2]);

                wallChance = int.Parse(input[4]); 
                if (wallChance >= 100)
                {
                    throw new ArgumentException("Wall chance cannot be 100");
                }
                routeChance = int.Parse(input[5]);
            }
        }

        void GenerateLab()
        {
            inputBuilder = new();
            Random r = new();
            lab = new Node[HEIGHT][];
            for (int i = 0; i < HEIGHT; i++)
            {
                lab[i] = new Node[WIDTH];
                for (int j = 0; j < WIDTH; j++)
                {
                    int cost = r.Next(1, 101);
                    if(cost < wallChance)
                    {
                        inputBuilder.Append("# ");
                        cost = 0;
                    }
                    else if(cost < routeChance)
                    {
                        inputBuilder.Append(". ");
                        cost = 1;
                    }
                    else
                    {
                        inputBuilder.Append("f ");
                        cost = 4;
                    }

                    lab[i][j] = new(cost, i, j);
                }
                inputBuilder.Append('\n');
            }
            inputBuilder.Append("-\n");
            outputBuilder = new();
            for (int i = 0; i < PATHS; i++)
            {
                outputBuilder.AppendLine(FindRoute(PositionGen(r), PositionGen(r)).ToString());
                iterations++;
                Console.Clear();
                Console.WriteLine($"{iterations}/{PATHS}");
            }
            File.WriteAllText("input.in", inputBuilder.ToString());
            File.WriteAllText("output.out", outputBuilder.ToString());
            Console.WriteLine("Finished");
            
            Console.ReadKey();
        }

        Position PositionGen(Random r)
        {
            Position p = new(-1, -1);
            do
            {
                p.x = r.Next(1, HEIGHT);
                p.y = r.Next(1, WIDTH);
            } while (lab[p.x][p.y].cost == 0);
            return p;
        }

        StringBuilder FindRoute(Position start, Position end)
        {
            inputBuilder.Append($"{start.x} {start.y} {end.x} {end.y}\n");

            Queue<Node> queue = new Queue<Node>();
            lab[start.x][start.y].minCost = 0;
            lab[start.x][start.y].iter = iterations;
            lab[start.x][start.y].lastNode = null;
            queue.Enqueue(lab[start.x][start.y]);
            
            while (queue.Count > 0)
            {
                for(int i = queue.Count; i > 0; i--)
                {
                    Node n = queue.Dequeue();
                    for(int j = 0; j < 4; j++)
                    {
                        Position p = new(n.pos.x, n.pos.y);
                        switch (j)
                        {
                            case 0:
                                p.x += 1;
                                if (p.x == HEIGHT)
                                    p.x = 0;
                                break;
                            case 1:
                                p.x -= 1;
                                if (p.x == -1)
                                    p.x = HEIGHT-1;
                                break;
                            case 2:
                                p.y += 1;
                                if (p.y == WIDTH)
                                    p.y = 0;
                                break;
                            case 3:
                                p.y -= 1;
                                if (p.y == -1)
                                    p.y = WIDTH-1;
                                break;
                        }
                        if (lab[p.x][p.y].cost > 0)
                        {
                            int cost = n.minCost + n.cost;
                            if (lab[p.x][p.y].iter < iterations || lab[p.x][p.y].minCost > cost)
                            {
                                lab[p.x][p.y].minCost = cost;
                                lab[p.x][p.y].iter = iterations;
                                lab[p.x][p.y].lastNode = n;
                                queue.Enqueue(lab[p.x][p.y]);
                            }
                        }
                    }
                }

            }
            if (lab[end.x][end.y].iter == iterations)
            {
                StringBuilder s = new(lab[end.x][end.y].minCost.ToString());
                s.Append(NodePath(lab[end.x][end.y]));
                return s;
            }
            return new StringBuilder((-1).ToString());
        }

        string NodePath(Node n)
        {
            if (n.lastNode != null && n.lastNode.iter == iterations)
                return NodePath(n.lastNode) + " " + n.pos.ToString();
            else
                return " pp" + n.pos.ToString();
        }
    }
}
