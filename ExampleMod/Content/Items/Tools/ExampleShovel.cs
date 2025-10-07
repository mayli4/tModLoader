using ExampleMod.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Tools;

public class ExampleShovel : ModItem
{
	public override void SetDefaults() {
		Item.damage = 20;
		Item.DamageType = DamageClass.Melee;
		Item.width = 40;
		Item.height = 40;
		// On the official wiki, https://terraria.wiki.gg/wiki/Pickaxes, the "Use time" column corresponds to Item.useAnimation and the "Mining speed" column corresponds to Item.useTime.
		Item.useTime = 10;
		Item.useAnimation = 10;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.knockBack = 6;
		Item.value = Item.buyPrice(gold: 1); // Buy this item for one gold - change gold to any coin and change the value to any number <= 100
		Item.rare = ItemRarityID.Green;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = true;

		Item.pick = 35; // How strong the pickaxe is, see https://terraria.wiki.gg/wiki/Pickaxe_power for a list of common values
		Item.attackSpeedOnlyAffectsWeaponAnimation = true; // Melee speed affects how fast the tool swings for damage purposes, but not how fast it can dig
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient<ExampleItem>()
			.AddTile<Tiles.Furniture.ExampleWorkbench>()
			.Register();
	}

	public override void GetSpecialToolUsageSettings(ref Player.SpecialToolUsageSettings settings) {
		// Mark this item as a valid tool!
        settings.IsAValidTool = true;

        // Define a custom callback for the actual action when using this item.
        settings.UsageAction = MineIn3x3Area;

        // Define a custom callback for conditions where the tool can be used. If it returns false, it wont be called.
        settings.UsageCondition = CanMine;
    }

	private bool CanMine(Player user, Item item, int targetX, int targetY) {
		if (!WorldGen.InWorld(targetX, targetY)) {
			return false;
		}

		Tile tile = Main.tile[targetX, targetY];

		if (!tile.HasTile || !Main.tileSolid[tile.TileType]) {
			return false;
		}

		// Can only dig dirt and example block
		return tile.TileType == TileID.Dirt || tile.TileType == ModContent.TileType<ExampleBlock>();
	}

    private void MineIn3x3Area(Player user, Item item, int targetX, int targetY) {
        int radius = 1; // For a 3x3 area (1 tile around the center)

        // Iterate through the 3x3 area
        for (int x = targetX - radius; x <= targetX + radius; x++) {
            for (int y = targetY - radius; y <= targetY + radius; y++) {
                // Check if the tile is within world bounds
                if (WorldGen.InWorld(x, y)) {
                    Tile tile = Main.tile[x, y];

                    // Check if the tile is solid, and matches our desired tile types.
                    if (tile.HasTile && Main.tileSolid[tile.TileType] && tile.TileType == TileID.Dirt || tile.TileType == ModContent.TileType<ExampleBlock>()) {
                        // Call PickTile for each tile in the area. Here, we pass 'user' (the player) as the entity source, and 'item' (this shovel) as the item source, so that ModTile.ModifyTileDamage can react to this specific item.
                        user.PickTile(x, y, item.pick, user, item);
                    }
                }
            }
        }
    }
}