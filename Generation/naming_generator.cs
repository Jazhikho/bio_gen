using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using BioStructures;

public partial class NamingGenerator : Node
{
	public static NamingGenerator Instance { get; private set; }
	
	private static readonly string[] Prefixes = {
		"Xeno", "Neo", "Mega", "Micro", "Crypto", "Proto", "Pseudo", "Para", "Meta", "Hyper",
		"Ultra", "Super", "Sub", "Anti", "Quasi", "Semi", "Hemi", "Iso", "Mono", "Poly"
	};
	
	private static readonly string[] Roots = {
		"saur", "pod", "derm", "therm", "morph", "phyte", "zoa", "ceph", "arthro", "ichthy",
		"ornith", "mamma", "insect", "myc", "bact", "vir", "phag", "vor", "troph", "phyll"
	};
	
	private static readonly string[] Suffixes = {
		"us", "is", "um", "ix", "ox", "ax", "ex", "on", "oid", "idae",
		"inae", "ini", "ina", "ita", "ites", "opsis", "ella", "ula", "arium", "odon"
	};

	public override void _EnterTree()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			QueueFree();
		}
	}

	public override void _ExitTree()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	public void NameCreature(Creature creature)
	{
		creature.Name = GenerateScientificName(creature);
	}

	private string GenerateScientificName(Creature creature)
	{
		string prefix = SelectNameComponent(Prefixes, creature);
		string root = SelectNameComponent(Roots, creature);
		string suffix = SelectNameComponent(Suffixes, creature);

		// Sometimes add a second root for more complex names
		if (Roll.Dice(1) == 6)
		{
			string secondRoot = SelectNameComponent(Roots, creature);
			root = $"{root}{secondRoot}";
		}

		return $"{prefix}{root}{suffix}";
	}

	private string SelectNameComponent(string[] components, Creature creature)
	{
		var weightedComponents = new Dictionary<string, float>();
		
		foreach (var component in components)
		{
			float weight = CalculateComponentWeight(component, creature);
			weightedComponents[component] = weight;
		}

		// Normalize weights
		float totalWeight = weightedComponents.Values.Sum();
		var normalizedWeights = weightedComponents.Values.Select(w => w / totalWeight).ToArray();
		
		return Roll.Choice(components, normalizedWeights);
	}

	private float CalculateComponentWeight(string component, Creature creature)
	{
		float weight = 1.0f;

		// Adjust weights based on creature characteristics
		switch (component.ToLower())
		{
			case var s when s.Contains("mega"):
				weight *= creature.SizeCategory == "Large" ? 2.0f : 0.5f;
				break;
			case var s when s.Contains("micro"):
				weight *= creature.SizeCategory == "Small" ? 2.0f : 0.5f;
				break;
			case var s when s.Contains("therm"):
				weight *= creature.ChemicalBasis == "Thermosynthetic" ? 2.0f : 0.5f;
				break;
			case var s when s.Contains("phyte"):
				weight *= creature.TrophicLevel == "Photosynthetic" ? 2.0f : 0.5f;
				break;
			case var s when s.Contains("pod"):
				weight *= creature.Locomotion == "Walking" ? 1.5f : 0.7f;
				break;
			case var s when s.Contains("phag"):
				weight *= creature.TrophicLevel.Contains("Carnivore") ? 1.5f : 0.7f;
				break;
			// Add more cases as needed
		}

		return weight;
	}
}
