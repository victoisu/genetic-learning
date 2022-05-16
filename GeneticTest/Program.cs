using System;
using System.Linq;

namespace GeneticTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var learner = new CountLearner();
			learner.GeneratePopulation(1000);
			for (int i = 0; i < 1000; i++)
			{
				learner.NextGeneration(0.1, 0.8);
				//Console.WriteLine($"Best: {learner.Fitness(learner.BestInstance)}");
			}

			Console.WriteLine("Done!");
			Console.ReadKey();
		}
	}
}
