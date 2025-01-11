using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using BioLibrary;

namespace BioStructures
{
	[Serializable]
	public class Creature
	{
		// Basic Properties
		public string Name { get; set; }
		public string ChemicalBasis { get; set; }
		public string Habitat { get; set; }
		public string TrophicLevel { get; set; }
		
		// Physical Characteristics
		public string SizeCategory { get; set; }
		public float SpecificSize { get; set; }
		public float GravitySizeMultiplier { get; set; }
		public float WeightInPounds { get; set; }
		
		public string Symmetry { get; set; }
		public int? SymmetryNumber { get; set; }
		public string Locomotion { get; set; }
		public string BreathingMethod { get; set; }
		public string TemperatureRegulation { get; set; }
		
		// Limbs and Manipulation
		public string LimbStructure { get; set; } // This corresponds to the NumberOfLimbs dictionary keys
		public int ActualLimbCount { get; set; }  // This is the calculated number based on LimbStructure
		public string TailFeatures { get; set; }
		public string ManipulatorType { get; set; } // This corresponds to NumberOfManipulators dictionary keys
		public int ActualManipulatorCount { get; set; }
		
		// Body Structure
		public string Skeleton { get; set; }
		public string SkinCovering { get; set; } // The type of covering (fur, scales, etc.)
		public string SkinType { get; set; }     // The specific characteristics of that covering
		
		// Reproduction
		public string GrowthPattern { get; set; }
		public string Sexes { get; set; }
		public string Gestation { get; set; }
		public string SpecialGestation { get; set; }
		public string ReproductiveStrategy { get; set; }
		
		// Senses
		public string PrimarySense { get; set; }
		public Dictionary<string, string> SenseCapabilities { get; set; } = new Dictionary<string, string>();
		public List<string> SpecialSenses { get; set; } = new List<string>();
		
		// Intelligence and Behavior
		public string AnimalIntelligence { get; set; }
		public string MatingBehavior { get; set; }
		public string SocialOrganization { get; set; }
		public Dictionary<string, int> MentalTraits { get; set; } = new Dictionary<string, int>();
	}

	[Serializable]
	public class Ecosystem
	{
		public Ecosystem() { }
		public Creature[] Creatures { get; set; }
		public string HabitatType { get; set; }
		public int EcosystemID { get; set; }
		public int LocationID { get; set; }
	}
	
	[Serializable]
	public class Planet
	{
		public string Name { get; set; }
		public SettingsManager.PlanetSettings Settings { get; set; }
		public List<LandMass> LandMasses { get; set; }
		public List<WaterBody> WaterBodies { get; set; }

		public int GetTotalEcosystemCount()
		{
			int count = 0;
			count += LandMasses?.Sum(l => l.Biomes.Count) ?? 0;
			count += WaterBodies?.Sum(w => w.Biomes.Count) ?? 0;
			return count;
		}
	}
	
	[Serializable]
	public class LandMass
	{
		public string Name { get; set; }
		public List<Ecosystem> Biomes { get; set; }
	}
	
	[Serializable]
	public class WaterBody
	{
		public string Name { get; set; }
		public string WaterType { get; set; }
		public List<Ecosystem> Biomes { get; set; }
	}
}
