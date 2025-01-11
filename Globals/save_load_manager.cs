using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using BioStructures;

public partial class SaveLoadManager : Node
{
	public static SaveLoadManager Instance { get; private set; }

	[Signal]
	public delegate void EcosystemSavedEventHandler(string path);
	
	[Signal]
	public delegate void EcosystemLoadedEventHandler(string path);

	public override void _EnterTree()
	{
		if (Instance == null) Instance = this;
		else QueueFree();
	}

	public void Initialize(FileDialog saveDialog, FileDialog openDialog)
	{
		saveDialog.FileSelected += OnSaveFileSelected;
		openDialog.FileSelected += OnLoadFileSelected;
	}

	private void OnSaveFileSelected(string path)
	{
		try
		{
			// Ensure the file has .json extension
			if (!path.EndsWith(".json"))
			{
				path += ".json";
			}

			// Get the current planet data from the TreeManager
			var planetDict = TreeManager.Instance.GetCurrentPlanetData();
			if (planetDict != null)
			{
				DataManager.Instance.SaveEcosystem(planetDict, path);
				GD.Print($"Successfully saved planet data to {path}");
			}
			else
			{
				GD.PrintErr("No planet data to save!");
			}
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error saving file: {e.Message}");
		}
	}

	private void OnLoadFileSelected(string path)
	{
		try
		{
			var loadedDict = DataManager.Instance.LoadEcosystemAsDict(path);
			if (loadedDict != null)
			{
				GD.Print("Loaded dictionary contents:");
				foreach (var key in loadedDict.Keys)
				{
					GD.Print($"Key: {key}");
				}

				// Create a new planet from the loaded data
				var planet = new Planet
				{
					Name = loadedDict.ContainsKey("Name") ? loadedDict["Name"].AsString() : "Loaded Planet"
				};

				// Handle LandMasses
				if (loadedDict.ContainsKey("LandMasses"))
				{
					planet.LandMasses = new List<LandMass>();
					var landMassesArray = loadedDict["LandMasses"].AsGodotArray();
					foreach (var lm in landMassesArray)
					{
						var landMassDict = lm.AsGodotDictionary();
						var landMass = new LandMass
						{
							Name = landMassDict.ContainsKey("Name") ? landMassDict["Name"].AsString() : "Unknown Landmass",
							Biomes = new List<Ecosystem>()
						};

						if (landMassDict.ContainsKey("Biomes"))
						{
							var biomesArray = landMassDict["Biomes"].AsGodotArray();
							foreach (var biomeVar in biomesArray)
							{
								var biomeDict = biomeVar.AsGodotDictionary();
								var ecosystem = new Ecosystem
								{
									HabitatType = biomeDict.ContainsKey("Type") ? biomeDict["Type"].AsString() : "Unknown",
									EcosystemID = biomeDict.ContainsKey("EcosystemID") ? biomeDict["EcosystemID"].AsInt32() : 0,
									LocationID = biomeDict.ContainsKey("LocationID") ? biomeDict["LocationID"].AsInt32() : 0
								};

								if (biomeDict.ContainsKey("Creatures"))
								{
									var creatureArray = biomeDict["Creatures"].AsGodotArray();
									var creatures = new List<Creature>();
									foreach (var creatureVar in creatureArray)
									{
										var creatureDict = creatureVar.AsGodotDictionary();
										creatures.Add(DataManager.Instance.DictionaryToCreature(creatureDict));
									}
									ecosystem.Creatures = creatures.ToArray();
								}
								else
								{
									ecosystem.Creatures = Array.Empty<Creature>();
								}

								landMass.Biomes.Add(ecosystem);
							}
						}
						planet.LandMasses.Add(landMass);
					}
				}

				// Handle WaterBodies
				if (loadedDict.ContainsKey("WaterBodies"))
				{
					planet.WaterBodies = new List<WaterBody>();
					var waterBodiesArray = loadedDict["WaterBodies"].AsGodotArray();
					foreach (var wb in waterBodiesArray)
					{
						var waterBodyDict = wb.AsGodotDictionary();
						var waterBody = new WaterBody
						{
							Name = waterBodyDict.ContainsKey("Name") ? waterBodyDict["Name"].AsString() : "Unknown Water Body",
							WaterType = waterBodyDict.ContainsKey("WaterType") ? waterBodyDict["WaterType"].AsString() : "Unknown",
							Biomes = new List<Ecosystem>()
						};

						if (waterBodyDict.ContainsKey("Biomes"))
						{
							var biomesArray = waterBodyDict["Biomes"].AsGodotArray();
							foreach (var biomeVar in biomesArray)
							{
								var biomeDict = biomeVar.AsGodotDictionary();
								var ecosystem = new Ecosystem
								{
									HabitatType = biomeDict.ContainsKey("Type") ? biomeDict["Type"].AsString() : "Unknown",
									EcosystemID = biomeDict.ContainsKey("EcosystemID") ? biomeDict["EcosystemID"].AsInt32() : 0,
									LocationID = biomeDict.ContainsKey("LocationID") ? biomeDict["LocationID"].AsInt32() : 0
								};

								if (biomeDict.ContainsKey("Creatures"))
								{
									var creatureArray = biomeDict["Creatures"].AsGodotArray();
									var creatures = new List<Creature>();
									foreach (var creatureVar in creatureArray)
									{
										var creatureDict = creatureVar.AsGodotDictionary();
										creatures.Add(DataManager.Instance.DictionaryToCreature(creatureDict));
									}
									ecosystem.Creatures = creatures.ToArray();
								}
								else
								{
									ecosystem.Creatures = Array.Empty<Creature>();
								}

								waterBody.Biomes.Add(ecosystem);
							}
						}
						planet.WaterBodies.Add(waterBody);
					}
				}

				// Set default settings if none were loaded
				planet.Settings = planet.Settings ?? SettingsManager.Instance.CurrentSettings;

				GD.Print($"Successfully loaded planet from {path}");
				EmitSignal(SignalName.EcosystemLoaded, path);
				
				// Update the tree and display with loaded data
				TreeManager.Instance.DisplayPlanetData(planet, DisplayManager.Instance);
			}
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error loading file: {e.Message}");
			GD.PrintErr(e.StackTrace);
		}
	}

	private void HandleLoadedPlanet(Planet loadedPlanet)
	{
		// Update the tree with loaded data
		TreeManager.Instance.DisplayPlanetData(loadedPlanet, DisplayManager.Instance);
	}
}
