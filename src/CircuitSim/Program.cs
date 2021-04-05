using System;
using System.Collections.Generic;

namespace CircuitSim
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine(CSDL.FilterComments(@"
// Comment
/* comment */
/*
Constructs:
  Wires: one or more circuit nodes in a group
  Inputs: Wires that are strictly an input
  Outputs: Wires that are strictly an output
  Gates: Connections between wires

Compilation Process:
1. Enumate modules
2. Construct module dependency graph
3. Enumate nodes (inputs, outputs, and wires)
4. Enumerate gates
5. Construct circuit object
*/

module top(input A, input B, output C, output D)
{
    wire E;
    wire [2:0] F;

    E = ~A;
    F[1:0] = A & B; //F has 2 lines, access 1 at position 0
    F[1:1] = ~F[1:0];// access 1 at position 1
    C = NAND(A=E, B=F[1:0]).C; // referencing a module
    D = F[1:0] | F[1:1];
}

module NAND(input A, input B, output C)
{
    C = ~(A & B);
}
"));

            /*
            Console.WriteLine("Constructing circuit");
            Circuit circuit = new Circuit();
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

            double cycles = 5000000.0 / (double)circuit.Count;
            double subcycles = Math.Max(5000.0 / (double)circuit.Count, circuit.Count);
            for (int i = 0; i < cycles; i++)
            {
                circuit.Stimulate(0, (RNG.Next(100) > 50 ? 0 : 5));
                circuit.Stimulate(1, (RNG.Next(100) > 50 ? 0 : 5));
                circuit.Stimulate(2, (RNG.Next(100) > 50 ? 0 : 5));
                circuit.Iterate((int)subcycles);
                //Console.Write(circuit.Probe(8) + " ");
                if (i % 10000 == 0) { Console.Write('|'); }
            }
            watch.Stop();
            Console.Write('\n');
            double elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"{elapsedMs / (cycles * subcycles)}ms per cycle");
            Console.WriteLine($"{(int)Math.Round((cycles * subcycles) / (elapsedMs / 1000.0))} cycles per second");

            Console.Write(circuit.Probe(8) + "\n");

            */
            Console.ReadLine();
        }
    }
}