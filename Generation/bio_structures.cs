using Godot;
using System;
using BioLibrary;

namespace BioStructures
{
	[Serializable]
	public class Creature
	{
		public Creature() { }
		public string Name { get; set; }
		public string ChemicalBasis { get; set; }
		public string Habitat { get; set; }
		public string TrophicLevel { get; set; }
		public string Locomotion { get; set; }
		public string SizeCategory { get; set; }
		public float GravitySizeMultiplier { get; set; }
		public string Symmetry { get; set; }
		public int NumberOfLimbs { get; set; }
		public string TailFeatures { get; set; }
		public int NumberOfManipulators { get; set; }
		public string Skeleton { get; set; }
		public string GrowthPattern { get; set; }
		public string Sexes { get; set; }
		public string Gestation { get; set; }
		public string SpecialGestation { get; set; }
		public string ReproductiveStrategy { get; set; }
		public string Skin { get; set; }
		public string Senses { get; set; }
		public string AnimalIntelligence { get; set; }
		public string MentalQualities { get; set; }
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
		public Ecosystem[] Ecosystems { get; set; }
	}
}
