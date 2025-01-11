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

	private static readonly string[] BiomePrefixes = {
		"Great", "Lesser", "Northern", "Southern", "Eastern", "Western", "Central", "Coastal",
		"Inner", "Outer", "Upper", "Lower", "High", "Low", "Deep", "Shallow"
	};

	private static readonly string[] BiomeSuffixes = {
		"Lands", "Region", "Zone", "Territory", "Expanse", "Realm", "Domain", "Basin",
		"Valley", "Plains", "Heights", "Depths", "Waters", "Reaches", "Fields", "Shores"
	};

	private HashSet<string> usedBiomeNames = new HashSet<string>();
	private HashSet<string> usedSpeciesNames = new HashSet<string>();

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
		string name;
		do
		{
			name = GenerateScientificName(creature);
		} while (usedSpeciesNames.Contains(name));
		
		usedSpeciesNames.Add(name);
		creature.Name = name;
	}

	public string NameBiome(string habitatType)
	{
		string biomeName;
		do
		{
			biomeName = GenerateBiomeName(habitatType);
		} while (usedBiomeNames.Contains(biomeName));
		
		usedBiomeNames.Add(biomeName);
		return biomeName;
	}

	private string GenerateBiomeName(string habitatType)
	{
		float[] prefixProbs = Enumerable.Repeat(1.0f, BiomePrefixes.Length).ToArray();
		float[] suffixProbs = Enumerable.Repeat(1.0f, BiomeSuffixes.Length).ToArray();
		
		string prefix = Roll.Choice(BiomePrefixes, prefixProbs);
		string suffix = Roll.Choice(BiomeSuffixes, suffixProbs);
		
		// Sometimes add a descriptor based on habitat type
		string descriptor = "";
		if (Roll.Dice(1) > 3)
		{
			string[] descriptors;
			switch (habitatType.ToLower())
			{
				case "desert":
					descriptors = new[] { "Arid ", "Sandy ", "Barren " };
					descriptor = Roll.Choice(descriptors, Enumerable.Repeat(1.0f, descriptors.Length).ToArray());
					break;
				case "jungle":
					descriptors = new[] { "Verdant ", "Lush ", "Dense " };
					descriptor = Roll.Choice(descriptors, Enumerable.Repeat(1.0f, descriptors.Length).ToArray());
					break;
				case "arctic":
					descriptors = new[] { "Frozen ", "Icy ", "Frigid " };
					descriptor = Roll.Choice(descriptors, Enumerable.Repeat(1.0f, descriptors.Length).ToArray());
					break;
				case "ocean":
				case "sea":
					descriptors = new[] { "Azure ", "Vast ", "Endless " };
					descriptor = Roll.Choice(descriptors, Enumerable.Repeat(1.0f, descriptors.Length).ToArray());
					break;
			}
		}

		return $"{prefix} {descriptor}{habitatType} {suffix}".Trim();
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

		float[] weights = weightedComponents.Values.ToArray();
		return Roll.Choice(components, weights);
	}

	private float CalculateComponentWeight(string component, Creature creature)
	{
		float weight = 1.0f;

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
		}

		return weight;
	}

	public void ResetNameLists()
	{
		usedBiomeNames.Clear();
		usedSpeciesNames.Clear();
	}
}
