using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public static class Roll
{
	private static Random random = new Random();

	public static int Dice(int number = 3, int sides = 6, int modifier = 0, int low = int.MinValue, int high = int.MaxValue)
	{
		if (number <= 0 || sides <= 0) return 0;

		int total = Enumerable.Range(0, number)
			.Sum(_ => random.Next(1, sides + 1)) + modifier;

		return Math.Clamp(total, low, high);
	}

	public static float Vary(float amount, float factor = 0.05f)
	{
		float fudge = (float)random.NextDouble() * (2 * factor) + (1 - factor);
		return amount * fudge;
	}

	public static T Seek<T>(Dictionary<T, int> items, int? roll = null)
	{
		if (roll == null) roll = Dice();

		var sortedItems = items.OrderBy(x => x.Value).ToList();

		foreach (var item in sortedItems)
		{
			if (roll <= item.Value)
			{
				return item.Key;
			}
		}

		return sortedItems.Last().Key;
	}

	public static float Search(Dictionary<float, float> dictionary, float searchValue)
	{
		var sortedKeys = dictionary.Keys.OrderBy(k => k).ToList();

		foreach (float key in sortedKeys)
		{
			if (searchValue <= key)
			{
				return dictionary[key];
			}
		}

		return dictionary[sortedKeys.Last()];
	}

	private static float ConvertToFloat(object value)
	{
		return value switch
		{
			float f => f,
			int i => i,
			double d => (float)d,
			string s when float.TryParse(s, out float result) => result,
			_ => throw new ArgumentException($"Cannot convert {value} to float")
		};
	}

	public static T Choice<T>(T[] options, float[] probabilities)
	{
		if (options.Length != probabilities.Length)
			throw new ArgumentException("Options and probabilities must have the same length");

		float total = probabilities.Sum();
		float randomValue = (float)random.NextDouble() * total;
		float cumulativeProb = 0;

		for (int i = 0; i < options.Length; i++)
		{
			cumulativeProb += probabilities[i];
			if (randomValue <= cumulativeProb)
			{
				return options[i];
			}
		}

		return options[options.Length - 1];
	}
}
