using Godot;
using BioStructures;

public partial class DisplayManager : Node
{
	private RichTextLabel speciesDetails;

	public void Initialize(RichTextLabel details)
	{
		speciesDetails = details;
	}

	public void DisplayPlanetDetails(Godot.Collections.Dictionary dict)
	{
		if (speciesDetails == null) return;
		
		string planetInfo = $"[b]{dict["Name"].AsString()}[/b]\n\n";
		planetInfo += $"Temperature: {dict["Temperature"]}K\n";
		planetInfo += $"Hydrology: {dict["Hydrology"]}%\n";
		planetInfo += $"Gravity: {dict["Gravity"]}G\n";
		planetInfo += $"Land Masses: {dict["LandMasses"]}\n";
		planetInfo += $"Water Bodies: {dict["WaterBodies"]}\n";
		planetInfo += $"Total Ecosystems: {dict["TotalEcosystems"]}\n";
		
		speciesDetails.Text = planetInfo;
	}

	public void DisplayCreatureDetails(Creature creature)
	{
		if (speciesDetails == null) return;
		
		string details = $"[b]{creature.Name}[/b]\n\n";
		
		details += "[u]Basic Information[/u]\n";
		details += $"Chemical Basis: {creature.ChemicalBasis}\n";
		details += $"Habitat: {creature.Habitat}\n";
		details += $"Trophic Level: {creature.TrophicLevel}\n";
		details += $"Size: {creature.SizeCategory} ({creature.SpecificSize:F2} meters, x{creature.GravitySizeMultiplier:F2} gravity modifier)\n\n";
		
		details += "[u]Physical Characteristics[/u]\n";
		details += $"Symmetry: {creature.Symmetry}\n";
		details += $"Locomotion: {creature.Locomotion}\n";
		details += $"Limb Structure: {creature.LimbStructure}\n";
		details += $"Number of Limbs: {creature.ActualLimbCount}\n";
		details += $"Tail Features: {creature.TailFeatures}\n";
		details += $"Manipulator Type: {creature.ManipulatorType}\n";
		details += $"Number of Manipulators: {creature.ActualManipulatorCount}\n";
		details += $"Skeleton: {creature.Skeleton}\n";
		details += $"Skin Covering: {creature.SkinCovering}\n";
		details += $"Skin Type: {creature.SkinType}\n\n";
		
		details += "[u]Growth and Reproduction[/u]\n";
		details += $"Growth Pattern: {creature.GrowthPattern}\n";
		details += $"Sexes: {creature.Sexes}\n";
		details += $"Gestation: {creature.Gestation}\n";
		if (!string.IsNullOrEmpty(creature.SpecialGestation))
			details += $"Special Gestation: {creature.SpecialGestation}\n";
		details += $"Reproductive Strategy: {creature.ReproductiveStrategy}\n";
		details += $"Mating Behavior: {creature.MatingBehavior}\n\n";
		
		details += "[u]Senses and Intelligence[/u]\n";
		details += $"Primary Sense: {creature.PrimarySense}\n";
		details += "Sense Capabilities:\n";
		foreach (var sense in creature.SenseCapabilities)
		{
			details += $"  {sense.Key}: {sense.Value}\n";
		}
		if (creature.SpecialSenses.Count > 0)
		{
			details += "Special Senses: ";
			details += string.Join(", ", creature.SpecialSenses);
			details += "\n";
		}
		details += $"Animal Intelligence: {creature.AnimalIntelligence}\n\n";
		
		details += "[u]Social Behavior[/u]\n";
		details += $"Social Organization: {creature.SocialOrganization}\n";
		details += "Mental Traits:\n";
		foreach (var trait in creature.MentalTraits)
		{
			details += $"  {trait.Key}: {trait.Value}\n";
		}
		
		speciesDetails.Text = details;
	}

	public void DisplayBiomeDetails(Godot.Collections.Dictionary dict)
	{
		if (speciesDetails == null) return;

		string details = $"[b]{dict["Name"].AsString()}[/b]\n\n";
		details += $"Type: {dict["Type"].AsString()}\n";
		details += $"Species Count: {dict["SpeciesCount"].AsInt32()}\n";
		
		speciesDetails.Text = details;
	}

	public void DisplayLandMassDetails(Godot.Collections.Dictionary dict)
	{
		if (speciesDetails == null) return;

		string details = $"[b]{dict["Name"].AsString()}[/b]\n\n";
		details += $"Biome Count: {dict["BiomeCount"].AsInt32()}\n";
		details += $"Total Species: {dict["TotalSpecies"].AsInt32()}\n";
		
		speciesDetails.Text = details;
	}

	public void DisplayWaterBodyDetails(Godot.Collections.Dictionary dict)
	{
		if (speciesDetails == null) return;

		string details = $"[b]{dict["Name"].AsString()}[/b]\n\n";
		details += $"Type: {dict["WaterType"].AsString()}\n";
		details += $"Biome Count: {dict["BiomeCount"].AsInt32()}\n";
		details += $"Total Species: {dict["TotalSpecies"].AsInt32()}\n";
		
		speciesDetails.Text = details;
	}
	
	public void DisplayDetails(Godot.Collections.Dictionary dict)
	{
		string details = "";
		foreach (var key in dict.Keys)
		{
			details += $"{key}: {dict[key]}\n";
		}
		speciesDetails.Text = details;
	}
}
