using System;
using System.Collections.Generic;

namespace CircuitSim
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Constructing circuit");
            CircuitEngine circuit = new CircuitEngine();
            Console.WriteLine($"{circuit.Count} gates");

            Console.WriteLine("Running simulation");

            circuit.Iterate();
            circuit.Stimulate(0, 0);
            circuit.Stimulate(1, 0);
            circuit.Stimulate(2, 0);
            circuit.Iterate();
            Console.Write(circuit.Probe(8) + "\n");

            circuit.Stimulate(0, 0);
            circuit.Stimulate(1, 0);
            circuit.Stimulate(2, 5);
            Console.Write(circuit.Probe(8) + "\t");
            circuit.Iterate();
            Console.Write(circuit.Probe(8) + "\t");
            circuit.Iterate(0);
            Console.Write(circuit.Probe(8) + "\n");

            circuit.Stimulate(0, 0);
            circuit.Stimulate(1, 5);
            circuit.Stimulate(2, 0);
            circuit.Iterate(0);
            Console.Write(circuit.Probe(8) + "\n");

            circuit.Stimulate(0, 0);
            circuit.Stimulate(1, 5);
            circuit.Stimulate(2, 5);
            circuit.Iterate(0);
            Console.Write(circuit.Probe(8) + "\n");

            circuit.Stimulate(0, 5);
            circuit.Stimulate(1, 0);
            circuit.Stimulate(2, 0);
            circuit.Iterate(0);
            Console.Write(circuit.Probe(8) + "\n");

            circuit.Stimulate(0, 5);
            circuit.Stimulate(1, 0);
            circuit.Stimulate(2, 5);
            circuit.Iterate(0);
            Console.Write(circuit.Probe(8) + "\n");

            circuit.Stimulate(0, 5);
            circuit.Stimulate(1, 5);
            circuit.Stimulate(2, 0);
            circuit.Iterate(0);
            Console.Write(circuit.Probe(8) + "\n");

            circuit.Stimulate(0, 5);
            circuit.Stimulate(1, 5);
            circuit.Stimulate(2, 5);
            circuit.Iterate(0);
            Console.Write(circuit.Probe(8) + "\n");

            Console.WriteLine("Running performance test");
            Random RNG = new Random();
            var watch = System.Diagnostics.Stopwatch.StartNew();

            double cycles = 1000000;
            double subcycles = 1000;
            for (int i = 0; i < cycles; i++)
            {
                circuit.Stimulate(0, (RNG.Next(100) > 50 ? 0 : 5));
                circuit.Stimulate(1, (RNG.Next(100) > 50 ? 0 : 5));
                circuit.Stimulate(2, (RNG.Next(100) > 50 ? 0 : 5));
                circuit.Iterate((int)subcycles);
                //Console.Write(circuit.Probe(8) + " ");
            }
            watch.Stop();
            double elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"{elapsedMs / (cycles * subcycles)}ms per cycle");
            Console.WriteLine($"{(int)Math.Round((cycles * subcycles) / (elapsedMs / 1000.0))} cycles per second");

            Console.Write(circuit.Probe(8) + "\n");
            Console.ReadLine();
        }
    }

    public class CircuitEngine
    {
        public int Count { get => HighestId; }
        private Queue<int> DiscardedIDs = new Queue<int>();
        private int HighestId = -1;
        private LogicGate[] Gates = new LogicGate[1];
        private double[] Nets = new double[1];
        private double MaxVoltage = 5.0;
        private double MinVoltage = 0.0;
        private double HighThreshold = 1.8;
        private double LowThreshold = 1.5;

        public CircuitEngine()
        {
            AddGate(new LogicGate()
            {
                Mode = LogicGate.Modes.OR,
                Inputs = new int[] { 3, 7 },
                Outputs = new int[] { 8 },
            });
            AddGate(new LogicGate()
            {
                Mode = LogicGate.Modes.Buffer,
                Inputs = new int[] { 0 },
                Outputs = new int[] { 3 },
            });
            AddGate(new LogicGate()
            {
                Mode = LogicGate.Modes.AND,
                Inputs = new int[] { 6, 5 },
                Outputs = new int[] { 7 },
            });
            AddGate(new LogicGate()
            {
                Mode = LogicGate.Modes.Buffer,
                Inputs = new int[] { 2 },
                Outputs = new int[] { 5 },
            });
            AddGate(new LogicGate()
            {
                Mode = LogicGate.Modes.Buffer,
                Inputs = new int[] { 1 },
                Outputs = new int[] { 4 },
            });
            AddGate(new LogicGate()
            {
                Mode = LogicGate.Modes.NOT,
                Inputs = new int[] { 4 },
                Outputs = new int[] { 6 },
            });
        }

        public void Stimulate(int Net, double Value)
        {
            Nets[Net] = Value;
        }

        public double Probe(int Net)
        {
            return Nets[Net];
        }

        public void AddGate(LogicGate Gate)
        {
            int id = 0;
            if (DiscardedIDs.Count == 0)
            {
                HighestId++;
                id = HighestId;
                if (HighestId >= Gates.Length)
                {
                    LogicGate[] newGates = new LogicGate[Gates.Length * 2];
                    Array.Copy(Gates, newGates, Gates.Length);
                    Gates = newGates;
                }
            }
            else
            {
                id = DiscardedIDs.Dequeue();
            }

            foreach (int netId in Gate.Inputs)
            {
                while (netId >= Nets.Length - 1)
                {
                    double[] newNets = new double[Nets.Length * 2];
                    Array.Copy(Nets, newNets, Gates.Length);
                    Nets = newNets;
                }
            }

            foreach (int netId in Gate.Outputs)
            {
                while (netId >= Nets.Length - 1)
                {
                    double[] newNets = new double[Nets.Length * 2];
                    Array.Copy(Nets, newNets, Gates.Length);
                    Nets = newNets;
                }
            }

            Gates[id] = Gate;
        }

        public void Iterate(int Iterations)
        {
            int iterations = Iterations;
            if (iterations <= 0)
            {
                iterations = HighestId;
            }
            for (int i = 0; i < iterations; i++)
            {
                Iterate();
            }
        }

        public void Iterate()
        {
            for (int i = 0; i <= HighestId; i++)
            {
                if (Gates[i].Mode == LogicGate.Modes.Buffer)
                {
                    if (Nets[Gates[i].Inputs[0]] > HighThreshold)
                    {
                        Nets[Gates[i].Outputs[0]] = Math.Clamp(Nets[Gates[i].Outputs[0]] + MaxVoltage, MinVoltage, MaxVoltage);
                    }
                    else
                    {
                        Nets[Gates[i].Outputs[0]] = Math.Clamp(Nets[Gates[i].Outputs[0]] - MaxVoltage, MinVoltage, MaxVoltage);
                    }
                }
                else if (Gates[i].Mode == LogicGate.Modes.NOT)
                {
                    if (Nets[Gates[i].Inputs[0]] > HighThreshold)
                    {
                        Nets[Gates[i].Outputs[0]] = Math.Clamp(Nets[Gates[i].Outputs[0]] - MaxVoltage, MinVoltage, MaxVoltage);
                    }
                    else
                    {
                        Nets[Gates[i].Outputs[0]] = Math.Clamp(Nets[Gates[i].Outputs[0]] + MaxVoltage, MinVoltage, MaxVoltage);
                    }
                }
                else if (Gates[i].Mode == LogicGate.Modes.AND)
                {
                    if (Nets[Gates[i].Inputs[0]] > HighThreshold && Nets[Gates[i].Inputs[1]] > HighThreshold)
                    {
                        Nets[Gates[i].Outputs[0]] = Math.Clamp(Nets[Gates[i].Outputs[0]] + MaxVoltage, MinVoltage, MaxVoltage);
                    }
                    else
                    {
                        Nets[Gates[i].Outputs[0]] = Math.Clamp(Nets[Gates[i].Outputs[0]] - MaxVoltage, MinVoltage, MaxVoltage);
                    }
                }
                else if (Gates[i].Mode == LogicGate.Modes.OR)
                {
                    if (Nets[Gates[i].Inputs[0]] > HighThreshold || Nets[Gates[i].Inputs[1]] > HighThreshold)
                    {
                        Nets[Gates[i].Outputs[0]] = Math.Clamp(Nets[Gates[i].Outputs[0]] + MaxVoltage, MinVoltage, MaxVoltage);
                    }
                    else
                    {
                        Nets[Gates[i].Outputs[0]] = Math.Clamp(Nets[Gates[i].Outputs[0]] - MaxVoltage, MinVoltage, MaxVoltage);
                    }
                }
                else if (Gates[i].Mode == LogicGate.Modes.NAND)
                {
                    if (Nets[Gates[i].Inputs[0]] > HighThreshold && Nets[Gates[i].Inputs[1]] > HighThreshold)
                    {
                        Nets[Gates[i].Outputs[0]] = Math.Clamp(Nets[Gates[i].Outputs[0]] - MaxVoltage, MinVoltage, MaxVoltage);
                    }
                    else
                    {
                        Nets[Gates[i].Outputs[0]] = Math.Clamp(Nets[Gates[i].Outputs[0]] + MaxVoltage, MinVoltage, MaxVoltage);
                    }
                }
                else if (Gates[i].Mode == LogicGate.Modes.NOR)
                {
                    if (Nets[Gates[i].Inputs[0]] > HighThreshold || Nets[Gates[i].Inputs[1]] > HighThreshold)
                    {
                        Nets[Gates[i].Outputs[0]] = Math.Clamp(Nets[Gates[i].Outputs[0]] - MaxVoltage, MinVoltage, MaxVoltage);
                    }
                    else
                    {
                        Nets[Gates[i].Outputs[0]] = Math.Clamp(Nets[Gates[i].Outputs[0]] + MaxVoltage, MinVoltage, MaxVoltage);
                    }
                }
                else if (Gates[i].Mode == LogicGate.Modes.XOR)
                {
                    if (Nets[Gates[i].Inputs[0]] > HighThreshold != Nets[Gates[i].Inputs[1]] > HighThreshold)
                    {
                        Nets[Gates[i].Outputs[0]] = Math.Clamp(Nets[Gates[i].Outputs[0]] + MaxVoltage, MinVoltage, MaxVoltage);
                    }
                    else
                    {
                        Nets[Gates[i].Outputs[0]] = Math.Clamp(Nets[Gates[i].Outputs[0]] - MaxVoltage, MinVoltage, MaxVoltage);
                    }
                }
            }
        }

        public struct LogicGate
        {
            public int[] Inputs;
            public int[] Outputs;
            public Modes Mode;

            public enum Modes
            {
                Buffer,
                NOT,
                AND,
                OR,
                NAND,
                NOR,
                XOR
            }

            public override string ToString()
            {
                string result = "";

                if (Mode == Modes.Buffer) { result += "Buffer"; }
                if (Mode == Modes.NOT) { result += "NOT"; }
                if (Mode == Modes.AND) { result += "AND"; }
                if (Mode == Modes.OR) { result += "OR"; }
                if (Mode == Modes.NAND) { result += "NAND"; }
                if (Mode == Modes.NOR) { result += "NOR"; }
                if (Mode == Modes.XOR) { result += "XOR"; }

                return result;
            }
        }
    }
}