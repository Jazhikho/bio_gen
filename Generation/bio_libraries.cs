using Godot;
using System;
using System.Collections.Generic;

namespace BioLibrary
{
	public static class BioData
	{
		public static Dictionary<string, int> Chemical_Basis()
		{
			var chemistry = new Dictionary<string, int>
			{
				{"Hydrogen-Based", 5},
				{"Ammonia-Based", 7},
				{"Hydrocarbon-Based", 8},
				{"Carbon-Based", 11},
				{"Chlorine-Based", 12},
				{"Silicon-Based", 15},
				{"Sulfur-Based", 17},
				{"Machine", 18}
			};
			return chemistry;
		}

		public static Dictionary<string, Dictionary<string, int>> Habitats()
		{
			var habitats = new Dictionary<string, Dictionary<string, int>>
			{
				{
					"Land", new Dictionary<string, int>
					{
						{"Plain", 7},
						{"Desert", 8},
						{"Beach", 9},
						{"Woodland", 10},
						{"Swampland", 11},
						{"Mountain", 12},
						{"Arctic", 13},
						{"Jungle", 18}
					}
				},
				{
					"Water", new Dictionary<string, int>
					{
						{"Banks", 7},
						{"Ocean", 8},
						{"Lake", 9},
						{"River", 10},
						{"Lagoon", 11},
						{"Deep Ocean", 12},
						{"Sea", 13},
						{"Reef", 18}
					}
				}
			};
			return habitats;
		}

		public static Dictionary<string, Dictionary<string, int>> Trophic_Level()
		{
			var trophic = new Dictionary<string, Dictionary<string, int>>
			{
				{
					"Non-sapient", new Dictionary<string, int>
					{
						{"Chemosynthetic", 3},
						{"Photosynthetic", 4},
						{"Decomposer", 5},
						{"Scavenger", 6},
						{"Omnivore", 7},
						{"Gathering Herbivore", 9},
						{"Grazing Herbivore", 11},
						{"Pouncing Carnivore", 12},
						{"Chasing Carnivore", 13},
						{"Trapping Carnivore", 14},
						{"Highjacking Carnivore", 15},
						{"Filter Feeder", 16},
						{"Parasite", 17},
						{"Symbiote", 18}
					}
				},
				{
					"Sapient", new Dictionary<string, int>
					{
						{"Symbiote", 3},
						{"Parasite", 4},
						{"Filter-Feeder", 5},
						{"Pouncing Carnivore", 6},
						{"Scavenger", 7},
						{"Gathering Herbivore", 9},
						{"Omnivore", 10},
						{"Chasing Carnivore", 12},
						{"Grazing Herbivore", 13},
						{"Highjacking Carnivore", 14},
						{"Trapping Carnivore", 15},
						{"Decomposer", 16},
						{"Photosynthesis", 17},
						{"Chemosynthesis", 18}
					}
				}
			};
			return trophic;
		}

		public static Dictionary<string, Dictionary<string, int>> Primary_Locomotion()
		{
			var locomotion = new Dictionary<string, Dictionary<string, int>>
			{
				{
					"Arctic", new Dictionary<string, int>
					{
						{"immobile", 2},
						{"slithering", 4},
						{"swimming", 6},
						{"digging", 7},
						{"walking", 9},
						{"winged flight", 11}
					}
				},
				{
					"Banks", new Dictionary<string, int>
					{
						{"immobile", 3},
						{"floating", 4},
						{"sailing", 5},
						{"swimming", 8},
						{"winged flight", 11}
					}
				},
				{
					"Reef", new Dictionary<string, int>
					{
						{"immobile", 5},
						{"floating", 6},
						{"digging", 7},
						{"walking", 9},
						{"swimming", 13}
					}
				},
				{
					"Desert", new Dictionary<string, int>
					{
						{"immobile", 2},
						{"slithering", 4},
						{"digging", 5},
						{"walking", 8},
						{"winged flight", 11}
					}
				},
				{
					"Jovian", new Dictionary<string, int>
					{
						{"swimming", 5},
						{"winged flight", 8},
						{"buoyant flight", 13}
					}
				},
				{
					"Beach", new Dictionary<string, int>
					{
						{"immobile", 2},
						{"slithering", 4},
						{"digging", 5},
						{"walking", 7},
						{"climbing", 8},
						{"swimming", 9},
						{"winged flight", 11}
					}
				},
				{
					"Lagoon", new Dictionary<string, int>
					{
						{"immobile", 4},
						{"floating", 5},
						{"slithering", 6},
						{"walking", 7},
						{"digging", 8},
						{"swimming", 9},
						{"winged flight", 11}
					}
				},
				{
					"Lake", new Dictionary<string, int>
					{
						{"immobile", 3},
						{"floating", 4},
						{"walking", 5},
						{"slithering", 6},
						{"swimming", 9},
						{"winged flight", 11}
					}
				},
				{
					"Mountain", new Dictionary<string, int>
					{
						{"immobile", 2},
						{"slithering", 4},
						{"digging", 5},
						{"walking", 7},
						{"climbing", 8},
						{"winged flight", 11}
					}
				},
				{
					"Plain", new Dictionary<string, int>
					{
						{"immobile", 2},
						{"slithering", 4},
						{"digging", 5},
						{"walking", 8},
						{"winged flight", 11}
					}
				},
				{
					"River", new Dictionary<string, int>
					{
						{"immobile", 3},
						{"floating", 4},
						{"slithering", 5},
						{"digging", 6},
						{"walking", 7},
						{"swimming", 9},
						{"winged flight", 11}
					}
				},
				{
					"Swampland", new Dictionary<string, int>
					{
						{"immobile", 2},
						{"swimming", 5},
						{"slithering", 6},
						{"digging", 7},
						{"walking", 8},
						{"climbing", 9},
						{"winged flight", 11}
					}
				},
				{
					"Woodland", new Dictionary<string, int>
					{
						{"immobile", 2},
						{"slithering", 4},
						{"digging", 5},
						{"walking", 7},
						{"climbing", 9},
						{"winged flight", 11}
					}
				}
			};
			return locomotion;
		}

		public static Dictionary<string, Dictionary<float, int>> SizeCategory()
		{
			var sizeCategory = new Dictionary<string, Dictionary<float, int>>
			{
				{
					"Small", new Dictionary<float, int>
					{
						{0.05f, 1},
						{0.07f, 2},
						{0.1f, 3},
						{0.15f, 4},
						{0.2f, 5},
						{0.3f, 6}
					}
				},
				{
					"Medium", new Dictionary<float, int>
					{
						{0.5f, 1},
						{0.7f, 2},
						{1.0f, 3},
						{1.5f, 4},
						{2.0f, 5},
						{3.0f, 6}
					}
				},
				{
					"Large", new Dictionary<float, int>
					{
						{5.0f, 1},
						{7.0f, 2},
						{10.0f, 3},
						{15.0f, 4},
						{20.0f, 5}
					}
				}
			};
			return sizeCategory;
		}

		public static Dictionary<float, float> GravitySizeMultiplier()
		{
			var gravitySizeMultiplier = new Dictionary<float, float>
			{
				{4.6f, 0.1f},   // Multiplier, gravity
				{2.9f, 0.2f},
				{2.2f, 0.3f},
				{1.8f, 0.4f},
				{1.6f, 0.5f},
				{1.4f, 0.6f},
				{1.3f, 0.7f},
				{1.2f, 0.8f},
				{1.1f, 0.9f},
				{1.0f, 1.0f},
				{0.9f, 1.25f},
				{0.75f, 1.5f},
				{0.6f, 2.0f},
				{0.5f, 2.5f},
				{0.4f, 3.5f},
				{0.3f, 5f}
			};
			return gravitySizeMultiplier;
		}

		public static Dictionary<string, int> Symmetry()
		{
			var symmetry = new Dictionary<string, int>
			{
				{"Bilateral", 2},
				{"Trilateral", 3},
				{"Radial", 4}, // Roll 1d+3 to determine how many sides
				{"Spherical", 5}, // Roll 1d: 1: 4 sides, 2-3: 6 sides, 4: 8 sides, 5: 12 sides, 6: 20 sides
				{"Asymmetric", 6}
			};
			return symmetry;
		}

		public static Dictionary<string, int> NumberOfLimbs()
		{
			var numberOfLimbs = new Dictionary<string, int>
			{
				{"Limbless", 1},
				{"One segment", 2}, // One limb per side
				{"Two segments", 3}, // Two limbs per side
				{"1d segments", 4}, // Each segment has one limb per side
				{"2d segments", 5}, // Each segment has one limb per side
				{"3d segments", 6}  // Each segment has one limb per side
			};
			return numberOfLimbs;
		}

		public static Dictionary<string, int> TailFeatures()
		{
			var tailFeatures = new Dictionary<string, int>
			{
				{"No features (tail is a 0-point advantage)", 5},
				{"Striker tail (Striker doing crushing damage)", 6},
				{"Long tail (Long enhancement)", 7},
				{"Constricting tail (Constriction Attack)", 8},
				{"Barbed striker tail (Striker doing cutting or piercing damage)", 9},
				{"Gripping tail (counts as an Extra Arm with Bad Grip)", 10},
				{"Branching tail (tail splits according to body symmetry)", 11},
				{"Combination (roll 1d+5)", 12}
			};
			return tailFeatures;
		}

		public static Dictionary<string, int> NumberOfManipulators()
		{
			var numberOfManipulators = new Dictionary<string, int>
			{
				{"No manipulators; limbs are locomotive or Strikers only", 6},
				{"1 set of manipulators with Bad Grip", 7},
				{"Prehensile tail or trunk (roll 1d: on a 6 check again for other manipulators)", 8},
				{"1 set of manipulators with normal DX", 9},
				{"2 sets of manipulators (roll 1d for each pair: 1-4 Bad Grip, 5-6 normal DX)", 10},
				{"1d sets of manipulators (roll 1d for each pair: 1-4 Bad Grip, 5-6 normal DX)", 11},
				{"1d sets of manipulators (roll 1d for each pair: 1-4 normal DX, 5-6 High Manual Dexterity 1)", 12}
			};
			return numberOfManipulators;
		}

		public static Dictionary<string, int> Skeleton()
		{
			var skeleton = new Dictionary<string, int>
			{
				{"None", 3},
				{"Hydrostatic", 5},
				{"External skeleton", 7},
				{"Internal skeleton", 10},
				{"Combination", 12}
			};
			return skeleton;
		}

		public static Dictionary<string, Dictionary<string, int>> Skin()
		{
			var skin = new Dictionary<string, Dictionary<string, int>>
			{
				{
					"Covering", new Dictionary<string, int>
					{
						{"Skin", 2},
						{"Scales", 3},
						{"Fur", 4},
						{"Feathers", 5},
						{"Exoskeleton", 6}
					}
				},
				{
					"Skin", new Dictionary<string, int>
					{
						{"Soft skin", 4},
						{"Normal skin", 5},
						{"Hide (DR 1)", 7},
						{"Thick Hide (DR 4)", 8},
						{"Armor shell (DR 5)", 9},
						{"Blubber (DR 4 and 1d levels of Temperature Tolerance)", 12}
					}
					},
				{
					"Feathers", new Dictionary<string, int>
					{
						{"Normal skin", 5},
						{"Feathers (+1 level of Temperature Tolerance)", 8},
						{"Thick feathers (+2 levels of Temperature Tolerance)", 10},
						{"Feathers over Hide (DR 1 and +1 Temperature Tolerance)", 11},
						{"Spines", 14}
					}
				},
				{
					"Exoskeleton", new Dictionary<string, int>
					{
						{"Light exoskeleton (DR 0)", 2},
						{"Tough exoskeleton (DR 1)", 4},
						{"Heavy exoskeleton (DR 3)", 5},
						{"Armor shell (DR 5)", 8}
					}
				},
				{
					"Scales", new Dictionary<string, int>
					{
						{"Normal skin", 3},
						{"Scales (DR 1)", 8},
						{"Heavy scales (DR 3)", 10},
						{"Armor shell (DR 5)", 14}
					}
				},
				{
					"Fur", new Dictionary<string, int>
					{
						{"Normal skin", 5},
						{"Fur", 7},
						{"Thick fur (+1 level of Temperature Tolerance)", 9},
						{"Thick fur over Hide (DR 1 and +1 level of Temperature Tolerance)", 11},
						{"Spines", 14}
					}
				}
			};
			return skin;
		}

		public static Dictionary<string, int> GrowthPattern()
		{
			var growthPattern = new Dictionary<string, int>
			{
				{"Metamorphosis", 5},
				{"Molting", 6},
				{"Continuous Growth", 11},
				{"Unusual Growth Pattern (adding segments, branching, etc.)", 14}
			};
			return growthPattern;
		}

		public static Dictionary<string, int> Sexes()
		{
			var sexes = new Dictionary<string, int>
			{
				{"Asexual reproduction or Parthenogenesis", 4},
				{"Hermaphrodite", 5},
				{"Two Sexes", 7},
				{"Switching between male and female", 10},
				{"Three or more Sexes", 11},
				{"Roll twice and combine", 12}
			};
			return sexes;
		}

		public static Dictionary<string, int> Gestation()
		{
			var gestation = new Dictionary<string, int>
			{
				{"Spawning/Pollinating", 6},
				{"Egg-Laying", 7},
				{"Live-Bearing", 9},
				{"Live-Bearing with Pouch", 11}
			};
			return gestation;
		}

		public static Dictionary<string, int> SpecialGestation()
		{
			var specialGestation = new Dictionary<string, int>
			{
				{"Brood Parasite", 1},
				{"Parasitic Young", 2},
				{"Cannibalistic Young (fatal to parent)", 4},
				{"Cannibalistic Young (consume each other)", 6}
			};
			return specialGestation;
		}

		public static Dictionary<string, int> ReproductiveStrategy()
		{
			var strategy = new Dictionary<string, int>
			{
				{"Strong K-Strategy", 4},
				{"Moderate K-Strategy", 6},
				{"Median Strategy", 7},
				{"Moderate r-Strategy", 8},
				{"Strong r-Strategy", 10}
			};
			return strategy;
		}
		public static Dictionary<string, Dictionary<string, int>> Senses()
		{
			var senses = new Dictionary<string, Dictionary<string, int>>
			{
				{
					"Primary Sense", new Dictionary<string, int>
					{
						{"Hearing", 1},
						{"Vision", 2},
						{"Touch and Taste", 3}
					}
				},
				{
					"Vision", new Dictionary<string, int>
					{
						{"Blindness", 6},
						{"Blindness (Can sense light and dark, -10%) [-45]", 7},
						{"Bad Sight and Colorblindness", 8},
						{"Bad Sight or Colorblindness*", 9},
						{"Normal Vision*", 10},
						{"Telescopic Vision 4*", 15}
					}
				},
				{
					"Hearing", new Dictionary<string, int>
					{
						{"Deafness", 6},
						{"Hard of Hearing", 7},
						{"Normal Hearing*", 9},
						{"Normal Hearing with extended range (Subsonic Hearing if Large, Ultrahearing otherwise)*", 11},
						{"Acute Hearing 4**", 12},
						{"Acute Hearing 4 and either Subsonic Hearing or Ultrahearing**", 13},
						{"Acute Hearing 4 with Ultrasonic Hearing and Sonar**", 14}
					}
				},
				{
					"Touch", new Dictionary<string, int>
					{
						{"Numb-2 DX from poor sense of touch", 2},
						{"-1 DX from poor sense of touch", 3},
						{"Human-level touch", 5},
						{"Acute Touch 4*", 7},
						{"Acute Touch 4 and either Sensitive Touch or Vibration Sense*", 9}
					}
				},
				{
					"Taste/Smell", new Dictionary<string, int>
					{
						{"No Sense of Smell/Taste", 3},
						{"No Sense of Smell (can taste, -50%) [-2 points]", 4},
						{"Normal taste/smell", 6},
						{"Acute Taste/Smell 4 (aquatic organisms use Acute Taste only)*", 9},
						{"Acute Taste/Smell 4 and Discriminatory Smell (aquatic organisms use Discriminatory Taste)*", 11}
					}
				},
				{
					"Special Senses", new Dictionary<string, int>
					{
						{"360° Vision*", 11},
						{"Absolute Direction", 11},
						{"Discriminatory Hearing*", 11},
						{"Peripheral Vision (10-12)", 11},
						{"Night Vision 1d+3*", 11},
						{"Ultravision*", 11},
						{"Detect (Heat)*", 11},
						{"Detect (Electric Fields)*", 11},
						{"Perfect Balance", 11},
						{"Scanning Sense (Radar)*", 11}
					}
				}
			};
			return senses;
		}
		public static Dictionary<string, Dictionary<string, int>> AnimalIntelligence()
		{
			var intelligence = new Dictionary<string, Dictionary<string, int>>
			{
				{
					"Animal Intelligence", new Dictionary<string, int>
					{
						{"Mindless (IQ 0)", 3},
						{"Preprogrammed (IQ 1 and Cannot Learn)", 4},
						{"Low Intelligence (IQ 1-3 and Bestial)", 6},
						{"High Intelligence (IQ 3-5 and Bestial)", 9},
						{"Presapient (IQ 5)", 11}
					}
				},
				{
					"Mating Behavior", new Dictionary<string, int>
					{
						{"Mating only, no pair bond", 5},
						{"Temporary pair bond", 6},
						{"Permanent pair bond", 8},
						{"Harem", 9},
						{"Hive", 11}
					}
				},
				{
					"Social Organization Type", new Dictionary<string, int>
					{
						{"Solitary", 6},
						{"Pair-bonded", 7},
						{"Small group of 2d members", 9},
						{"Medium group of 4d members", 11},
						{"Large Herd of 1d x 10 members", 12}
					}
				}
			};
			return intelligence;
		}

		public static Dictionary<string, Dictionary<string, int>> MentalQualities()
		{
			var mentalQualities = new Dictionary<string, Dictionary<string, int>>
			{
				{
					"Curiosity", new Dictionary<string, int>
					{
						{"Curious (9)", 3},
						{"Curious (12)", 2},
						{"Nosy (quirk)", 1},
						{"Normal", 0},
						{"Staid (quirk)", -1},
						{"Incurious (12)", -2},
						{"Incurious (9)", -3}
					}
				},
				{
					"Chauvinism", new Dictionary<string, int>
					{
						{"Chauvinistic (quirk)", 3},
						{"Normal", 0},
						{"Broad-Minded (quirk)", -1},
						{"Undiscriminating (quirk)", -3}
					}
				},
				{
					"Concentration", new Dictionary<string, int>
					{
						{"Single-Minded and High Pain Threshold", 3},
						{"Single-Minded", 2},
						{"Attentive (quirk)", 1},
						{"Normal", 0},
						{"Distractible (quirk)", -1},
						{"Short Attention Span (12)", -2},
						{"Short Attention Span (9)", -3}
					}
				},
				{
					"Egoism", new Dictionary<string, int>
					{
						{"Selfish (9)", 3},
						{"Selfish (12)", 2},
						{"Proud (quirk)", 1},
						{"Normal", 0},
						{"Humble (quirk)", -1},
						{"Selfless (12)", -2},
						{"Selfless (6)", -3}
					}
				},
				{
					"Empathy", new Dictionary<string, int>
					{
						{"Empathy", 3},
						{"Empathy (Sensitive)", 2},
						{"Responsive (quirk)", 1},
						{"Normal", 0},
						{"Oblivious", -1},
						{"Callous", -2},
						{"Low Empathy", -3}
					}
				},
				{
					"Gregariousness", new Dictionary<string, int>
					{
						{"Gregarious", 3},
						{"Chummy", 2},
						{"Congenial (quirk)", 1},
						{"Normal", 0},
						{"Uncongenial (quirk)", -1},
						{"Loner (12)", -2},
						{"Loner (9)", -3}
					}
				},
				{
					"Imagination", new Dictionary<string, int>
					{
						{"Imaginative (quirk)", 2},
						{"Normal", 0},
						{"Dull (quirk)", -1},
						{"Hidebound", -2},
						{"Hidebound and -1 IQ", -3}
					}
				},
				{
					"Suspicion", new Dictionary<string, int>
					{
						{"Fearfulness 1", 2},
						{"Careful (quirk)", 1},
						{"Normal", 0},
						{"Fearlessness 1", -1},
					}
				},
				{
					"Playfulness", new Dictionary<string, int>
					{
						{"Compulsive Playfulness (12)", 2},
						{"Playful (quirk)", 1},
						{"Normal", 0},
						{"Serious (quirk)", -1},
						{"Odious Racial Habit (Wet Blanket)", -2},
						{"No Sense of Humor", -3}
					}
				}
			};
			return mentalQualities;
		}
	}
}
