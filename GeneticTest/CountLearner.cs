using GeneticLearning;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticTest
{
	class CountLearner : GeneticLearner<List<bool>>
	{
		public override int GenomeSize => 1000;

		public override double Fitness(List<bool> instance)
		{
			var trues = instance.Count(i => i);
			var falses = GenomeSize - trues;
			return 96-abs(96 - trues);
		}

		protected override List<bool> GenesToInstance(BitArray genes)
		{
			var res = new List<bool>(GenomeSize);
			for (int i = 0; i < GenomeSize; i++)
			{
				res.Add(genes[i]);
			}
			return res;
		}

		protected override BitArray InstanceToGenes(List<bool> instance)
		{
			var res = new BitArray(GenomeSize);
			for (int i = 0; i < GenomeSize; i++)
			{
				res[i] = instance[i];
			}
			return res;
		}

		private int abs(int x)
		{
			return x > 0 ? x : -x;
		}
	}
}
